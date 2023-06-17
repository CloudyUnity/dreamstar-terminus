using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSystems : Singleton
{
    public int StartingHP;
    [SerializeField] float _invSeconds;
    float _invTimer;
    public int HP;

    Vector2 _lastSafePos;

    PlayerMovement _move;

    public bool Invincible => _invTimer > 0;
    public bool Dead => HP <= 0;

    private void Start()
    {
        if (StartingHP == 0)
            throw new System.Exception("Starting HP not set");

        if (HP == 0)
            HP = StartingHP;

        _move = GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        _invTimer -= Time.deltaTime;

        RaycastHit2D hitL = M_Extensions.Ray(transform.position + new Vector3(-0.3f, -0.3f), Vector2.down, 0.1f, M_LayerMasks.Ground);
        bool safeL = hitL.collider != null && !hitL.collider.gameObject.CheckTag("NotSafe");
        RaycastHit2D hitR = M_Extensions.Ray(transform.position + new Vector3(0.3f, -0.3f), Vector2.down, 0.1f, M_LayerMasks.Ground);
        bool safeR = hitR.collider != null && !hitR.collider.gameObject.CheckTag("NotSafe");

        if (_move.Grounded && safeL && safeR)
        {
            _lastSafePos = transform.position;
        }
    }

    public void TakeDamage(int amount)
    {
        Debug.Log("Took " + amount + " damage");
        if (Dead || Invincible)
            return;

        HP -= amount;

        _invTimer = _invSeconds;

        if (HP <= 0)
            Die();
    }

    void Die()
    {
        Get<PlayerMovement>().DisableMovement();

        // SFX, Effects

        Get<M_World>().LoadScene("SampleScene");
    }

    void SendToLastSafePos()
    {
        transform.position = _lastSafePos;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent(out ContactDamage contact))
        {
            TakeDamage(contact.Damage);

            if (contact.gameObject.CheckTag("SendPlayer"))
                SendToLastSafePos();
        }
    }
}
