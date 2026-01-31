using UnityEngine;


namespace TextureGeneration
{

    [System.Serializable]
    public class TextureToDrawTo
    {
        [SerializeField, Min(1)] private int _width = 1024;
        [SerializeField, Min(1)] private int _height = 1024;
        [SerializeField] private RenderTextureReadWrite _readWrite = RenderTextureReadWrite.Default;
        [SerializeField] private Color _backgroundColor = new Color(1, 1, 1, 0);

        public RenderTexture DrawnRenderTexture { get; private set; } = null;


        public TextureToDrawTo(int width, int height, RenderTextureReadWrite readWrite, Color backgroundColor)
        {
            _width = Mathf.Max(1, width);
            _height = Mathf.Max(1, height);
            _readWrite = readWrite;
            _backgroundColor = backgroundColor;
        }


        public void InitializeTexture(RenderTexture renderTextureToCopy)
        {
            DrawnRenderTexture = new RenderTexture(_width, _height, renderTextureToCopy.depth, renderTextureToCopy.format, _readWrite);
            Clear();
        }

        public void Clear()
        {
            RenderTexture originalActiveRenderTexture = RenderTexture.active;
            RenderTexture.active = DrawnRenderTexture;
            GL.Clear(clearDepth: true, clearColor: true, _backgroundColor);
            RenderTexture.active = RenderTexture.active;            
        }

        public void DrawTile(Vector2Int position, RenderTexture renderTextureToCopy, Vector2Int renderTexturePosition, Vector2Int renderTextureTileSize)
        {
            Graphics.CopyTexture(
                renderTextureToCopy, srcElement: 0, srcMip: 0,
                srcX: renderTexturePosition.x,
                srcY: renderTexturePosition.y,
                srcWidth: renderTextureTileSize.x,
                srcHeight: renderTextureTileSize.y,

                DrawnRenderTexture, dstElement: 0, dstMip: 0,
                dstX: position.x,
                dstY: position.y
            );
        }
    }

}