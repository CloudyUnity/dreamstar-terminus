using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider2D))]
public class Interactable : MonoBehaviour
{
    PlayerInput _input;
    [SerializeField] UnityEvent<object> _event;
    [SerializeField] bool _reusable;
    bool _used;


    private void Start()
    {
        _input = Singleton.Get<PlayerInput>();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (_used && !_reusable)
            return;

        if (_input.Interact)
        {
            _event.Invoke(null);
            _used = true;
        }
    }
}
