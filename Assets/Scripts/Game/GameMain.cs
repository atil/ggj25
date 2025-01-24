using JamKit;
using UnityEngine;

namespace Game
{
    public class GameMain : SceneRoot
    {
        [SerializeField] private LineRenderer _circle;

        private float _radius = 1;

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
            if (Input.anyKey)
            {
                _radius += Time.deltaTime * 0.1f;
                for (int i = 0; i < 360; i++)
                {
                    float cos = Mathf.Cos(i * Mathf.Deg2Rad);
                    float sin = Mathf.Sin(i * Mathf.Deg2Rad);
                    Vector3 center = Vector3.zero;
                    _circle.SetPosition(i, center + new Vector3(_radius * cos, _radius * sin, 0));
                }
            }

            return "Game";
        }

    }
}