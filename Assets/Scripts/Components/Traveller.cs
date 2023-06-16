using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Traveller : MonoBehaviour
{
    SpriteRenderer _rend;

    public bool Destroyed;

    private void Awake()
    {
        _rend = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        Singleton.Get<M_Travel>().CurrentTravs.Add(this);
    }

    private void Update()
    {

    }

    public void CheckDeath()
    {
        _rend.enabled = !Destroyed;
    }
}
