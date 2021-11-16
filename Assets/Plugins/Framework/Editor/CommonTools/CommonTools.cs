using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Text.RegularExpressions;
using System.IO;

public class CommonTools : Editor
{

    public enum FindItemType
    {
        FindUI = 0,
        FindObj = 1,
    }

    //方法1
    [MenuItem("Tools/清除丢失脚本")]
    public static void RemoveMissingScript()
    {
        var gos = GameObject.FindObjectsOfType<GameObject>();
        foreach (var item in gos)
        {
            Debug.Log(item.name);
            SerializedObject so = new SerializedObject(item);
            var soProperties = so.FindProperty("m_Component");
            var components = item.GetComponents<Component>();
            int propertyIndex = 0;
            foreach (var c in components)
            {
                if (c == null)
                {
                    soProperties.DeleteArrayElementAtIndex(propertyIndex);
                }
                ++propertyIndex;
            }
            so.ApplyModifiedProperties();
        }
        
        AssetDatabase.Refresh();
        Debug.Log("清理完成!"); 
    }

    //方法2
    static void ClearMissingScripts()
    {
        //测试一个
        //List<GameObject> prefabList = new List<GameObject>(Selection.gameObjects);//GetAllUIPrefab();
        GameObject[] prefabList = GameObject.FindObjectsOfType<GameObject>();

        int prefabCount = prefabList.Length;
        Debug.Log("prefabCount = " + prefabCount);

        for (int i = prefabCount - 1; i >= 0; i--)
        {
            DeleteMissingScripts(prefabList[i]);
        }

        AssetDatabase.SaveAssets();
        Debug.Log("ClearMissingScripts Success");
    }


    static void DeleteMissingScripts(GameObject obj)
    {
        Regex guidRegex = new Regex("m_Script: {fileID: (.*), guid: (?<GuidValue>.*?), type:");
        Regex fileRegex = new Regex("--- !u!(?<groupNum>.*?) &(?<fileID>.*?)\r\n");
        int fileStrLenght = 30;
        string filePath = AssetDatabase.GetAssetPath(obj);
        string s = File.ReadAllText(filePath);
        string groupSpilChar = "---";
        string fileStr = "";
        bool isChange = false;
        MatchCollection matchList = guidRegex.Matches(s);
        if (matchList != null)
        {
            for (int i = matchList.Count - 1; i >= 0; i--)
            {

                string guid = matchList[i].Groups["GuidValue"].Value;
                if (AssetDatabase.GUIDToAssetPath(guid) == "")
                {
                    isChange = true;
                    int startIndex = s.LastIndexOf(groupSpilChar, matchList[i].Index);
                    int endIndex = s.IndexOf(groupSpilChar, matchList[i].Index);

                    Match fileMatch = fileRegex.Match(s.Substring(startIndex, fileStrLenght));
                    fileStr = "- " + fileMatch.Groups["groupNum"].Value + ": {fileID: " + fileMatch.Groups["fileID"].Value + "}\r\n  ";

                    s = s.Replace(s.Substring(startIndex, endIndex - startIndex), "");
                    s = s.Replace(fileStr, "");
                }
            }
        }
        if (isChange)
        {
            File.WriteAllText(filePath, s);
            Debug.Log(obj.name + " missing scripts destory success!");
        }
    }
}
