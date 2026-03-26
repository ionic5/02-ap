using System.Collections.Generic;
using System;
using TaskForce.AP.Client.Core.View.BattleFieldScene;
using TaskForce.AP.Client.Core.View.BattleFieldScene.Windows;

namespace TaskForce.AP.Client.Core.BattleFieldScene
{
    public class WindowOpener
    {
        private readonly IWindowStack _windowStack;
        private readonly IWorld _world;
        private readonly TextStore _textStore;
        private readonly ISoundPlayer _soundPlayer;
        private readonly ILogger _logger;

        public WindowOpener(IWindowStack windowStack, IWorld world, TextStore textStore, ISoundPlayer soundPlayer, ILogger logger)
        {
            _windowStack = windowStack;
            _world = world;
            _textStore = textStore;
            _soundPlayer = soundPlayer;
            _logger = logger;
        }

        public void OpenPerkSelectionWindow(Entity.Unit unit, IEnumerable<Entity.ISkill> skills)
        {
            var window = _windowStack.OpenSkillSelectionWindow();
            TryPauseWorld();
            AttachResumeHandler(window);

            var ctrl = new SkillSelectionWindowController(window, skills, unit, _textStore);
            ctrl.Start();
        }

        public void OpenSettingWindow(Action onGoToLobby = null)
        {
            var window = _windowStack.OpenSettingWindow();
            TryPauseWorld();
            AttachResumeHandler(window);

            var ctrl = new SettingWindowController(window, _soundPlayer, onGoToLobby);
            ctrl.Start();
        }

        private void TryPauseWorld()
        {
            if (_windowStack.GetOpenedWindowCount() != 1)
                return;
            _world.Pause();
        }

        private void TryResumeWorld()
        {
            if (_windowStack.GetOpenedWindowCount() > 0)
                return;
            _world.Resume();
        }

        private void AttachResumeHandler(IWindow window)
        {
            EventHandler handler = null;
            handler = (sender, args) =>
            {
                window.ClosedEvent -= handler;
                TryResumeWorld();
            };
            window.ClosedEvent += handler;
        }

        public void OpenDeathWindow(int level, int kills, float survivalTime, Action onRestart, Action onRevive)
        {
            var window = _windowStack.OpenDeathWindow();
            TryPauseWorld();
            AttachResumeHandler(window);

            var ctrl = new DeathWindowController(window, level, kills, survivalTime, onRestart, onRevive);
            ctrl.Start();
        }
    }
}
