using System;
using TaskForce.AP.Client.Core.View.BattleFieldScene;

namespace TaskForce.AP.Client.UnityWorld.View.BattleFieldScene
{
    public class PausePanel : Object, IPausePanel
    {
        public event EventHandler ClickedEvent;

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void OnPanelClicked()
        {
            ClickedEvent?.Invoke(this, EventArgs.Empty);
        }

        protected override void CleanUp()
        {
            base.CleanUp();
            ClickedEvent = null;
        }
    }
}
