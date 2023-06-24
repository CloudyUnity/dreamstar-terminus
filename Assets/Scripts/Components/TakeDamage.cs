using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TakeDamage : MonoBehaviour
{
    [SerializeField] int _startingHP;
    public float HP;

    private void Start()
    {
        if (HP == 0)
            HP = _startingHP;
    }

    public virtual void DealDamage(int damage)
    {
        if (HP <= 0)
            return;

        HP -= damage;

        if (HP <= 0)
            Die();
    }

    protected virtual void Die()
    {
        Destroy(gameObject);
    }
}
