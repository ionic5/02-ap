using System.Collections.Generic;
using System;
using TaskForce.AP.Client.Core.View.BattleFieldScene;
using TaskForce.AP.Client.Core.View.BattleFieldScene.Windows;
using TaskForce.AP.Client.Core.View.Windows;

namespace TaskForce.AP.Client.Core.BattleFieldScene
{
    public class WindowOpener
    {
        private readonly IWindowStack _windowStack;
        private readonly IWorld _world;
        private readonly TextStore _textStore;
        private readonly ISoundPlayer _soundPlayer;
        private readonly ILogger _logger;
        private readonly IAdvertisementPlayer _advertisementPlayer;

        public WindowOpener(IWindowStack windowStack, IWorld world, TextStore textStore, ISoundPlayer soundPlayer, ILogger logger, IAdvertisementPlayer advertisementPlayer)
        {
            _windowStack = windowStack;
            _world = world;
            _textStore = textStore;
            _soundPlayer = soundPlayer;
            _logger = logger;
            _advertisementPlayer = advertisementPlayer;
        }

        public void OpenLevelUpWindow(Entity.Unit unit, IEnumerable<Entity.ISkill> skills)
        {
            var window = _windowStack.OpenLevelUpWindow();
            TryPauseWorld();

            var ctrl = new LevelUpWindowController(window, skills, unit, _textStore, _advertisementPlayer);
            ctrl.Start();
        }

        public void OpenSettingWindow(Action onGoToLobby = null)
        {
            var window = _windowStack.OpenSettingWindow();
            TryPauseWorld();

            var commonCtrl = new Core.SettingWindowController(window, _soundPlayer);
            var ctrl = new SettingWindowController(window, commonCtrl, onGoToLobby);
            ctrl.Start();
        }

        private void TryPauseWorld()
        {
            if (_windowStack.GetOpenedWindowCount() != 1)
                return;
            _world.Pause();
        }

        public void OpenDeathWindow(int level, int kills, float survivalTime, Action onRestart, Action onRevive)
        {
            var window = _windowStack.OpenDeathWindow();
            TryPauseWorld();

            var ctrl = new DeathWindowController(window, level, kills, survivalTime, onRestart, onRevive);
            ctrl.Start();
        }
    }
}
