using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;



namespace ScreenVelocity
{
    public class VelocityMapManager
    {
        [System.Serializable]
        public class Configuration
        {
            [Header("CONFIGURATION")]
            [SerializeField, Range(0.0f, 1.0f)] private float _maxScreenSpeedNormalized = 0.05f;
            public float MaxScreenSpeed { get; private set; }

            public void Validate()
            {
                MaxScreenSpeed = new Vector2(Screen.width * _maxScreenSpeedNormalized, Screen.height * _maxScreenSpeedNormalized).magnitude;
            }
        }

        private Configuration _configuration;
        private VelocityCellsContainer _velocityCellsContainer;
        private Texture2D _velocityTexture;
        private Vector2 _previousPosition;
        private Vector2 _currentPosition;


        public Texture2D VelocityTexture => _velocityTexture;


        public VelocityMapManager(Configuration configuration)
        {
            _previousPosition = _currentPosition = Vector2.zero;

            _configuration = configuration;
            _configuration.Validate();

            int downscaleFactor = 32;
            int width = Screen.width / downscaleFactor;
            int height = Screen.height / downscaleFactor;
            _velocityCellsContainer = new VelocityCellsContainer(width, height);

            _velocityTexture = new Texture2D(width, height, TextureFormat.RGBA32_SIGNED, mipChain: false, linear: false, createUninitialized: false);
            _velocityTexture.wrapMode = TextureWrapMode.Clamp;
        }

        ~VelocityMapManager()
        {
            Object.Destroy(_velocityTexture);
        }

        public void StartInitialize()
        {
            Color[] pixels = _velocityTexture.GetPixels();
            for (int i = 0; i < pixels.Length; ++i)
            {
                pixels[i] = Color.black;
            }
            _velocityTexture.SetPixels(pixels);
        }

        public void Update(float deltaTime, Vector2 newPosition01)
        {
            newPosition01.x = Mathf.Clamp01(newPosition01.x);
            newPosition01.y = Mathf.Clamp01(newPosition01.y);            
            _currentPosition = newPosition01;
            Vector2 rawVelocity = (_currentPosition - _previousPosition) / Time.deltaTime;
            Vector2 velocity = Vector2.ClampMagnitude(rawVelocity, _configuration.MaxScreenSpeed);


            TransformToTexturePosition(_previousPosition, out int texturePositionX_Start, out int texturePositionY_Start);
            TransformToTexturePosition(_currentPosition, out int texturePositionX_End, out int texturePositionY_End);
            Vector2Int textureDirection = new Vector2Int(
                Mathf.Clamp(texturePositionX_End - texturePositionX_Start, -1, 1),
                Mathf.Clamp(texturePositionY_End - texturePositionY_Start, -1, 1)
            );

            do
            {
                if (texturePositionX_Start != texturePositionX_End) texturePositionX_Start += textureDirection.x;
                if (texturePositionY_Start != texturePositionY_End) texturePositionY_Start += textureDirection.y;

                VelocityCellsContainer.Cell cellToChange = _velocityCellsContainer.GetCell(texturePositionX_Start, texturePositionY_Start);
                cellToChange.SetTargetVelocity(velocity);
            }
            while (texturePositionX_Start != texturePositionX_End || texturePositionY_Start != texturePositionY_End);


            VelocityCellsContainer.Cell[] cells = _velocityCellsContainer.GetCells();
            foreach (VelocityCellsContainer.Cell cell in cells)
            {
                cell.Update(deltaTime, out bool changed);
                if (changed)
                {
                    Color color = new Color(cell.Velocity.x, cell.Velocity.y, 0.0f, 0.0f);
                    _velocityTexture.SetPixel(cell.TextureX, cell.TextureY, color);
                }                
            }

            _velocityTexture.Apply();

            _previousPosition = _currentPosition;
        }

        public void TransformToTexturePosition(Vector2 position01, out int texturePositionX, out int texturePositionY)
        {
            texturePositionX = Mathf.RoundToInt(position01.x * _velocityTexture.width);
            texturePositionY = Mathf.RoundToInt(position01.y * _velocityTexture.height);
        }
    }


    public class VelocityCellsContainer
    {
        public class Cell
        {
            public int TextureX { get; private set; }
            public int TextureY { get; private set; }
            public Vector2 Velocity { get; private set; }
            private Vector2 _targetVelocity;
            private Vector2 _velocityDamp;
            private bool _ramping;

            public Cell(int textureX, int textureY)
            {
                TextureX = textureX;
                TextureY = textureY;
                Velocity = Vector2.zero;
                _targetVelocity = Vector2.zero;
                _velocityDamp = Vector2.zero;
                _ramping = true;
            }

            public void SetTargetVelocity(Vector2 velocity)
            {
                _targetVelocity = velocity;
                _ramping = true;
            }

            public void Update(float deltaTime, out bool changed)
            {
                changed = _ramping || Velocity.sqrMagnitude > 0.0f;

                float smoothTime = _ramping ? 0.15f : 1.0f;
                Velocity = Vector2.SmoothDamp(Velocity, _targetVelocity, ref _velocityDamp, smoothTime, maxSpeed: float.PositiveInfinity, deltaTime);
                if (_ramping && ReachedTargetVelocity())
                {
                    _targetVelocity = Vector2.zero;
                    _ramping = false;
                }
            }

            private bool ReachedTargetVelocity()
            {
                return (_targetVelocity - Velocity).sqrMagnitude < 0.3f;
            }
        }


        private Cell[] _cells;
        private int _width;
        private int _height;

        public VelocityCellsContainer(int width, int height)
        {
            int totalCells = width * height;
            _cells = new Cell[totalCells];
            for (int i = 0; i < totalCells; ++i)
            {
                int x = i % width;
                int y = i / width;
                _cells[i] = new Cell(x, y);
            }
            _width = width;
            _height = height;
        }


        public Cell GetCell(int positionX, int positionY)
        {
            positionX = Mathf.Min(positionX, _width - 1);
            positionY = Mathf.Min(positionY, _height - 1);
            int index = positionX + (positionY * _width);
            return _cells[index];
        }

        public Cell[] GetCells()
        {
            return _cells;
        }
    }



}
