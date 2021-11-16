using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
using System;

public class ExcelTools : EditorWindow{
	/// <summary>
	/// 当前编辑器窗口实例
	/// </summary>
	private static ExcelTools instance;

	/// <summary>
	/// 滚动窗口初始位置
	/// </summary>
	private static Vector2 scrollPos;

	/// <summary>
	/// 显示当前窗口	
	/// </summary>
	//[MenuItem("Tools/Excel To Lua Tool")]
	static void ShowExcelTools() {
		Init();
		instance.Show();
	}

	private static void Init() {
		//获取当前实例
		instance=EditorWindow.GetWindow<ExcelTools>();
		scrollPos=new Vector2(instance.position.x,instance.position.y+75);
		
        //注意这里需要对路径进行处理
        //目的是去除Assets这部分字符以获取项目目录
        ExcelToolsData.pathRoot = EditorPrefs.GetString("excelPath", "");//Application.dataPath.Substring(0, Application.dataPath.LastIndexOf("/"));
		ExcelToolsData.LoadExcel();
	}

	void Awake() {
		ExcelToolsController.editorWindow = this;
	}

	void OnDestroy() {
		ExcelToolsController.editorWindow = null;
	}

	private string SelectFolder()
    {
        var path = EditorUtility.OpenFolderPanel("Select folder", "", "");
        return path;
    }
	
	void OnGUI() {
		// DrawOptions();
		DrawExport();
	}

	/// <summary>
	/// 绘制插件界面输出项
	/// </summary>
	private void DrawExport()
	{
		GUILayout.Space(10);
        GUILayout.BeginHorizontal();
        GUILayout.Label("excel path: ", EditorStyles.boldLabel, GUILayout.Width(90));
        if (GUILayout.Button("select", GUILayout.Width(60)))
        {
            ExcelToolsData.pathRoot = SelectFolder();
			EditorPrefs.SetString("excelPath", ExcelToolsData.pathRoot);
			ExcelToolsData.LoadExcel();
        }
        ExcelToolsData.pathRoot = GUILayout.TextField(ExcelToolsData.pathRoot, GUILayout.Width(240));
		GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
		if (GUILayout.Button("全选", GUILayout.Width(60)))
		{
			for(int i = 0; i < ExcelToolsData.excelList.Count; i++) {
				ExcelToolsData.excelList[i].selected = true;
			}
		}
		if (GUILayout.Button("清空", GUILayout.Width(60)))
		{
			for(int i = 0; i < ExcelToolsData.excelList.Count; i++) {
				ExcelToolsData.excelList[i].selected = false;
			}			
		}
		GUILayout.EndHorizontal();

		if(ExcelToolsData.excelList==null) return;
		if(ExcelToolsData.excelList.Count<1) {
			EditorGUILayout.LabelField("目前没有Excel文件被选中哦!");
		} else {
			EditorGUILayout.LabelField(string.Format("下列[{0}]个项目将被转换为 LUA:",ExcelToolsData.excelList.Count));
			GUILayout.BeginVertical();
			scrollPos=GUILayout.BeginScrollView(scrollPos,false,true,GUILayout.Height(550));
			for(int i = 0; i < ExcelToolsData.excelList.Count; i++) {
				GUILayout.BeginHorizontal();
				ExcelToolsData.excelList[i].selected = GUILayout.Toggle(ExcelToolsData.excelList[i].selected, ExcelToolsData.excelList[i].fileName, GUILayout.Width(200));
				ExcelToolsData.excelList[i].exportToClient = GUILayout.Toggle(ExcelToolsData.excelList[i].exportToClient, "客户端");
				ExcelToolsData.excelList[i].exportToServer = GUILayout.Toggle(ExcelToolsData.excelList[i].exportToServer, "服务器");
				GUILayout.EndHorizontal();
			}
			if(GUI.changed)
			{
				ExcelToolsData.SaveSelectInfo();
			}
			GUILayout.EndScrollView();
			GUILayout.EndVertical();

			//输出
			if(GUILayout.Button("转换")) {
				ExcelToolsController.Convert(); 
			}

            GUILayout.BeginHorizontal();
            GUILayout.Label(ExcelToolsData.clientOutPath, EditorStyles.boldLabel);
            if (GUILayout.Button("客户端导出路径", GUILayout.Width(100)))
            {
				string p = SelectFolder();
				if(!string.IsNullOrEmpty(p))
				{
					ExcelToolsData.clientOutPath = p;
					EditorPrefs.SetString("clientOutPath", ExcelToolsData.clientOutPath); 
				}
            }
        	GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label(ExcelToolsData.serverOutPath, EditorStyles.boldLabel);
            if (GUILayout.Button("服务器导出路径", GUILayout.Width(100)))
            {
				string p = SelectFolder();
				if(!string.IsNullOrEmpty(p))
				{
					ExcelToolsData.serverOutPath = p;
					EditorPrefs.SetString("serverOutPath", ExcelToolsData.serverOutPath); 
				}
            }
        	GUILayout.EndHorizontal();

            // if(GUILayout.Button("添加额外的导出路径")) {
			// 	string p = SelectFolder();
			// 	if(!string.IsNullOrEmpty(p) && !ExcelToolsData.extenOutPath.Contains(p))
			// 	{
			// 		ExcelToolsData.extenOutPath.Add(p);
			// 		SaveExtenPath();
			// 	}
			// }

			// if( ExcelToolsData.extenOutPath != null && ExcelToolsData.extenOutPath.Count > 0 )
			// {
            //     for (int i = 0; i < ExcelToolsData.extenOutPath.Count; i++)
            //     {
            //         GUILayout.BeginHorizontal();
			// 		GUILayout.Label(ExcelToolsData.extenOutPath[i], EditorStyles.boldLabel);

			// 		if (GUILayout.Button("删除", GUILayout.Width(60)))
			// 		{
			// 			ExcelToolsData.extenOutPath.RemoveAt(i);
			// 			SaveExtenPath();
			// 		}

            //         GUILayout.EndHorizontal();
            //     }
			// }
		}
	}

