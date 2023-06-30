using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : Singleton
{
    public Vector2 ArrowKeys;
    public Vector2 ArrowKeysUnRaw;
    public bool Jump, JumpUp, Attack, Interact, CheatTravel;

    public int LastPressed = 1;

    public InputControls Controls;

    PlayerMovement _move;

    private void OnEnable()
    {
        Controls = new InputControls();
        Controls.Gameplay.Enable();
    }

    private void OnDisable()
    {
        if (Controls != null)
            Controls.Gameplay.Disable();
    }

    private void Start()
    {
        _move = Get<PlayerMovement>();
    }

    private void Update()
    {
        if (Controls.Gameplay.Pause.triggered)
            Get<UIPauseMenu>().CloseChainMenu();

        if (_move.MovementDisabled)
        {
            ArrowKeys = Vector2.zero;
            ArrowKeysUnRaw = Vector2.zero;
            Jump = false;
            JumpUp = false;
            Attack = false;
            Interact = false;
            CheatTravel = false;
            return;
        }

        if (Controls.Gameplay.QuickRestart.triggered && Time.timeSinceLevelLoad >= 1)
            Get<M_World>().Restart();

        #region ARROWKEYS
        if (Controls.Gameplay.Left.triggered)
            LastPressed = -1;
        else if (Controls.Gameplay.Right.triggered)
            LastPressed = 1;

        ArrowKeys = MovementArrowKeys();

        ArrowKeysUnRaw.y = Controls.Gameplay.Up.ReadValue<float>() - Controls.Gameplay.Down.ReadValue<float>();
        ArrowKeysUnRaw.x = Controls.Gameplay.Right.ReadValue<float>() - Controls.Gameplay.Left.ReadValue<float>();
        #endregion

        Jump = Controls.Gameplay.Jump.triggered;
        JumpUp = Controls.Gameplay.Jump.WasReleasedThisFrame();

        Attack = Controls.Gameplay.Attack.triggered;

        Interact = Controls.Gameplay.Interact.triggered;

        // TO-DO: Make ManagerCheat class to manage cheats, editor only (w/ secret option to enable?)
    }

    Vector2 MovementArrowKeys()
    {
        bool left = Controls.Gameplay.Left.ReadValue<float>() > 0;
        bool right = Controls.Gameplay.Right.ReadValue<float>() > 0;
        bool up = Controls.Gameplay.Up.ReadValue<float>() > 0;
        bool down = Controls.Gameplay.Down.ReadValue<float>() > 0;

        Vector2 result = Vector2.zero;

        if (left && right)
            result.x = LastPressed;
        else if (!left && !right)
            result.x = 0;
        else
            result.x = left ? -1 : 1;

        result.y = down ? -1 : (up ? 1 : 0);

        return result;
    }
}
