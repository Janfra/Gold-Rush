
using System;
using System.Collections;
using UnityEngine;

public class Dynamic_GridUpdating : Static_GridUpdating
{
    public static event Action<IGridUpdatable, Vector3> OnDynamicUpdateGrid;
    private static float updateOffset = 0.3f;

    private void Awake()
    {
        Init();
        OnDynamicUpdateGrid += DynamicGridUpdate;
    }

    public static void NewDynamicGridUpdate(IGridUpdatable objToUpdate, Vector3 oldPos)
    {
        OnDynamicUpdateGrid?.Invoke(objToUpdate, oldPos);
    }

    private void DynamicGridUpdate(IGridUpdatable objToUpdate, Vector3 oldPos)
    {
        if (objToUpdate.GridUpdateIndex == ownedUpdatableObject.GridUpdateIndex)
        {
            StartCoroutine(UpdateGrid(objToUpdate, oldPos));
        }
    }

    private IEnumerator UpdateGrid(IGridUpdatable objToUpdate, Vector3 oldPos)
    {
        Pathfind_Grid.DynamicUpdateGrid(transform.position, GetXSize(), GetZSize(), ownedUpdatableObject.GridUpdateIndex);
        yield return new WaitForSeconds(updateOffset);
        Pathfind_Grid.DynamicUpdateGrid(oldPos, GetXSize() + 1, GetZSize() + 1, -1);
    }
}

