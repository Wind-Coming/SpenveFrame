using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using UnityEngine.Networking;
using Object = UnityEngine.Object;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Spenve
{
    internal class AssetDatabaseLoader : LoaderInterface
    {
        StringBuilder m_sStringBuilder = new StringBuilder();
        public void Init()
        {
        }

        //同步加载assetbundle中的资源
        public T LoadAsset<T>(string address) where T : UnityEngine.Object
        {
#if UNITY_EDITOR
            AssetInfo ai = null;
            if(!AssetConfig.Instance.AddressToAsset.TryGetValue(address, out ai)){
                Debug.LogError("Cant find address: " + address);
                return null;
            }

            string path = AssetDatabase.GUIDToAssetPath( ai.guid );
            UnityEngine.Object n = AssetDatabase.LoadAssetAtPath(path, typeof(T));
            if (n != null)
            {
                return n as T;
            }

            if (path.EndsWith(".lua"))
            {
                string fullpath = GlobalFunc.GetFullPath(path);
                string s = System.IO.File.ReadAllText(fullpath);
                TextAsset textAsset = new TextAsset(s);
                return textAsset as T;
            }
            
            Debug.LogError("Cant find res : " + address);
#endif
            return null;
        }

        public Object LoadAsset(string address, Type type)
        {
#if UNITY_EDITOR
            AssetInfo ai = null;
            if(!AssetConfig.Instance.AddressToAsset.TryGetValue(address, out ai)){
                Debug.LogError("Cant find address: " + address);
                return null;
            }

            string path = AssetDatabase.GUIDToAssetPath( ai.guid );
            UnityEngine.Object n = AssetDatabase.LoadAssetAtPath(path, type);
            if (n != null)
            {
                return n;
            }

            if (path.EndsWith(".lua"))
            {
                string fullpath = GlobalFunc.GetFullPath(path);
                string s = System.IO.File.ReadAllText(fullpath);
                TextAsset textAsset = new TextAsset(s);
                return textAsset;
            }
            
            Debug.LogError("Cant find res : " + address);
#endif
            return null;
        }

        public void LoadAssetAsync<T>(string address, Action<T> callback) where T : UnityEngine.Object
        {
            callback?.Invoke(LoadAsset<T>(address));
        }

        public AssetBundleRequest LoadAssetAsync<T>(string address) where T : UnityEngine.Object
        {
#if UNITY_EDITOR
            var req = new AssetRequest {asset = LoadAsset<T>(address)};
            return req;
            #else
            return null;
#endif
        }

        public void LoadSceneRes(string sceneName)
        {
            //throw new NotImplementedException();
        }
#pragma warning disable 0162

        public AssetConfig LoadConfig()
        {
#if UNITY_EDITOR
            return AssetDatabase.LoadAssetAtPath<AssetConfig>("Assets/Art/Config/AssetConfig.asset");
#endif
            return null;
        }
#pragma warning disable 0162

        public void UnloadBundle(string assetBundleName)
        {

        }
    }
}