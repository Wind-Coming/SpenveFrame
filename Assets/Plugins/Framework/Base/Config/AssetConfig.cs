using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

#if UNITY_EDITOR
using UnityEditor;
using System.IO;
#endif

namespace Spenve
{
    [Serializable]
    public class AssetInfo
    {
        #region 序列化数据
        public string address;
        public string bundleName = "bundleName";
        public string guid;
        #endregion
    }

#if UNITY_EDITOR
    public class Procecer : AssetPostprocessor
    {
        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            AssetConfig.RefreshAsset();
        }
    }
#endif
    
    [Serializable]
    [CreateAssetMenu(menuName = "Config/Create AssetConfig Asset")]
    public class AssetConfig : ScriptableObject
    {
        private static AssetConfig _instance;

        public static AssetConfig Instance
        {
            get
            {
                if (null == _instance) {
                    Init();
                }

                return _instance;
            }
        }

        public List<AssetInfo> GetAll()
        {
            return allAssets;
        }

        public void ClearAll()
        {
            allAssets?.Clear();
            AddressToAsset.Clear();
            GuidToAsset.Clear();
        }
        
        private Dictionary<string, int> bundleRefCount = new Dictionary<string, int>();//资源对bundle的引用计数
        public static Dictionary<string, int> BundleRefCount
        {
            get
            {
                return Instance.bundleRefCount;
            }
        }

        private Dictionary<string, AssetInfo> guidToAsset;

        public Dictionary<string, AssetInfo> GuidToAsset
        {
            get
            {
                if(guidToAsset == null)
                {
                    InitGuidDic();
                }
                return guidToAsset;
            }
        }

        private Dictionary<string, AssetInfo> addressToAsset;

        public Dictionary<string, AssetInfo> AddressToAsset
        {
            get
            {
                if(addressToAsset == null)
                {
                    InitAddressDic();
                }
                return addressToAsset; 
            }
        }

        private static void Init()
        {
#if UNITY_EDITOR
            _instance = AssetConfig.CreateInstance<AssetConfig>();
            var a = _instance.AddressToAsset;
            var b = _instance.GuidToAsset;
            List<BundleAttribute> bundles = BundleConfig.Instance.allBundle;
            for (int i = 0; i < bundles.Count; i++)
            {
                BundleAttribute info = bundles[i];
                info.RefreshFiles(_instance);
            }
#else
            _instance = Res.LoadConfig();
#endif
        }

#if UNITY_EDITOR
        internal static void RefreshAsset()
        {
            if (_instance != null)
            {
                _instance.ClearAll();
                var a = _instance.AddressToAsset;
                var b = _instance.GuidToAsset;
                List<BundleAttribute> bundles = BundleConfig.Instance.allBundle;
                for (int i = 0; i < bundles.Count; i++)
                {
                    BundleAttribute info = bundles[i];
                    info.RefreshFiles(_instance);
                }
            }
        }
#endif

        private void InitGuidDic()
        {
            guidToAsset = new Dictionary<string, AssetInfo>();
            for (int i = 0; i < allAssets.Count; i++)
            {
                guidToAsset.Add(allAssets[i].guid, allAssets[i]);
            }
        }

        private void InitAddressDic()
        {
            addressToAsset = new Dictionary<string, AssetInfo>();
            for (int i = 0; i < allAssets.Count; i++)
            {
                addressToAsset.Add(allAssets[i].address, allAssets[i]);
            }
        }
        
        public void AddAsset(AssetInfo ai)
        {
#if UNITY_EDITOR
            //地址重复或者guid重复，则删除重新加
            AssetInfo old = allAssets.Find(t => t.address == ai.address);
            if (old == null)
            {
                old = allAssets.Find(t => t.guid == ai.guid);
            }
            
            if ( old != null)
            {
                Debug.LogError("重复address:" + old.address + "！已经移除旧的");
                allAssets.Remove(old);
                AddressToAsset.Remove(old.address);
                GuidToAsset.Remove(old.guid);
            }
            
            allAssets.Add(ai);
            AddressToAsset.Add(ai.address, ai);  
            GuidToAsset.Add(ai.guid, ai);
#endif
        }

        public bool RemoveGuid(string guid)
        {
#if UNITY_EDITOR
            if(!GuidToAsset.ContainsKey(guid))
            {
                Debug.LogError("没有找到guid！");
                return false;
            }
            AssetInfo ai = GuidToAsset[guid];
            allAssets.Remove(ai);
            AddressToAsset.Remove(ai.address);
            GuidToAsset.Remove(guid);            
#endif
            return true;
        }

        public bool RemoveAddress(string address)
        {
#if UNITY_EDITOR
            if(!AddressToAsset.ContainsKey(address))
            {
                Debug.LogError("没有找到address！");
                return false;
            }
            AssetInfo ai = AddressToAsset[address];
            allAssets.Remove(ai);
            AddressToAsset.Remove(ai.address);
            GuidToAsset.Remove(ai.guid);            
#endif
            return true;
        }


        [SerializeField]
        public List<AssetInfo> allAssets = new List<AssetInfo>();
    }
}