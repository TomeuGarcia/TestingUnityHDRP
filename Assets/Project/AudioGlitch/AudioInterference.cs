using System.Collections;
using UnityEngine;




public class AudioInterference : MonoBehaviour
{
    [System.Serializable]
    public class Configuration
    {
        [System.Serializable]
        public class AudioVariation
        {
            [SerializeField] private Vector2 _volumeInterval = new Vector2(0.2f, 0.4f);
            [SerializeField] private Vector2 _pitchInterval = new Vector2(0.9f, 1.2f);

            public float Volume => _volumeInterval.Random();
            public float Pitch => _pitchInterval.Random();

            public AudioVariation(Vector2 volumeInterval, Vector2 pitchInterval)
            {
                _volumeInterval = volumeInterval;
                _pitchInterval = pitchInterval;
            }

            public void Validate()
            {
                _volumeInterval.Validate_Clamp01();
                _volumeInterval.Validate_YBiggerThanX();

                _pitchInterval.Validate_Clamp(min: 0.0f, max: 2.0f);
                _pitchInterval.Validate_YBiggerThanX();
            }
        }

        [Header("Main")]
        [SerializeField] public float mainAudioVolume = 0.5f;

        [Header("Background Interference")]
        [SerializeField] public float backgroundInterferenceAudioVolume_Muted = 0.2f;
        [SerializeField] public float backgroundInterferenceAudioVolume_Unmuted = 0.1f;

        [Header("Short Interferences")]
        [SerializeField] public AudioVariation shortInterferenceAudioVariation = new AudioVariation(
            volumeInterval: new(0.2f, 0.3f), pitchInterval: new(1.5f, 2.0f)
        );
        [SerializeField] public AudioVariation waitingShortInterferenceAudioVariation = new AudioVariation(
            volumeInterval: new(0.05f, 0.1f), pitchInterval: new(1.5f, 2.0f)
        );

        [Header("Durations")]
        [SerializeField] public Vector2 intervalDuration_Muted = new Vector2(2.0f, 5.0f);
        [SerializeField] public Vector2 intervalDuration_Unmuted = new Vector2(0.2f, 1.0f);
        [SerializeField] public Vector2 intervalDuration_MildShortInterference = new Vector2(0.1f, 0.2f);

        public void Validate()
        {
            shortInterferenceAudioVariation.Validate();
            waitingShortInterferenceAudioVariation.Validate();

            intervalDuration_Muted.Validate_YBiggerThanX();
            intervalDuration_Unmuted.Validate_YBiggerThanX();
            intervalDuration_MildShortInterference.Validate_YBiggerThanX();
        }
    }


    [Header("CONFIGURATION")]
    [SerializeField] private Configuration _configuration;

    [Header("AUDIOS")]
    [SerializeField] private AudioSource _mainAudioSource;
    [SerializeField] private AudioSource _backgroundInterferenceAudioSource;
    [SerializeField] private AudioSource _shortInterferenceAudioSource;

    private Coroutine _interferenceLoopCoroutine;


    private void OnValidate()
    {
        _configuration.Validate();
    }

    private void Awake()
    {
        OnValidate();

        _shortInterferenceAudioSource.loop = false;
        _shortInterferenceAudioSource.spatialBlend = 0.0f;

        _backgroundInterferenceAudioSource.loop = true;
        _backgroundInterferenceAudioSource.spatialBlend = 0.0f;
    }


    private void Start()
    {
        StartInterference();
    }

    private void OnDestroy()
    {
        StopInterference();
    }


    public void StartInterference()
    {
        _interferenceLoopCoroutine = StartCoroutine(InterferenceLoop());
    }

    public void StopInterference()
    {
        if (_interferenceLoopCoroutine != null)
        {
            StopCoroutine(_interferenceLoopCoroutine);
            FinishInterferenceLoop();
        }
    }

    private void FinishInterferenceLoop()
    {
        _interferenceLoopCoroutine = null;

        _shortInterferenceAudioSource.Stop();
        _backgroundInterferenceAudioSource.Stop();

        _mainAudioSource.volume = _configuration.mainAudioVolume;
    }

    private IEnumerator InterferenceLoop()
    {
        // Setup
        _mainAudioSource.Play();
        _backgroundInterferenceAudioSource.Play();

        while (true)
        {
            // Mute
            _mainAudioSource.volume = 0.0f;
            _backgroundInterferenceAudioSource.volume = _configuration.backgroundInterferenceAudioVolume_Muted;
            float durationMuted = _configuration.intervalDuration_Muted.Random(); 
            yield return StartCoroutine(ShortInterferencePattern(durationMuted));


            // Unmute
            _mainAudioSource.volume = _configuration.mainAudioVolume;
            _backgroundInterferenceAudioSource.volume = _configuration.backgroundInterferenceAudioVolume_Unmuted;
            float durationUnmuted = _configuration.intervalDuration_Unmuted.Random();
            yield return StartCoroutine(ShortInterferencePattern(durationUnmuted));
        }
    }

    private IEnumerator ShortInterferencePattern(float duration)
    {
        float startTime = Time.timeSinceLevelLoad;
        yield return StartCoroutine(PlayShortInterference(_configuration.shortInterferenceAudioVariation));        
        float delayTimer = Time.timeSinceLevelLoad - startTime;

        while (delayTimer < duration)
        {
            float mildInterferenceDelay = _configuration.intervalDuration_MildShortInterference.Random();
            yield return new WaitForSeconds(mildInterferenceDelay);

            delayTimer += mildInterferenceDelay;
            if (delayTimer < duration)
            {
                startTime = Time.timeSinceLevelLoad;
                yield return StartCoroutine(PlayShortInterference(_configuration.waitingShortInterferenceAudioVariation));
                delayTimer += Time.timeSinceLevelLoad - startTime;
            }
        }
    }

    private IEnumerator PlayShortInterference(Configuration.AudioVariation audioVariation)
    {
        _shortInterferenceAudioSource.volume = audioVariation.Volume;
        _shortInterferenceAudioSource.pitch = audioVariation.Pitch;
        _shortInterferenceAudioSource.Play();

        yield return new WaitUntil(() => _shortInterferenceAudioSource.isPlaying);
    }
}



public static class Vector2Extensions
{
    public static void Validate_Clamp(this ref Vector2 vector, float min, float max)
    {
        vector.x = Mathf.Clamp(vector.x, min, max);
        vector.y = Mathf.Clamp(vector.y, min, max);
    }
    public static void Validate_Clamp01(this ref Vector2 vector)
    {
        vector.x = Mathf.Clamp(vector.x, 0.0f, 1.0f);
        vector.y = Mathf.Clamp(vector.y, 0.0f, 1.0f);
    }
    public static void Validate_YBiggerThanX(this ref Vector2 vector, float minAmount = 0.01f)
    {
        vector.y = Mathf.Max(vector.y, vector.x + minAmount);
    }
    public static float Random(this Vector2 vector)
    {
        return UnityEngine.Random.Range(vector.x, vector.y);
    }
}