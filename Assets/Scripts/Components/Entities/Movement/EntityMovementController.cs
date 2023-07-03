using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EntityMovementController : MonoBehaviour
{
    protected Rigidbody2D _rb;

    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    public void Move(Vector2 force)
    {
        _rb.AddForce(force);
    }

    public void Fling(Vector2 force)
    {
        _rb.AddForce(force, ForceMode2D.Impulse);
    }
}
