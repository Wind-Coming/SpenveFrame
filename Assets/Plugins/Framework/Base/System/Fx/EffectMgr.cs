using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EffectMgr
{
    private static List<GameObject> allEffects = new List<GameObject>();
    
    public static void PlayEffect(string address, Vector3 pos, float time, Quaternion rotation)
    {
        GameObject go = Pool.LoadGo(address);
        if (go == null)
            return;
        go.transform.position = pos;
        go.transform.rotation = rotation;
        allEffects.Add(go);
        go.DelayFunc(time, () =>
        {
            if (go != null)
            {
                allEffects.Remove(go);
                Pool.UnLoad(go);
            }
        });
    }

    public static void PlayEffect(string address, Vector3 pos, float time, Quaternion rotation, Transform _parent)
    {
        GameObject go = Pool.LoadGo(address, pos, rotation, _parent);
        if (go == null)
            return;
        allEffects.Add(go);
        go.DelayFunc(time, () =>
        {
            if (go != null)
            {
                allEffects.Remove(go);
                Pool.UnLoad(go);
            }
        });
    }

    public static void Clear()
    {
        foreach (var effect in allEffects)
        {
            if(effect != null)
                Pool.UnLoad(effect);
        }
        allEffects.Clear();;
    }
}
