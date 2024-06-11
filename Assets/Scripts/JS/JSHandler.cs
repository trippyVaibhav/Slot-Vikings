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

    internal void RetrieveAuthToken(string cookieName, Action<string> callback)
    {
        IntPtr tokenPtr = IntPtr.Zero;
        try
        {
            tokenPtr = GetAuthToken(cookieName);
        }
        catch (Exception ex)
        {
            Debug.LogError("Exception while calling GetAuthToken: " + ex.Message);
            callback(null);
            return;
        }

        if (tokenPtr != IntPtr.Zero)
        {
            try
            {
                string token = Marshal.PtrToStringUTF8(tokenPtr);
                Debug.Log("Token successfully retrieved and converted: " + token);
                FreeMemory(tokenPtr);
                callback(token);
            }
            catch (Exception ex)
            {
                Debug.LogError("Exception while converting token pointer to string: " + ex.Message);
                callback(null);
            }
        }
        else
        {
            Debug.LogWarning("Token not found. Pointer is null.");
            callback(null);
        }
    }


    [DllImport("__Internal")]
    private static extern void _free(IntPtr ptr);

    private void FreeMemory(IntPtr ptr)
    {
        try
        {
            _free(ptr);
            Debug.Log("Memory successfully freed.");
        }
        catch (Exception ex)
        {
            Debug.LogError("Exception while freeing memory: " + ex.Message);
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
