using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public partial class CameraRenderer
{
    private ScriptableRenderContext _context;
    private Camera _camera;

    const string bufferName = "Render Camera";
    private CommandBuffer _buffer = new CommandBuffer()
    {
        name = bufferName
    };

    private CullingResults _cullingResults;
    private static ShaderTagId unlitShaderTagId = new ShaderTagId("SRPDefaultUnlit");
    
    public void Render(ScriptableRenderContext context, Camera camera)
    {
        _context = context;
        _camera = camera;

        PrepareBuffer();
        PrepareForSceneWindow();
        
        if(!Cull())
            return;

        Setup();
        DrawVisibleGeometry();
        DrawLegacyShaders();
        DrawGizmos();
        Submit();
    }

    private bool Cull()
    {
        if (_camera.TryGetCullingParameters(out ScriptableCullingParameters p))
        {
            _cullingResults = _context.Cull(ref p);
            return true;
        }

        return false;
    }

    private void Setup()
    {
        _context.SetupCameraProperties(_camera);
        CameraClearFlags flag = _camera.clearFlags;
        _buffer.ClearRenderTarget(flag < CameraClearFlags.Nothing, flag >= CameraClearFlags.Skybox && flag <= CameraClearFlags.SolidColor,
            flag == CameraClearFlags.SolidColor ? _camera.backgroundColor.linear : Color.clear);
        _buffer.BeginSample(SampleName);
        ExcuteBuffer();
    }

    private void DrawVisibleGeometry()
    {
        var sortingSetting = new SortingSettings(_camera){criteria = SortingCriteria.CommonOpaque};
        var drawingSetting = new DrawingSettings(unlitShaderTagId, sortingSetting);
        var filteringSetting = new FilteringSettings(RenderQueueRange.opaque);
        _context.DrawRenderers(_cullingResults, ref drawingSetting, ref filteringSetting);
        
        _context.DrawSkybox(_camera);

        sortingSetting.criteria = SortingCriteria.CommonTransparent;
        drawingSetting.sortingSettings = sortingSetting;
        filteringSetting.renderQueueRange = RenderQueueRange.transparent;
        _context.DrawRenderers(_cullingResults, ref drawingSetting, ref filteringSetting);
    }

    partial void DrawLegacyShaders();
    partial void DrawGizmos();
    partial void PrepareForSceneWindow();
    partial void PrepareBuffer();

    private void Submit()
    {
        _buffer.EndSample(SampleName);
        ExcuteBuffer();
        _context.Submit();
    }

    private void ExcuteBuffer()
    {
        _context.ExecuteCommandBuffer(_buffer);
        _buffer.Clear();
    }
}
