
using UnityEditor;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.Rendering;

partial class CameraRenderer
{
#if UNITY_EDITOR
    static ShaderTagId[] legacyShaderTagIds = {
        new ShaderTagId("Always"),
        new ShaderTagId("ForwardBase"),
        new ShaderTagId("PrepassBase"),
        new ShaderTagId("Vertex"),
        new ShaderTagId("VertexLMRGBM"),
        new ShaderTagId("VertexLM")
    };
    
    private static Material errorMaterial;

    
    partial void DrawLegacyShaders()
    {
        if (errorMaterial == null)
        {
            errorMaterial = new Material(Shader.Find("Hidden/InternalErrorShader"));
        }
        var sortingSetting = new SortingSettings(_camera){criteria = SortingCriteria.CommonOpaque};
        var drawingSetting = new DrawingSettings(legacyShaderTagIds[0], sortingSetting)
        {
            overrideMaterial = errorMaterial
        };
        for (int i = 0; i < legacyShaderTagIds.Length; i++)
        {
            drawingSetting.SetShaderPassName(i, legacyShaderTagIds[i]);
        }
        var filteringSetting = new FilteringSettings(RenderQueueRange.opaque);
        _context.DrawRenderers(_cullingResults, ref drawingSetting, ref filteringSetting);
    }

    partial void DrawGizmos()
    {
        if (Handles.ShouldRenderGizmos())
        {
            _context.DrawGizmos(_camera, GizmoSubset.PreImageEffects);
            _context.DrawGizmos(_camera, GizmoSubset.PostImageEffects);
        }
    }

    partial void PrepareForSceneWindow()
    {
        if (_camera.cameraType == CameraType.SceneView)
        {
            ScriptableRenderContext.EmitWorldGeometryForSceneView(_camera);
        }
    }

    partial void PrepareBuffer()
    {
        Profiler.BeginSample("Editor Only");
        _buffer.name = SampleName = _camera.name;
        Profiler.EndSample();
    }
    
    private string SampleName { get; set; }

#else
    const string SampleName = bufferName;
#endif

}