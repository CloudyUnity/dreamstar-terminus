using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC_Test : DialogueLogic
{
    private void Update()
    {
        if (Input.Jump && Dia.ReadDefault)
            Dia.PlayDialogue("Jumped");
    }
}
