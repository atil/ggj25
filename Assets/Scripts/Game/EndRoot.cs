using JamKit;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public class EndRoot : SceneRoot
    {
        protected override void InitScene()
        {
            Camera.backgroundColor = JamKit.Globals.EndSceneCameraBackgroundColor;
        }

        public override string Tick()
        {
            if (Input.anyKeyDown)
            {
                return "Game";
            }
            return "End";
        }
    }
}