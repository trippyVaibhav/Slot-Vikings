using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

public class JSHandler : MonoBehaviour
{
    private System.Action<string> authTokenCallback;
    private System.Action<bool> isFullScreenCallback;

    // Call this method to retrieve the auth token
    internal void RetrieveAuthToken(System.Action<string> callback)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        authTokenCallback = callback;
        GetAuthToken();
#else
        Debug.LogWarning("GetAuthToken can only be called in a WebGL build.");
        callback?.Invoke(null); // Invoke callback with null value
#endif
    }

    private void Awake()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        StartCoroutine(StartFullScreenCheck());
#endif
    }

    private IEnumerator StartFullScreenCheck()
    {
        while (true)
        {
        IsFullscreen((authToken) =>
        {
            if (!authToken)
            {
                Debug.Log("Already full screen");
            }
            else
            {
                Debug.Log("Not full screen");
                RequestFullscreen();
            }
        });
    }
}

    private void IsFullscreen(System.Action<bool> callback)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        isFullScreenCallback = callback;
        GetFullScreenCheck();
#endif
    }

    private void RequestFullscreen()
    {
        // Call JavaScript function from C# to request fullscreen
        Application.ExternalEval("RequestFullscreen();");
    }

    // This method will be called asynchronously from JavaScript
    private void ReceiveAuthToken(string authToken)
    {
        Debug.Log("Auth Token: " + authToken);
        authTokenCallback?.Invoke(authToken);
    }

    [System.Obsolete]
    private void GetAuthToken()
    {
        // Call JavaScript function from C#
        Application.ExternalEval("GetAuthToken(callbackFunction);");
    }

    [System.Obsolete]
    private void GetFullScreenCheck()
    {
        // Call JavaScript function from C#
        Application.ExternalEval("IsFullscreen(callbackFunction);");
    }
}
