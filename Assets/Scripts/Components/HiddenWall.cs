using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HiddenWall : TakeDamage
{
    [SerializeField] bool _outer;

    [HideInInspector] public bool Broken;

    static List<Vector3> _checkPos = new List<Vector3>()
    {
        new Vector2(-1,0),
        new Vector2(0, -1),
        new Vector2(1, 0),
        new Vector2(0, 1),
    };

    public override void DealDamage(int damage)
    {
        if (!_outer)
            return;

        base.DealDamage(damage);
    }

    protected override void Die()
    {
        if (Broken)
            return;

        Broken = true;

        foreach (var pos in _checkPos)
        {
            var hit = M_Extensions.Ray(transform.position + pos * 0.5f, Vector3.back);

            if (hit.collider == null || !hit.collider.TryGetComponent(out HiddenWall wall) || wall.Broken)
                continue;

            wall.Die();
        }

        Destroy(gameObject); // TODO: Make this a traveller
    }
}
