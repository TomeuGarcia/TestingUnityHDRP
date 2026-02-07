using UnityEngine;
using UnityEngine.UI;



namespace RadialNoise
{
    public class RadialNoiseTester : MonoBehaviour
    {
        [System.Serializable]
        public class Configuration
        {
            [Header("State Update")]
            [SerializeField, Range(0.0f, 0.99f)] public float stateChangeDamping = 0.9f;
            [SerializeField] public AnimationCurve stateEaseCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

            [Header("View")]
            [SerializeField] public Gradient colorGradient;
            [SerializeField] public Vector2Int thicknessInPixelsOverState = new Vector2Int(4, 10);
            [SerializeField] public Vector2Int amplitudeInPixelsOverState = new Vector2Int(80, 160);
            [SerializeField] public Vector2 noiseFrequencyOverState = new Vector2(6.0f, 15.0f);
            [SerializeField] public Vector2 noiseSpeedOverState = new Vector2(0.1f, 1.0f);
        }


        [Header("COMPONENTS")]
        [SerializeField] private Image _radialNoiseImage;

        [Header("STATE")]
        [SerializeField, Range(0, 1)] private float _targetStateT = 0.0f;
        [SerializeField, Range(0, 1)] private float _previewOnlyCurrentStateT = 0.0f;

        [Header("CONFIURATION")]
        [SerializeField] private Configuration _configuration;

        private Material _radialNoiseMaterial;
        private int _materialPropertyID_ThicknessInPixels;
        private int _materialPropertyID_AmplitudeInPixels;
        private int _materialPropertyID_NoiseFrequency;
        private int _materialPropertyID_NoiseSpeed;
        private float _currentStateT;
        private float _currentStateChangeSpeed;


        private void OnValidate()
        {
            _previewOnlyCurrentStateT = _targetStateT;
        }

        private void Awake()
        {
            Initialize();
        }

        private void Update()
        {
            UpdateState(Time.deltaTime);
        }


        private void Initialize()
        {
            _radialNoiseMaterial = _radialNoiseImage.material;
            _materialPropertyID_ThicknessInPixels = Shader.PropertyToID("_Thickness_In_Pixels");
            _materialPropertyID_AmplitudeInPixels = Shader.PropertyToID("_Amplitude_In_Pixels");
            _materialPropertyID_NoiseFrequency = Shader.PropertyToID("_Noise_Frequency");
            _materialPropertyID_NoiseSpeed = Shader.PropertyToID("_Noise_Speed");

            _currentStateT = _targetStateT;
            _currentStateChangeSpeed = 0.0f;
        }

        private void UpdateState(float deltaTime)
        {
            _currentStateT = Mathf.LerpUnclamped(_targetStateT, _currentStateT, _configuration.stateChangeDamping);
            if (Mathf.Abs(_currentStateT - _targetStateT) < 0.01f) _currentStateT = _targetStateT;

            _previewOnlyCurrentStateT = _currentStateT;


            _radialNoiseImage.color = _configuration.colorGradient.Evaluate(_currentStateT);

            float easedStateT = _configuration.stateEaseCurve.Evaluate(_currentStateT);
            _radialNoiseMaterial.SetFloat(_materialPropertyID_ThicknessInPixels,    _configuration.thicknessInPixelsOverState.Lerp(easedStateT));
            _radialNoiseMaterial.SetFloat(_materialPropertyID_AmplitudeInPixels,    _configuration.amplitudeInPixelsOverState.Lerp(easedStateT));
            _radialNoiseMaterial.SetFloat(_materialPropertyID_NoiseFrequency,       _configuration.noiseFrequencyOverState.Lerp(easedStateT));
            _radialNoiseMaterial.SetFloat(_materialPropertyID_NoiseSpeed,           _configuration.noiseSpeedOverState.Lerp(easedStateT));
        }        

    }


    public static class Extensions
    {
        public static float Lerp(this Vector2 vector, float t)
        {
            return Mathf.LerpUnclamped(vector.x, vector.y, t);
        }
        public static int Lerp(this Vector2Int vector, float t)
        {
            return Mathf.RoundToInt(Mathf.LerpUnclamped(vector.x, vector.y, t));
        }
    }
}


