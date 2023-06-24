using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : Singleton
{
    /* DoAttack()
     * Instantiate
     * DisableMove
     * PlayAnimation
     * Wait
     * BoxCastAll
     * SFX
     */

    [SerializeField] GameObject _attack01;
    [SerializeField] int _attack01Damage;
    [SerializeField] Vector2 _knockback;
    PlayerInput _input;
    PlayerMovement _move;

    [SerializeField] Vector3 _dis;
    [SerializeField] Vector3 _size;
    [SerializeField] Vector2 _playerKnockback;

    private void Start()
    {
        _input = Get<PlayerInput>();
        _move = GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        if (_input.Attack)
            Attack01();
    }

    public void Attack01()
    {
        Vector2 pos = transform.position + _input.LastPressed * _dis;

        GameObject go = Instantiate(_attack01, pos, Quaternion.Euler(0, 0, _input.LastPressed == -1 ? 180 : 0));

        var hits = Physics2D.BoxCastAll(pos, _size, 0, Vector3.back, 1, M_LayerMasks.Entity);

        foreach (var hit in hits)
        {
            if (hit.collider.TryGetComponent(out TakeDamage td))
            {
                td.DealDamage(_attack01Damage);
            }
            if (hit.collider.TryGetComponent(out EntityMovementController emc))
            {
                emc.Fling(_knockback);
            }
        }

        _move.Fling(_playerKnockback * -_input.LastPressed, ForceMode2D.Impulse);

        StartCoroutine(C_DelayDestroy(go, 3));
    }

    IEnumerator C_DelayDestroy(GameObject go, float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(go);
    }

    #region EDITOR METHODS
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireCube(transform.position + _dis, _size);
    }
    #endregion
}
