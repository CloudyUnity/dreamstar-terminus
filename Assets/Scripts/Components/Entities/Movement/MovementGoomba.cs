using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementGoomba : EntityMovementController
{
    [SerializeField] float _speed;
    [SerializeField] bool _goingRight;

    private void Update()
    {
        Vector2 dir = _goingRight ? Vector2.right : Vector2.left;

        _rb.velocity = _speed * dir * Time.deltaTime;

        RaycastHit2D hit = M_Extensions.Ray(transform.position, dir, M_LayerMasks.Ground, 0.25f);

        if (hit.collider != null)
            _goingRight = !_goingRight;
    }
}
