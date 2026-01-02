using System.Collections.Generic;
using UnityEngine;


namespace TextureGeneration
{

    [System.Serializable]
    public class TextureTextEntries
    {
        [SerializeField] private List<TextureTextEntry> _entries = new List<TextureTextEntry>(new[] { new TextureTextEntry("ABC", Vector2Int.zero) });
        public List<TextureTextEntry> Entries => _entries;
    }


    [System.Serializable]
    public class TextureTextEntry
    {
        [SerializeField] private string _text;
        [SerializeField] private Vector2Int _offset;

        public Vector2Int Offset => _offset;

        public TextureTextEntry(string text, Vector2Int offset)
        {
            _text = text;
            _offset = offset;
        }

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

}