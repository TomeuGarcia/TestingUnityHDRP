using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;



public class ButtonCustom1 : Selectable
{
    [Space(10)]
    [Header("CUSTOM")]
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField, Min(0)] private float _characterDelay = 0.01f;
    [Header("Content")]
    [SerializeField] private string _promptText = "> ";
    [SerializeField] private string _defaultText = "Example";

    private BondedStrings _bondedStrings;
    private BondedStrings.Iteration _bondedStringsIteration;
    private List<Coroutine> _activeCoroutines;


    protected override void OnValidate()
    {
        if (_text != null)
        {            
            _text.text = _promptText + _defaultText;
        }
    }


    protected override void Awake()
    {
        Initialize();
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);
        StartTextToCapital();
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);
        StartTextToLower();
    }



    private void Initialize()
    {
        BakeTexts();

        string initialText = string.Empty;
        IReadOnlyList<BondedStrings.StringPair> stringPairs = _bondedStrings.StringPairs;
        for (int i = 0; i < stringPairs.Count; ++i)
        {
            initialText += stringPairs[i].EntryA.Value;
        }

        _text.text = initialText;

        _activeCoroutines = new List<Coroutine>(2);
    }


    private void StartTextToCapital()
    {
        StopActiveCoroutines();
        _activeCoroutines.Add(StartCoroutine(ProgressiveToCapital()));
    }
    private void StartTextToLower()
    {
        StopActiveCoroutines();
        _activeCoroutines.Add(StartCoroutine(ProgressiveToLower()));
    }


    private void BakeTexts()
    {
        _bondedStrings = new BondedStrings();

        for (int i = 0; i < _promptText.Length; ++i)
        {
            BakeTextEntry(_promptText, i, ' ');
        }

        for (int i = 0; i < _defaultText.Length; ++i)
        {
            BakeTextEntry(_defaultText, i, ' ');
        }

        _bondedStringsIteration = new BondedStrings.Iteration(_bondedStrings);
    }
    private void BakeTextEntry(string baseText, int baseTextCharacterIndex, char capitalTextExtra)
    {
        string lower = baseText[baseTextCharacterIndex].ToString();
        string capital = char.ToUpper(baseText[baseTextCharacterIndex]).ToString();
        if (baseTextCharacterIndex < baseText.Length - 1) capital += capitalTextExtra;

        _bondedStrings.WithStringPair(new BondedStrings.StringPair(lower, capital));
    }

    private void StopActiveCoroutines()
    {
        for (int i = 0; i < _activeCoroutines.Count; ++i)
        {
            StopCoroutine(_activeCoroutines[i]);
        }
        _activeCoroutines.Clear();
    }


    private IEnumerator ProgressiveToCapital()
    {
        ApplyNextIteration(forward: true, out bool finished);
        while (!finished)
        {
            yield return StartCoroutine(Delay());
            ApplyNextIteration(forward: true, out finished);
        }
    }
    private IEnumerator ProgressiveToLower()
    {
        ApplyNextIteration(forward: false, out bool finished);
        while (!finished)
        {
            yield return StartCoroutine(Delay());
            ApplyNextIteration(forward: false, out finished);
        }
    }

    private IEnumerator Delay()
    {
        if (_characterDelay > 0)    yield return new WaitForSeconds(_characterDelay);
        else                        yield return null;
    }

    private void ApplyNextIteration(bool forward, out bool finished)
    {
        string result;
        if (forward)    ComputeNextIterationForward(out finished, out result);
        else            ComputeNextIterationBackwards(out finished, out result);
        _text.text = result;
    }
    private void ComputeNextIterationForward(out bool finished, out string result)
    {
        _bondedStringsIteration.GetCurrentAndMoveForward(out int currentIndex, out finished);

        IReadOnlyList<BondedStrings.StringPair> stringPairs = _bondedStrings.StringPairs;
        result = string.Empty;
        for (int i = 0; i <= currentIndex; ++i)
        {
            result += stringPairs[i].EntryB.Value;
        }
        for (int i = currentIndex + 1; i < stringPairs.Count; ++i)
        {
            result += stringPairs[i].EntryA.Value;
        }
    }
    private void ComputeNextIterationBackwards(out bool finished, out string result)
    {
        _bondedStringsIteration.GetCurrentAndMoveBackwards(out int currentIndex, out finished);

        IReadOnlyList<BondedStrings.StringPair> stringPairs = _bondedStrings.StringPairs;
        result = string.Empty;
        for (int i = 0; i < currentIndex; ++i)
        {
            result += stringPairs[i].EntryB.Value;
        }
        for (int i = currentIndex; i < stringPairs.Count; ++i)
        {
            result += stringPairs[i].EntryA.Value;
        }
    }


    private class BondedStrings
    {
        public class StringPair
        {
            public class Entry
            {
                public string Value { get; private set; }
                public Entry(string value) { Value = value; }
            }

            public Entry EntryA { get; private set; }
            public Entry EntryB { get; private set; }
            public StringPair(string valueA, string valueB)
            {
                EntryA = new Entry(valueA);
                EntryB = new Entry(valueB);
            }
        }

        private List<StringPair> _stringPairs;
        public IReadOnlyList<StringPair> StringPairs => _stringPairs;
        public BondedStrings(int capacity = 1)
        {
            _stringPairs = new List<StringPair>(Mathf.Max(capacity, 0));
        }
        public void WithStringPair(StringPair stringPair)
        {
            _stringPairs.Add(stringPair);
        }


        public class Iteration
        {
            private BondedStrings _bondedStrings;
            private int _currentIndex;
            public Iteration(BondedStrings bondedStrings)
            {
                _bondedStrings = bondedStrings;
                _currentIndex = 0;
            }

            public void GetCurrentAndMoveForward(out int currentIndex, out bool reachedLast)
            {
                IReadOnlyList<StringPair> stringPairs = _bondedStrings.StringPairs;

                _currentIndex = Mathf.Clamp(_currentIndex, 0, stringPairs.Count - 1);
                currentIndex = _currentIndex;
                _currentIndex = Mathf.Min(_currentIndex + 1, stringPairs.Count - 1);
                reachedLast = currentIndex == stringPairs.Count - 1;
            } 
            public void GetCurrentAndMoveBackwards(out int currentIndex, out bool reachedFirst)
            {
                IReadOnlyList<StringPair> stringPairs = _bondedStrings.StringPairs;

                _currentIndex = Mathf.Clamp(_currentIndex, 0, stringPairs.Count - 1);
                currentIndex = _currentIndex;
                _currentIndex = Mathf.Max(_currentIndex - 1, 0);
                reachedFirst = currentIndex == 0;
            } 
        }
    }
    


}
