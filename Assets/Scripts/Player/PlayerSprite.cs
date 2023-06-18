using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSprite : Singleton
{
    [SerializeField] bool _debugColors;
    SpriteRenderer _rend;
    PlayerInput _input;
    Animator _anim;
    PlayerSystems _systems;

    bool _squashing;
    PlayerMovement _move;

    private void Start()
    {
        _rend = GetComponent<SpriteRenderer>();
        _input = Get<PlayerInput>();
        _anim = GetComponent<Animator>();
        _move = Get<PlayerMovement>();
        _systems = Get<PlayerSystems>();
    }

    private void Update()
    {
        if (_input.ArrowKeys.x != 0)
            _rend.flipX = _input.ArrowKeys.x == -1;

        if (_debugColors)
            _rend.color = Debug_Colors();

        _anim.SetBool("Jumping", _move.Jumping);        
        _anim.SetBool("Falling", _move.JumpFalling);
        _anim.SetBool("Running", _input.ArrowKeys.x != 0 && _move.Grounded);
        // Sliding
        // Taking Damage
        // Invincibility
        // Death
        // Gain ability
    }

    Color Debug_Colors()
    {
        if (_systems.Invincible)
            return Color.white;

        if (_move.Grounded && _move.Walled)
            return Color.magenta;

        if (_move.Grounded)
            return Color.green;

        if (_move.Walled)
            return Color.blue;

        return Color.red;
    }

    public void Squash() => StartCoroutine(C_SqaushStretch(0.2f, new Vector2(0.1f, -0.1f)));
    public void Stretch() => StartCoroutine(C_SqaushStretch(0.2f, new Vector2(-0.1f, 0.1f)));

    IEnumerator C_SqaushStretch(float dur, Vector2 mag)
    {
        if (_squashing)
            yield break;

        _squashing = true;
        float elapsed = 0;
        Vector2 start = transform.localScale;

        while (elapsed < dur)
        {
            float humped = M_Extensions.HumpCurveV2(elapsed / dur, 1);

            transform.localScale = start + humped * mag;

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localScale = start;
        _squashing = false;
    }
}
