using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour, IDamagable, IGridUpdatable
{
    #region Variables

    /// <summary>
    /// Scriptable Object for HP
    /// </summary>
    [SerializeField] protected float maxHealth;

    /// <summary>
    /// UI loading bar
    /// </summary>
    [SerializeField] protected Stored_LB healthBar;

    /// <summary>
    ///  Damage type that affects this object
    /// </summary>
    [SerializeField] protected IDamagable.DamageType damagableBy;

    /// <summary>
    /// Current HP
    /// </summary>
    protected float health;

    #endregion

    #region IGridUpdatable Index

    public int GridUpdateIndex
    {
        get
        {
            return gridUpdateIndex;
        }
        set
        {

        }
    }
    private int gridUpdateIndex;

    #endregion

    private void Start()
    {
        Init();
        healthBar.OnStart();
    }

    protected virtual void Init()
    {
        healthBar.Init(maxHealth, GetComponent<MeshFilter>().mesh, transform);
        gridUpdateIndex = GridUpdateManager.Instance.GetIndex(gameObject);

        health = maxHealth;
    }

    #region Damage Handling

    /// <summary>
    /// Deal damage, show it on the UI and disable object if dead.
    /// </summary>
    /// <param name="damageTaken">The damage dealt to the object</param>
    virtual public void Damaged(float damageTaken)
    {
        health -= damageTaken;

        if (!IsAlive())
        {
            gameObject.SetActive(false);
            Static_GridUpdating.NewStaticGridUpdate(this);
        }
        else
        {
            healthBar.SetLoadTarget(health, health + damageTaken);
        }
    }

    /// <summary>
    /// Check if the object still has HP left
    /// </summary>
    /// <returns>Is the object alive</returns>
    protected bool IsAlive()
    {
        return health > 0f;
    }

    /// <summary>
    /// Checks if the object is damagable
    /// </summary>
    /// <param name="damageType">The type of damage being applied</param>
    /// <returns>If it is possible to damage the object</returns>
    public bool IsDamagableBy(IDamagable.DamageType damageType)
    {
        return damagableBy == damageType || damagableBy == IDamagable.DamageType.Every;
    }

    #endregion
}