using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : Singleton
{
    public Vector2 ArrowKeys;
    public bool Jump;
    public bool JumpUp;
    public bool GainDoubleJump;
    public bool LoseDoubleJump;

    public bool CheatTravel;

    int _lastPressed = 1;

    ManagerKeyBinds _keyBinds;
    KeyCode _leftCode, _rightCode, _upCode, _downCode, _jumpCode, _debugGainDoubleJump, _debugLoseDoubleJump, _quit;

    private void OnEnable()
    {
        ManagerEvents.ReassignKeyCodes += AssignKeyCodes;
    }

    private void OnDisable()
    {
        ManagerEvents.ReassignKeyCodes -= AssignKeyCodes;
    }

    private void Start()
    {
        _keyBinds = Get<ManagerKeyBinds>();
        AssignKeyCodes();
    }

    void AssignKeyCodes()
    {
        _leftCode = _keyBinds.GetKey("Left");
        _rightCode = _keyBinds.GetKey("Right");
        _upCode = _keyBinds.GetKey("Up");
        _downCode = _keyBinds.GetKey("Down");
        _jumpCode = _keyBinds.GetKey("Jump");
        _quit = KeyCode.Escape;
        //_debugGainDoubleJump = KeyCode.Mouse0;
        //_debugLoseDoubleJump = KeyCode.Mouse1;
    }

    private void Update()
    {       
        if (Input.GetKeyDown(_leftCode))
            _lastPressed = -1;
        else if (Input.GetKeyDown(_rightCode))
            _lastPressed = 1;

        if (Input.GetKey(_leftCode) && Input.GetKey(_rightCode))
            ArrowKeys.x = _lastPressed;
        else if (!Input.GetKey(_leftCode) && !Input.GetKey(_rightCode))
            ArrowKeys.x = 0;
        else
            ArrowKeys.x = Input.GetKey(_leftCode) ? -1 : 1;

        ArrowKeys.y = Input.GetKey(_downCode) ? -1 : (Input.GetKey(_upCode) ? 1 : 0);

        Jump = Input.GetKeyDown(_jumpCode);
        JumpUp = Input.GetKeyUp(_jumpCode);

        GainDoubleJump = Input.GetKeyDown(_debugGainDoubleJump);
        LoseDoubleJump = Input.GetKeyDown(_debugLoseDoubleJump);

        if (Input.GetKeyDown(_quit))
            Application.Quit();

        // TO-DO: Make ManagerCheat class to manage cheats, editor only (w/ secret option to enable?)
    }
}