    void OnEnable() {
		ExcelToolsController.editorWindow = this;

		ExcelToolsData.LoadExcel(); 
	}

	void OnSelectionChange() {
		//当选择发生变化时重绘窗体
		Show();
		ExcelToolsData.LoadExcel();
		Repaint();
	}

	static void SaveExtenPath()
	{
        EditorPrefs.DeleteKey("extenOutPath");
        if (ExcelToolsData.extenOutPath != null && ExcelToolsData.extenOutPath.Count > 0)
        {
            string paths = "";
            for (int i = 0; i < ExcelToolsData.extenOutPath.Count; i++)
            {
                paths += ExcelToolsData.extenOutPath[i] + ";";
            }

			EditorPrefs.SetString("extenOutPath", paths); 
        }
	}
}




class ExcelToolsController {
/////////////////////控制
	static public ScriptableObject editorWindow;

	/// <summary>
	/// 转换Excel文件
	/// </summary>
	public static void Convert()
	{
		if (editorWindow == null)
		{
			Debug.Log("请重新打开窗口！");
		 	return;
		}
		// 编码
		Encoding encoding = new UTF8Encoding(false);

		// // 输出文件夹
		// string output= Path.Combine(Application.dataPath, "OtherLuaScripts/Game/Profiles/").Replace("\\", "/");
		// if (!Directory.Exists(output)) {
		// 	Directory.CreateDirectory(output);
		// }


		// 客户端导出
		if(!string.IsNullOrEmpty( ExcelToolsData.clientOutPath ))
		{
			List<string> fullPaths = new List<string>();//.Select(x => ExcelToolsData.pathRoot + "/" + x).ToList();
			for(int i = 0; i < ExcelToolsData.excelList.Count; i++)
			{
				if(ExcelToolsData.excelList[i].selected && ExcelToolsData.excelList[i].exportToClient)
				{
					fullPaths.Add(ExcelToolsData.excelList[i].path);
				}
			}

			//构造Excel工具类
			ExcelUtility excel = new ExcelUtility(fullPaths);
			excel.ConvertToLuaPython(ExcelToolsData.clientOutPath);
		}
		else
		{
			Debug.LogWarning("没有选择客户端导出路径！");
		}

		// 服务端导出
		if(!string.IsNullOrEmpty( ExcelToolsData.serverOutPath ))
		{
			List<string> fullPaths = new List<string>();//.Select(x => ExcelToolsData.pathRoot + "/" + x).ToList();
			for(int i = 0; i < ExcelToolsData.excelList.Count; i++)
			{
				if(ExcelToolsData.excelList[i].selected && ExcelToolsData.excelList[i].exportToServer)
				{
					fullPaths.Add(ExcelToolsData.excelList[i].path);
				}
			}

			//构造Excel工具类
			ExcelUtility excel = new ExcelUtility(fullPaths);
			excel.ConvertToLuaPython(ExcelToolsData.serverOutPath);
		}
		else
		{
			Debug.LogWarning("没有选择服务器导出路径！（不需要可以忽略此信息）");
		}

		// //Todo：现在比较暴力，这儿应该把额外的路径传到打表工具里，打完后copy到其他路径，以提升效率
        // if (ExcelToolsData.extenOutPath != null && ExcelToolsData.extenOutPath.Count > 0)
        // {
        //     for (int i = 0; i < ExcelToolsData.extenOutPath.Count; i++)
        //     {
        //         excel.ConvertToLuaPython(ExcelToolsData.extenOutPath[i] + "/");
        //     }
        // }

		//刷新本地资源
		AssetDatabase.Refresh();

        //转换完后关闭插件
        //这样做是为了解决窗口
        //再次点击时路径错误的Bug
        // instance.Close();
        ExcelToolsData.LoadExcel();
    }
}





