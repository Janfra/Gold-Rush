using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowingBlocks : MonoBehaviour
{
    [SerializeField] private float slowModifier = 1f;

    private void Start()
    {
        GetComponent<Collider>().isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent(out Movement movement))
        {
            Debug.Log($"Slowing Block entered by: {other.gameObject.name}");
            movement.AddSpeedModifier(-slowModifier);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.TryGetComponent(out Movement movement))
        {
            movement.AddSpeedModifier(slowModifier);
        }
    }
}
