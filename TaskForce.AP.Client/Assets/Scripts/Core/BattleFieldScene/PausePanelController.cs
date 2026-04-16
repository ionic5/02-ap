using System;
using TaskForce.AP.Client.Core.View.BattleFieldScene;

namespace TaskForce.AP.Client.Core.BattleFieldScene
{
    public class PausePanelController
    {
        private readonly IPausePanel _panel;
        private readonly IWorld _world;
        private readonly IWindowStack _windowStack;

        public PausePanelController(IPausePanel panel, IWorld world, IWindowStack windowStack)
        {
            _panel = panel;
            _world = world;
            _windowStack = windowStack;
        }

        public void Start()
        {
            _world.PausedEvent += OnWorldPaused;
            _world.ResumedEvent += OnWorldResumed;
            _windowStack.WindowCountChangedEvent += OnWindowCountChanged;
            _panel.ClickedEvent += OnPanelClicked;
            _panel.DestroyEvent += OnPanelDestroyed;
            
            UpdatePanelVisibility();
        }

        public void Stop()
        {
            _world.PausedEvent -= OnWorldPaused;
            _world.ResumedEvent -= OnWorldResumed;
            _windowStack.WindowCountChangedEvent -= OnWindowCountChanged;
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
            UpdatePanelVisibility();
        }

        private void OnWindowCountChanged(object sender, EventArgs e)
        {
            UpdatePanelVisibility();
        }

        private void UpdatePanelVisibility()
        {
            if (_world.IsPaused() && _windowStack.GetOpenedWindowCount() == 0)
            {
                _panel.Show();
            }
            else
            {
                _panel.Hide();
            }
        }
    }
}
