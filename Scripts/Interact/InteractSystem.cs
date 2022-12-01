using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractSystem : MonoBehaviour
{
    #region Variables

    /// <summary>
    /// Sets the layer that gets affected by the interaction ray
    /// </summary>
    [SerializeField] private LayerMask interactableMask;

    [SerializeField] private Transform interactionPoint;

    /// <summary>
    /// Max range to be able to interact / Interaction ray range
    /// </summary>
    private float interactRange;

    #endregion

    /// <summary>
    /// Max range before interaction gets cancelled
    /// </summary>
    public const float maxRange = 1.5f;

    private void Start()
    {
        interactRange = maxRange;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Interact();
        }
    }

    /// <summary>
    /// Tries to interact with an object in front of the player. NOTE: Doesnt use TryGetComponent cause only interactable objects should be in the interact layer
    /// </summary>
    private void Interact()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, interactRange, interactableMask))
        {
            // If the collider's object is interactable start interaction
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();
            interactable.OnInteract(interactionPoint);
            Debug.Log("Interacting");
        }
    }
}
