using JamKit;
using UnityEngine;

namespace Game
{
    public class GameMain : SceneRoot
    {
        [SerializeField] private LineRenderer _circle;
        [SerializeField] private float _timeLimit = 5;

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
            for (int i = 0; i < 360; i++)
            {
                float cos = Mathf.Cos(i * Mathf.Deg2Rad);
                float sin = Mathf.Sin(i * Mathf.Deg2Rad);
                Vector3 center = Vector3.zero;
                _circle.SetPosition(i, center + new Vector3(_radius * cos, _radius * sin, 0));
            }
        }

        public override string Tick()
        {
            switch (_gameState)
            {
                case GameState.Initial:
                    break;
                case GameState.Pressing:
                    _pressedTime += Time.deltaTime * 1f;

                    if (_pressedTime > _timeLimit)
                    {
                        _gameState = GameState.GameOver;
                        _circle.gameObject.SetActive(false);
                        break;
                    }

                    _radius = _pressedTime * 0.4f + 1;

                    for (int i = 0; i < 360; i++)
                    {
                        float cos = Mathf.Cos(i * Mathf.Deg2Rad);
                        float sin = Mathf.Sin(i * Mathf.Deg2Rad);
                        Vector3 center = Vector3.zero;
                        _circle.SetPosition(i, center + new Vector3(_radius * cos, _radius * sin, 0));
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
                    _gameState = GameState.Lifted;
                    Debug.Log($"Score: {score}");
                }
            }

            return "Game";
        }

    }
}