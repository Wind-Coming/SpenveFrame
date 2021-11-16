using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;


public static class Pool
{
    private static Dictionary<string, List<GameObject>> unActivePool = new Dictionary<string, List<GameObject>>();
    private static Dictionary<string, List<GameObject>> activePool = new Dictionary<string, List<GameObject>>();
    private static Dictionary<GameObject, string> GObjToAddress = new Dictionary<GameObject, string>();
    private static ResLoader Loader;
    private static Transform root;
    
    static Pool()
    {
        if(!Application.isPlaying)
            return;
        root = new GameObject("Pool").transform;
        root.position = Vector3.zero;
        GameObject.DontDestroyOnLoad(root);
    }
    
    //提前放入pool中
    public static void Preload(string address)
    {
        GameObject go = LoadGo(address);
        UnLoad(go);
    }

    //编辑器下使用的接口
    public static GameObject GetTemplateGo(string address)
    {
        if(Loader == null)
            Loader = ResLoader.Get();
        return Loader.LoadAsset<GameObject>(address);
    }
    
    public static GameObject LoadGo(string address)
    {
        GameObject go = _LoadGo(address, Vector3.zero,Quaternion.identity, null);
        if (go == null)
            return null;
        go.SetActive(true);
        return go;
    }
    
    public static GameObject LoadGo(string address, Vector3 pos)
    {
        GameObject go = _LoadGo(address, pos,Quaternion.identity, null);
        go.name = address;
        go.SetActive(true);
        return go;
    }

    public static GameObject LoadGo(string address, Vector3 pos,Quaternion rot)
    {
        GameObject go = _LoadGo(address, pos,rot,null);
        go.name = address;
        go.SetActive(true);
        return go;
    }

    public static GameObject LoadGo(string address, Vector3 pos, Quaternion rot,Transform par)
    {
        GameObject go = _LoadGo(address, pos, rot, par);
        go.name = address;
        go.SetActive(true);
        return go;
    }

    // public static void LoadGoAsync(string address,Action<GameObject> complete, Vector3 pos, Quaternion rot, Transform par)
    // {
    //     //从unActivePool 拿出来 放到 activePool， 如果unActivePool里不存在，则加载
    //     if (unActivePool.TryGetValue(address, out var gos))
    //     {
    //         if (gos.Count > 0)
    //         {
    //             var g = gos[0];
    //             g.transform.position = pos;
    //             g.transform.rotation = rot;
    //             g.transform.parent = par;
    //             gos.RemoveAt(0);
    //     
    //             if (!activePool.ContainsKey(address))
    //             {
    //                 activePool.Add(address, new List<GameObject>());
    //             }
    //             activePool[address].Add(g);
    //             g.SetActive(true);
    //             complete?.Invoke(g);
    //             return;
    //         }
    //     }
    //     
    //     void CreateInstance(Object ob)
    //     {
    //         var obj = Object.Instantiate(ob, pos, rot,par) as GameObject;
    //         GObjToAddress.Add(obj, address);
    //         if(!activePool.ContainsKey(address))
    //         {
    //             activePool.Add(address, new List<GameObject>());
    //         }
    //         activePool[address].Add(obj);
    //         obj.SetActive(true);
    //         complete?.Invoke(obj);
    //     }
    //     
    //     if (!AddressToObj.TryGetValue(address, out var oo))
    //     {
    //         Res.LoadAssetAsync<Object>(address, ob =>
    //         {
    //             if (ob == null)
    //             {
    //                 complete?.Invoke(null);
    //                 return;
    //             }
    //             
    //             AddressToObj.Add(address, ob);
    //             CreateInstance(ob);
    //         });
    //     }else
    //     {
    //         CreateInstance(oo);
    //     }
    // }

