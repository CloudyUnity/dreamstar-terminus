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

    InputControls _controls;

    private void OnEnable()
    {
        _controls = new InputControls();
        _controls.Gameplay.Enable();
    }

    private void OnDisable()
    {
        _controls.Gameplay.Disable();
    }

    private void Update()
    {
        if (_controls.Gameplay.Left.triggered)
            _lastPressed = -1;
        else if (_controls.Gameplay.Right.triggered)
            _lastPressed = 1;

        bool left = _controls.Gameplay.Left.ReadValue<float>() > 0;
        bool right = _controls.Gameplay.Right.ReadValue<float>() > 0;
        bool up = _controls.Gameplay.Up.ReadValue<float>() > 0;
        bool down = _controls.Gameplay.Down.ReadValue<float>() > 0;

        if (left && right)
            ArrowKeys.x = _lastPressed;
        else if (!left && !right)
            ArrowKeys.x = 0;
        else
            ArrowKeys.x = left ? -1 : 1;

        ArrowKeys.y = down ? -1 : (up ? 1 : 0);

        Jump = _controls.Gameplay.Jump.triggered;
        JumpUp = _controls.Gameplay.Jump.WasReleasedThisFrame();

        if (_controls.Gameplay.Pause.triggered)
            Application.Quit();

        // TO-DO: Make ManagerCheat class to manage cheats, editor only (w/ secret option to enable?)
    }
}
