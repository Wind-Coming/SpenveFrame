using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;
using DG.Tweening;
using System;

public class UIMgr : SingleInstance<UIMgr>
{
    private static Dictionary<Type, UIWindowBase> windows = new Dictionary<Type, UIWindowBase>();

    // public void Init()
    // {
    //     FontManager.RegisterFont(FontManager.GetFont("LongWar"), "长城特圆体");
    //     MsgSystem.Instance.AddListener("updateinfo", OnBalanceChanged);
    // }

    static UIPackage.LoadResource _loadFromResourcesPath = (string name, string extension, System.Type type, out DestroyMethod destroyMethod) =>
    {
        destroyMethod = DestroyMethod.Unload;
        return ResLoader.Global.LoadAsset(name, type);
    };
    
    public static void AddPackage(string pkgName)
    {
        UIPackage.AddPackage(pkgName, _loadFromResourcesPath);
    }
    
    public static Window Show<T>(object param = null) where  T: UIWindowBase
    {
        UIWindowBase win = null;
        Type type = typeof(T);
        if(windows.TryGetValue(type, out win))
        {
            win.param = param;
            win.Show();
            return win;
        }

        win = Activator.CreateInstance<T>();
        windows.Add(type, win);
        win.param = param;
        win.Show();
        return win;
    }

    public static void Hide<T>() where T : UIWindowBase
    {
        UIWindowBase win = null;
        Type type = typeof(T);
        if (windows.TryGetValue(type, out win)) {
            win.Hide();
        }
    }

    public static void DisposeWindow<T>() where T : UIWindowBase
    {
        UIWindowBase win = null;
        Type type = typeof(T);
        if (windows.TryGetValue(type, out win)) {
            windows.Remove(type);
            win.Dispose();
        }
    }
    
    public static T GetWindow<T>() where T : UIWindowBase
    {
        UIWindowBase win = null;
        Type type = typeof(T);
        if (windows.TryGetValue(type, out win))
        {
            return win as T;
        }
        return null;
    }

    public static void DisposeAll()
    {
        foreach(var v in windows) {
            v.Value.Dispose();
        }
        windows.Clear();
    }

    public static void ShowTips(string tip)
    {
        GComponent tipOrg = UIPackage.CreateObject("Main", "Tips").asCom;
        GComponent gc = tipOrg.GetChild("n5").asCom;
        for (int i = 1; i <= 6; i++) {
            gc.GetChild("t" + i).visible = false;
        }

        bool special = false;
        for (int i = 1; i <= 6; i++) {
            if (tip.Equals("t" + i)) {
                special = true;
                gc.GetChild("t" + i).visible = true;
            }
        }

        if (!special) {
            gc.GetChild("tip").visible = true;
            gc.GetChild("tip").text = tip;
        }
        else {
            gc.GetChild("tip").visible = false;
        }
        GRoot.inst.AddChild(tipOrg);
        tipOrg.GetTransition("t1").Play( ()=> gc.Dispose());
    }

    // public static void ShowConfirm(string tip, EventCallback1 callback)
    // {
    //     UI_Confirm gc = (UI_Confirm)UIPackage.CreateObject("Main", "Confirm").asCom;
    //     gc.m_tip.text = tip;
    //     gc.m_time.text = "";
    //     gc.m_btnSure.onClick.Clear();
    //     gc.m_btnSure.onClick.Add(callback);
    //     gc.m_btnSure.onClick.Add(() => gc.Dispose());
    //     gc.m_btnClose.onClick.Add( () => gc.Dispose() );
    //     GRoot.inst.AddChild(gc);
    // }
}
