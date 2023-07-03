using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class M_LayerMasks
{
    public static LayerMask Ground = 1 << 7;
    public static LayerMask Entity = 1 << 8;
    public static LayerMask All = 1 << 0 | 1 << 1 | 1 << 2 | 1 << 3 | 1 << 4 | 1 << 5 | 1 << 6 | 1 << 7 | 1 << 8 | 1 << 9 |
        1 << 10 | 1 << 11 | 1 << 12 | 1 << 13 | 1 << 14 | 1 << 15;
}
