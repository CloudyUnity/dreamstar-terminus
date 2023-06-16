using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttacks : Singleton
{
    PlayerInput _input;
    [SerializeField] Vector2 _lightAttackPos;
    [SerializeField] Vector2 _lightAttackSize;
    [SerializeField] PlayerData _data;
    [SerializeField] Vector2 _knockback;

    private void Start()
    {
        _input = Get<PlayerInput>();
    }

    private void Update()
    {
        if (_input.Attack)
        {
            Attack();
        }
    }

    void Attack()
    {
        var hits = Physics2D.BoxCastAll(transform.position + new Vector3(_lightAttackPos.x * _input.ArrowKeys.x, _lightAttackPos.y), _lightAttackSize, 0, Vector3.back, 1, M_LayerMasks.Entity);

        foreach (var hit in hits)
        {
            if (hit.collider.TryGetComponent(out TakeDamage td))
            {
                td.DealDamage(_data.lightAttackDamage);
            }

            if (hit.collider.TryGetComponent(out EntityMovementController emc))
            {
                emc.Fling(_knockback);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position + (Vector3)_lightAttackPos, _lightAttackSize);
    }
}
