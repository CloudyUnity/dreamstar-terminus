using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class M_Time : Singleton
{
    public float TimePassed;
    public float TimeLeft = 60;

    UIPauseMenu _pause;

    private void Start()
    {
        _pause = Get<UIPauseMenu>();
    }

    private void Update()
    {
        if (_pause.Paused)
        {
            // TODO: Change color or somethin
            return;
        }

        TimePassed += Time.deltaTime;

        if (TimePassed > TimeLeft)
        {
            Debug.Log("Implosion");
        }
    }

    public void ReduceTime(float reduction)
    {
        TimePassed -= reduction;

        M_Events.IvkCheckDynamicTimePointChanges();
    }

    public void AddTime(float time) => TimeLeft += time;
}
