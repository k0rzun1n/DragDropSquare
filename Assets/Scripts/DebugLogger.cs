using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugLogger : Singleton<DebugLogger>
{
    private Text mText;
    public static void Log(string msg)
    {
        if (Instance.mText != null)
            Instance.mText.text += msg + " ";
        else
            Debug.Log(msg);
    }
    void Awake()
    {
        // mText = GameObject.Find("ScreenDebugLogger")?.GetComponent<Text>();
        var go = GameObject.Find("ScreenDebugLogger");
        if (go == null) return;
        mText = go.GetComponent<Text>();

    }
    protected DebugLogger() { }

}
