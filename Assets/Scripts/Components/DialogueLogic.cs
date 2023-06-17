using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Dialogue))]
public class DialogueLogic : MonoBehaviour
{
    protected Dialogue Dia;
    protected PlayerMovement Move;
    protected PlayerSystems Systems;
    protected PlayerInput Input;

    protected virtual void Start()
    {
        Dia = GetComponent<Dialogue>();
        Move = Singleton.Get<PlayerMovement>();
        Systems = Singleton.Get<PlayerSystems>();
        Input = Singleton.Get<PlayerInput>();
    }
}
