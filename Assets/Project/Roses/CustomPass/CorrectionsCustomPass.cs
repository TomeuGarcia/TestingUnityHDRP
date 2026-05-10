using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;




public class CorrectionsCustomPass : FullScreenCustomPass
{
    public static string LogInfo_1 { get; private set; }
    public static string LogInfo_2 { get; private set; }
    public static string LogInfo_3 { get; private set; }

    private static int Sizes_Viewport_RenderTarget_ID = Shader.PropertyToID("_Sizes_Viewport_RenderTarget");


    protected override void Execute(CustomPassContext ctx)
    {
        base.Execute(ctx);
        
        Camera.main.allowMSAA = false;

        float scalableBuffer_widthScale = ScalableBufferManager.widthScaleFactor;
        float scalableBuffer_heightScale = ScalableBufferManager.heightScaleFactor;

        int maxWidth = RTHandles.maxWidth;
        int maxHeight = RTHandles.maxHeight;

        RTHandleProperties properties = RTHandles.rtHandleProperties;
        Vector4 rtHandleScale = properties.rtHandleScale; // Scale = Viewport / RenderTarget
        Vector2Int viewportSize = properties.currentViewportSize;
        Vector2Int renderTargetSize = properties.currentRenderTargetSize;

        LogInfo_1 = $"ScalableBufferManager scale factors:   " +
            $"{scalableBuffer_widthScale}x{scalableBuffer_heightScale}";

        LogInfo_2 = $"Max Width Height:   " +
            $"{maxWidth}x{maxHeight}";

        LogInfo_3 = $"RTHandleProperties:   " +
            $"Scale: {rtHandleScale.x}x{rtHandleScale.y}   |   " +
            $"Viewport: {viewportSize.x}x{viewportSize.y}   |   " +
            $"Target: {renderTargetSize.x}x{renderTargetSize.y}";

        Debug.Log(LogInfo_1);
        Debug.Log(LogInfo_2);
        Debug.Log(LogInfo_3);

        Vector4 sizes_Viewport_RenderTarget = new Vector4(viewportSize.x, viewportSize.y,   renderTargetSize.x, renderTargetSize.y);
        Shader.SetGlobalVector(Sizes_Viewport_RenderTarget_ID, sizes_Viewport_RenderTarget);
    }
}
