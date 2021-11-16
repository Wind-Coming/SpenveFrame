
using UnityEngine;
using UnityEditor;

public class LTextureFormat : MonoBehaviour
{


    [MenuItem("Assets/工具/Format设置/压缩格式批量修改")]
    static void ChangeTextureInfo()
    {
        Object[] textures = GetSelectedTextures();
        foreach (Texture2D texture in textures)
        {
            string path = AssetDatabase.GetAssetPath(texture);
            TextureImporter textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;
            textureImporter.textureType = TextureImporterType.Default;
            textureImporter.textureShape = TextureImporterShape.Texture2D;
            textureImporter.npotScale = TextureImporterNPOTScale.None;
            textureImporter.mipmapEnabled = false;
            textureImporter.isReadable = false;
            AssetDatabase.ImportAsset(path);
        }
        Debug.Log("Default Importer is finish!");
    }

    [MenuItem("Assets/工具/Format设置/AtlasTextureFormat")]
    static void AtlasTextureFormat()
    {
        AndroidTextureFormat_ETC2_8bits();
        IOSTextureFormat_PVRTC_RGBA4();
        Debug.Log("AtlasTextureFormat is finish!");
    }

    //[MenuItem("Assets/工具/Format设置/Android Auto")]
    //static void AndroidTextureFormat_Auto()
    //{
    //    AChangeTextureFormatSettings(TextureImporterFormat.Automatic);
    //}

    [MenuItem("Assets/工具/Format设置/Android eTC2 4 bits")]
    static void AndroidTextureFormat_DXT5()
    {
        AChangeTextureFormatSettings(TextureImporterFormat.ETC2_RGB4);
    }

    [MenuItem("Assets/工具/Format设置/Android ETC2 8 bits")]
    static void AndroidTextureFormat_ETC2_8bits()
    {
        AChangeTextureFormatSettings(TextureImporterFormat.ETC2_RGBA8);
    }

    [MenuItem("Assets/工具/Format设置/IOS ASTC_8X8")]
    static void IOSTextureFormat_PVRTC_RGBA4()
    {
        IAutoChangeTextureFormatSettings(TextureImporterFormat.ASTC_8x8);
    }

    //[MenuItem("Assets/工具/Format设置/IOS RGBA32")]
    //static void IOSTextureFormat_PVRTC_RGBA32()
    //{
    //    IChangeTextureFormatSettings(TextureImporterFormat.ETC2_RGBA8);
    //}

    static void AChangeTextureFormatSettings(TextureImporterFormat tf)
    {
        Object[] textures = GetSelectedTextures();
        foreach (Texture2D texture in textures)
        {
            string path = AssetDatabase.GetAssetPath(texture);
            TextureImporter textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;
            TextureImporterPlatformSettings tp = textureImporter.GetPlatformTextureSettings("Android");
            tp.overridden = true;
            tp.format = tf;
            textureImporter.SetPlatformTextureSettings(tp);
            AssetDatabase.ImportAsset(path);
        }
        Debug.Log("[Android] >>> TextureFormatSettings is finish!");
    }

    static void IChangeTextureFormatSettings(TextureImporterFormat tf)
    {
        Object[] textures = GetSelectedTextures();
        foreach (Texture2D texture in textures)
        {

            string path = AssetDatabase.GetAssetPath(texture);
            TextureImporter textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;
            TextureImporterPlatformSettings tp = textureImporter.GetPlatformTextureSettings("iPhone");
            tp.overridden = true;
            tp.format = tf;
            textureImporter.SetPlatformTextureSettings(tp);
            AssetDatabase.ImportAsset(path);
        }
        Debug.Log("[IOS] >>> TextureFormatSettings is finish!");
    }

    static void IAutoChangeTextureFormatSettings(TextureImporterFormat tf)
    {
        Object[] textures = GetSelectedTextures();
        foreach (Texture2D texture in textures)
        {
            string path = AssetDatabase.GetAssetPath(texture);
            TextureImporter textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;
            TextureImporterPlatformSettings tp = textureImporter.GetPlatformTextureSettings("iPhone");
            tp.overridden = true;
            tp.format = texture.width == texture.height ? tf : TextureImporterFormat.RGBA32;
            textureImporter.SetPlatformTextureSettings(tp);
            AssetDatabase.ImportAsset(path);
        }
        Debug.Log("[IOS] >>> TextureFormatSettings is finish!");
    }

    static Object[] GetSelectedTextures()
    {
        return Selection.GetFiltered(typeof(Texture2D), SelectionMode.DeepAssets);
    }

}


