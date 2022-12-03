using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldPointer : MonoBehaviour
{
    [SerializeField] private RectTransform pointerTransform;
    [SerializeField] private Transform objPointedTo;
    [SerializeField] private Camera uiCamera;
    [SerializeField] private float borderPadding;
    [SerializeField] private float rotationOffset;
    private Action OnLookAt;

    private void Start()
    {
        GoldBlockManager.OnBlockSpawned += SetObjPointedTo;
    }

    private void SetObjPointedTo(Transform newObj)
    {
        objPointedTo = newObj;
        OnLookAt = TargetSet;
    }

    private void SetRotatingObj(RectTransform newObj)
    {
        pointerTransform = newObj;
        OnLookAt = TargetSet;
    }

    private void LateUpdate()
    {
        OnLookAt?.Invoke();
    }

    private void TargetSet()
    {
        if(pointerTransform != null && objPointedTo != null)
        {
            RotateToTarget();
            // SetPointerPos();
        }
        else
        {
            Debug.Log($"Rotating object or object pointed to is null in {gameObject.name}!");
            OnLookAt = null;
        }
    }

    private void RotateToTarget()
    {
        Vector3 toPos = objPointedTo.position;
        Vector3 fromPos = Camera.main.transform.position;

        fromPos.z = 0;
        Vector3 dir = (toPos - fromPos).normalized;
        float angle = GetAngle(dir);
        pointerTransform.localEulerAngles = new Vector3(0, 0, angle);
    }

    private float GetAngle(Vector3 dir)
    {
        dir = dir.normalized;
        float n = ((Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg) + rotationOffset) % 360;
        return n;
    }

    private void SetPointerPos()
    {
        Vector3 targetPositionOnScreen = Camera.main.WorldToScreenPoint(objPointedTo.position);
        if (IsTargetOffScreen(targetPositionOnScreen))
        {
            SetPositionWithPadding(targetPositionOnScreen);
        }
    }

    private bool IsTargetOffScreen(Vector3 targetPositionOnScreen)
    {
        return targetPositionOnScreen.x <= borderPadding || targetPositionOnScreen.x >= Screen.width - borderPadding || targetPositionOnScreen.y <= borderPadding || targetPositionOnScreen.y >= Screen.height - borderPadding;
    }

    private void SetPositionWithPadding(Vector3 targetPosOnScreen)
    {
        Vector3 targetPos = targetPosOnScreen;
        if (targetPos.x <= borderPadding) targetPos.x = borderPadding;
        if (targetPos.x >= Screen.width - borderPadding) targetPos.x = Screen.width - borderPadding;        
        if (targetPos.y <= borderPadding) targetPos.y = borderPadding;
        if (targetPos.y >= Screen.width - borderPadding) targetPos.y = Screen.width - borderPadding;

        Vector3 pointerWorldPos = uiCamera.ScreenToWorldPoint(targetPos);
        pointerTransform.position = pointerWorldPos;
        pointerTransform.localPosition = new Vector3(pointerTransform.localPosition.x, pointerTransform.localPosition.y, 0f); 
    }
}
