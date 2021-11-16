using System;
using UnityEditor;
using UnityEngine;
public class Terrain_4Normal_Inspector : ShaderGUI
{
    //切换PBR和兰伯特光照
    //MaterialProperty _UseSampleDiffuse = null;
    //材质属性
    MaterialProperty _TexNum = null;
    MaterialProperty _Splat0 = null;
    MaterialProperty _Normal1 = null;
    MaterialProperty _uv1 = null;

    MaterialProperty _Splat1 = null;
    MaterialProperty _Normal2 = null;
    MaterialProperty _uv2 = null;

    MaterialProperty _Splat2 = null;
    MaterialProperty _Normal3 = null;
    MaterialProperty _uv3 = null;

    MaterialProperty _Splat3 = null;
    MaterialProperty _Normal4 = null;
    MaterialProperty _uv4 = null;

    MaterialProperty _maskMap = null;

    MaterialEditor _Editor = null;
    Material TargetMat = null;
    //查找shader中对应的属性
    public void FindProperties(MaterialProperty[] props)
    {
        _TexNum = FindProperty("_TexNum", props);

        _Splat0 = FindProperty("_tex1", props);
        _Normal1 = FindProperty("_Normal1", props);
        _uv1 = FindProperty("_uv1", props);

        _Splat1 = FindProperty("_tex2", props);
        _Normal2 = FindProperty("_Normal2", props);
        _uv2 = FindProperty("_uv2", props);

        _Splat2 = FindProperty("_tex3", props);
        _Normal3 = FindProperty("_Normal3", props);
        _uv3 = FindProperty("_uv3", props);

        _Splat3 = FindProperty("_tex4", props);
        _Normal4 = FindProperty("_Normal4", props);
        _uv4 = FindProperty("_uv4", props);

        _maskMap = FindProperty("_maskMap", props);
        //切换PBR和兰伯特光照
        //_UseSampleDiffuse = FindProperty("_UseSampleDiffuse", props);
    }
    //当材质从其他shader切换到此shader时执行
    public override void AssignNewShaderToMaterial(Material material, Shader oldShader, Shader newShader)
    {
        base.AssignNewShaderToMaterial(material, oldShader, newShader);
    }

    //基础设置
    public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
    {
        _Editor = materialEditor;
        TargetMat = materialEditor.target as Material;
        //使用默认UI宽度
        _Editor.SetDefaultGUIWidths();

        //初始化属性
        FindProperties(properties);

        //切换PBR和兰伯特光照
        //_Editor.ShaderProperty(_UseSampleDiffuse, "使用简单光照");

        //画各属性界面
        _Editor.TextureProperty(_maskMap, "遮罩贴图");
        TexNum();
        _Editor.RenderQueueField();
    }
    private void TexNum()
    {
        _Editor.ShaderProperty(_TexNum, "贴图数量");
        EditorGUILayout.Space();
        Map(_Splat0, "材质1", _Normal1, "法线1", _uv1, "UV");
        Map(_Splat1, "材质2", _Normal2, "法线2", _uv2, "UV");
        if (TargetMat.GetFloat("_TexNum") == 1)
        {
            Map(_Splat2, "材质3", _Normal3, "法线3", _uv3, "UV");
        }
        else if (TargetMat.GetFloat("_TexNum") == 2)
        {
            Map(_Splat2, "材质3", _Normal3, "法线3", _uv3, "UV");
            Map(_Splat3, "材质4", _Normal4, "法线4", _uv4, "UV");
        }
    }
    private void Map(MaterialProperty _tex, string _texName, MaterialProperty _tex_normal, string _normalName, MaterialProperty _uv, string _uvName)
    {
        _Editor.TextureProperty(_tex, _texName);
        _Editor.TextureProperty(_tex_normal, _normalName);
        _Editor.ShaderProperty(_uv, _uvName);
        EditorGUILayout.Space();
    }
}