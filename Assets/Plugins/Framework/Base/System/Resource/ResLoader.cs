using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spenve;
using Object = UnityEngine.Object;

public class ResLoader: IReset
{
    //全局的，通过这个加载的永远不会卸载
    public static ResLoader Global = new ResLoader();
        
    public static ResLoader Get()
    {
        return ClassPool<ResLoader>.Get();
    }

    public static void Release(ResLoader loader)
    {
        if(loader != null)
            ClassPool<ResLoader>.Release(loader);
    }
    
    internal class ObjectRefNum
    {
        public Object obj;
        public int num;
    }
    
    //全局 AddressToObj索引和计数
    internal static Dictionary<string, ObjectRefNum> AddressObj = new Dictionary<string, ObjectRefNum>();

    //此loader的计数
    public Dictionary<string, int> refObjNum = new Dictionary<string, int>();

    public T LoadAsset<T>(string resName) where T : UnityEngine.Object
    {
        T obj = null;
        if (!AddressObj.ContainsKey(resName)) {
            obj = Res.LoadAsset<T>(resName);
            if(obj == null) {
                return null;
            }
            AddressObj.Add(resName, new ObjectRefNum(){obj = obj, num = 0});
        }
        else
        {
            obj = AddressObj[resName].obj as T;
        }

        AddressObj[resName].num++;
        
        //局部实际引用计数
        if (refObjNum.ContainsKey(resName))
        {
            refObjNum[resName]++;
        }
        else
        {
            refObjNum.Add(resName, 1);
        }

        return obj;
    }
    
    public Object LoadAsset(string resName, System.Type type)
    {
        Object obj = null;
        if (!AddressObj.ContainsKey(resName)) {
            obj = Res.LoadAsset(resName, type);
            if(obj == null) {
                return null;
            }
            AddressObj.Add(resName, new ObjectRefNum(){obj = obj, num = 0});
        }
        else
        {
            obj = AddressObj[resName].obj;
        }

        AddressObj[resName].num++;
        
        //局部实际引用计数
        if (refObjNum.ContainsKey(resName))
        {
            refObjNum[resName]++;
        }
        else
        {
            refObjNum.Add(resName, 1);
        }

        return obj;
    }

    public AssetBundleRequest LoadAssetAsync<T>(string address) where T : UnityEngine.Object
    {
        return Res.LoadAssetAsync<T>(address);
    }
    
    public void Unload(string resName)
    {
        if (refObjNum.ContainsKey(resName))
        {
            refObjNum[resName]--;
            AddressObj[resName].num --;
            if (AddressObj[resName].num <= 0)
            {
                Res.UnLoad(AddressObj[resName].obj);
                AddressObj.Remove(resName);
            }
        }
    }
    
    public void Unload(Object obj)
    {
        if(Res.ObjToAddress.TryGetValue(obj, out string resName))
        {
            if (refObjNum.ContainsKey(resName))
            {
                refObjNum[resName]--;
                AddressObj[resName].num --;
                if (AddressObj[resName].num <= 0)
                {
                    Res.UnLoad(AddressObj[resName].obj);
                    AddressObj.Remove(resName);
                }
            }
        }
    }

    public void Reset()
    {
        foreach(var v in refObjNum)
        {
            AddressObj[v.Key].num -= v.Value;
            if (AddressObj[v.Key].num <= 0)
            {
                Res.UnLoad(AddressObj[v.Key].obj);
                AddressObj.Remove(v.Key);
            }
        }
        refObjNum.Clear();
    }
}

