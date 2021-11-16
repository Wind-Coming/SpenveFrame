using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using AssetBundleBrowser.AssetBundleDataSource;
using Spenve;

#pragma warning disable 0219  

namespace AssetBundleBrowser
{
    [System.Serializable]
    internal class AssetBundleSettingTab
    {
        AssetBundleSettingTab m_InspectTab;
        Vector2 m_ScorllPosition;
        BundleConfig m_kBundleConfig;

        internal AssetBundleSettingTab()
        {
        }

        internal void OnEnable(Rect subPos, EditorWindow parent)
        {

        }

        internal void OnEnable(EditorWindow parent)
        {
            m_InspectTab = (parent as AssetBundleBrowserMain).m_BundleSettingTab;
            m_kBundleConfig = BundleConfig.Instance;
        }

        internal void OnDisable()
        {

        }

        public void OnGUI(Rect rect)
        {
            m_ScorllPosition = EditorGUILayout.BeginScrollView(m_ScorllPosition, false, true);

            GUILayout.Space(5);

            int _height = 30;
            int _butW = 20;
            int _toggleW = 120; 
            int _floderW = 160;
            int _pathfW = 300;

            float y = rect.y;

            EditorGUILayout.LabelField("自动设置Address的配置，配置文件夹后，可以批量设置address");
            EditorGUILayout.Space();

            Undo.RecordObject(m_kBundleConfig, "");
            List<BundleAttribute> bundles = m_kBundleConfig.allBundle;

            for (int i = 0; i < bundles.Count; i++) {
                BundleAttribute info = bundles[i];
                EditorGUILayout.BeginHorizontal();


                info.bundleName = GUILayout.TextArea(info.bundleName, GUILayout.Width(_floderW), GUILayout.Height(_height));

                GUILayout.Space(_butW);
                if (GUILayout.Button("+添加文件夹", GUILayout.Width(80), GUILayout.Height(_height))) {
                    string newPath = EditorUtility.OpenFolderPanel("Select a  folder .", "Assets/", "");
                    if (!string.IsNullOrEmpty(newPath)) {
                        int x2 = newPath.IndexOf("Assets");

                        string realPath = newPath.Substring(x2);
                        info.paths.Add(realPath);
                    }
                }

                GUILayout.Space(_butW);
                info.split = GUILayout.Toggle(info.split, "每个文件独立打包",GUILayout.Width(_toggleW), GUILayout.Height(_height));

                info.includeChildFolder = GUILayout.Toggle(info.includeChildFolder, "是否包含子文件夹",GUILayout.Width(_toggleW), GUILayout.Height(_height));

                if (GUILayout.Button("-", GUILayout.Width(_butW), GUILayout.Height(_height))) { 
                    //info.ClearAllBundleName();
                    bundles.RemoveAt(i);
                    break;
                }

                EditorGUILayout.EndHorizontal();


                for (int j = 0; j < info.paths.Count; j++) {
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Space(_floderW);
                    GUILayout.Space(_butW);

                    info.paths[j] = GUILayout.TextArea(info.paths[j], GUILayout.Width(300), GUILayout.Height(_height));

                    if (GUILayout.Button("-", GUILayout.Width(_butW), GUILayout.Height(_height))) {
                        //info.ClearPathBundleName(info.paths[j]);
                        info.paths.RemoveAt(j);
                        break;
                    }

                    EditorGUILayout.EndHorizontal();
                }


                GUILayout.Space(10);
                GUIStyle colorStyle = new GUIStyle();
                colorStyle.normal.textColor = new Color(0, 0.6f, 0);
                GUILayout.Label("____________________________________________________________________________________________________________________________", colorStyle);
            }



            GUILayout.Space(20);

            EditorGUILayout.BeginHorizontal();

            int addButW = 100;
            int saveXmlButW = 100;
            int resetAllButW = 100;

            int rExpand = 40;

            if (GUILayout.Button(" + ", GUILayout.Width(addButW), GUILayout.Height(_height))) {
                BundleAttribute ba = new BundleAttribute();
                bundles.Add(ba);
            }

            GUILayout.Space(200);

            if (GUILayout.Button("刷新文件并保存", GUILayout.Width(saveXmlButW), GUILayout.Height(_height))) {
                RefreshFiles();
                EditorUtility.SetDirty(BundleConfig.Instance);
                AssetDatabase.SaveAssets();
            }
            
            GUILayout.Space(rExpand);

            // if (GUILayout.Button("SetFileAbName", GUILayout.Width(resetAllButW), GUILayout.Height(_height))) {
            //     SetBundleName();
            // }
            //

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndScrollView();

        }


