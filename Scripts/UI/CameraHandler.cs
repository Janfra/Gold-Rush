using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraHandler : MonoBehaviour
{
    [SerializeField] private Transform playerPos;

    #region Constants

    private const float yOffset = 10f;
    private const float zOffset = 10f;

    #endregion

    private void LateUpdate()
    {
        FollowPlayer();
        transform.LookAt(playerPos);
    }

    /// <summary>
    /// Sets transform to follow the player at the constants offset distance.
    /// </summary>
    private void FollowPlayer()
    {
        transform.position = new Vector3(playerPos.position.x, playerPos.position.y + yOffset, playerPos.position.z - zOffset);
    }
}