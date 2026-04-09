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
        private readonly IMockSoundPlayer _mockSoundPlayer;
        private readonly ILogger _logger;
        private readonly IAdvertisementPlayer _advertisementPlayer;
        private readonly GameDataStore _gameDataStore;
        private readonly Random _random;
        private readonly Func<string, Entity.ISkill> _createSkillEntity;

        public WindowOpener(IWindowStack windowStack, IWorld world, TextStore textStore, IMockSoundPlayer mockSoundPlayer, ILogger logger, IAdvertisementPlayer advertisementPlayer,
            GameDataStore gameDataStore, Random random, Func<string, Entity.ISkill> createSkillEntity)
        {
            _windowStack = windowStack;
            _world = world;
            _textStore = textStore;
            _mockSoundPlayer = mockSoundPlayer;
            _logger = logger;
            _advertisementPlayer = advertisementPlayer;
            _gameDataStore = gameDataStore;
            _random = random;
            _createSkillEntity = createSkillEntity;
        }

        public void OpenLevelUpWindow(Entity.Unit unit)
        {
            var window = _windowStack.OpenLevelUpWindow();
            TryPauseWorld();

            var ctrl = new LevelUpWindowController(window, unit, _textStore, _advertisementPlayer,
                _gameDataStore, _random, _createSkillEntity);
            ctrl.Start();
        }

        public void OpenSettingWindow(Action onGoToLobby = null)
        {
            var window = _windowStack.OpenSettingWindow();
            TryPauseWorld();

            var commonCtrl = new Core.SettingWindowController(window, _mockSoundPlayer);
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
