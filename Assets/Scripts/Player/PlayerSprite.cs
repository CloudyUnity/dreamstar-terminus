using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSprite : Singleton
{
    [SerializeField] bool _debugColors;
    [SerializeField] bool _runOn, _jumpOn, _fallOn;
    SpriteRenderer _rend;
    PlayerInput _input;
    Animator _anim;
    PlayerSystems _systems;

    bool _squashing;
    PlayerMovement _move;

    public Vector2 ForceMoveSceneChange;

    bool _invFramesRunning;

    private void Start()
    {
        _rend = GetComponent<SpriteRenderer>();
        _anim = GetComponent<Animator>();

        _input = Get<PlayerInput>();
        _move = Get<PlayerMovement>();
        _systems = Get<PlayerSystems>();
    }

    private void Update()
    {
        float movement = ForceMoveSceneChange.x != 0 ? ForceMoveSceneChange.x : _input.ArrowKeys.x;
        if (movement != 0)
            _rend.flipX = movement < 0;

        bool run = _runOn && _move.Grounded && movement != 0;        
        _anim.SetBool("Running", run);

        bool jump = _jumpOn && (_move.Jumping || _move.WallJumping || ForceMoveSceneChange.y > 0);
        _anim.SetBool("Jumping", jump);

        bool fall = _fallOn && (_move.JumpFalling || ForceMoveSceneChange.y < 0);
        _anim.SetBool("Falling", fall);

        if (_systems.Invincible && !_invFramesRunning)
            StartCoroutine(C_InvicibilityFrames());

        if (_debugColors)
            _rend.color = Debug_Colors();

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

    IEnumerator C_InvicibilityFrames()
    {
        _invFramesRunning = true;

        float elapsed = 0;
        float changeDur = 0.5f;

        while (_systems.Invincible)
        {
            elapsed += Time.deltaTime;

            if (elapsed > changeDur)
            {
                _rend.enabled = !_rend.enabled;
                elapsed = 0;
            }

            yield return null;
        }

        _rend.enabled = true;
        _invFramesRunning = false;
    }
}
