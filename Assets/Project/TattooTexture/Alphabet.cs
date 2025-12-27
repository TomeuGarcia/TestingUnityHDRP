using System.Collections.Generic;
using UnityEngine;



namespace TextureGeneration
{
    [System.Serializable]
    public class Alphabet
    {        
        [SerializeField] private Texture2D _alphabetTexture;
        [SerializeField] private RenderTextureFormat _renderTextureFormat = RenderTextureFormat.ARGB32;
        [SerializeField] private RenderTextureReadWrite _renderTextureReadWrite = RenderTextureReadWrite.Default;
        [SerializeField, Min(1)] private int _rows = 5;
        [SerializeField, Min(1)] private int _columns = 6;

        [SerializeField] private List<AlphabetElement> _elements;

        public Texture2D AlphabetTexture => _alphabetTexture;
        public RenderTextureFormat RenderTextureFormat => _renderTextureFormat;
        public RenderTextureReadWrite RenderTextureReadWrite => _renderTextureReadWrite;
        public int Rows => _rows;
        public int Columns => _columns;
        public int PixelsPerRow => _alphabetTexture == null ? 0 : _alphabetTexture.height / _rows;
        public int PixelsPerColumn => _alphabetTexture == null ? 0 : _alphabetTexture.width / _columns;
        public Vector2Int TextureTileSize => new Vector2Int(PixelsPerColumn, PixelsPerRow);


        public void Validate()
        {
            int currentTotalElements = _elements.Count;
            int desiredTotalElements = _rows * _columns;
            char firstCharacter = currentTotalElements == 0 ? 'A' : _elements[0].Character;

            for (int i = currentTotalElements; i < desiredTotalElements; ++i)
            {
                _elements.Add(new AlphabetElement((char)(firstCharacter + i)));
            }

            while (_elements.Count > desiredTotalElements)
            {
                _elements.RemoveAt(_elements.Count - 1);
            }
        }

        public void Setup()
        {
            for (int i = 0; i < _elements.Count; ++i)
            {
                int rowIndex = i / _columns;
                int columnIndex = i % _columns;

                _elements[i].Setup(rowIndex, columnIndex);
            }
        }

        public bool GetElement(char character, out AlphabetElement element)
        {
            for (int i = 0; i < _elements.Count; ++i)
            {
                if (_elements[i].Character == character)
                {
                    element = _elements[i];
                    return true;
                }
            }

            element = null;
            return false;
        }
    }




    [System.Serializable]
    public class AlphabetElement
    {
        [SerializeField] private int _character = 'A';
        [SerializeField, Range(0.0f, 1.0f)] private float _widthRatio = 1.0f;

        public char Character => (char)_character;
        public float WidthRatio => _widthRatio;
        public int RowIndex { get; private set; }
        public int ColumnIndex { get; private set; }


        public AlphabetElement(char character)
        {
            _character = character;
            _widthRatio = 1.0f;
        }

        public void Setup(int rowIndex, int columnIndex)
        {
            RowIndex = rowIndex;
            ColumnIndex = columnIndex;
        }
    }

}

