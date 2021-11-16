using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using UnityEditor.SceneManagement;
using System.IO;

#pragma warning disable 0414

//继承自EditorWindow类
public class ReplacePrefabWindow : EditorWindow
{
    string bugReporterName = "";
    string description = "";
    public GameObject buggyGameObject;

    public int _first;
    public int _last;
    public string _lastName;

    //利用构造函数来设置窗口名称
    ReplacePrefabWindow()
    {
        this.titleContent = new GUIContent("ReplacePrefabWindow");
    }

    //添加菜单栏用于打开窗口
    [MenuItem("Tools/美术工具/预制体批量修改工具")]
    static void showWindow()
    {
        EditorWindow.GetWindow(typeof(ReplacePrefabWindow));
    }
    void OnGUI()
    {
        GUILayout.BeginVertical();

        //绘制标题s
        GUILayout.Space(10);
        GUI.skin.label.fontSize = 13;
        GUI.skin.label.alignment = TextAnchor.MiddleCenter;
        GUILayout.Label("胖胖预制体生成器");

        //绘制文本


        //绘制当前正在编辑的场景
        //GUILayout.Space(10);
        //GUI.skin.label.fontSize = 12;
        //GUI.skin.label.alignment = TextAnchor.UpperLeft;
        //GUILayout.Label("Currently Scene:" + EditorSceneManager.GetActiveScene().name);

        //绘制当前时间
        //GUILayout.Space(10);
        //GUILayout.Label("Time:" + System.DateTime.Now);

        //绘制对象
        GUILayout.Space(10);
        buggyGameObject = (GameObject)EditorGUILayout.ObjectField("被替换的预制体", buggyGameObject, typeof(GameObject), true);

        //绘制描述文本区域
        //GUILayout.Space(10);
        //GUILayout.BeginHorizontal();
        //GUILayout.Label("Description", GUILayout.MaxWidth(80));
        //description = EditorGUILayout.TextArea(description, GUILayout.MaxHeight(75));
        //GUILayout.EndHorizontal();

        //EditorGUILayout.Space();

        //添加名为"Save Bug"按钮，用于调用SaveBug()函数
        if (GUILayout.Button("GOGOGO"))
        {
            RepalceSelectPrefab();
        }

        GUILayout.Space(10);
        GUI.skin.label.fontSize = 13;
        GUI.skin.label.alignment = TextAnchor.MiddleCenter;
        GUILayout.Label("胖胖群体改名");

        GUILayout.Space(10);
        _first = EditorGUILayout.IntField("从第几个开始", _first);

        GUILayout.Space(10);
        _last = EditorGUILayout.IntField("到第几个结束", _last);

        GUILayout.Space(10);
        _lastName = EditorGUILayout.TextField("需要改成的后缀", _lastName);

        if (GUILayout.Button("GOGOGO"))
        {
            Rename();
        }

        //添加名为"Save Bug with Screenshot"按钮，用于调用SaveBugWithScreenshot() 函数
        //if (GUILayout.Button("Save Bug With Screenshot"))
        //{
        //    //SaveBugWithScreenshot();
        //}

        GUILayout.EndVertical();
    }

    //用于保存当前信息
    //void SaveBug()
    //{
    //    Directory.CreateDirectory("Assets/BugReports/" + bugReporterName);
    //    StreamWriter sw = new StreamWriter("Assets/BugReports/" + bugReporterName + "/" + bugReporterName + ".txt");
    //    sw.WriteLine(bugReporterName);
    //    sw.WriteLine(System.DateTime.Now.ToString());
    //    sw.WriteLine(EditorSceneManager.GetActiveScene().name);
    //    sw.WriteLine(description);
    //    sw.Close();
    //}

    //void SaveBugWithScreenshot()
    //{
    //    Directory.CreateDirectory("Assets/BugReports/" + bugReporterName);
    //    StreamWriter sw = new StreamWriter("Assets/BugReports/" + bugReporterName + "/" + bugReporterName + ".txt");
    //    sw.WriteLine(bugReporterName);
    //    sw.WriteLine(System.DateTime.Now.ToString());
    //    sw.WriteLine(EditorSceneManager.GetActiveScene().name);
    //    sw.WriteLine(description);
    //    sw.Close();
    //    Application.CaptureScreenshot("Assets/BugReports/" + bugReporterName + "/" + bugReporterName + "Screenshot.png");
    //}

    private static void Replace(GameObject[] target, GameObject originPrefab)
    {
        foreach (GameObject item in target)
        {
            GameObject newPrefab = PrefabUtility.InstantiatePrefab(originPrefab) as GameObject;
            if (newPrefab != null)
            {
                newPrefab.transform.position = item.transform.position;
                newPrefab.transform.rotation = item.transform.rotation;
                newPrefab.transform.localScale = item.transform.localScale;
                newPrefab.transform.parent = item.transform.parent;
                GameObject.DestroyImmediate(item);
            }
            else
            {
                Debug.Log("load prefab is null Enemy New");
            }
        }
        EditorSceneManager.MarkAllScenesDirty();
    }

    public void RepalceSelectPrefab()
    {
        //手动选中需要替换的对象
        GameObject[] m_objects = Selection.gameObjects;
        string path = "Assets/Art_Res/ReplacementTarget/OrcPBR 1.prefab";
        GameObject originPrefab = buggyGameObject;
        if (originPrefab == null)
        {
            Debug.LogError("originPrefab is null" + path);
        }
        else
        {
            Replace(m_objects, originPrefab);
        }
    }

    public void Rename()
    {
        UnityEngine.Object[] m_objects = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.TopLevel);

        int index = 0;//序号  

        foreach (var item in m_objects)
        {
            if (Path.GetExtension(AssetDatabase.GetAssetPath(item)) != "")//判断路径是否为空  
            {

                string path = AssetDatabase.GetAssetPath(item);

                string newName = item.name.Remove(_first-1, _last) + _lastName;
                AssetDatabase.RenameAsset(path, newName);
                index++;
            }

        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    static void ShaderSoftEdgeUnlit()
    {
        Shader shader = Shader.Find("Unlit/Soft Edge Unlit");
        string[] tempMaterialsPath = AssetDatabase.GetAllAssetPaths();
        List<Material> tempMaterials = new List<Material>();
        for (int i = 0; i < tempMaterialsPath.Length; i++)
        {
            string ext = Path.GetExtension(tempMaterialsPath[i]);
            if (ext != ".mat")
            {
                continue;
            }
            tempMaterials.Add(AssetDatabase.LoadAssetAtPath(tempMaterialsPath[i], typeof(Material)) as Material);
        }
        if (tempMaterials.Count != 0)
        {
            for (int i = 0; i < tempMaterials.Count; i++)
            {
                if (tempMaterials[i] == null)
                {
                    continue;
                }
                if (tempMaterials[i].shader.name == "Legacy Shaders/Transparent/Cutout/Soft Edge Unlit")
                {
                    tempMaterials[i].shader = shader;
                }
            }
        }
    }

}
#pragma warning restore 0414
