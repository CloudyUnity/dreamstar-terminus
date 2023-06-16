using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;

public class DynamicTime : MonoBehaviour
{
    M_Time _time;

    [Serializable]
    public struct TimePoint
    {
        public float Time;
        public bool FlipBool;
        public UnityEvent<bool> Method;
        [HideInInspector] public bool Active;
    }

    [SerializeField] TimePoint[] TimePoints;

    private void OnEnable()
    {
        M_Events.CheckTimePoints += CheckTimePoints;
    }

    private void OnDisable()
    {
        M_Events.CheckTimePoints -= CheckTimePoints;
    }

    private void Start()
    {
        _time = Singleton.Get<M_Time>();
    }

    private void Update()
    {
        for (int i = 0; i < TimePoints.Length; i++)
        {
            if (_time.TimePassed > TimePoints[i].Time && !TimePoints[i].Active)
            {
                TimePoints[i].Method.Invoke(!TimePoints[i].FlipBool); // Default: true
                TimePoints[i].Active = true;
            }
        }
    }

    void CheckTimePoints()
    {
        for (int i = 0; i < TimePoints.Length; i++)
        {
            if (TimePoints[i].Active && _time.TimePassed < TimePoints[i].Time)
            {
                TimePoints[i].Method.Invoke(TimePoints[i].FlipBool); // Default: false
                TimePoints[i].Active = false;
            }
        }
    }
}
