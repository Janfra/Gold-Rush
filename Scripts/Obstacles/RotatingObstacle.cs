using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingObstacle : MonoBehaviour
{
    [SerializeField] Transform pivotPos;
    [SerializeField] float orbitSpeed;
    [SerializeField] float rotationSpeed;

    private void Start()
    {
        StartCoroutine(Rotate());
    }

    private IEnumerator Rotate()
    {
        while (true)
        {
            transform.RotateAround(pivotPos.position, new Vector3(0, 1, 0), Time.deltaTime * orbitSpeed);
            transform.Rotate(new Vector3(0, rotationSpeed, 0) * Time.deltaTime);
            yield return null;
        }
        yield return null;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(pivotPos.position, transform.localPosition.magnitude);
    }
}
