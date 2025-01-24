using JamKit;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public class EndRoot : SceneRoot
    {
        [SerializeField] private Button _playButton;

        private bool _playButtonClicked = false;

        protected override void InitScene()
        {
            Camera.backgroundColor = JamKit.Globals.EndSceneCameraBackgroundColor;
        }

        public void OnClickedPlayButton()
        {
            _playButton.interactable = false;
            _playButtonClicked = true;
        }

        public override string Tick()
        {
            return _playButtonClicked ? "Game" : "End";
        }
    }
}