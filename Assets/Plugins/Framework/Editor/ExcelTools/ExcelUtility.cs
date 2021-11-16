using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Reflection;
using System.Reflection.Emit;
using System;
using System.Diagnostics;

public class ExcelUtility 
{

	/// <summary>
	/// 表格数据集合
	/// </summary>
	private DataSet mResultSet;
	private List<string> excelPaths;

	public ExcelUtility (List<string> excelFiles)
	{
		excelPaths = excelFiles;
	}
	
	public void ConvertToLuaPython(string luaPath) {
		try
		{
            String xls2luaBase = Directory.GetParent(Application.dataPath) + "/Tools/xls2lua-tools";
            String xls2LuaPath = xls2luaBase + "/xls2lua.py";
            var pythonPath = ToolsUtils.getPythonPath();
			String outputPath = luaPath; 
			if (luaPath.Substring(luaPath.Length - 4).Equals("lua", StringComparison.OrdinalIgnoreCase)) {
				outputPath = outputPath.Substring(0, luaPath.LastIndexOf("/"));
			}

			List<Process> convertionProcesses = new List<Process>();
			foreach (string excelPath in excelPaths) {
				String argString = xls2LuaPath + " " +
					excelPath + " " +
					outputPath;
				ProcessStartInfo psi = new ProcessStartInfo(pythonPath, argString);
				psi.UseShellExecute = false;
				psi.CreateNoWindow = true;
				try{
				psi.RedirectStandardOutput = true;
				Process p = System.Diagnostics.Process.Start(psi);
				p.BeginOutputReadLine();
                p.OutputDataReceived += new DataReceivedEventHandler((object sender, DataReceivedEventArgs e) =>
				{
					if (!string.IsNullOrEmpty(e.Data))
					{
						UnityEngine.Debug.Log(e.Data);
					}
				});

                convertionProcesses.Add(p);
				}
				catch(Exception e)
				{
					UnityEngine.Debug.LogError(e.Message);
				}
			}

			foreach (System.Diagnostics.Process p in convertionProcesses) {
				if(p != null && !p.HasExited) {
					p.WaitForExit();
					p.Close();
				}
			}
		}
		catch (System.Exception ex)
		{
			UnityEngine.Debug.LogError(ex.Message);
		}
	}
}

