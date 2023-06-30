using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class M_SequencingLogger : Singleton
{
    protected override void Awake()
    {
        Debug.Log("Awake Invoked");

        base.Awake();
    }

    void OnEnable()
    {
        Debug.Log("OnEnable Invoked");
    }

    void Start()
    {
        Debug.Log("Start Invoked");
    }
}
