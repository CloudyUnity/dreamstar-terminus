using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider2D))]
public class Interactable : MonoBehaviour
{
    public static Interactable MainInteraction;

    PlayerInput _input;
    [SerializeField] UnityEvent<object> _event;
    [SerializeField] bool _reusable;
    SpriteRenderer _rend;
    M_Materials _mats;
    bool _used;

    private void Start()
    {
        _input = Singleton.Get<PlayerInput>();
        _mats = Singleton.Get<M_Materials>();
        _rend = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        _rend.material = MainInteraction == this ? _mats.Outline : _mats.SpriteLit;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        MainInteraction = this;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (MainInteraction == this)
            MainInteraction = null;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (MainInteraction == null)
            MainInteraction = this;

        if (MainInteraction != this)
            return;

        if (_used && !_reusable)
            return;

        if (_input.Interact)
        {
            _event.Invoke(null);
            _used = true;
        }
    }
}
