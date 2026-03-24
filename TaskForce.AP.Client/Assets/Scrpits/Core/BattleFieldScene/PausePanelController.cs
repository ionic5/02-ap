using System;
using TaskForce.AP.Client.Core.View.BattleFieldScene;

namespace TaskForce.AP.Client.Core.BattleFieldScene
{
    public class PausePanelController
    {
        private readonly IPausePanel _panel;
        private readonly IWorld _world;

        public PausePanelController(IPausePanel panel, IWorld world)
        {
            _panel = panel;
            _world = world;
        }

        public void Start()
        {
            _world.PausedEvent += OnWorldPaused;
            _world.ResumedEvent += OnWorldResumed;
            _panel.ClickedEvent += OnPanelClicked;
            _panel.DestroyEvent += OnPanelDestroyed;
            
            _panel.Hide();
        }

        public void Stop()
        {
            _world.PausedEvent -= OnWorldPaused;
            _world.ResumedEvent -= OnWorldResumed;
            _panel.ClickedEvent -= OnPanelClicked;
            _panel.DestroyEvent -= OnPanelDestroyed;
        }

        private void OnPanelDestroyed(object sender, DestroyEventArgs e)
        {
            Stop();
        }

        private void OnPanelClicked(object sender, EventArgs e)
        {
            _world.Resume();
        }

        private void OnWorldResumed(object sender, EventArgs e)
        {
            _panel.Hide();
        }

        private void OnWorldPaused(object sender, EventArgs e)
        {
            _panel.Show();
        }
    }
}
