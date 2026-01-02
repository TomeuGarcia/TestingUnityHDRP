using UnityEngine;



namespace TextureGeneration
{

    [System.Serializable]
    public class TextureToCopy
    {
        public enum DepthBufferBits { Bits_0, Bits_16, Bits_24, Bits_32 }

        [SerializeField] private Texture2D _texture;
        [SerializeField, Tooltip("Only 24 and 32 have stencil buffer support")] private DepthBufferBits _depthBufferBits = DepthBufferBits.Bits_16;
        [SerializeField] private RenderTextureFormat _renderTextureFormat = RenderTextureFormat.ARGB32;
        [SerializeField] private RenderTextureReadWrite _renderTextureReadWrite = RenderTextureReadWrite.Default;

        public RenderTexture RenderTexture { get; private set; } = null;
       

        public TextureToCopy(Texture2D texture, DepthBufferBits depth, RenderTextureFormat renderTextureFormat, RenderTextureReadWrite renderTextureReadWrite)
        {
            _texture = texture;
            _depthBufferBits = depth;
            _renderTextureFormat = renderTextureFormat;
            _renderTextureReadWrite = renderTextureReadWrite;
        }

        public void InitializeRenderTexture()
        {
            RenderTexture = new RenderTexture(_texture.width, _texture.height, depth: GetDepthBufferBits(), _renderTextureFormat, _renderTextureReadWrite);
            Graphics.Blit(_texture, RenderTexture);
        }

        private int GetDepthBufferBits()
        {
            if (_depthBufferBits == DepthBufferBits.Bits_0)  return  0;
            if (_depthBufferBits == DepthBufferBits.Bits_16) return 16;
            if (_depthBufferBits == DepthBufferBits.Bits_24) return 24;
            if (_depthBufferBits == DepthBufferBits.Bits_32) return 36;
            return 0;
        }
    }

}