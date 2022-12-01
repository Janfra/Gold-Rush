using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldHealth : Health
{
    public static event Action OnGoldBlockDestroyed;

    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    protected override void Init()
    {
        healthBar.SetSliderAndCanvas(UIManager.Instance.GetGoldSlider(), UIManager.Instance.GetGoldCanvas());
        base.Init();
        healthBar.SetVisibility(true);
    }

    public override void Damaged(float damageTaken)
    {
        health -= damageTaken;

        if (!IsAlive())
        {
            gameObject.SetActive(false);
            Static_GridUpdating.NewStaticGridUpdate(this);
            OnGoldBlockDestroyed?.Invoke();
        }
        else
        {
            healthBar.SetLoadTarget(health, health + damageTaken);
        }
        healthBar.SetVisibility(true);
    }
}
