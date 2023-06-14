using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSprite : Singleton
{
    SpriteRenderer _rend;
    PlayerInput _input;
    Animator _anim;

    bool _squashing;
    PlayerMovement _move;

    private void Start()
    {
        _rend = GetComponent<SpriteRenderer>();
        _input = Get<PlayerInput>();
        _anim = GetComponent<Animator>();
        _move = Get<PlayerMovement>();
    }

    private void Update()
    {
        if (_input.ArrowKeys.x != 0)
            _rend.flipX = _input.ArrowKeys.x == -1;

        _rend.color = Debug_Colors();
    }

    Color Debug_Colors()
    {
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
            float humped = ManagerExtensions.HumpCurveV2(elapsed / dur, 1);

            transform.localScale = start + humped * mag;

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localScale = start;
        _squashing = false;
    }
}
