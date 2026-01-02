using UnityEngine;


namespace TextureGeneration
{

    [System.Serializable]
    public class TextureToDrawTo
    {
        [SerializeField, Min(1)] private int _width = 1024;
        [SerializeField, Min(1)] private int _height = 1024;
        [SerializeField] private TextureFormat _format = TextureFormat.ARGB32;
        [SerializeField] private bool _mipChain = true;
        [SerializeField] private bool _linear = true;
        [SerializeField] private bool _createUninitialized = false;
        [SerializeField] private Color _backgroundColor = new Color(1, 1, 1, 0);

        public Texture2D Texture { get; private set; } = null;


        public TextureToDrawTo(int width, int height, Color backgroundColor, 
            TextureFormat format = TextureFormat.ARGB32, bool mipChain = true, bool linear = true, bool createUninitialized = false)
        {
            _width = Mathf.Max(1, width);
            _height = Mathf.Max(1, height);
            _format = format;
            _mipChain = mipChain;
            _linear = linear;
            _createUninitialized = createUninitialized;
            _backgroundColor = backgroundColor;
        }


        public void InitializeTexture()
        {
            Texture = new Texture2D(_width, _height, _format, _mipChain, _linear, _createUninitialized);
            Clear();
        }

        public void Clear()
        {
            Color[] pixels = Texture.GetPixels();
            for (int i = 0; i < pixels.Length; ++i)
            {
                pixels[i] = _backgroundColor;
            }

            Texture.SetPixels(pixels);
            Texture.Apply(updateMipmaps: false);
        }

        public void DrawTile(Vector2Int position, RenderTexture renderTextureToCopy, Vector2Int renderTexturePosition, Vector2Int renderTextureTileSize)
        {
            Graphics.CopyTexture(
                renderTextureToCopy, srcElement: 0, srcMip: 0,
                srcX: renderTexturePosition.x, 
                srcY: renderTexturePosition.y, 
                srcWidth: renderTextureTileSize.x, 
                srcHeight: renderTextureTileSize.y,

                Texture, dstElement: 0, dstMip: 0, 
                dstX: position.x, 
                dstY: position.y
            );
        }
    }

}