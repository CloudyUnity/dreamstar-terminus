using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagerEvents
{
    public static event Action ReassignKeyCodes;
    public static void IvkReassignKeyCodes() => ReassignKeyCodes?.Invoke();
}
