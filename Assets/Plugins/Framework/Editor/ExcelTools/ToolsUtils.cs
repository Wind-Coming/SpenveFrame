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

public class ToolsUtils
{
    public static String getPythonPath(){
#if UNITY_EDITOR_WIN 
    String xls2luaBase = Directory.GetParent(Application.dataPath) + "/Tools/xls2lua-tools";
    String pythonPath = xls2luaBase + "/App/python.exe"; 
    return pythonPath; 
#endif

#if UNITY_EDITOR_OSX 
    return "python"; 
#endif
    }


    public static String getProtocPath()
    {
        String pathBase = Directory.GetParent(Application.dataPath) + "/Tools/protoc";
#if UNITY_EDITOR_WIN
    String protocPath = pathBase + "/protoc.exe"; 
#endif
#if UNITY_EDITOR_OSX
        String protocPath = pathBase + "/protoc";
#endif
        return protocPath;
    }
}
