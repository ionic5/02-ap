using System.Collections.Generic;
using System;
using TaskForce.AP.Client.Core.View.BattleFieldScene;

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

            var ctrl = new SkillSelectionWindowController(window, skills, unit, _textStore);
            ctrl.Start();
        }

        public void OpenSettingWindow()
        {
            var window = _windowStack.OpenSettingWindow();
            TryPauseWorld();

            var ctrl = new SettingWindowController(window, _soundPlayer);
            ctrl.Start();
        }

        private void TryPauseWorld()
        {
            if (_windowStack.GetOpenedWindowCount() != 1)
                return;
            _world.Pause();
        }

        public void OpenDeathWindow(Action onRestart, Action onRevive)
        {
            var window = _windowStack.OpenDeathWindow();
            TryPauseWorld();

            var ctrl = new DeathWindowController(window, onRestart, onRevive);
            ctrl.Start();
        }
    }
}
