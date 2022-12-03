using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Stored_LB : LoadingBar
{
    #region Variables

    /// <summary>
    /// Lerp function values, made into variables to update them at any point and check progress.
    /// </summary>
    private float targetValue, initialValue, currentValue, duration, alpha;

    /// <summary>
    /// Checks if the target has been achieved.
    /// </summary>
    private bool isCompleted => targetValue == currentValue;

    /// <summary>
    /// Sets if the target is now being healed
    /// </summary>
    private bool isHealed;

    /// <summary>
    /// Sets if there is a target
    /// </summary>
    private bool isTarget;

    #endregion

    #region Constants

    /// <summary>
    /// Minimun duration of the lerp
    /// </summary>
    private const float minDuration = 0.2f;
    /// <summary>
    /// Max duration of the lerp
    /// </summary>
    private const float maxDuration = 1f;

    #endregion

    public override void OnStart()
    {
        base.OnStart();
        isHealed = false;
        duration = maxDuration;
        targetValue = sliderBar.maxValue;
        currentValue = targetValue;
        initialValue = targetValue;
    }

    #region Clamp & Setters

    /// <summary>
    /// Clamps duration to the min duration constant.
    /// </summary>
    /// <param name="newDuration">New duration</param>
    private void SetDuration(float newDuration)
    {
        if (newDuration <= minDuration)
        {
            duration = minDuration;
        }
        else
        {
            duration = newDuration;
        }
    }

    #endregion

    #region Loading Methods

    /// <summary>
    /// Sets the target to start loading towards. Lower duration if is already loading.
    /// </summary>
    /// <param name="newTargetValue">Target to lerp to</param>
    /// <param name="newInitialValue">Starting value</param>
    public void SetLoadTarget(float newTargetValue, float newInitialValue)
    {
        SetVisibility(true);
        isHealed = false;
        targetValue = newTargetValue;
        // Every time it is damaged while loading, make it faster
        SetDuration(duration - 0.25f);

        if (!isTarget)
        {
            isTarget = true;
            OnStartLoading(newInitialValue);
        }
        else if (newTargetValue > currentValue || newTargetValue > initialValue)
        {
            isHealed = true;
            OnStartLoading(newInitialValue);
        }
    }

    /// <summary>
    /// Sets 'initialValue' and restarts variables to then start loading towards target.
    /// </summary>
    /// <param name="newInitialValue">New starting value</param>
    private void OnStartLoading(float newInitialValue)
    {
        SetDuration(maxDuration);
        initialValue = newInitialValue;
        alpha = 0f;
        // StartCoroutine(LoadToTarget());
    }

    /// <summary>
    /// Loads to the target while keeping the UI active until completation.
    /// </summary>
    public IEnumerator LoadToTarget(float newTargetValue, float newInitialValue)
    {
        SetLoadTarget(newTargetValue, newInitialValue);
        while (!isCompleted && !isHealed)
        {
            SetVisibility(true);

            alpha += Time.deltaTime;
            LerpTowardsTarget(alpha);

            yield return null;
        }
        if (isHealed)
        {
            while (!isCompleted)
            {
                SetVisibility(true);

                alpha += Time.deltaTime;
                LerpTowardsTarget(alpha);

                yield return null;
            }
        }
        yield return new WaitForSeconds(0.5f);
        SetVisibility(false);
    }

    /// <summary>
    /// Changes the slider value to the return of the lerp function interpolating towards the target value.
    /// </summary>
    /// <param name="alpha"></param>
    private void LerpTowardsTarget(float alpha)
    {
        currentValue = Lerp(initialValue, targetValue, duration, AlphaClamp(alpha, duration));
        sliderBar.value = currentValue;
        isTarget = !isCompleted;
    }

    #endregion
}