using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace AssetBundleBrowser
{
    public class RefreshAssetConfig : BasicPipeline
    {
        private BuildTarget buildTarget;
        private GUIContent m_TargetContent;
        
        public RefreshAssetConfig()
        {
            name = "刷新AssetConfig";
            tip = @"刷新AssetConfig";

            canDisable = false;
            configable = true;
            showConfig = true;
        }
        
        public override int Process(Dictionary<string, object> objectInPipeline)
        {            
            AssetBundleSettingTab.RefreshFiles();
            return 0;
        }
    }
}
