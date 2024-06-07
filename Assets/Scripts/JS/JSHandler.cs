using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

public class JSHandler : MonoBehaviour
{
    [DllImport("__Internal")]
    private static extern string GetAuthToken();
    [DllImport("__Internal")]
    private static extern bool IsFullscreen();
    [DllImport("__Internal")]
    private static extern void RequestFullscreen();

    internal string RetrieveAuthToken()
    {
        string authToken = GetAuthToken();
        Debug.Log("Auth Token: " + authToken);
        return authToken;
    }

    private void Update()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        if (IsFullscreen())
        {
            Debug.Log("Running in fullscreen mode");
        }
        else
        {
            RequestFullscreen();
            Debug.Log("Not running in fullscreen mode");
        }
#endif
    }
}
