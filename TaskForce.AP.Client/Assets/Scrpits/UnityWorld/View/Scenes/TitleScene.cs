using System;
using TaskForce.AP.Client.Core.View.Scenes;

namespace TaskForce.AP.Client.UnityWorld.View.Scenes
{
    public class TitleScene : Scene, ITitleScene
    {
        public event EventHandler StartButtonClickedEvent;

        public void OnStartButtonClicked()
        {
            StartButtonClickedEvent?.Invoke(this, EventArgs.Empty);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            StartButtonClickedEvent = null;
        }
    }
}
