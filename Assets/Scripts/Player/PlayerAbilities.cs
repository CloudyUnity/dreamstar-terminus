using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAbilities : Singleton
{
    public bool WallJumpOn;
    public int DoubleJumps;
    /* Ability Ideas:
     * 
     * Time-Slow
     * Ground former
     * Double jump
     * Dash
     * Grappling Hook
     * Gravity Swapper
     * Ascend/Descend
     * Glide
     * 10 Second Time Machine
     */

    PlayerInput _input;

    private void Start()
    {
        _input = Get<PlayerInput>();
    }

    private void Update()
    {
        if (_input.GainDoubleJump)
            DoubleJumps++;

        if (_input.LoseDoubleJump)
            DoubleJumps--;
    }
}