    private static GameObject _LoadGo(string address, Vector3 pos,Quaternion _rot,Transform _parent)
    {
        //从unActivePool 拿出来 放到 activePool， 如果unActivePool里不存在，则加载
        List<GameObject> gos = null;
        if (unActivePool.TryGetValue(address, out gos))
        {
            if (gos.Count > 0)
            {
                GameObject g = gos[0];
                g.transform.position = pos;
                g.transform.rotation = _rot;
                g.transform.parent = _parent;
                gos.RemoveAt(0);

                if (!activePool.ContainsKey(address))
                {
                    activePool.Add(address, new List<GameObject>());
                }
                activePool[address].Add(g);
                return g;
            }
        }

        if(Loader == null)
            Loader = ResLoader.Get();
        
        var ob = Loader.LoadAsset<Object>(address);
        if (ob == null)
            return null;
        
        //编辑器下非运行模式直接加载
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            return GameObject.Instantiate(ob, pos, _rot,_parent) as GameObject;
        }
#endif

        GameObject obj = GameObject.Instantiate(ob, pos, _rot,_parent) as GameObject;
        
        GObjToAddress.Add(obj, address);
        if(!activePool.ContainsKey(address))
        {
            activePool.Add(address, new List<GameObject>());
        }
        activePool[address].Add(obj);
        return obj;
    }

    public static void UnLoad(GameObject obj, bool pushInPool = true)
    {
        //编辑器下非运行模式直接摧毁
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            GameObject.DestroyImmediate(obj);
            return;
        }
#endif
        
        if (GObjToAddress.ContainsKey(obj))
        {
            //放入库中，从activePool 拿出来 放到 unActivePool
            string address = GObjToAddress[obj];
            if (pushInPool)
            {
                if(!activePool.ContainsKey(address) || !activePool[address].Contains(obj))
                {
                   //Debug.LogError("没有加载过这个GameObj或者已经卸载过了？:" + obj.name);
                }
                else
                {
                    activePool[address].Remove(obj);
                }

                if (!unActivePool.ContainsKey(address))
                {
                    unActivePool.Add(address, new List<GameObject>());
                }
                
                //obj.transform.parent = root;
                obj.SetActive(false);
                
                if(!unActivePool[address].Contains(obj))
                    unActivePool[address].Add(obj);
            }
            //不放入库中
            else
            {
                GObjToAddress.Remove(obj);

                //从activePool中移除
                if(activePool.ContainsKey(address) && activePool[address].Contains(obj))
                {
                    activePool[address].Remove(obj);
                }
                else
                {
                    Debug.LogError("没有加载过这个GameObj？");
                }

                //如果Pool中还有其他的，则只摧毁
                if ( (unActivePool.ContainsKey(address) && unActivePool[address].Count > 0) ||
                     (activePool.ContainsKey(address) && activePool[address].Count > 0) )
                {
                    GameObject.Destroy(obj);
                }
                else//如果库中没有其他的了，摧毁并卸载Object
                {
                    GameObject.Destroy(obj);
                    Loader.Unload(address);
                }
            }
        }
        else
        {
            Debug.LogWarning("没有找到已经加载的GameObject：" + obj.name + "(直接摧毁)");
            GameObject.Destroy(obj);
        }
    }

    public static void Clear()
    {
        foreach (var VARIABLE in activePool)
        {
            foreach (var go in VARIABLE.Value)
            {
                if (Application.isPlaying)
                {
                    GameObject.Destroy(go);
                }
                else
                {
                    GameObject.DestroyImmediate(go);
                }
            }
        }
        
        foreach (var VARIABLE in unActivePool)
        {
            foreach (var go in VARIABLE.Value)
            {
                if (Application.isPlaying)
                {
                    GameObject.Destroy(go);
                }
                else
                {
                    GameObject.DestroyImmediate(go);
                }
            }
        }

        unActivePool.Clear();
        activePool.Clear();

        if (Loader != null)
        {
            ResLoader.Release(Loader);
            Loader = null;
        }
    }
}
