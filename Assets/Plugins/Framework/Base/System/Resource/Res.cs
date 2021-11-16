using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using AsyncOperation = UnityEngine.AsyncOperation;
#if UNITY_EDITOR
using UnityEditor;
#endif
using Spenve;

//namespace Spenve
//{
    public class Res : SingleInstance<Res>
    {
        private LoaderInterface m_loader;


        public static Dictionary<UnityEngine.Object, string> ObjToAddress =
            new Dictionary<UnityEngine.Object, string>();

        public Res()
        {
            Init();
        }

        protected void Init()
        {
#if UNITY_EDITOR
            if (!EditorConfig.SimulateAssetBundleInEditor)
            {
                m_loader = new AssetDatabaseLoader();
            }
            else
#endif
            {
                m_loader = new AssetBundleLoader();
            }

            m_loader.Init();
        }

        public static AssetConfig LoadConfig()
        {
            return Instance.m_loader.LoadConfig();
        }

        public static AssetBundle LoadBundle(string pkg)
        {
            if (Instance.m_loader is AssetBundleLoader loader)
            {
                return loader.LoadAssetBundle(pkg);
            }

            return null;
        }

        //同步加载assetbundle中的资源
        public static T LoadAsset<T>(string address) where T : UnityEngine.Object
        {
            AssetInfo ba = null;
            if (!AssetConfig.Instance.AddressToAsset.TryGetValue(address, out ba))
            {
                Debug.LogError("没有找到， address:" + address);
                return null;
            }

            T obj = Instance.m_loader.LoadAsset<T>(address);

            if (obj != null)
            {
                //引用计数
                if (AssetConfig.BundleRefCount.ContainsKey(ba.bundleName))
                {
                    AssetConfig.BundleRefCount[ba.bundleName]++;
                }
                else
                {
                    AssetConfig.BundleRefCount.Add(ba.bundleName, 1);
                }

#if RES_LOG
            Debug.LogWarning("加载" + address + ", bundle的引用计数为" + AssetConfig.BundleRefCount[ba.bundleName]);
#endif

                ObjToAddress.Add(obj, address);
            }

            return obj;
        }

        public static UnityEngine.Object LoadAsset(string address, Type type)
        {
            AssetInfo ba = null;
            if (!AssetConfig.Instance.AddressToAsset.TryGetValue(address, out ba))
            {
                Debug.LogError("没有找到， address:" + address);
                return null;
            }

            var obj = Instance.m_loader.LoadAsset(address, type);

            if (obj != null)
            {
                //引用计数
                if (AssetConfig.BundleRefCount.ContainsKey(ba.bundleName))
                {
                    AssetConfig.BundleRefCount[ba.bundleName]++;
                }
                else
                {
                    AssetConfig.BundleRefCount.Add(ba.bundleName, 1);
                }

#if RES_LOG
            Debug.LogWarning("加载" + address + ", bundle的引用计数为" + AssetConfig.BundleRefCount[ba.bundleName]);
#endif

                ObjToAddress.Add(obj, address);
            }

            return obj;
        }

        public static void LoadAssetAsync<T>(string address, Action<T> complete) where T : UnityEngine.Object
        {
            AssetInfo ba = null;
            if (!AssetConfig.Instance.AddressToAsset.TryGetValue(address, out ba))
            {
                Debug.LogError("没有找到， address:" + address);
                complete?.Invoke(null);
                return;
            }

            Instance.m_loader.LoadAssetAsync<T>(address, obj =>
            {
                if (obj != null)
                {
                    //引用计数
                    if (AssetConfig.BundleRefCount.ContainsKey(ba.bundleName))
                    {
                        AssetConfig.BundleRefCount[ba.bundleName]++;
                    }
                    else
                    {
                        AssetConfig.BundleRefCount.Add(ba.bundleName, 1);
                    }

#if RES_LOG
            Debug.LogWarning("加载" + address + ", bundle的引用计数为" + AssetConfig.BundleRefCount[ba.bundleName]);
#endif

                    ObjToAddress.Add(obj, address);
                }

                complete?.Invoke(obj);
            });
        }

        public static AssetBundleRequest LoadAssetAsync<T>(string address) where T : UnityEngine.Object
        {
            AssetInfo ba = null;
            if (!AssetConfig.Instance.AddressToAsset.TryGetValue(address, out ba))
            {
                Debug.LogError("没有找到， address:" + address);
                return null;
            }

            var req = Instance.m_loader.LoadAssetAsync<T>(address);
            req.completed += operation =>
            {
                var obj = req.asset;
                if (obj == null) return;
                //引用计数
                if (AssetConfig.BundleRefCount.ContainsKey(ba.bundleName))
                {
                    AssetConfig.BundleRefCount[ba.bundleName]++;
                }
                else
                {
                    AssetConfig.BundleRefCount.Add(ba.bundleName, 1);
                }

#if RES_LOG
            Debug.LogWarning("加载" + address + ", bundle的引用计数为" + AssetConfig.BundleRefCount[ba.bundleName]);
#endif
                ObjToAddress.Add(obj, address);
            };
            return req;
        }

        public static void UnLoad(UnityEngine.Object obj)
        {
            string address = null;
            if (!ObjToAddress.TryGetValue(obj, out address))
            {
                Debug.LogError("没有加载过么:" + address);
                return;
            }

            ObjToAddress.Remove(obj);
            UnLoad(address, obj);
        }


        private static void UnLoad(string address, UnityEngine.Object obj)
        {
            AssetInfo ai = null;
            if (!AssetConfig.Instance.AddressToAsset.TryGetValue(address, out ai))
            {
                Debug.LogError("卸载没有找到:" + address);
                return;
            }

            //引用计数减一
            if (AssetConfig.BundleRefCount.ContainsKey(ai.bundleName))
            {
                AssetConfig.BundleRefCount[ai.bundleName]--;

#if RES_LOG
            Debug.LogWarning("卸载" + address + ", bundle的引用计数为" + AssetConfig.BundleRefCount[ai.bundleName]);
#endif

                if (AssetConfig.BundleRefCount[ai.bundleName] == 0)
                {
                    Instance.m_loader.UnloadBundle(ai.bundleName);
                }
            }
            else
            {
                Debug.LogError("Bundle引用计数错误！ address:" + address);
            }
        }

        //同步加载场景
        public static void LoadScene(string sceneName)
        {
            Instance.m_loader.LoadSceneRes(sceneName);
            SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
        }

        //异步加载场景
        public static void LoadSceneAsync(string sceneName, Action<AsyncOperation> complete = null)
        {
            Instance.m_loader.LoadSceneRes(sceneName);
            SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single).completed += complete;
        }

        //清理
        public static void Clear()
        {
            ObjToAddress.Clear();
        }
    }
//}