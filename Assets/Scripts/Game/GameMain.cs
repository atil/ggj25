// - more circle colors
// - circle wobbliness
// - carrying run result to endscene

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

        [Header("Balancing")]
        [SerializeField] private float _timeLimit = 5;
        [SerializeField] private float _circleSizeCoeff = 0.4f;
        [SerializeField] private float _circleGrowSpeed = 1.0f;

        [Header("Juice")]
        [SerializeField] private AnimationCurve _colorCurve;
        [SerializeField] private Color[] _circleColors;
        [SerializeField] private AnimationCurve _wobblyCurve;
        [SerializeField] private float _wobblinessIntensity = 3;

        private Material _circleMaterialInstance;

        private enum GameState
        {
            Initial,
            Pressing,
            Lifted,
            GameOver
        }

        private float _radius = 1;
        private float _pressedTime = 0;

        private GameState _gameState = GameState.Initial;

        protected override void InitScene()
        {
            _circleMaterialInstance = _circle.materials[0];

            for (int i = 0; i <= 360; i++)
            {
                float cos = Mathf.Cos(i * Mathf.Deg2Rad);
                float sin = Mathf.Sin(i * Mathf.Deg2Rad);
                Vector3 center = Vector3.zero;
                _circle.SetPosition(i, Vector3.zero + new Vector3(_radius * cos, _radius * sin, 0));
            }
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
                        break;
                    }

                    float t = _colorCurve.Evaluate(_pressedTime / _timeLimit);
                    Color c = Color.Lerp(_circleColors[0], _circleColors[1], t);
                    _circleMaterialInstance.SetColor("_Color1", c);

                    _radius = _pressedTime * _circleSizeCoeff + 1;

                    for (int i = 0; i <= 360; i++)
                    {
                        float cos = Mathf.Cos(i * Mathf.Deg2Rad);
                        float sin = Mathf.Sin(i * Mathf.Deg2Rad);
                        _circle.SetPosition(i, Vector3.zero + new Vector3(_radius * cos, _radius * sin, 0));

                    }
                    break;
                case GameState.Lifted:

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
                    Debug.Log($"Score: {score}");
                }
            }

            return "Game";
        }

    }
}