class ExcelToolsData {

	public class ExcelData
	{
		public string path;
		public string fileName;
		public bool selected;
		public bool exportToClient = true;
		public bool exportToServer = true;
	}

//////////////////////数据
	/// <summary>
	/// Excel文件列表
	/// </summary>
	public static List<ExcelData> excelList = new List<ExcelData>();
	public static string serverOutPath = "";
	public static string clientOutPath = Application.dataPath + "/LuaScripts/Config";
	public static List<string> extenOutPath = new List<string>();

	/// <summary>
	/// 项目根路径	
	/// </summary>
	public static string pathRoot;


	/// <summary>
	/// 加载Excel
	/// </summary>
	public static void LoadExcel()
	{
		if(excelList==null) excelList=new List<ExcelData>();
		excelList.Clear();

		if(pathRoot == null)
			pathRoot = EditorPrefs.GetString("excelPath", "");

		ExcelToolsData.clientOutPath =  EditorPrefs.GetString("clientOutPath", "");
		ExcelToolsData.serverOutPath =  EditorPrefs.GetString("serverOutPath", "");

		if (ExcelToolsData.extenOutPath == null || ExcelToolsData.extenOutPath.Count == 0)
        {
            string[] paths = EditorPrefs.GetString("extenOutPath", "").Split(';');
            for (int i = 0; i < paths.Length; i++)
            {
				if(!string.IsNullOrEmpty(paths[i])){
                	ExcelToolsData.extenOutPath.Add(paths[i]);
				}
            }
        }

		OpenSelectInfo();

		bool modify = false;
		string[] files = Directory.GetFiles(pathRoot, "*.xlsx", SearchOption.AllDirectories);
		for (int i = excelList.Count - 1; i >=0 ; i--)
		{
			bool find = false;
            for (int i1 = 0; i1 < files.Length; i1++)
            {
                string path = files[i1].Replace('\\', '/');
                string fileName = path.Substring(path.LastIndexOf('/') + 1);
				if (excelList[i].fileName == fileName)
				{
                    find = true;
					break;
				}
			}
			if (! find)
			{
                excelList.RemoveAt(i);
			}
		}
		for(int i = 0; i < files.Length; i++)
		{
			string path = files[i].Replace('\\', '/');
			string fileName = path.Substring(path.LastIndexOf('/') + 1);

			if (!fileName.StartsWith("~$"))
			{
                if (excelList.Find((ExcelData v) => v.fileName == fileName) == null)
                {
                    ExcelData ed = new ExcelData();
                    ed.path = path;
                    ed.fileName = fileName;
                    ed.selected = true;
                    excelList.Add(ed);

                    modify = true;
                }
			}
			
		}

		if(modify)
		{
			SaveSelectInfo();
		}
	}

	public static void OpenSelectInfo()
    {
		if(excelList != null && excelList.Count > 0)
		{
			return;
		}
		string configFile = pathRoot+ "/exportConfig.txt";
		FileStream fs = File.Open(configFile, FileMode.OpenOrCreate);
		StreamReader sr = new StreamReader(fs);
		string all = sr.ReadToEnd().Replace("\n", "").Replace("\r", "");
		string[] nodes = all.Split(';');
        for (int i = 0; i < nodes.Length; i++)
        {
            string[] nodeString = nodes[i].Split(',');
            if (nodeString.Length < 4)
            {
                continue;
            }
            if (nodeString[0].StartsWith("~$"))
			{
				continue;
			}
			if (excelList.Find((ExcelData v) => v.fileName == nodeString[0]) == null)
			{
				ExcelData ed = new ExcelData();
				ed.fileName = nodeString[0];
				ed.path = pathRoot + "/" + nodeString[0];
				ed.selected = int.Parse(nodeString[1]) == 1;
				ed.exportToClient = int.Parse(nodeString[2]) == 1;
				ed.exportToServer = int.Parse(nodeString[3]) == 1;
				excelList.Add(ed);
			}
			
        }
		sr.Close();
		fs.Close();
    }


    public static void SaveSelectInfo()
    {
		string configFile = pathRoot+ "/exportConfig.txt";
		FileStream fs = new FileStream(configFile, FileMode.Create,FileAccess.Write );
		//FileStream fs = File.Open(configFile, FileMode.OpenOrCreate);
		StreamWriter sw = new StreamWriter(fs);
		StringBuilder sb = new StringBuilder();
		for(int i = 0; i < excelList.Count; i++) 
		{
			sb.Append(excelList[i].fileName);
			sb.Append(",");
			sb.Append(excelList[i].selected ? 1 : 0);
			sb.Append(",");
			sb.Append(excelList[i].exportToClient ? 1 : 0);
			sb.Append(",");
			sb.Append(excelList[i].exportToServer ? 1 : 0);
			sb.Append(";");
			sw.WriteLine(sb.ToString());
			sb.Clear();
		}

		sw.Close();
		fs.Close();
    }
}