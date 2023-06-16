using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Debug_Curver : MonoBehaviour
{
    void Update()
    {
        Vector2 pos = transform.position;
        pos.x += Time.deltaTime;
        pos.y = M_Extensions.HumpCurveV2(pos.x, 1);
        transform.position = pos;
    }
}
