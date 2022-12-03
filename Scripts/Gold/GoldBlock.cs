using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class GoldBlock : InteractableObject, IGridUpdatable
{
    public static event Action<Vector3> OnGoldBlockDisabled;

    #region Variables

    /// <summary>
    /// Prefab information.
    /// </summary>
    [SerializeField] protected GameObject resourcePrefab;

    /// <summary>
    /// Changes the mesh to match the visual.
    /// </summary>
    [SerializeField] protected MeshFilter meshFilter;

    /// <summary>
    /// Visual of each stage.
    /// </summary>
    [SerializeField] protected List<Mesh> stageMesh;

    /// <summary>
    /// Index for current state of the mesh.
    /// </summary>
    protected int currentStageIndex;

    /// <summary>
    /// Initial scale of object to reset it
    /// </summary>
    protected Vector3 originalScale;

    /// <summary>
    /// Initial position of object to reset it
    /// </summary>
    protected Vector3 originalPos;

    #endregion

    #region IGridUpdatable Index

    public int GridUpdateIndex
    {
        get
        {
            return gridUpdateIndex;
        }
        set
        {

        }
    }
    private int gridUpdateIndex;

    #endregion

    #region Constants

    /// <summary>
    /// Max stage for the index.
    /// </summary>
    private const int maxStageIndex = 2;

    /// <summary>
    ///  Added to the currentStage index to match loading bar values.
    /// </summary>
    private const int stageOffset = 1;

    #endregion

    private void Awake()
    {
        interactLoadBar.OnAwake(GetComponentInChildren<RotationConstraint>());
    }

    void Start()
    {
        OnStart();
        interactLoadBar.OnStart();
        GoldHealth.OnGoldBlockDestroyed += OnBlockDisabled;
    }

    protected override void OnStart()
    {
        base.OnStart();
        currentStageIndex = 0;
        originalScale = transform.localScale;

        // Loading
        interactLoadBar.Init(maxStageIndex + stageOffset, meshFilter.mesh, transform);

        // Grid Updating
        gridUpdateIndex = GridUpdateManager.Instance.GetIndex(gameObject);
    }

    #region Interact

    /// <summary>
    /// Starts interaction
    /// </summary>
    /// <param name="playerPos">Players position to check if its in range</param>
    public override void OnInteract(Transform playerPos)
    {
        if (!isInteractable)
            return;

        isInteractable = false;
        StartLoading(playerPos);
    }

    /// <summary>
    /// Starts the handling of the loading and the result.
    /// </summary>
    /// <param name="playerPos"></param>
    protected void StartLoading(Transform playerPos)
    {
        StartCoroutine(interactLoadBar.StartInteraction(new Interact_LB.LoadInteractionInfo(durationPerInteractStage, currentStageIndex, currentStageIndex + stageOffset, transform, playerPos)));
    }

    /// <summary>
    /// Function delegated to the loading that gets called once finished
    /// </summary>
    /// <param name="isLoadSuccessful"></param>
    /// <param name="playerPos"></param>
    protected override void LoadResult(bool isLoadSuccessful, Transform playerPos)
    {
        if (!isLoadSuccessful)
        {
            isInteractable = true;
            return;
        }
        if (IsNotLastStage())
        {
            UpdateMeshStage();
        }
        else
        {
            GenerateResource();
            return;
        }
        StartLoading(playerPos);
    }

    /// <summary>
    /// Updates the mesh filter to the next stage, while updating relevant scripts
    /// </summary>
    protected void UpdateMeshStage()
    {
        const float sizeOffset = 0.7f;

        currentStageIndex++;
        meshFilter.mesh = stageMesh[currentStageIndex];
        transform.localScale = new Vector3(transform.localScale.x * sizeOffset, transform.localScale.y * sizeOffset, transform.localScale.z * sizeOffset);

        if(transform.localScale.magnitude < 0)
        {
            transform.localScale = Vector3.one;
        }

        if(currentStageIndex == maxStageIndex)
        {
            originalPos = transform.position;
            transform.position = new Vector3(transform.position.x, 1, transform.position.z);
        }

        // Updates load bar position and mesh size for grid updates
        interactLoadBar.SetOnTop(meshFilter.mesh, transform);
        Static_GridUpdating.MeshUpdated(meshFilter.mesh, this);
    }

    /// <summary>
    /// Generates a resource and disables game object 
    /// </summary>
    protected void GenerateResource()
    {
        ObjectPooler.Instance.SpawnFromPool(ObjectPooler.PoolObjName.GoldNugget, transform.position, Quaternion.identity);
        DisableBlock();
    }

    /// <summary>
    /// Checks if the resource is in its last stage
    /// </summary>
    /// <returns>Is the resource not at its last stage</returns>
    protected bool IsNotLastStage()
    {
        return currentStageIndex != maxStageIndex;
    }

    #endregion

    /// <summary>
    /// Resets the block to be able to be reused
    /// </summary>
    protected void Reset()
    {
        currentStageIndex = 0;
        meshFilter.mesh = stageMesh[currentStageIndex];
        transform.localScale = originalScale;
        transform.position = originalPos;
        isInteractable = true;

        // Updates load bar position and mesh size for grid updates
        interactLoadBar.SetOnTop(meshFilter.mesh, transform);
        Static_GridUpdating.MeshUpdated(meshFilter.mesh, this);
    }

    /// <summary>
    /// Disables the block
    /// </summary>
    protected void DisableBlock()
    {
        gameObject.SetActive(false);
        OnBlockDisabled();
    }

    /// <summary>
    /// Spawns a new block and resets for reuse
    /// </summary>
    protected void OnBlockDisabled()
    {
        Reset();
        OnGoldBlockDisabled?.Invoke(transform.position);
    }
}