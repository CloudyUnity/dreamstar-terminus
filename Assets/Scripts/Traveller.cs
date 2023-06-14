using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Traveller : MonoBehaviour
{
    [SerializeField] public int ID { get; private set; }

    SpriteRenderer _rend;

    public bool Destroyed;

    private void OnValidate()
    {
        ID = gameObject.GetInstanceID();
    }

    private void Awake()
    {
        _rend = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        ID = gameObject.GetInstanceID();

        Singleton.Get<ManagerTravel>().CurrentTravs.Add(this);
    }

    private void Update()
    {
        // TEMP CODE
        if (Singleton.Get<PlayerInput>().CheatTravel)
            Singleton.Get<ManagerTravel>().RollBackTime(3);
    }

    public void CheckDeath()
    {
        _rend.enabled = !Destroyed;
    }
}
