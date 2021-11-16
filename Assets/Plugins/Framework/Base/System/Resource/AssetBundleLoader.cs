using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spenve;
using System.IO;
using Object = UnityEngine.Object;

namespace Spenve
{
    internal class AssetBundleLoader : LoaderInterface
    {

        private Dictionary<string, AssetBundle> m_LoadedAssetBundles = new Dictionary<string, AssetBundle>();
        private Dictionary<string, string[]> m_Dependencies = new Dictionary<string, string[]>();
        private Dictionary<string, int> m_BundleReferencddCount = new Dictionary<string, int>();//bundle与bundle的引用计数

        private AssetBundleManifest m_AssetBundleManifest = null;

        public void Init()
        {
            LoadAssetBundleManifest();
        }

        //加载全局依赖文件
        void LoadAssetBundleManifest()
        {
            AssetBundle assetb = LoadAssetBundle(Utils.GetPlatformFolder());
            m_AssetBundleManifest = assetb.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
            //UnloadBundle(Utils.GetPlatformFolder());
        }

        private void SetRef(string assetbundleName)
        {
            if (m_BundleReferencddCount.ContainsKey(assetbundleName))
            {
                m_BundleReferencddCount[assetbundleName]++;
            }
            else
            {
                m_BundleReferencddCount.Add(assetbundleName, 1);
            }
        }

        
        //同步加载assetbundle
        public AssetBundle LoadAssetBundle(string assetbundleName)
        {
            AssetBundle bundle = null;
            m_LoadedAssetBundles.TryGetValue(assetbundleName, out bundle);
            if (bundle != null)
            {
                SetRef(assetbundleName);
                return bundle;
            }

            string mPath = Utils.GetExternalPath(false, false) + assetbundleName;
            GLog.Log("Load in outStore : " + mPath, GameLogType.LOG_RES);

            AssetBundle asb = null;

            if (File.Exists(mPath))
            {
                asb = AssetBundle.LoadFromFile(mPath);
            }

            if (asb == null)
            {
                mPath = Utils.GetInnerPath(false) + assetbundleName;
                GLog.Log("Load in InnerStore : " + mPath, GameLogType.LOG_RES);

                asb = AssetBundle.LoadFromFile(mPath);
            }

            //没有找到资源
            if (asb == null)
            {
                GLog.Warning("资源" + assetbundleName + "不存在!", GameLogType.LOG_RES);
                return null;
            }
            else
            {
#if RES_LOG
                Debug.LogWarning("Bundle加载：" + assetbundleName);
#endif
                m_LoadedAssetBundles.Add(assetbundleName, asb);
                SetRef(assetbundleName);

                LoadDependencies(assetbundleName);
                return asb;
            }
        }

        //加载依赖
        private void LoadDependencies(string assetBundleName)
        {
            if (m_AssetBundleManifest == null)
            {
                return;
            }

            string[] dependencies = m_AssetBundleManifest.GetAllDependencies(assetBundleName);
            if (dependencies.Length == 0)
                return;

            m_Dependencies.Add(assetBundleName, dependencies);
            for (int i = 0; i < dependencies.Length; i++)
            {
                LoadAssetBundle(dependencies[i]);
            }
        }

        //卸载ab
        public void UnloadBundle(string assetBundleName)
        {
            AssetBundle bundle = null;
            m_LoadedAssetBundles.TryGetValue(assetBundleName, out bundle);
            if (bundle == null)
                return;

            if (!m_BundleReferencddCount.ContainsKey(assetBundleName) || --m_BundleReferencddCount[assetBundleName] == 0)
            {
#if RES_LOG
                Debug.LogWarning("Bundle卸载：" + assetBundleName);
#endif
                bundle.Unload(true);
                m_LoadedAssetBundles.Remove(assetBundleName);

                string[] dependencies = null;
                if (!m_Dependencies.TryGetValue(assetBundleName, out dependencies))
                    return;

                foreach (string depen in dependencies)
                {
                    UnloadBundle(depen);
                }

                m_Dependencies.Remove(assetBundleName);
                m_BundleReferencddCount.Remove(assetBundleName);
            }

            GLog.Log("Unload : " + assetBundleName, GameLogType.LOG_RES);
        }


        //加载资源
        public T LoadAsset<T>(string address) where T : UnityEngine.Object
        {
            AssetInfo ai = null;
            if(!AssetConfig.Instance.AddressToAsset.TryGetValue(address, out ai)){
                Debug.LogError("Cant find address: " + address);
                return null;
            }
            
            AssetBundle ab = null;
            ab = LoadAssetBundle(ai.bundleName);
            T obj = ab.LoadAsset<T>(ai.address);

            if(obj == null) {
                Debug.LogError("Cant find: " + address);
                return null;
            }
            return obj;
        }

        public Object LoadAsset(string address, Type type)
        {
            AssetInfo ai = null;
            if(!AssetConfig.Instance.AddressToAsset.TryGetValue(address, out ai)){
                Debug.LogError("Cant find address: " + address);
                return null;
            }
            
            AssetBundle ab = null;
            ab = LoadAssetBundle(ai.bundleName);
            var obj = ab.LoadAsset(ai.address, type);

            if(obj == null) {
                Debug.LogError("Cant find: " + address);
                return null;
            }
            return obj;
        }

        public void LoadAssetAsync<T>(string address, Action<T> complete) where T : Object
        {
            AssetInfo ai = null;
            if(!AssetConfig.Instance.AddressToAsset.TryGetValue(address, out ai)){
                Debug.LogError("Cant find address: " + address);
                complete?.Invoke(null);
                return;
            }
            
            var ab = LoadAssetBundle(ai.bundleName);
            var req = ab.LoadAssetAsync<T>(ai.address);
            req.completed += operation =>
            {
                complete?.Invoke(req.asset as T);
            };
        }

        public AssetBundleRequest LoadAssetAsync<T>(string address) where T : UnityEngine.Object
        {
            if(!AssetConfig.Instance.AddressToAsset.TryGetValue(address, out var ai)){
                Debug.LogError("Cant find address: " + address);
                return null;
            }
            
            var ab = LoadAssetBundle(ai.bundleName);
            return ab.LoadAssetAsync<T>(ai.address);
        }

        public void LoadSceneRes(string sceneName)
        {
            LoadAssetBundle(sceneName);
        }

        public AssetConfig LoadConfig()
        {
            AssetBundle ab = LoadAssetBundle("config");
            return ab.LoadAsset<AssetConfig>("AssetConfig");
        }
    }
}