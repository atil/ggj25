// Juice ideas
// - end state rainbow
// - end state hareketli
// - SFX
// - highscore text
// - balancing
// - Build
// - Itch page / GGJ page
// - multiplayer version

using JamKit;
using TMPro;
using UnityEngine;

namespace Game
{
    public class GameMain : SceneRoot
    {
        [Header("Scene")]
        [SerializeField] private LineRenderer _circle;
        [SerializeField] private TextMeshProUGUI _scoreText;
        [SerializeField] private MeshRenderer _backgroundQuadRenderer;
        [SerializeField] private Texture2D[] _backgroundPatterns;

        [Header("Balancing")]
        [SerializeField] private float _timeLimit = 7.5f;
        [SerializeField] private float _timeLimitRandomness = 2.5f;
        [SerializeField] private float _circleSizeCoeff = 0.4f;
        [SerializeField] private float _circleGrowSpeed = 1.0f;

        [Header("Juice")]
        [SerializeField] private AnimationCurve _colorCurve;
        [SerializeField] private Color[] _circleColors;
        [SerializeField] private AnimationCurve _wobblinessCurve;
        [SerializeField] private float _wobblinessIntensity = 8.0f;
        [SerializeField] private float _wobblinessAmount = 2.5f;
        [SerializeField] private Vector2 _backgroundMinMaxSpeeds;

        private Material _circleMaterialInstance;
        private Material _backgroundMaterialInstance;

        private enum GameState
        {
            Initial,
            Pressing,
            Lifted,
            GameOver
        }

        private float _radius = 1;
        private float _pressedTime = 0;
        private float _levelLoadTime = 0;

        private GameState _gameState = GameState.Initial;

        protected override void InitScene()
        {
            _timeLimit += Random.Range(0, _timeLimitRandomness);
            _circleMaterialInstance = _circle.materials[0];

            AnimationCurve curve = new();

            for (int i = 0; i <= 360; i++)
            {
                float cos = Mathf.Cos(i * Mathf.Deg2Rad);
                float sin = Mathf.Sin(i * Mathf.Deg2Rad);
                Vector3 center = Vector3.zero;
                _circle.SetPosition(i, Vector3.zero + new Vector3(_radius * cos, _radius * sin, 0));

                curve.AddKey(i / 360.0f, 1.0f);
            }

            _circle.widthCurve = curve;
            _levelLoadTime = Time.time;

            _backgroundMaterialInstance = _backgroundQuadRenderer.material;
            _backgroundMaterialInstance.SetTexture("_Pattern", _backgroundPatterns.GetRandom());
        }

        public override string Tick()
        {
            switch (_gameState)
            {
                case GameState.Initial:
                    break;
                case GameState.Pressing:
                    _pressedTime += Time.deltaTime * _circleGrowSpeed;

                    if (_pressedTime > _timeLimit)
                    {
                        _gameState = GameState.GameOver;
                        _circle.gameObject.SetActive(false);
                        _backgroundMaterialInstance.SetFloat("_SpeedCoeff", _backgroundMinMaxSpeeds.x);
                        break;
                    }

                    int colorSize = 10;
                    float t = _colorCurve.Evaluate(_pressedTime / _timeLimit);
                    int rawColorIndex = (int)(t * colorSize);
                    int colorIndex = Mathf.Clamp(rawColorIndex, 0, colorSize-2);
                    float t2 = (t*colorSize - colorIndex) / (1);
                    Color c = Color.Lerp(_circleColors[colorIndex], _circleColors[colorIndex + 1], t2);
                    _circleMaterialInstance.SetColor("_Color1", c);

                    _radius = _pressedTime * _circleSizeCoeff + 1;

                    float wobblinessT = _wobblinessCurve.Evaluate(_pressedTime / _timeLimit);
                    for (int i = 0; i <= 360; i++)
                    {
                        float cos = Mathf.Cos(i * Mathf.Deg2Rad);
                        float sin = Mathf.Sin(i * Mathf.Deg2Rad);

                        float levelTime = Time.time - _levelLoadTime;
                        float radiusOffset = Mathf.PerlinNoise(cos * _wobblinessIntensity + levelTime, sin * _wobblinessIntensity + levelTime) * _wobblinessAmount * wobblinessT;
                        float pointRadius = _radius + radiusOffset;
                        _circle.SetPosition(i, Vector3.zero + new Vector3(pointRadius * cos, pointRadius * sin, 0));
                    }

                    AnimationCurve widthCurveCopy = _circle.widthCurve;
                    AdjustWidthCurveWithTime(ref widthCurveCopy, t);
                    _circle.widthCurve = widthCurveCopy;

                    float backgroundSpeed = Mathf.Lerp(_backgroundMinMaxSpeeds.x, _backgroundMinMaxSpeeds.y, t);
                    _backgroundMaterialInstance.SetFloat("_SpeedCoeff", backgroundSpeed);

                    break;
                case GameState.Lifted:
                    _backgroundMaterialInstance.SetFloat("_SpeedCoeff", _backgroundMinMaxSpeeds.x);

                    break;
            }

            if (Input.anyKeyDown && _gameState == GameState.GameOver)
            {
                return SameScene;
            }

            if (Input.anyKey)
            {
                if (_gameState == GameState.Lifted)
                {
                    return null;
                }
                else if (_gameState == GameState.Initial)
                {
                    _gameState = GameState.Pressing;
                }
            }
            else
            {
                if (_gameState == GameState.Pressing)
                {
                    float score = _timeLimit - _pressedTime;
                    _scoreText.gameObject.SetActive(true);
                    _scoreText.text = score.ToString("0.000");
                    _gameState = GameState.Lifted;
                }
            }

            return "Game";
        }

        private void AdjustWidthCurveWithTime(ref AnimationCurve curve, float timeNormalized)
        {
            Keyframe[] keys = curve.keys;
            for (int i = 0; i <= 360; i++)
            {
                float t = i / 360.0f;
                keys[i].value = 1.0f - (timeNormalized * 0.9f);
            }
            curve.keys = keys;
        }
    }
}