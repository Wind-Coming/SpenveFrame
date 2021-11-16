using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

#if UNITY_EDITOR
using UnityEditor;
using System.IO;
#endif

namespace Spenve
{
    [Serializable]
    public class BundleAttribute
    {
        #region 序列化数据
        public string bundleName = "bundleName";
        public List<string> paths = new List<string>();     //该bundle包含的路径
        public bool split;                                  //是否拆分为每个文件一个bundle
        public bool includeChildFolder = false;
        #endregion

        #region 运行时数据

        [NonSerialized]
        public Dictionary<string, int> resRefCount = new Dictionary<string, int>();//资源的引用计数

        #endregion

        #region 编辑器函数
#if UNITY_EDITOR
        public void RefreshFiles(AssetConfig assetConfig)
        {
            for (int k = 0; k < paths.Count; k++) {
                string fullPath = GlobalFunc.GetFullPath(paths[k]);
                var dir = new DirectoryInfo(fullPath);
                var allFiles = dir.GetFiles("*", includeChildFolder?SearchOption.AllDirectories:SearchOption.TopDirectoryOnly);
                for (var i = 0; i < allFiles.Length; ++i) {
                    var fileInfo = allFiles[i];

                    if (!fileInfo.Name.EndsWith(".meta") && !fileInfo.Directory.Name.StartsWith(".") && !fileInfo.Name.StartsWith(".")) {
                        var basePath = fileInfo.FullName.Replace('\\', '/');
                        string path = GlobalFunc.GetLocalPath(basePath);
                        string guid = AssetDatabase.AssetPathToGUID(path);
                        AssetInfo ai = null;
                        if(!assetConfig.GuidToAsset.ContainsKey(guid))
                        {
                            ai = new AssetInfo();
                            ai.address = GlobalFunc.GetFileNameWithoutExtend(path);
                            ai.guid = guid;
                            if(split)
                            {
                                ai.bundleName = ai.address;
                            }
                            else
                            {
                                ai.bundleName = bundleName;
                            }
                            assetConfig.AddAsset(ai);
                        }
                        else
                        {
                            ai = assetConfig.GuidToAsset[guid];
                            ai.address = GlobalFunc.GetFileNameWithoutExtend(path);
                            ai.guid = guid;
                            if(split)
                            {
                                ai.bundleName = ai.address;
                            }
                            else
                            {
                                ai.bundleName = bundleName;
                            }
                        }
                    }
                }
            }
        }

        // public void SetBundleName()
        // {
        //     AssetDatabase.RemoveUnusedAssetBundleNames();
        //
        //     for (int k = 0; k < paths.Count; k++) {
        //         string fullPath = GlobalFunc.GetFullPath(paths[k]);
        //         var dir = new DirectoryInfo(fullPath);
        //         var allFiles = dir.GetFiles("*", SearchOption.TopDirectoryOnly);
        //         for (var i = 0; i < allFiles.Length; ++i) {
        //             EditorUtility.DisplayProgressBar("设置AssetName名称", bundleName, k * 1.0f / allFiles.Length);
        //             var fileInfo = allFiles[i];
        //
        //             if (!fileInfo.Name.EndsWith(".meta")) {
        //                 var basePath = fileInfo.FullName.Replace('\\', '/');
        //                 string path = GlobalFunc.GetLocalPath(basePath);
        //                 var importer = AssetImporter.GetAtPath(path);
        //                 if (split || basePath.EndsWith(".unity")) {//设置为单独打包的或者场景文件
        //                     importer.assetBundleName = GlobalFunc.GetFileNameWithoutExtend(basePath).ToLower();
        //                 }
        //                 else {
        //                     importer.assetBundleName = bundleName;
        //                 }
        //             }
        //         }
        //     }
        //
        //     EditorUtility.ClearProgressBar();
        // }
        //
        // public void ClearAllBundleName()
        // {
        //     for (int k = 0; k < paths.Count; k++) {
        //         ClearPathBundleName(paths[k]);
        //     }
        // }
        //
        //
        // public void ClearPathBundleName(string bpath)
        // {
        //     string fullPath = GlobalFunc.GetFullPath(bpath);
        //     var dir = new DirectoryInfo(fullPath);
        //     var allFiles = dir.GetFiles("*", SearchOption.TopDirectoryOnly);
        //     for (var i = 0; i < allFiles.Length; ++i)
        //     {
        //         EditorUtility.DisplayProgressBar("设置AssetName名称", bundleName, i * 1.0f / allFiles.Length);
        //         var fileInfo = allFiles[i];
        //
        //         if (!fileInfo.Name.EndsWith(".meta"))
        //         {
        //             var basePath = fileInfo.FullName.Replace('\\', '/');
        //             string path = GlobalFunc.GetLocalPath(basePath);
        //             var importer = AssetImporter.GetAtPath(path);
        //             importer.assetBundleName = null;
        //         }
        //     }
        //
        //     EditorUtility.ClearProgressBar();
        // }
        //
        // public void RefreshAndSetAbName()
        // {
        //     RefreshFiles();
        //     SetBundleName(); 
        // }
#endif

        #endregion 
    }

    [Serializable]
    [CreateAssetMenu(menuName = "Config/Create BundleConfig Asset")]
    public class BundleConfig : ScriptableObject
    {
        private static BundleConfig _instance;

        public static BundleConfig Instance
        {
            get
            {
                if (null == _instance) {
                    Init();
                }

                return _instance;
            }
        }

        public static List<BundleAttribute> Bundles
        {
            get
            {
                return Instance.allBundle;
            }
        }

        private static void Init()
        {
#if UNITY_EDITOR
            //if (!EditorApplication.isPlaying) {
                string xmlPath = "Assets/Art/Config/BundleConfig.asset";
                string folder = Application.dataPath +  "/Art/Config";
                _instance = AssetDatabase.LoadAssetAtPath<BundleConfig>(xmlPath);
                if(_instance == null)
                {
                    if (!Directory.Exists(folder))
                    {
                        Directory.CreateDirectory(folder);
                    }
                    _instance = BundleConfig.CreateInstance<BundleConfig>();
                    AssetDatabase.CreateAsset(_instance, xmlPath);
                    AssetDatabase.Refresh();
                }
            //}
#endif
        }

        public List<BundleAttribute> allBundle = new List<BundleAttribute>();
    }
}