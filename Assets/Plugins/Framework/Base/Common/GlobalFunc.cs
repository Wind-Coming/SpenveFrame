using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using DG.Tweening;


public class GlobalFunc
{
    public static string[] Suffix = new string[] { ".prefab", ".asset", ".mat", ".png", ".ogg", ".tga",  ".mp3", ".wav", ".shader", ".anim", ".unity" };


    public static string GetFullPath(string localPath)
    {
        var fullPath = Application.dataPath.Replace("Assets", "") + localPath;
        return fullPath;
    }

    public static string GetLocalPath(string fullPath)
    {
        var localPath = fullPath.Replace(Application.dataPath, "Assets");
        return localPath;
    }

    public static string GetFileName(string path)
    {
        string[] ppp = path.Split('/');
        string pp = ppp[ppp.Length - 1];
        return pp;

    }

    public static string GetFileNameWithoutExtend(string path)
    {
        string[] ppp = path.Split('/');
        string pp = ppp[ppp.Length - 1];
        string p = pp.Remove(pp.IndexOf('.'));
        return p;
    }

    public static string GetFilePathNameWithoutExtend(string path)
    {
        string p = path.Remove(path.IndexOf('.'));
        return p;
    }


    public static string GetFolderName(string path)
    {
        int lastindex = path.LastIndexOf('/');
        string ppp = path.Substring(0, lastindex);
        return ppp;
    }

    public static string GetFileExtend(string fileName)
    {
        int lastindex = fileName.LastIndexOf('.');
        string ppp = fileName.Remove(0, lastindex);
        return ppp;
    }

    public static bool SupportSuffix(string suffix)
    {
        for (int n = 0; n < Suffix.Length; n++) {
            if (Suffix[n] == GlobalFunc.GetFileExtend(suffix)) {
                return true;
            }
        }
        return false;
    }

    public static int GetSuffixIndex(string suffix)
    {
        for (int n = 0; n < Suffix.Length; n++) {
            if (Suffix[n] == GlobalFunc.GetFileExtend(suffix)) {
                return n;
            }
        }
        return -1;
    }

    public static string GetSuffix(int index)
    {
        return Suffix[index];
    }

    public static bool CanInstantiate(int suffixIndex)
    {
        return suffixIndex == 0;
    }

    public static Transform GetTransform(Transform father, string name)
    {
        foreach (Transform t in father) {
            if (t.name.Equals(name)) {
                return t;
            }
            else {
                Transform tt = GetTransform(t, name);
                if (tt != null) {
                    return tt;
                }
            }
        }
        return null;
    }

    public static float NearestFloat(float v, float f)
    {
        float m = v % f;
        if (m < f / 2) {
            return v - m;
        }
        else {
            return v - m + f;
        }
    }

    //点到直线的距离
    public static float DisOfPointToLine(Vector3 point, Vector3 linePoint1, Vector3 linePoint2)
    {
        Vector3 vec1 = point - linePoint1;
        Vector3 vec2 = linePoint2 - linePoint1;
        Vector3 vecProj = Vector3.Project(vec1, vec2);
        float dis = Mathf.Sqrt(Mathf.Pow(Vector3.Magnitude(vec1), 2) - Mathf.Pow(Vector3.Magnitude(vecProj), 2));
        return dis;
    }

    /// <summary>
    /// 在animator中获取动画片段
    /// </summary>
    /// <param name="animator"></param>
    /// <param name="clipName"></param>
    /// <returns></returns>
    public static AnimationClip GetAnimationClip(Animator animator, string clipName)
    {
        AnimationClip[] clips = animator.runtimeAnimatorController.animationClips;
        for (int i = 0; i < clips.Length; i++) {
            if (clips[i].name.Equals(clipName)) {
                return clips[i];
            }
        }
        return null;
    }

    public static void DelayFunc(Action action, float delayTime)
    {
        float v = 0;
        DOTween.To(() => v, x => v = x, 1, delayTime).OnComplete(() => action());
    }
    
    //消耗统计
    private static Stack<float> timeStack = new Stack<float>();
    public static void BeginSample()
    {
        float startSampleTime = Time.realtimeSinceStartup * 1000;
        timeStack.Push(startSampleTime);
    }

    public static void EndSample(string name = null)
    {
        if(timeStack.Count == 0)
            return;
        
        float startSampleTime = timeStack.Pop();
        if (string.IsNullOrEmpty(name))
        {
            Debug.Log( "耗时 ：" + (Time.realtimeSinceStartup * 1000 - startSampleTime) + "毫秒");
        }
        else
        {
            Debug.Log( name + "耗时 ：" + (Time.realtimeSinceStartup * 1000 - startSampleTime) + "毫秒");
        }
    }

    public static Bounds GetObjectWorldBounds(GameObject gameObject)
    {
        Renderer[] renders = gameObject.GetComponentsInChildren<Renderer>();
        if(renders.Length == 0)
            return new Bounds(gameObject.transform.position, Vector3.zero);
        
        Bounds worldBounds = renders[0].bounds;
        for (int i = 1; i < renders.Length; i++)
        {
            worldBounds.Encapsulate(renders[i].bounds);
        }

        return worldBounds;
    }
}