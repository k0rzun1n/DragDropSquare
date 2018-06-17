using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class Settings
{
    public float circleTimeoutMin;
    public float circleTimeoutMax;
    [SerializeField]
    private List<string> colorStrings;
    public string asd;
    public List<Color> getColors()
    {
        var ret = new List<Color>(colorStrings.Count);
        foreach (var cs in colorStrings)
        {
            Color col = new Color();
            if (ColorUtility.TryParseHtmlString(cs, out col))
                ret.Add(col);
        }
        return ret;
    }
}
