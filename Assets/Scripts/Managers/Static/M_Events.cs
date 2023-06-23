using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class M_Events
{
    #region SETTINGS
    public static event Action ReassignKeyCodes;
    public static void IvkReassignKeyCodes() => ReassignKeyCodes?.Invoke();

    public static event Action ReassignSettings;
    public static void IvkReassignSettings() => ReassignSettings?.Invoke();
    #endregion

    #region GAMEPLAY
    public static event Action CheckTimePoints;
    public static void IvkCheckTimePoints() => CheckTimePoints?.Invoke();
    #endregion

    #region SEQUENCING
    public static event Action SingletonRecieve;
    public static void IvkRecieveSingleton() => SingletonRecieve?.Invoke();

    public static event Action SingletonAssign;
    public static void IvkAssignSingletons() => SingletonAssign?.Invoke();
    #endregion
}
