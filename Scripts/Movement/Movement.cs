using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public abstract class Movement : MonoBehaviour
{
    [SerializeField] protected float baseSpeed;
    protected float speedModifier;

    private void Awake()
    {
        Init();
    }

    protected void Init()
    {

    }

    virtual protected float GetCurrentSpeed()
    {
        float newSpeed = baseSpeed + speedModifier;
        if (newSpeed < 0)
        {
            return 0;
        }
        else
        {
            return newSpeed;
        }
    }

    public void AddSpeedModifier(float newModifier)
    {
        speedModifier += newModifier;
    }

    abstract protected void Move();
}