        public static void RefreshFiles()
        {
            AssetConfig assetConfig = AssetDatabase.LoadAssetAtPath<AssetConfig>("Assets/Art/Config/AssetConfig.asset");
            if (assetConfig == null)
            {
                string xmlPath = "Assets/Art/Config/AssetConfig.asset";
                string folder = Application.dataPath + "/Art/Config";
                if (!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                }
                assetConfig = BundleConfig.CreateInstance<AssetConfig>();
                AssetDatabase.CreateAsset(assetConfig, xmlPath);
                AssetDatabase.Refresh();
            }

            assetConfig.ClearAll();
            
            // List<AssetInfo> nilGuid = new List<AssetInfo>();
            // List<AssetInfo> all = assetConfig.GetAll();
            //
            // //检测重复资源
            // for (int i = 0; i < all.Count; i++)
            // {
            //     AssetInfo ai = all[i];
            //     for (int j = i + 1; j < all.Count; j++)
            //     {
            //         AssetInfo aj = all[j];
            //         if (ai.address == aj.address || ai.guid == aj.guid)
            //         {
            //             Debug.LogError("发现重复address或者重复guid,已经做删除处理!:" + aj.address);
            //             nilGuid.Add(aj);
            //         }
            //     }
            // }
            //
            // //检测已经删除的资源
            // foreach (var v in all)
            // {
            //     string path = AssetDatabase.GUIDToAssetPath( v.guid );
            //     if (string.IsNullOrEmpty(path))
            //     {
            //         nilGuid.Add(v);
            //     }
            //     else
            //     {
            //         var importer = AssetImporter.GetAtPath(path);
            //         if (importer == null)
            //         {
            //             nilGuid.Add(v);
            //         }
            //     }
            // }
            //
            // foreach (var VARIABLE in nilGuid)
            // {
            //     if (!AssetConfig.Instance.RemoveGuid(VARIABLE.guid))
            //     {
            //         AssetConfig.Instance.RemoveAddress(VARIABLE.address);
            //     }
            // }

            List<BundleAttribute> bundles = BundleConfig.Instance.allBundle;
            for (int i = 0; i < bundles.Count; i++) {
                BundleAttribute info = bundles[i];
                info.RefreshFiles(assetConfig);
            }
            Debug.Log("刷新文件成功！"); 
            
            EditorUtility.SetDirty(assetConfig);
            AssetDatabase.SaveAssets();
        }

        public static void SetBundleName()
        {
            AssetDatabase.RemoveUnusedAssetBundleNames();

            int num = 0;
            List<AssetInfo> all = AssetConfig.Instance.GetAll();

            foreach (var v in all)
            {
                string path = AssetDatabase.GUIDToAssetPath( v.guid );
                var importer = AssetImporter.GetAtPath(path);
                if (importer != null)
                {
                    importer.assetBundleName = v.bundleName;
                }
                else
                {
                    Debug.Log(path + " 不存在");
                }

                num ++;
                EditorUtility.DisplayProgressBar("设置AssetName名称", v.address, num * 1.0f / all.Count);
            }
            EditorUtility.ClearProgressBar();
            Debug.Log("设置文件BundleName成功！");
        }

        [MenuItem("资源工具/刷新设置x _%_q", priority = 2051)]
        static void ShowWindow()
        {
            Debug.LogError("不用再手动刷新了，如果新加了文件夹请到资源工具->资源配置打包窗口->BundleSetting里配置");
            //RefreshFiles();
            //暂时不设置bundlename
            //SetBundleName();
        }

    }
}

#pragma warning restore 0219
