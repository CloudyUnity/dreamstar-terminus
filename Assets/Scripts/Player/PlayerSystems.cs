using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSystems : Singleton
{
    public int StartingHP;
    [SerializeField] float _invSeconds;
    [SerializeField] float _knockback;
    float _invTimer;
    public int HP;

    Vector2 _lastSafePos;

    PlayerMovement _move;
    M_Transition _transition;

    public bool Invincible => _invTimer > 0;
    public bool Dead => HP <= 0;

    private void Start()
    {
        if (StartingHP == 0)
            throw new System.Exception("Starting HP not set");

        if (HP == 0)
            HP = StartingHP;

        _move = GetComponent<PlayerMovement>();
        _transition = Get<M_Transition>();
    }

    private void Update()
    {
        _invTimer -= Time.deltaTime;

        RaycastHit2D hitL = M_Extensions.Ray(transform.position + new Vector3(-0.3f, -0.3f), Vector2.down, M_LayerMasks.Ground, 0.1f);
        bool safeL = hitL.collider != null && !M_Tags.CheckTag(hitL.collider.gameObject, "NotSafe");

        RaycastHit2D hitR = M_Extensions.Ray(transform.position + new Vector3(0.3f, -0.3f), Vector2.down, M_LayerMasks.Ground, 0.1f);
        bool safeR = hitR.collider != null && !M_Tags.CheckTag(hitR.collider.gameObject, "NotSafe");

        if (_move.Grounded && safeL && safeR)
        {
            _lastSafePos = transform.position;
        }
    }

    public void TakeDamage(int amount, GameObject cause = null)
    {
        if (Dead || Invincible || _move.MovementDisabled)
            return;

        Debug.Log("Took " + amount + " damage");

        HP -= amount;

        _invTimer = _invSeconds;

        if (cause != null)
        {
            Vector2 dir = cause.transform.position.x > transform.position.x ? Vector2.left : Vector2.right;
            dir.y = 0.1f;
            _move.Fling(_knockback * dir, ForceMode2D.Impulse);
        }

        if (HP <= 0)
            Die();
    }

    void Die()
    {
        // SFX, Effects

        Get<M_World>().Restart();
    }

    async void SendToLastSafePos()
    {
        _move.DisableMovement();

        await _transition.TransitionAsync(inwards: true);

        Debug.Log("Waited");

        transform.position = _lastSafePos;

        await _transition.TransitionAsync(inwards: false);

        _move.ReEnableMovement();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (Dead || Invincible || _move.MovementDisabled)
            return;

        if (collision.gameObject.TryGetComponent(out ContactDamage contact))
        {
            TakeDamage(contact.Damage, contact.gameObject);

            if (M_Tags.CheckTag(contact.gameObject, "SendPlayer"))
                SendToLastSafePos();
        }
    }
}
