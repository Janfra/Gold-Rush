using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class SlowingBlocks : MovingObstacle
{
    [SerializeField] private float slowModifier = 1f;

    private void Start()
    {
        GetComponent<Collider>().isTrigger = true;
        OnStart();
    }

    protected override void OnStart()
    {
        base.OnStart();
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

    public void ChangeLerpModifier(float newSpeed)
    {
        SetLerpModifier(newSpeed);
    }
}
