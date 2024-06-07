using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using System;

public class JSHandler : MonoBehaviour
{
    [DllImport("__Internal")]
    private static extern IntPtr GetAuthToken(string cookieName);
    [DllImport("__Internal")]
    private static extern int IsFullScreen();
    [DllImport("__Internal")]
    private static extern void ForceFullScreen();

    internal string RetrieveAuthToken(string cookieName)
    {
        IntPtr tokenPtr = GetAuthToken(cookieName);

        // If the token is not null, convert it to a C# string
        if (tokenPtr != IntPtr.Zero)
        {
            string token = Marshal.PtrToStringUTF8(tokenPtr);
            return token;
        }
        else
        {
            Debug.Log("Token not found");
            return null;
        }
    }

    private void Update()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        bool isFullScreen = IsFullScreen() == 1;

        if(isFullScreen)
        {
            ForceFullScreen();
            Debug.Log("Is Fullscreen: " + isFullScreen);
        }
        else
        {
            Debug.Log("Is not Fullscreen: " + isFullScreen);
        }
#endif
    }
}
