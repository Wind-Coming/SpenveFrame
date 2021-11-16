using System;
using UnityEngine;
using UnityEditor;
using UnityEngine.Assertions;
using System.Collections.Generic;
using System.Linq;
using Spenve;
using UnityEditor.IMGUI.Controls;

namespace AssetBundleBrowser.AssetBundleDataSource
{
    internal class CustomABDataSource : ABDataSource
    {
        public static List<ABDataSource> CreateDataSources()
        {
            var op = new CustomABDataSource();
            var retList = new List<ABDataSource>();
            retList.Add(op);
            return retList;
        }

        public string Name {
            get {
                return "Custom";
            }
        }

        public string ProviderName {
            get {
                return "Built-in";
            }
        }

        public string[] GetAssetPathsFromAssetBundle (string assetBundleName)
        {
            List<string> paths = new List<string>();
            foreach (var VARIABLE in AssetConfig.Instance.GetAll())
            {
                if (VARIABLE.bundleName == assetBundleName)
                {
                    paths.Add(AssetDatabase.GUIDToAssetPath( VARIABLE.guid));
                }
            }
            return paths.ToArray();
        }

        public string GetAssetBundleName(string assetPath)
        {
            string guid = AssetDatabase.AssetPathToGUID(assetPath);
            if(AssetConfig.Instance.GuidToAsset.ContainsKey(guid))
                return AssetConfig.Instance.GuidToAsset[guid].bundleName;
            else
            {
                return null;
            }
        }

        public string GetImplicitAssetBundleName(string assetPath) {
            string guid = AssetDatabase.AssetPathToGUID(assetPath);
            if(AssetConfig.Instance.GuidToAsset.ContainsKey(guid))
                return AssetConfig.Instance.GuidToAsset[guid].bundleName;
            else
            {
                return null;
            }
        }

        public string[] GetAllAssetBundleNames() {
            List<BundleAttribute> bas = BundleConfig.Bundles;
            string[] bds = new string[bas.Count];
            for (int i = 0; i < bas.Count; i++)
            {
                bds[i] = bas[i].bundleName;
            }
            return bds;
        }

        public bool IsReadOnly() {
            return false;
        }

        public void SetAssetBundleNameAndVariant (string assetPath, string bundleName, string variantName) {
            //AssetImporter.GetAtPath(assetPath).SetAssetBundleNameAndVariant(bundleName, variantName);
        }

        public void RemoveUnusedAssetBundleNames() {
            //AssetDatabase.RemoveUnusedAssetBundleNames ();
        }

        public bool CanSpecifyBuildTarget { 
            get { return true; } 
        }
        public bool CanSpecifyBuildOutputDirectory { 
            get { return true; } 
        }

        public bool CanSpecifyBuildOptions { 
            get { return true; } 
        }

        public bool BuildAssetBundles (ABBuildInfo info) {
            if(info == null)
            {
                Debug.Log("Error in build");
                return false;
            }

            var buildManifest = BuildPipeline.BuildAssetBundles(info.outputDirectory, info.options, info.buildTarget);
            if (buildManifest == null)
            {
                Debug.Log("Error in build");
                return false;
            }

            foreach(var assetBundleName in buildManifest.GetAllAssetBundles())
            {
                if (info.onBuild != null)
                {
                    info.onBuild(assetBundleName);
                }
            }
            return true;
        }
    }
}
