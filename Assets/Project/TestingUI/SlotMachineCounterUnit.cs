using DG.Tweening;
using System;
using System.Collections;
using TMPro;
using UnityEngine;



public class SlotMachineCounterUnit : MonoBehaviour
{
    [System.Serializable]
    public class Configuration
    {
        [SerializeField, Min(0)] public float baseDuration = 0.5f;
        [SerializeField, Min(0)] public float hotQueueDuration = 0.01f;
        [SerializeField, Min(2)] public float hotQueueCount = 10;
        [SerializeField] public Ease ease = Ease.OutCirc;
    }

    [SerializeField] private TextMeshProUGUI _textA;
    [SerializeField] private TextMeshProUGUI _textB;
    [SerializeField] private Transform _topAnchor;
    [SerializeField] private Transform _centerAnchor;
    [SerializeField] private Transform _bottomAnchor;

    private Configuration _configuration;
    private int _currentNumber;
    private int _queueCount;

    private Action<SlotMachineCounterUnit> _onStartOverflowingMax;
    private Action<SlotMachineCounterUnit> _onStartOverflowingMin;



    public void Initialize(Configuration configuration, int startingNumber, 
        Action<SlotMachineCounterUnit> onStartOverflowingMax, Action<SlotMachineCounterUnit> onStartOverflowingMin)
    {
        _configuration = configuration;
        _currentNumber = startingNumber;
        _queueCount = 0;
        _onStartOverflowingMax = onStartOverflowingMax;
        _onStartOverflowingMin = onStartOverflowingMin;

        _textA.text = startingNumber.ToString();
        _textA.transform.position = _centerAnchor.position;
        _textB.text = string.Empty;
        _textB.transform.position = _bottomAnchor.position;
    }

    public void QueueUpdate(int signedAddAmount)
    {
        if (signedAddAmount == 0)
        {
            return;
        }

        bool queueIsStarting = Mathf.Abs(_queueCount) == 0;
        _queueCount += signedAddAmount;

        if (queueIsStarting)
        {
            StartCoroutine(UpdateLoop());
        }
    }


    private IEnumerator UpdateLoop()
    {
        yield return new WaitForEndOfFrame();

        while (_queueCount != 0)
        {
            while (_queueCount > 0)
            {
                float duration = GetNumberChangeDuration();

                int previousNumber = _currentNumber;
                _currentNumber = (_currentNumber + 1) % 10;

                PlayTextAnimation(isIncrement: true, duration, _currentNumber, previousNumber);

                if (_currentNumber == 0)
                {
                    _onStartOverflowingMax?.Invoke(this);
                }

                yield return new WaitForSeconds(duration);

                --_queueCount;
            }

            while (_queueCount < 0)
            {
                float duration = GetNumberChangeDuration();

                int previousNumber = _currentNumber;
                _currentNumber = ((_currentNumber - 1) + 10) % 10;

                PlayTextAnimation(isIncrement: false, duration, _currentNumber, previousNumber);

                if (_currentNumber == 9)
                {
                    _onStartOverflowingMin?.Invoke(this);
                }

                yield return new WaitForSeconds(duration);

                ++_queueCount;
            }
        }        
    }

    private void PlayTextAnimation(bool isIncrement, float duration, int currentNumber, int previousNumber)
    {
        Transform origin_TextA = isIncrement ? _bottomAnchor : _topAnchor;
        Transform target_TextB = isIncrement ? _topAnchor : _bottomAnchor;

        _textA.text = currentNumber.ToString();
        _textA.transform.position = origin_TextA.position;
        _textA.transform.DOKill();
        _textA.transform.SetParent(_centerAnchor);
        _textA.transform.DOLocalMove(Vector3.zero, duration).SetEase(_configuration.ease);

        _textB.text = previousNumber.ToString();
        _textB.transform.position = _centerAnchor.position;
        _textB.transform.DOKill();
        _textB.transform.SetParent(target_TextB);
        _textB.transform.DOLocalMove(Vector3.zero, duration).SetEase(_configuration.ease);
    }



    private float GetNumberChangeDuration()
    {
        int queueCount = Mathf.Abs(_queueCount);
        float queueT = Mathf.Clamp01(Mathf.InverseLerp(1, _configuration.hotQueueCount, queueCount));
        float duration = Mathf.LerpUnclamped(_configuration.baseDuration, _configuration.hotQueueDuration, queueT);
        return duration;
    }


    public int GetCurrentNumber()
    {
        return _currentNumber;
    }
    public int GetDesiredNumber()
    {
        int plus = Mathf.Max(0, Mathf.Max(0, _queueCount) - 1);
        int minus = Mathf.Max(0, Mathf.Max(0, -_queueCount) - 1) % 10;
        int desiredNumber = ((_currentNumber + plus - minus) + 10) % 10;
        return desiredNumber;
    }
    public int GetSignedQueuedCount()
    {
        return _queueCount;
    }

    public void SetTextColor(Color color)
    {
        _textA.color = color;
        _textB.color = color;
    }
    public Color GetTextColor()
    {
        return _textA.color;
    }

}
