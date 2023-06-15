using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Should be called Smelly Solid
public class SemiSolid : MonoBehaviour
{
    [SerializeField] BoxCollider2D _col;
    PlayerMovement _move;
    PlayerInput _input;

    const bool JUMP_FOR_DOWN = false;

    private void Reset()
    {
        _col = GetComponent<BoxCollider2D>();
    }

    private void Start()
    {
        _move = Singleton.Get<PlayerMovement>();
        _input = Singleton.Get<PlayerInput>();
    }

    private void Update()
    {
        Vector2 playerFeet = _move.transform.position + new Vector3(0, -0.32f);

        bool dropDown = _input.ArrowKeys.y < 0 && _input.ArrowKeys.x == 0 && (_input.Jump || !JUMP_FOR_DOWN);
        _col.enabled = playerFeet.y > _col.bounds.max.y && !dropDown;
    }
}
