using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldNugget : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject alert;
    [SerializeField] private BoxCollider pickingCollider;
    private bool isGrabbed = false;

    public void OnInteract(Transform transform)
    {
        StartInteract();
        StartCoroutine(Grabbing(transform));
    }

    private void StartInteract()
    {
        pickingCollider.enabled = false;
        alert.SetActive(false);
    }

    private void OnDropped()
    {
        pickingCollider.enabled = true;
        alert.SetActive(true);
    }

    private void OnEnable()
    {
        OnDropped();
    }

    private IEnumerator Grabbing(Transform interactingTransform)
    {
        isGrabbed = true;
        while (isGrabbed)
        {
            this.transform.position = interactingTransform.position;
            yield return null;

            if (Input.GetKeyDown(KeyCode.E))
            {
                isGrabbed = false;
                OnDropped();
            }
        }
        yield return null;
    }

    public void CancelGrabbing()
    {
        isGrabbed = false;
    }

    private IEnumerator Alert()
    {
        Vector3 initialPos = alert.transform.position;
        const float lerpSpeed = 2f;
        const float maxUpMovement = 0.5f;
        const float maxDownMovement = -0.5f;

        alert.transform.position = Vector3.Lerp(new Vector3(initialPos.x, initialPos.y + maxDownMovement, initialPos.z), new Vector3(initialPos.x, initialPos.y + maxUpMovement, initialPos.z), Time.deltaTime * lerpSpeed);
        yield return null;
    }
}
