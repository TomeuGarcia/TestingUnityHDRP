using System.Collections.Generic;
using UnityEngine;


namespace TextureGeneration
{

    public class AlphabetTextureDrawer
    {

        [System.Serializable]
        public class TexturesPair
        {
            [SerializeField] private TextureToCopy _alphabetTextureToCopy;
            [SerializeField] private TextureToDrawTo _textureToDrawTo;

            public TextureToCopy AlphabetTextureToCopy => _alphabetTextureToCopy;
            public TextureToDrawTo TextureToDrawTo => _textureToDrawTo;

            public TexturesPair(TextureToCopy alphabetTextureToCopy, TextureToDrawTo textureToDrawTo)
            {
                _alphabetTextureToCopy = alphabetTextureToCopy;
                _textureToDrawTo = textureToDrawTo;
            }
        }


        private Alphabet _alphabet;
        private TextureTextEntries _textureTextEntries;
        private TexturesPair[] _texturesPairs;

        private TextureToCopy Current_AlphabetTextureToCopy { get; set; }
        private TextureToDrawTo Current_TextureToDrawTo { get; set; }


        public AlphabetTextureDrawer()
        {            
        }

        public void Initialize(Alphabet alphabet, TextureTextEntries textureTextEntries, TexturesPair[] texturesPairs)
        {
            _alphabet = alphabet;
            _textureTextEntries = textureTextEntries;
            _texturesPairs = texturesPairs;

            foreach (TexturesPair texturesPair in _texturesPairs)
            {
                texturesPair.AlphabetTextureToCopy.InitializeRenderTexture();
                texturesPair.TextureToDrawTo.InitializeTexture(texturesPair.AlphabetTextureToCopy.RenderTexture);
            }            
        }

        public void DrawTextEntries()
        {
            _alphabet.Setup();

            foreach (TexturesPair texturesPair in _texturesPairs)
            {
                Current_AlphabetTextureToCopy = texturesPair.AlphabetTextureToCopy;
                Current_TextureToDrawTo = texturesPair.TextureToDrawTo;

                Current_TextureToDrawTo.Clear();
                for (int i = 0; i < _textureTextEntries.Entries.Count; ++i)
                {
                    DrawTextEntry(_textureTextEntries.Entries[i]);
                }
            }

            Current_AlphabetTextureToCopy = null;
            Current_TextureToDrawTo = null;
        }

        private void DrawTextEntry(TextureTextEntry textEntry)
        {
            List<AlphabetElement> alphabetText = textEntry.TextToAlphabetEntries(_alphabet);
            Vector2Int accumulatedPosition = textEntry.Offset;

            int alphabetTextureTileWidth = Current_AlphabetTextureToCopy.RenderTexture.width / _alphabet.Columns;
            int alphabetTextureTileHeight = Current_AlphabetTextureToCopy.RenderTexture.height / _alphabet.Rows;
            Vector2Int alphabetTextureTileSize = new Vector2Int(alphabetTextureTileWidth, alphabetTextureTileHeight);

            for (int i = 0; i < alphabetText.Count; ++i)
            {
                AlphabetElement alphabetElement = alphabetText[i];
                DrawAlphabetElement(alphabetElement, accumulatedPosition, alphabetTextureTileSize);
                accumulatedPosition.x += Mathf.RoundToInt(alphabetTextureTileWidth * alphabetElement.WidthRatio);
            }
        }

        private void DrawAlphabetElement(AlphabetElement alphabetElement, Vector2Int targetTexturePosition, Vector2Int alphabetTextureTileSize)
        {
            int exceedingWidth = Mathf.RoundToInt(alphabetTextureTileSize.x * (1.0f - alphabetElement.WidthRatio));
            int halfExceedingWidth = exceedingWidth / 2;

            Vector2Int alphabetTexturePosition = new Vector2Int((alphabetTextureTileSize.x * alphabetElement.ColumnIndex) + halfExceedingWidth,
                                                                alphabetTextureTileSize.y * (_alphabet.Rows - alphabetElement.RowIndex - 1));
            alphabetTextureTileSize.x -= exceedingWidth;

            Draw(targetTexturePosition, alphabetTextureTileSize, alphabetTexturePosition);
        }

        private void Draw(Vector2Int targetTexturePosition, Vector2Int alphabetTextureTileSize, Vector2Int alphabetTexturePosition)
        {
            targetTexturePosition.y = Current_TextureToDrawTo.DrawnRenderTexture.height - targetTexturePosition.y - alphabetTextureTileSize.y;
            RenderTexture renderTextureToCopy = Current_AlphabetTextureToCopy.RenderTexture;

            Debug.Log($"Drawing   from position: {alphabetTexturePosition}, with tile size {alphabetTextureTileSize},     to position {targetTexturePosition}");

            Current_TextureToDrawTo.DrawTile(targetTexturePosition, renderTextureToCopy, alphabetTexturePosition, alphabetTextureTileSize);
        }
    }

}