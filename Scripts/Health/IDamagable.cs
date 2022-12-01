using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamagable
{
    /// <summary>
    /// Damage handling of the object
    /// </summary>
    /// <param name="damageTaken">The damage dealt to the object</param>
    void Damaged(float damageTaken);

    /// <summary>
    /// Checks if the object is damagable
    /// </summary>
    /// <param name="damageType">The type of damage being applied</param>
    /// <returns>If it is possible to damage the object</returns>
    bool IsDamagableBy(DamageType damageType);

    /// <summary>
    /// Damage types available
    /// </summary>
    [System.Serializable]
    public enum DamageType
    {
        Every,
        Enemy,
        Player,
    }
}