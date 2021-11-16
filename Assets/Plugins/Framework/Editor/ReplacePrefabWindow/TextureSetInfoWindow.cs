using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using Sirenix.OdinInspector;
using System.IO;

public class TextureSetInfoWindow : OdinEditorWindow
{
    [MenuItem("Tools/美术工具/Texture信息查询工具")]
    private static void OpenWindow()
    {

        GetWindow<TextureSetInfoWindow>().Show();
    }

    [Button(20)]
    private void FindTexture()
    {
        FindTextureUnlit();
    }
    [TableList]
    public List<TextureTypes> TextureData;

    public void FindTextureUnlit()
    {
        //Shader shader = Shader.Find(_rplacementShader);
        TextureData = new List<TextureTypes>();
        string[] tempMaterialsPath = AssetDatabase.GetAllAssetPaths();
        for (int i = 0; i < tempMaterialsPath.Length; i++)
        {
            string ext = Path.GetExtension(tempMaterialsPath[i]);
            if (ext == ".png"||ext==".psd"|| ext == ".tga"||ext==".jpg")
            {
                var path = tempMaterialsPath[i];
                var importer = AssetImporter.GetAtPath(path) as TextureImporter;
                var mat = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
                //if (mat.width>=1024)
                //{
                //    Debug.LogError(mat.name + "太大了");
                //}
                var node = new TextureTypes
                {
                    _texture2D = mat,
                    _textureSize = mat.width.ToString() + "X" + mat.height.ToString(),
                    _textureSRGB = importer.sRGBTexture.ToString(),
                    _textureType=importer.textureType.ToString()
                };
                TextureData.Add(node);
            }
            else
            {
                continue;
            }

        }
        //if (tempMaterials.Count != 0)
        //{
        //    for (int i = 0; i < tempMaterials.Count; i++)
        //    {
        //        if (tempMaterials[i] == null)
        //        {
        //            continue;
        //        }
        //if (tempMaterials[i].shader.name == _rplacedmentShader)
        //{
        //    tempMaterials[i].shader = shader;
        //}
    }
}

public class TextureTypes
{
    public Texture2D _texture2D;
    public string _textureSize;
    public string _textureType;
    public string _textureSRGB;
}
