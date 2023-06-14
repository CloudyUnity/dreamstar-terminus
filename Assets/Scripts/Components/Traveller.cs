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
#if UNITY_EDITOR
        return;
#endif
        // TEMP CODE
#pragma warning disable CS0162 // Unreachable code detected
        if (Singleton.Get<PlayerInput>().CheatTravel)
            Singleton.Get<ManagerTravel>().RollBackTime(3);
#pragma warning restore CS0162 // Unreachable code detected
    }

    public void CheckDeath()
    {
        _rend.enabled = !Destroyed;
    }
}
