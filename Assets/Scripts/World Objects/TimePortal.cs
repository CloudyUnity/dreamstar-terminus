using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimePortal : MonoBehaviour
{
    M_Travel _travel;
    PlayerMovement _move;
    M_Transition _transition;

    [SerializeField] float _timeBack;
    [SerializeField] bool _collisionBased = false;

    private void Start()
    {
        _travel = Singleton.Get<M_Travel>();
        _move = Singleton.Get<PlayerMovement>();
        _transition = Singleton.Get<M_Transition>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!_collisionBased)
            return;

        if (collision.gameObject == _move.gameObject)
        {
            TimeTravel();
        }
    }

    public async void TimeTravel()
    {
        _move.DisableMovement();
        await _transition.TransitionAsync(inwards: true);
        await _travel.TimeTravelBackAsync(_timeBack);
        await _transition.TransitionAsync(inwards: false);
        _move.ReEnableMovement();
    }
}
