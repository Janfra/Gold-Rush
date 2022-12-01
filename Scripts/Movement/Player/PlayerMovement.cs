using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : Movement
{
    /// <summary>
    /// Players movement direction.
    /// </summary>
    private Vector3 direction;
    [SerializeField] PlayerRotation rotation;

    private void Start()
    {
        rotation.Init();
    }

    void Update()
    {
        rotation.GetRotation(transform);
        GetPlayersInput();
    }

    private void FixedUpdate()
    {
        if (direction.magnitude > 0f)
        {
            Move();
        }
    }

    /// <summary>
    /// Sets the player new position depending on direction and speed.
    /// </summary>
    protected override void Move()
    {
        transform.position += direction * GetCurrentSpeed() * Time.fixedDeltaTime;
        // Debug.Log($"Current move should be: {direction * stats.speed * Time.fixedDeltaTime}");
    }

    /// <summary>
    /// Gets the players input and normalizes it to set the direction.
    /// </summary>
    private void GetPlayersInput()
    {
        float xInput = Input.GetAxisRaw("Horizontal");
        float zInput = Input.GetAxisRaw("Vertical");
        direction = new Vector3(x: xInput, y: 0, z: zInput).normalized;
    }
}