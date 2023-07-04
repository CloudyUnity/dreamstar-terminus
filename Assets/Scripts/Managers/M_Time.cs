using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class M_Time : Singleton
{
    public float TimePassed;
    public float TimeLeft = 60;

    UIPauseMenu _pause;
    M_Transition _trans;

    public bool InHalfTime => TimePassed >= 5 && TimePassed < 10;

    private void Start()
    {
        _pause = Get<UIPauseMenu>();
        _trans = Get<M_Transition>();
    }

    private void Update()
    {
        Time.timeScale = GetTimeScale();

        if (_pause.Paused || _trans.Transitioning)
        {
            // TODO: Change color or somethin
            return;
        }

        TimePassed += Time.deltaTime;

        if (TimePassed > TimeLeft)
        {
            //Debug.Log("Implosion");
        }
    }

    public void ReduceTime(float reduction)
    {
        TimePassed -= reduction;

        M_Events.IvkCheckDynamicTimePointChanges();
    }

    public void AddTime(float time) => TimeLeft += time;

    public float GetTimeScale()
    {
        if (_pause.Paused)
            return 0;

        if (InHalfTime)
            return 0.5f;

        return 1;
    }
}
