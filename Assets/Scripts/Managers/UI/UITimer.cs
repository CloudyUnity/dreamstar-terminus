using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UITimer : Singleton
{
    [SerializeField] TMP_Text _text;

    M_Time _time;

    private void Start()
    {
        _time = Get<M_Time>();
    }

    private void Update()
    {
        _text.text = Mathf.Round(_time.TimePassed).ToString();
    }
}
