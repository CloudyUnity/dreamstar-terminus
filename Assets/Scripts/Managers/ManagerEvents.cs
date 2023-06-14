using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagerEvents
{
    #region Settings
    public static event Action ReassignKeyCodes;
    public static void IvkReassignKeyCodes() => ReassignKeyCodes?.Invoke();

    public static event Action ReassignSettings;
    public static void IvkReassignSettings() => ReassignSettings?.Invoke();
    #endregion
}
