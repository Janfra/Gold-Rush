using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointScoringArea : MonoBehaviour
{
    public static event Action<int> OnPointScored;
    [SerializeField] private int pointCount = 0;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out GoldNugget gold))
        {
            pointCount++;
            gold.CancelGrabbing();
            gold.gameObject.SetActive(false);
            OnPointScored?.Invoke(pointCount);
        }
    }
}
