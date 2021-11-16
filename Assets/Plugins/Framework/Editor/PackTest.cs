using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using Spenve;
using Object = UnityEngine.Object;

public class PackTest : Editor
{
    // [MenuItem("Assets/工具/打包", priority = 2051)]
    // static void ShowWindow()
    // {
    //     string outputPath = Path.Combine(Utils.OutsideAbFolder, Utils.GetPlatformFolder());
    //
    //     BuildAssetBundleOptions opt = BuildAssetBundleOptions.None;
    //
    //     opt |= BuildAssetBundleOptions.ChunkBasedCompression;
    //
    //     BuildTarget target = EditorUserBuildSettings.activeBuildTarget;
    //
    //     AssetBundleBuild[] abbs = new AssetBundleBuild[1];
    //     abbs[0].assetBundleName = "test";
    //     Object[] objs = Selection.objects;
    //     abbs[0].assetNames = new string[objs.Length];
    //     abbs[0].addressableNames = new string[objs.Length];
    //     for(int i = 0; i < objs.Length; i++)
    //     {
    //         abbs[0].assetNames[i] = AssetDatabase.GetAssetPath(objs[i]);
    //         if(objs[i].name == "Sphere")
    //         abbs[0].addressableNames[i] = objs[i].name + "1";
    //         Debug.LogWarning(AssetDatabase.GetAssetPath(objs[i]));
    //     }
    //     BuildPipeline.BuildAssetBundles(outputPath, abbs, opt, target);
    // }

    [MenuItem("Tools/清除所有BundleName", priority = 2051)]
    static void set()
    {
        string[] bds = AssetDatabase.GetAllAssetBundleNames();
        for (int i = 0; i < bds.Length; i++)
        {
            var path = AssetDatabase.GetAssetPathsFromAssetBundle(bds[i]);
            foreach (var VARIABLE in path)
            {
                AssetImporter ai = AssetImporter.GetAtPath(VARIABLE );
                ai.assetBundleName = "";
            }
        }
        AssetDatabase.Refresh();
    }
}
