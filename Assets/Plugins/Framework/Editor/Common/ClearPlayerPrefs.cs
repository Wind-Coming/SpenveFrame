using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ClearPlayerPrefs : Editor
{
    [MenuItem("Tools/清除缓存数据")]
    public static void ClearAllPlayerPre()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
    }    
}
