using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Spenve;

namespace AssetBundleBrowser
{
    using Object = UnityEngine.Object;
    
    public static class CommonStrings
    {
        public const string UnityEditorResourcePath = "library/unity editor resources";
        public const string UnityDefaultResourcePath = "library/unity default resources";
        public const string UnityBuiltInExtraPath = "resources/unity_builtin_extra";
        public const string AssetBundleNameFormat = "archive:/{0}/{0}";
        public const string SceneBundleNameFormat = "archive:/{0}/{1}.sharedAssets";
    }

    [InitializeOnLoad]
    static class AddressableAssetInspectorGUI
    {
        static GUIContent s_AddressableAssetToggleText;

        static AddressableAssetInspectorGUI()
        {
            s_AddressableAssetToggleText = new GUIContent("Addressable", "Check this to mark this asset as an Addressable Asset, which includes it in the bundled data and makes it loadable via script by its address.");
            Editor.finishedDefaultHeaderGUI += OnPostHeaderGUI;
        }

        static void SetAaEntry(AssetConfig aaSettings, Object target, bool create)
        {
            Undo.RecordObject(aaSettings, "AddressableAssetSettings");
            if(create)
            {
                AssetInfo ai = new AssetInfo();
                string path = AssetDatabase.GetAssetPath(target);
                ai.address = GlobalFunc.GetFileNameWithoutExtend(path);
                ai.guid = AssetDatabase.AssetPathToGUID(path);
                ai.bundleName = "default";
                aaSettings.AddAsset(ai);
            }
            else
            {
                string path = AssetDatabase.GetAssetPath(target);
                string guid = AssetDatabase.AssetPathToGUID(path);
                if (!aaSettings.RemoveGuid(guid))
                {
                    aaSettings.RemoveAddress(GlobalFunc.GetFileNameWithoutExtend(path));
                }
            }
            
            EditorUtility.SetDirty(aaSettings);
        }

        internal static bool GetPathAndGUIDFromTarget(Object t, out string path, ref string guid, out Type mainAssetType)
        {
            mainAssetType = null;
            path = AssetDatabase.GetAssetOrScenePath(t);
            if (!IsPathValidForEntry(path))
                return false;
            guid = AssetDatabase.AssetPathToGUID(path);
            if (string.IsNullOrEmpty(guid))
                return false;
            mainAssetType = AssetDatabase.GetMainAssetTypeAtPath(path);
            if (mainAssetType != t.GetType() && !typeof(AssetImporter).IsAssignableFrom(t.GetType()))
                return false;
            return true;
        }
        
        internal static bool IsPathValidForEntry(string path)
        {
            if (string.IsNullOrEmpty(path))
                return false;
            path = path.ToLower();
            if (path == CommonStrings.UnityEditorResourcePath ||
                path == CommonStrings.UnityDefaultResourcePath ||
                path == CommonStrings.UnityBuiltInExtraPath)
                return false;
            return true;
        }

        static void OnPostHeaderGUI(Editor editor)
        {
            AssetConfig aaSettings = AssetConfig.Instance;
            var guid = string.Empty;

            if (editor.targets.Length == 1)
            {
                string path;
                Type mainAssetType;
                if (!GetPathAndGUIDFromTarget(editor.targets[0], out path, ref guid, out mainAssetType))
                {
                    return;
                }
                    
                AssetInfo ai = null;
                AssetConfig.Instance.GuidToAsset.TryGetValue(guid, out ai);

                if (ai != null)
                {
                    GUILayout.BeginHorizontal();
                    if (!GUILayout.Toggle(true, s_AddressableAssetToggleText, GUILayout.ExpandWidth(false)))
                    {
                        Debug.LogError("不要手动操作单个文件的Address了，请到BundleSetting里配自动设置路径！");
                        //SetAaEntry(aaSettings, editor.targets[0], false);//不让手动操作了，傻瓜式的用buildConfig设置
                    }
                    EditorGUILayout.DelayedTextField(ai.address, GUILayout.ExpandWidth(true));//不让自己改了，免得出错
                    GUILayout.EndHorizontal();
                }
                else
                {
                    if (GUILayout.Toggle(false, s_AddressableAssetToggleText, GUILayout.ExpandWidth(false))){
                        Debug.LogError("不要手动操作单个文件的Address了，请到BundleSetting里配自动设置路径！");
                        //SetAaEntry(aaSettings, editor.targets[0], true);//不让手动操作了，傻瓜式的用buildConfig设置
                    }
                }


                // if (addressableCount == 0)
                // {
                //     if (GUILayout.Toggle(false, s_AddressableAssetToggleText, GUILayout.ExpandWidth(false)))
                //         SetAaEntry(AddressableAssetSettingsDefaultObject.GetSettings(true), editor.targets, true);
                // }
                // else if (addressableCount == editor.targets.Length)
                // {
                //     GUILayout.BeginHorizontal();
                //     if (!GUILayout.Toggle(true, s_AddressableAssetToggleText, GUILayout.ExpandWidth(false)))
                //         SetAaEntry(aaSettings, editor.targets, false);

                //     if (editor.targets.Length == 1 && entry != null)
                //     {
                //         entry.address = EditorGUILayout.DelayedTextField(entry.address, GUILayout.ExpandWidth(true));
                //     }
                //     GUILayout.EndHorizontal();
                // }
                // else
                // {
                //     GUILayout.BeginHorizontal();
                //     if (s_ToggleMixed == null)
                //         s_ToggleMixed = new GUIStyle("ToggleMixed");
                //     if (GUILayout.Toggle(false, s_AddressableAssetToggleText, s_ToggleMixed, GUILayout.ExpandWidth(false)))
                //         SetAaEntry(AddressableAssetSettingsDefaultObject.GetSettings(true), editor.targets, true);
                //     EditorGUILayout.LabelField(addressableCount + " out of " + editor.targets.Length + " assets are addressable.");
                //     GUILayout.EndHorizontal();
                // }
            }
        }
    }
}
