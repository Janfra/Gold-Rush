using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InteractableObject : MonoBehaviour, IInteractable
{
    /// <summary>
    /// Sets if it is possible to interact with the object.
    /// </summary>
    protected bool isInteractable;

    /// <summary>
    /// Duration of the transition to next stage.
    /// </summary>
    [SerializeField] protected float durationPerInteractStage;

    /// <summary>
    /// Timer to be used in between stages and UI handling.
    /// </summary>
    [SerializeField] protected Interact_LB interactLoadBar;


    // Start is called before the first frame update
    void Start()
    {
        OnStart();
    }

    protected virtual void OnStart()
    {
        isInteractable = true;
        if (durationPerInteractStage == 0f)
        {
            durationPerInteractStage = 1f;
            Debug.LogError($"Duration Per Stage in {gameObject.name} has not been set");
        }

        interactLoadBar.OnStart();
        interactLoadBar.isInteractionSuccesful = LoadResult;
    }

    protected abstract void LoadResult(bool isLoadSuccesful, Transform playerPos);

    public abstract void OnInteract(Transform transform);
}
