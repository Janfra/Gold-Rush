using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Static_GridUpdating : MonoBehaviour
{
    protected static event Action<Mesh, IGridUpdatable> OnUpdateMesh;
    protected static event Action<IGridUpdatable> OnUpdateGrid;
    [SerializeField] protected IGridUpdatable ownedUpdatableObject;

    private float xMeshSize;
    private float zMeshSize;

    private void Awake()
    {
        Init();
    }

    protected void Init()
    {
        Mesh mesh = GetComponent<MeshFilter>().mesh;
        xMeshSize = mesh.bounds.extents.x;
        zMeshSize = mesh.bounds.extents.z;

        if(TryGetComponent<IGridUpdatable>(out IGridUpdatable setOwned))
        {
            ownedUpdatableObject = setOwned;
        }
        else
        {
            Debug.LogError($"Grid updating component in object {gameObject.name} could not be found.");
        }

        OnUpdateMesh += UpdateMesh;
        OnUpdateGrid += UpdateGrid;
    }

    #region Functionality

    public static void NewStaticGridUpdate(IGridUpdatable objToUpdate)
    {
        OnUpdateGrid?.Invoke(objToUpdate);
    }

    public void UpdateGrid(IGridUpdatable objToUpdate)
    {
        if(objToUpdate.GridUpdateIndex == ownedUpdatableObject.GridUpdateIndex)
        {
            Pathfind_Grid.UpdateGrid(transform.position, GetXSize(), GetZSize());
            // Debug.Log(GetXSize() + " X / Z " + GetZSize());
        }
    }

    protected int GetXSize()
    {
        return Mathf.CeilToInt(xMeshSize * transform.localScale.x);
    }

    protected int GetZSize()
    {
        return Mathf.CeilToInt(zMeshSize * transform.localScale.z);
    }
    protected void UpdateMesh(Mesh mesh)
    {
        xMeshSize = mesh.bounds.extents.x;
        zMeshSize = mesh.bounds.extents.z;
    }

    public void UpdateMesh(Mesh mesh, IGridUpdatable objUpdating)
    {
        if(objUpdating.GridUpdateIndex == ownedUpdatableObject.GridUpdateIndex)
        {
            xMeshSize = mesh.bounds.extents.x;
            zMeshSize = mesh.bounds.extents.z;
        }
    }

    public static void MeshUpdated(Mesh mesh, IGridUpdatable thisObj)
    {
        OnUpdateMesh?.Invoke(mesh, thisObj);
    }

    #endregion
}
