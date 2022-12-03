using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingObstacle : MonoBehaviour
{
    [SerializeField] private float lerpSpeed = 1f;
    [SerializeField] private float delay;
    [SerializeField] private bool delayOnce;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Vector3 moveTowardsPos;
    private float lerpModifier;

    // Start is called before the first frame update
    void Start()
    {
        OnStart();
    }

    protected virtual void OnStart()
    {
        lerpModifier = 1;
        if (rb != null)
        {
            rb = GetComponent<Rigidbody>();
        }

        if (rb != null && moveTowardsPos != null)
        {
            StartMovement();
        }
    }

    protected void StartMovement()
    {
        if (delayOnce)
        {
            StartCoroutine(MoveToPosDelayedOnce());
            return;
        }

        StartCoroutine(MoveToPos(delay));
    }

    protected IEnumerator MoveToPos(float delayBeforeStart)
    {
        // Initializing
        Vector3 startingPos = transform.position;
        Vector3 currentPos = Vector3.zero;
        float alpha = 0f;

        yield return new WaitForSeconds(delayBeforeStart);
        while (true)
        {
            // Go up
            alpha += Time.deltaTime * (lerpSpeed * lerpModifier);
            alpha = Mathf.Clamp01(alpha);

            // Move object
            currentPos = Vector3.Lerp(startingPos, moveTowardsPos, alpha);
            rb.MovePosition(currentPos);

            // If at target, stop
            if (alpha == 1)
            {
                break;
            }
            yield return null;
        }

        moveTowardsPos = startingPos;
        StartCoroutine(MoveToPos(delayBeforeStart));
        yield return null;
    }

    protected IEnumerator MoveToPos()
    {
        // Initializing
        Vector3 startingPos = transform.position;
        Vector3 currentPos = Vector3.zero;
        float alpha = 0f;

        while (true)
        {
            // Go up
            alpha += Time.deltaTime * (lerpSpeed * lerpModifier);
            alpha = Mathf.Clamp01(alpha);

            // Move object
            currentPos = Vector3.Lerp(startingPos, moveTowardsPos, alpha);
            rb.MovePosition(currentPos);

            // If at target, stop
            if (alpha == 1)
            {
                break;
            }
            yield return null;
        }

        moveTowardsPos = startingPos;
        StartCoroutine(MoveToPos());
        yield return null;
    }

    protected IEnumerator MoveToPosDelayedOnce()
    {
        // Initializing
        Vector3 startingPos = transform.position;
        Vector3 currentPos = Vector3.zero;
        float alpha = 0f;
        yield return new WaitForSeconds(delay);

        while (true)
        {
            // Go up
            alpha += Time.deltaTime * (lerpSpeed * lerpModifier);
            alpha = Mathf.Clamp01(alpha);

            // Move object
            currentPos = Vector3.Lerp(startingPos, moveTowardsPos, alpha);
            rb.MovePosition(currentPos);

            // If at target, stop
            if (alpha == 1)
            {
                break;
            }
            yield return null;
        }

        moveTowardsPos = startingPos;
        StartCoroutine(MoveToPos(0));
        yield return null;
    }

    protected void SetLerpModifier(float newModifier)
    {
        if(newModifier >= 0)
        {
            lerpModifier = newModifier;
        }
    }

    protected float GetLerpSpeed()
    {
        return lerpSpeed;
    }

    protected void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, moveTowardsPos);
    }
}
