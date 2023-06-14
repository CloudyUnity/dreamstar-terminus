using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class OpenLink : MonoBehaviour
{
    public void OpenLinkFromURL(string url)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        OpenTab(url);
#endif
    }

    [DllImport("__Internal")]
    private static extern void OpenTab(string url);
}
