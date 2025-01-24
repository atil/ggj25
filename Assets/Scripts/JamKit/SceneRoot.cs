using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace JamKit
{
    public interface IScene
    {
        void Init(JamKit jamKit, Camera camera);
        string Tick();
        IEnumerator Exit();
    }

    public abstract class SceneRoot : MonoBehaviour, IScene
    {
        private enum FadeType
        {
            FadeIn, FadeOut
        }

        [SerializeField] private Image _coverImage;

        protected JamKit JamKit { get; private set; }
        protected Camera Camera { get; private set; }

        public void Init(JamKit jamKit, Camera camera)
        {
            JamKit = jamKit;
            Camera = camera;

            Fade(FadeType.FadeIn, JamKit.Globals.SceneTransitionParams, null);

            InitScene();
        }

        protected abstract void InitScene();

        public abstract string Tick();

        public virtual IEnumerator Exit()
        {
            bool isFadeTransitioning = true;
            Fade(FadeType.FadeOut, JamKit.Globals.SceneTransitionParams, () =>
            {
                isFadeTransitioning = false;
            });

            yield return new WaitWhile(() => { return isFadeTransitioning; });
        }

        private void Fade(FadeType type, SceneTransitionParams sceneTransitionParams, Action postAction)
        {
            if (sceneTransitionParams == null)
            {
                sceneTransitionParams = JamKit.Globals.SceneTransitionParams;
            }

            Color srcColor = type == FadeType.FadeIn ? sceneTransitionParams.Color : Color.clear;
            Color targetColor = type == FadeType.FadeIn ? Color.clear : sceneTransitionParams.Color;

            if (sceneTransitionParams.IsDiscrete)
            {
                JamKit.TweenDiscrete(sceneTransitionParams.Curve,
                    sceneTransitionParams.Duration,
                    JamKit.Globals.DiscreteTickInterval,
                    t =>
                    {
                        _coverImage.color = Color.Lerp(srcColor, targetColor, t);
                    },
                    () =>
                    {
                        _coverImage.color = targetColor;
                        postAction?.Invoke();
                    });
            }
            else
            {
                JamKit.Tween(sceneTransitionParams.Curve,
                    sceneTransitionParams.Duration,
                    t =>
                    {
                        _coverImage.color = Color.Lerp(srcColor, targetColor, t);
                    },
                    () =>
                    {
                        _coverImage.color = targetColor;
                        postAction?.Invoke();
                    });
            }
        }

    }
}