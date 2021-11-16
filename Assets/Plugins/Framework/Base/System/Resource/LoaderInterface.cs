using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Spenve
{
    public interface LoaderInterface
    {

        void Init();
        AssetConfig LoadConfig();
        T LoadAsset<T>(string address) where T : UnityEngine.Object;
        
        Object LoadAsset(string address, Type type);

        void LoadAssetAsync<T>(string address, Action<T> callback) where T : UnityEngine.Object;

        AssetBundleRequest LoadAssetAsync<T>(string address) where T : UnityEngine.Object;
        
        void LoadSceneRes(string sceneName);//场景的ab名字和场景名字要一样
        void UnloadBundle(string bundleName);
    }

    public class AssetRequest : AssetBundleRequest
    {
#if UNITY_EDITOR
        public new Object asset;
#endif
    }
}
