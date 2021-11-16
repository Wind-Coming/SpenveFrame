using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

public class Cap : Editor
{
    [MenuItem("Tools/Capture")]
    public static void Capt()
    {
        for (int i = 0; i < 100; i++)
        {
            string path = Application.dataPath.Replace("Assets", "cap" + i + ".png");
            if (File.Exists(path))
            {
                
            }
            else
            {
                ScreenCapture.CaptureScreenshot(path);
                break;
            }
        }
    }
}
