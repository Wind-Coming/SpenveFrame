using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class CameraRenderer
{
    private ScriptableRenderContext _context;
    private Camera _camera;

    const string bufferName = "Render Camera";
    private CommandBuffer _buffer = new CommandBuffer()
    {
        name = "Render_Camera"
    };
    
    public void Render(ScriptableRenderContext context, Camera camera)
    {
        _context = context;
        _camera = camera;

        Setup();
        DrawVisibleGeometry();
        Submit();
    }

    private void Setup()
    {
        _context.SetupCameraProperties(_camera);
        _buffer.ClearRenderTarget(true, true, Color.clear);
        _buffer.BeginSample(bufferName);
        ExcuteBuffer();
    }

    private void DrawVisibleGeometry()
    {
        _context.DrawSkybox(_camera);
    }

    private void Submit()
    {
        _buffer.EndSample(bufferName);
        ExcuteBuffer();
        _context.Submit();
    }

    private void ExcuteBuffer()
    {
        _context.ExecuteCommandBuffer(_buffer);
        _buffer.Clear();
    }
}
