using System.Collections.Generic;
using UnityEngine;



namespace TextureGeneration
{
    public class DynamicTextureGenerator : MonoBehaviour
    {
        [System.Serializable]
        public class Configuration
        {
            [Header("OUTPUT TEXTURE")]
            [SerializeField, Min(0)] public int textureWidth = 1024;
            [SerializeField, Min(0)] public int textureHeight = 1024;
            [SerializeField] public TextureFormat textureFormat = TextureFormat.ARGB32;
            [SerializeField] public bool mipChain = true;
            [SerializeField] public bool linear = true;
            [SerializeField] public bool createUninitialized = false;
            [SerializeField] public Color backgroundColor = Color.LerpUnclamped(Color.white, Color.cyan, 0.1f);

            [Header("ALPHABET")]
            [SerializeField] public Alphabet alphabet;
        }


        [System.Serializable]
        public class TextEntry
        {
            [SerializeField] private string _text;
            [SerializeField] private Vector2Int _offset;

            public Vector2Int Offset => _offset;

            public List<AlphabetElement> TextToAlphabetEntries(Alphabet alphabet)
            {
                List<AlphabetElement> alphabetText = new List<AlphabetElement>(_text.Length);
                for (int i = 0; i < _text.Length; ++i)
                {
                    if (alphabet.GetElement(_text[i], out AlphabetElement element))
                    {
                        alphabetText.Add(element);
                    }
                }

                return alphabetText;
            }
        }



        [Header("CONFIGURATION")]
        [SerializeField] private Configuration _configuration;

        [Header("MESH")]
        [SerializeField] private Renderer _displayRenderer;
        [SerializeField] private string _materialTextureProperty = "_BaseColorTexture";

        [Header("TO DISPLAY")]
        [SerializeField] private bool _autoUpdateBake = false;
        [SerializeField] private TextEntry[] _textEntries;


        private Material _displayMaterial;
        private RenderTexture _alphabetRenderTexture;
        private Texture2D _dynamicTexture_BaseColor;
        
        private float _lastUpdateTimestamp = 0; 


        private void OnValidate()
        {
            _configuration.alphabet.Validate();
        }

        void Start()
        {
            GenerateTexture();
            GenerateMaterial();
        }

        private void Update()
        {
            if (_autoUpdateBake)
            {
                float timeSinceLastUpdate = Time.time - _lastUpdateTimestamp;
                if (timeSinceLastUpdate > 1.0f)
                {
                    BakeTextEntries();
                    _lastUpdateTimestamp = Time.time;
                }
            }
        }

        private void GenerateTexture()
        {
            _alphabetRenderTexture = new RenderTexture(_configuration.alphabet.AlphabetTexture.width, _configuration.alphabet.AlphabetTexture.height, depth: 16,
                _configuration.alphabet.RenderTextureFormat, _configuration.alphabet.RenderTextureReadWrite);
            Graphics.Blit(_configuration.alphabet.AlphabetTexture, _alphabetRenderTexture);

            _dynamicTexture_BaseColor = new Texture2D(_configuration.textureWidth, _configuration.textureHeight,
                _configuration.textureFormat, _configuration.mipChain, _configuration.linear, _configuration.createUninitialized);

            Clear();
            Graphics.CopyTexture(
                _alphabetRenderTexture, srcElement: 0, srcMip: 0, srcX: 50, srcY: 100, srcWidth: 100, srcHeight: 300,
                _dynamicTexture_BaseColor, dstElement: 0, dstMip: 0, dstX: 0, dstY: 20
            );
        }

        private void GenerateMaterial()
        {
            _displayMaterial = _displayRenderer.material;
            _displayMaterial.SetTexture(_materialTextureProperty, _dynamicTexture_BaseColor);
        }


        public void BakeTextEntries()
        {
            Clear();
            SetupAlphabet();

            for (int i = 0; i < _textEntries.Length; ++i)
            {
                BakeTextEntry(_configuration.alphabet, _textEntries[i]);
            }
        }

        private void Clear()
        {
            Color[] pixels = _dynamicTexture_BaseColor.GetPixels();
            for (int i = 0; i < pixels.Length; ++i)
            {
                pixels[i] = _configuration.backgroundColor;
            }

            _dynamicTexture_BaseColor.SetPixels(pixels);
            _dynamicTexture_BaseColor.Apply(updateMipmaps: false);
        }

        private void SetupAlphabet()
        {
            _configuration.alphabet.Setup();
        }

        private void BakeTextEntry(Alphabet alphabet, TextEntry textEntry)
        {
            List<AlphabetElement> alphabetText = textEntry.TextToAlphabetEntries(alphabet);
            Vector2Int accumulatedPosition = textEntry.Offset;

            for (int i = 0; i < alphabetText.Count; ++i)
            {
                AlphabetElement alphabetElement = alphabetText[i];
                BakeAlphabetElement(alphabet, alphabetElement, accumulatedPosition);
                accumulatedPosition.x += Mathf.RoundToInt(alphabet.PixelsPerColumn * alphabetElement.WidthRatio);
            }
        }

        private void BakeAlphabetElement(Alphabet alphabet, AlphabetElement alphabetElement, Vector2Int targetTexturePosition)
        {
            Vector2Int alphabetTextureTileSize = alphabet.TextureTileSize;
            int exceedingWidth = Mathf.RoundToInt(alphabetTextureTileSize.x * (1.0f - alphabetElement.WidthRatio));
            int halfExceedingWidth = exceedingWidth / 2;

            Vector2Int alphabetTexturePosition = new Vector2Int((alphabetTextureTileSize.x * alphabetElement.ColumnIndex) + halfExceedingWidth,
                                                                alphabetTextureTileSize.y * (alphabet.Rows - alphabetElement.RowIndex - 1));
            alphabetTextureTileSize.x -= exceedingWidth;

            Draw(targetTexturePosition, alphabetTextureTileSize, alphabetTexturePosition);
        }

        private void Draw(Vector2Int targetTexturePosition, Vector2Int alphabetTextureTileSize, Vector2Int alphabetTexturePosition)
        {
            targetTexturePosition.y = _dynamicTexture_BaseColor.height - targetTexturePosition.y - alphabetTextureTileSize.y;

            Graphics.CopyTexture(
                _alphabetRenderTexture, srcElement: 0, srcMip: 0,
                srcX: alphabetTexturePosition.x, srcY: alphabetTexturePosition.y, srcWidth: alphabetTextureTileSize.x, srcHeight: alphabetTextureTileSize.y,

                _dynamicTexture_BaseColor, dstElement: 0, dstMip: 0,
                dstX: targetTexturePosition.x, dstY: targetTexturePosition.y
            );
        }

    }



}