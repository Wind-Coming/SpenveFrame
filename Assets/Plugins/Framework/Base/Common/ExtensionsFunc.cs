using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spenve;
using System;

/// <summary>
/// 扩展函数
/// </summary>
public static class ExtensionsFunc {

    public static void UnloadAfterTime(this GameObject target, float time)
    {
        TimerSystem.Instance.Add(() => Pool.UnLoad(target), time);
    }
    public static void SetFalseAfterTime(this GameObject target, float time)
    {
        TimerSystem.Instance.Add(() => target.SetActive(false), time);
    }

    public static Timer DelayFunc(this GameObject target, float time, Action action)
    {
        return TimerSystem.Instance.Add(() => action(), time);
    }
}
