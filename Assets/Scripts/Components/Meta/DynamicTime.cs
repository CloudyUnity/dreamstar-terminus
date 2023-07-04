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
        public bool FalseForwards;
        public UnityEvent<bool> Method;
        [HideInInspector] public bool Active;
    }

    [SerializeField] TimePoint[] TimePoints;

    Vector2 _startPos;
    [SerializeField] Vector2 _endPos;

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

        _startPos = transform.position;
    }

    private void Update()
    {
        for (int i = 0; i < TimePoints.Length; i++)
        {
            if (_time.TimePassed > TimePoints[i].Time && !TimePoints[i].Active)
            {
                TimePoints[i].Method.Invoke(!TimePoints[i].FalseForwards); // Default: true
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
                TimePoints[i].Method.Invoke(TimePoints[i].FalseForwards); // Default: false
                TimePoints[i].Active = false;
            }
        }
    }

    // Parameter must be 'Dynamic bool'
    #region CUSTOM HELPER EVENTS
    public void LerpTo(bool inwards)
    {
        if (!inwards)
        {
            transform.position = _startPos;
            return;
        }

        StartCoroutine(C_LerpTo());
    }

    IEnumerator C_LerpTo()
    {
        float elapsed = 0;
        float dur = 0.5f;

        while (elapsed < dur)
        {
            float curved = M_Extensions.CosCurve(elapsed / dur);

            transform.position = Vector2.Lerp(_startPos, _endPos, curved);
            elapsed += Time.deltaTime;
            yield return null;
        }
    }
    #endregion
}
