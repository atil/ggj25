using JamKit;
using UnityEngine;

namespace Game
{
    public class SplashRoot : SceneRoot
    {
        protected override void InitScene()
        {
            // @jamkit TODO this color lingers to the game scene, which is unintuitive
            Camera.backgroundColor = JamKit.Globals.SplashSceneCameraBackgroundColor;
            JamKit.StartMusic("ZenAmbient", true);
        }


        public override string Tick()
        {
            if (Input.anyKeyDown)
            {
                return "Game";
            }
            return "Splash";
        }

    }
}