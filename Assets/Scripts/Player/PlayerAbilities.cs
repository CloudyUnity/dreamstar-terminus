using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAbilities : Singleton
{
    public bool WallJumpOn;
    public int DoubleJumps;
    public bool PogoOn;

    PlayerInput _input;

    private void Start()
    {
        _input = Get<PlayerInput>();
    }
}
