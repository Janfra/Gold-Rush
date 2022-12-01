using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    /// <summary>
    /// Interaction handling defined by object.
    /// </summary>
    /// <param name="transform">Players transform to check distance</param>
    void OnInteract(Transform transform);
}
