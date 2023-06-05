using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : Singleton
{
    public Vector2 ArrowKeys;
    public bool Jump;
    public bool JumpUp;

    public bool CheatTravel;

    int _lastPressed = 1;

    private void Update()
    {
        KeyCode leftCode = KeyCode.A;
        KeyCode rightCode = KeyCode.D;
        //KeyCode downCode = KeyCode.S;
        //KeyCode upCode = KeyCode.W;
       
        if (Input.GetKeyDown(leftCode))
            _lastPressed = -1;
        else if (Input.GetKeyDown(rightCode))
            _lastPressed = 1;

        if (Input.GetKey(leftCode) && Input.GetKey(rightCode))
            ArrowKeys.x = _lastPressed;
        else if (!Input.GetKey(leftCode) && !Input.GetKey(rightCode))
            ArrowKeys.x = 0;
        else
            ArrowKeys.x = Input.GetKey(leftCode) ? -1 : 1;

        ArrowKeys.y = Input.GetAxisRaw("Vertical");

        KeyCode jumpCode = KeyCode.Mouse0;
        Jump = Input.GetKeyDown(jumpCode);
        JumpUp = Input.GetKeyUp(jumpCode);

        KeyCode cheatTravel = KeyCode.P;
        CheatTravel = Input.GetKeyDown(cheatTravel);
    }
}
