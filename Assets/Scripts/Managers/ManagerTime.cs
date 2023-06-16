using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagerTime : Singleton
{
    public float TimePassed;
    public float TimeLeft = 60;

    private void Update()
    {
        TimePassed += Time.deltaTime;
        Debug.Log(TimePassed);

        if (TimePassed > TimeLeft)
        {
            Debug.Log("Implosion");
        }
    }

    void TimeTravelled(float reduction)
    {
        TimePassed -= reduction;
    }

    public void AddTime(float time) => TimeLeft += time;
}
