using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Traveller : MonoBehaviour
{
    private void Start()
    {
        Singleton.Get<TravelManager>().CurrentTravs.Add(this);
    }

    private void Update()
    {
        // TEMP CODE
        if (Singleton.Get<PlayerInput>().CheatTravel)
            Singleton.Get<TravelManager>().RollBackTime(5);
    }
}
