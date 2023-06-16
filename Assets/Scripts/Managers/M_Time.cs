using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class M_Time : Singleton
{
    public float TimePassed;
    public float TimeLeft = 60;

    private void Update()
    {
        TimePassed += Time.deltaTime;

        if (TimePassed > TimeLeft)
        {
            Debug.Log("Implosion");
        }
    }

    public void TimeTravelled(float reduction)
    {
        TimePassed -= reduction;

        M_Events.IvkCheckTimePoints();
    }

    public void AddTime(float time) => TimeLeft += time;
}
