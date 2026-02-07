using System.Collections.Generic;
using UnityEngine;



namespace TextureGeneration
{
    public class AlphabetTextureDrawerTester : MonoBehaviour
    {
        [System.Serializable]
        public class Configuration
        {
            [Header("OUTPUT TEXTURE")]
            [SerializeField] private AlphabetTextureDrawer.TexturesPair[] _texturesPairs;

            [Header("ALPHABET")]
            [SerializeField] private AlphabetSO _alphabetSO;

            public AlphabetTextureDrawer.TexturesPair[] TexturesPairs => _texturesPairs;
            public Alphabet Alphabet => _alphabetSO.Alphabet;
        }


        [Header("CONFIGURATION")]
        [SerializeField] private Configuration _configuration;

        [Header("MESH")]
        [SerializeField] private Renderer _displayRenderer;
        [SerializeField] private string _materialTextureProperty = "_BaseColorTexture";
        [SerializeField] private string _materialTextureProperty2 = "_BaseColorTexture";

        [Header("TO DISPLAY")]
        [SerializeField] private bool _autoDrawUpdate = false;
        [SerializeField] private TextureTextEntries _textEntries;


        private AlphabetTextureDrawer _alphabetTextureDrawer;
        private Material _displayMaterial;        
        private float _lastUpdateTimestamp = 0; 



        private void OnValidate()
        {
            _configuration.Alphabet.Validate();
        }

        void Start()
        {
            // Initialize Texture Generator
            _alphabetTextureDrawer = new AlphabetTextureDrawer();
            _alphabetTextureDrawer.Initialize(_configuration.Alphabet, _textEntries, _configuration.TexturesPairs);

            // Initialize Display Material
            _displayMaterial = _displayRenderer.material;
            if (_configuration.TexturesPairs.Length >= 1) _displayMaterial.SetTexture(_materialTextureProperty, _configuration.TexturesPairs[0].TextureToDrawTo.DrawnRenderTexture);
            if (_configuration.TexturesPairs.Length >= 2) _displayMaterial.SetTexture(_materialTextureProperty2, _configuration.TexturesPairs[1].TextureToDrawTo.DrawnRenderTexture);

            DrawTextEntries();
        }

        private void Update()
        {
            if (_autoDrawUpdate)
            {
                float timeSinceLastUpdate = Time.time - _lastUpdateTimestamp;
                if (timeSinceLastUpdate > 1.0f)
                {
                    DrawTextEntries();
                }
            }
        }

        public void DrawTextEntries()
        {
            _alphabetTextureDrawer.DrawTextEntries();
            _lastUpdateTimestamp = Time.time;
        }
    }

}