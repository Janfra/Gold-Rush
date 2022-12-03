using System.Collections;
using System.Collections.Generic;
using System.Security.Authentication.ExtendedProtection;
using UnityEngine;

public class SlowBlockInteract : InteractableObject
{
    [SerializeField] private SlowingBlocks[] slowBlocksAffected;
    [SerializeField] private float[] speedModifierAtStage;
    [SerializeField] private float durationToReset;
    [SerializeField] private int totalStages;
    private int currentStage;

    private void OnValidate()
    {
        if (speedModifierAtStage.Length != totalStages || speedModifierAtStage == null)
        {
            speedModifierAtStage = new float[totalStages];
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        OnStart();
    }

    protected override void OnStart()
    {
        base.OnStart();
        interactLoadBar.isLoadingSuccessful = LoadResult;
        interactLoadBar.Init(totalStages, GetComponent<MeshFilter>().mesh, transform);
        currentStage = totalStages;
    }

    public override void OnInteract(Transform transform)
    {
        if (!isInteractable)
            return;

        isInteractable = false;
        StopAllCoroutines();
        LoadResult(false);
        StartLoadingDown(transform);
    }

    /// <summary>
    /// Set to run when loading down is completed
    /// </summary>
    /// <param name="isLoadSuccesful"></param>
    /// <param name="playerPos"></param>
    protected override void LoadResult(bool isLoadSuccesful, Transform playerPos)
    {
        // If the loading fails, reset the current stage.
        if (!isLoadSuccesful && !isInteractable)
        {
            isInteractable = true;
            StartLoadingUp();
            return;
        }

        ChangeStageBy(-1);
        StartLoadingDown(playerPos);
    }

    /// <summary>
    /// Set to run when loading up is completed
    /// </summary>
    /// <param name="isLoadSuccesful"></param>
    protected void LoadResult(bool isLoadSuccesful)
    {
        if (!isLoadSuccesful)
        {
            Debug.Log($"{gameObject.name} loading has been cancelled.");
            return;
        }

        if(currentStage == totalStages - 1)
        {
            Debug.Log($"{gameObject.name} has loaded to the max!");
            return;
        }
        ChangeStageBy(1);
        StartLoadingUp();
    }

    private void StartLoadingDown(Transform transformChecked)
    {
        if(currentStage > 0)
        {
            int targetStage = Mathf.Clamp(currentStage - 1, 0, totalStages);
            StartCoroutine(interactLoadBar.StartInteraction(new Interact_LB.LoadInteractionInfo(durationPerInteractStage, currentStage, targetStage, this.transform, transformChecked)));
        }
        else
        {
            StartLoadingUp();
        }
    }

    private void StartLoadingUp()
    {
        if(currentStage < totalStages)
        {
            int targetStage = Mathf.Clamp(currentStage + 1, 0, totalStages);
            StartCoroutine(interactLoadBar.LoadToTarget(new LoadingBar.LoadToTargetInfo(durationToReset, currentStage, targetStage)));
        }
    }

    private void ChangeStageBy(int numberToAdd)
    {
        currentStage += numberToAdd;
        currentStage = Mathf.Clamp(currentStage, 0, totalStages - 1);
        Debug.Log($"{gameObject.name} current stage is: {currentStage}, new modifier: {speedModifierAtStage[currentStage]}");

        foreach(SlowingBlocks slowingBlocks in slowBlocksAffected)
        {
            slowingBlocks.ChangeLerpModifier(speedModifierAtStage[currentStage]);
        }
    }
}
