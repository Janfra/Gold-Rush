using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerRotation
{
    /// <summary>
    /// Layer that will collide with the rotation ray
    /// </summary>
    [SerializeField] private LayerMask hitOnly;

    /// <summary>
    /// Collider for the ray to set rotation
    /// </summary>
    [SerializeField] private BoxCollider rotationPlane;

    /// <summary>
    /// Main camera
    /// </summary>
    private Camera cam;

    public void Init()
    {
        cam = Camera.main;
    }

    /// <summary>
    /// Shoots a ray that is used to change the players rotation
    /// </summary>
    public void GetRotation(Transform transform)
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, hitOnly))
        {
            Vector3 hitPos = hit.point;
            hitPos.y = transform.position.y;
            Vector3 rotation = hitPos - transform.position;
            transform.forward = rotation;
        }
    }

    #region Outdated

    // Found out about layers matrix so this is unnecessary now
    //private void IgnoreCollision(Collider collider)
    //{
    //    Physics.IgnoreLayerCollision(rotationPlane.gameObject.layer, collider.gameObject.layer, true);
    //    Debug.Log($"Rotation plane is now ignoring layer: {collider.gameObject.layer}");
    //}

    #endregion
}
