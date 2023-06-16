using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimePortal : MonoBehaviour
{
    M_Travel _travel;
    PlayerMovement _move;

    [SerializeField] float _timeBack;

    private void Start()
    {
        _travel = Singleton.Get<M_Travel>();
        _move = Singleton.Get<PlayerMovement>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject == _move.gameObject)
        {
            _move.DisableMovement();
            _travel.RollBackTime(_timeBack);
            _move.ReEnableMovement();
        }
    }
}
