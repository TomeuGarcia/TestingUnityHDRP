using UnityEngine;
using UnityEngine.UI;



namespace ScreenVelocity
{
    public class ScreenVelocityMapTester : MonoBehaviour
    {
        [Header("COMPONENTS")]
        [SerializeField] private Image _image;
        private Material _imageMaterial;

        [Header("CONFIGURATION")]
        [SerializeField] private VelocityMapManager.Configuration _velocityMapManagerConfiguration;

        private VelocityMapManager _velocityMapManager;


        private void OnValidate()
        {
            _velocityMapManagerConfiguration.Validate();
        }

        private void Awake()
        {
            _velocityMapManager = new VelocityMapManager(_velocityMapManagerConfiguration);
            _imageMaterial = _image.material;
            _imageMaterial.SetTexture("_VelocityMap", _velocityMapManager.VelocityTexture);            
        }

        private void Start()
        {
            _velocityMapManager.StartInitialize();
        }

        private void Update()
        {
            Vector3 rawMousePosition = Input.mousePosition;
            Vector2 newPosition01 = new Vector2(
                Mathf.Clamp01(rawMousePosition.x / Screen.width),
                Mathf.Clamp01(rawMousePosition.y / Screen.height)
            );

            _velocityMapManager.Update(Time.deltaTime, newPosition01);
        }
    }
}