using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FocalPoint : MonoBehaviour
{
    public int Bias;
    private void Start()
    {
        Singleton.Get<M_Camera>().AddFocalPoint(this);
    }
}
