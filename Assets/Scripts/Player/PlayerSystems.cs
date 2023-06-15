using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSystems : Singleton
{
    [SerializeField] int _startingHP;
    public int HP;

    private void Start()
    {
        if (HP == 0)
            HP = _startingHP;
    }

    void TakeDamage(int amount)
    {
        if (HP <= 0)
            return;

        HP -= amount;

        if (HP <= 0)
            Die();
    }

    void Die()
    {
        Get<PlayerMovement>().DisableMovement();

        // SFX, Effects

        Get<ManagerWorld>().LoadScene("SampleScene");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent(out ContactDamage contact))
        {
            TakeDamage(contact.Damage);
        }
    }
}
