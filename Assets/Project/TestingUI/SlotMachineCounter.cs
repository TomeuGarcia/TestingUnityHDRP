using DG.Tweening;
using System.Collections;
using UnityEngine;




public class SlotMachineCounter : MonoBehaviour
{
    [System.Serializable]
    public class Configuration
    {
        [System.Serializable]
        public class State
        {
            [SerializeField] public Color targetColor;
            [SerializeField, Min(0)] public float targetColorReachTime = 0.25f;
            [SerializeField] public Ease targetColorReachEase = Ease.InOutSine;
        }
        [System.Serializable]
        public class StateChange
        {
            [SerializeField] public State state;
            [SerializeField, Min(0)] public float targetColorReachedDeadTime = 1.0f;
        }


        [Header("UNITS")]
        [SerializeField] public SlotMachineCounterUnit.Configuration unit;

        [Header("STATES")]
        [SerializeField] public State atRestState;
        [SerializeField] public StateChange incrementingStateChange;
        [SerializeField] public StateChange decrementingStateChange;
    }

    private enum State { AtRest, Incrementing, Decrementing };



    [Header("CONFIGURATION")]
    [SerializeField] private Configuration _configuration;
    [Space(8)]
    [SerializeField, Range(0, 999)] private int _startingValue = 130;
    [Space(8)]
    [SerializeField] private KeyCode _testKey_Increment = KeyCode.I;
    [SerializeField] private KeyCode _testKey_Decrement = KeyCode.K;
    [SerializeField, Min(1)] private int _testTimes = 10;

    [Space(16)]
    [Header("COMPONENTS")]
    [SerializeField] private SlotMachineCounterUnit _unitUnits;
    [SerializeField] private SlotMachineCounterUnit _unitTens;
    [SerializeField] private SlotMachineCounterUnit _unitHundreds;

    private SlotMachineCounterUnit[] _allUnits;
    private Coroutine _viewUpdateCoroutine;



    private void Awake()
    {
        _viewUpdateCoroutine = null;
        _allUnits = new SlotMachineCounterUnit[] { _unitUnits, _unitTens, _unitHundreds };
        for (int i = 0; i < _allUnits.Length; ++i)
        {
            int place = Mathf.RoundToInt(Mathf.Pow(10, i));
            int startingNumber = (_startingValue / place) % 10;
            _allUnits[i].Initialize(_configuration.unit, startingNumber, OnCounterOverflewMax, OnCounterOverflewMin);
            _allUnits[i].SetTextColor(_configuration.atRestState.targetColor);
        }
    }


    private void Update()
    {
        if (Input.GetKeyDown(_testKey_Increment))
        {
            AddToCounter(_testTimes);
        }
        else if (Input.GetKeyDown(_testKey_Decrement))
        {
            AddToCounter(-_testTimes);
        }
    }


    private void OnCounterOverflewMax(SlotMachineCounterUnit overflownUnit)
    {
        PropagateOverflowToNextUnit(overflownUnit, increment: true);
    }
    private void OnCounterOverflewMin(SlotMachineCounterUnit overflownUnit)
    {
        PropagateOverflowToNextUnit(overflownUnit, increment: false);
    }
    private void PropagateOverflowToNextUnit(SlotMachineCounterUnit overflownUnit, bool increment)
    {
        if (_allUnits.Length < 2)
        {
            return;
        }

        for (int i = 0; i < _allUnits.Length; ++i)
        {
            if (_allUnits[i] == overflownUnit)
            {
                int nextUnitIndex = (i + 1) % _allUnits.Length;
                SlotMachineCounterUnit nextUnit = _allUnits[nextUnitIndex];
                if (increment)  nextUnit.QueueUpdate(1);
                else            nextUnit.QueueUpdate(-1);
                return;
            }
        }
    }


    private int ComputeCurrentValue()
    {
        int value = 0;
        for (int i = 0; i < _allUnits.Length; ++i)
        {
            int desiredNumber = _allUnits[i].GetDesiredNumber();
            int place = Mathf.RoundToInt(Mathf.Pow(10, i));
            value += desiredNumber * place;
        }

        return value;
    }


    public void AddToCounter(int signedAmount)
    {
        int previousSignedQueuedCount = _unitUnits.GetSignedQueuedCount();
        _unitUnits.QueueUpdate(signedAmount);
        int newSignedQueuedCount = _unitUnits.GetSignedQueuedCount();


        bool changedToIncrement = previousSignedQueuedCount <= 0 && newSignedQueuedCount > 0;
        bool changedToDecrement = previousSignedQueuedCount >= 0 && newSignedQueuedCount < 0;
        if (changedToIncrement)
        {
            StartViewUpdate(State.Incrementing);
        }
        else if (changedToDecrement)
        {
            StartViewUpdate(State.Decrementing);
        }
    }



    private void StartViewUpdate(State targetState)
    {
        if (_viewUpdateCoroutine != null)
        {
            StopCoroutine(_viewUpdateCoroutine);
        }
        _viewUpdateCoroutine = StartCoroutine(ViewUpdate(targetState));
    }
    private IEnumerator ViewUpdate(State targetState)
    {
        Configuration.State state = null;
        Configuration.StateChange stateChange = null;
        if (targetState == State.AtRest)
        {
            state = _configuration.atRestState;
        }
        else if (targetState == State.Incrementing)
        {
            stateChange = _configuration.incrementingStateChange;
            state = stateChange.state;
        }
        else if (targetState == State.Decrementing)
        {
            stateChange = _configuration.decrementingStateChange;
            state = stateChange.state;
        }


        Color initialColor = _unitUnits.GetTextColor();
        Color targetColor = state.targetColor;
        float timer = 0;
        while (timer < state.targetColorReachTime)
        {
            timer += Time.deltaTime;

            float t = Mathf.Min(1, timer / state.targetColorReachTime);
            t = DOVirtual.EasedValue(0, 1, t, state.targetColorReachEase);

            Color color = Color.LerpUnclamped(initialColor, targetColor, t);
            SetTextsColor(color);

            yield return null;
        }

        while (_unitUnits.GetSignedQueuedCount() != 0)
        {
            yield return null;
        }

        if (stateChange != null)
        {
            timer = 0;
            while (timer < stateChange.targetColorReachedDeadTime)
            {
                timer += Time.deltaTime;
                yield return null;
            }

            _viewUpdateCoroutine = StartCoroutine(ViewUpdate(State.AtRest));
        }
        else
        {
            _viewUpdateCoroutine = null;
        }
    }


    private void SetTextsColor(Color color)
    {
        for (int i = 0; i < _allUnits.Length; ++i)
        {
            _allUnits[i].SetTextColor(color);
        }
    }

}
