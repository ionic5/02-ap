using System;
using System.Collections.Generic;
using System.Linq;
using TaskForce.AP.Client.Core.BattleFieldScene;
using TaskForce.AP.Client.Core.View.BattleFieldScene;
using TaskForce.AP.Client.Core.View.Scenes;
using IUnit = TaskForce.AP.Client.Core.View.BattleFieldScene.IUnit;

namespace TaskForce.AP.Client.Core.LobbyScene
{
    public class LobbySceneController : IUpdatable
    {
        private readonly ILobbyScene _scene;
        private readonly IWorld _world;
       
        private readonly WindowOpener _windowOpener;
        private readonly GameDataStore _gameDataStore;
        private readonly Core.Random _random;
        private readonly ILogger _logger;
        private readonly Func<Entity.Unit, string, int, Entity.ISkill> _createSkillEntity;
        private readonly Core.Timer _timer;
        
        private readonly UserDataStore _userDataStore;

        private bool _isDestroyed;
        private IUnit _unit;

        public LobbySceneController(ILobbyScene scene, IWorld world, 
            WindowOpener windowOpener, GameDataStore gameDataStore, Random random, 
            ILogger logger, Core.Timer timer, UserDataStore userDataStore)
        {
            _scene = scene;
            _world = world;
            
            _windowOpener = windowOpener;
            _isDestroyed = false;
            _gameDataStore = gameDataStore;
            _random = random;
            _logger = logger;
            _timer = timer;
     
        
            _userDataStore = userDataStore;
        }

        public void Update()
        {
            // TODO: JW: 타이머 참조
            // _scene.SetBattleTime(_battleLog.BattleTime);
            // _scene.SetKillCount(_battleLog.KillCount);
            // UpdateGold();
        }

        public void Start()
        {
            UpdateGold();
            
            _scene.DestroyEvent += OnDestroySceneEvent;
            _scene.PauseButtonClickedEvent += OnPauseButtonClickedEvent;
        }

        private void OnPauseButtonClickedEvent(object sender, EventArgs e)
        {
            // _windowOpener.OpenSettingWindow();
        }

        private void UpdateGold()
        {
            _scene.SetGold(_userDataStore.GetGold());
        }

        private void OnDestroySceneEvent(object sender, EventArgs e)
        {
            Destroy();
        }

        private void Destroy()
        {
            if (_isDestroyed)
                return;
            _isDestroyed = true;
        }

        // TODO: JW: 윈도우 기능 참조
        // private void OnUnitDeathAnimationCompletedEvent(object sender, EventArgs e)
        // {
        //     // Death animation finished, show the death popup
        //     // _windowOpener.OpenDeathWindow(_unit.GetLevel(), _battleLog.KillCount, _battleLog.BattleTime, OnRestartGame, OnReviveUnit);
        // }

        private void OnRestartGame()
        {
            _logger.Info("Player chose to Restart Game."); // Changed to Info
            // In a real game, this would typically involve reloading the current scene
            // or transitioning to a game over screen and then restarting.
            // For now, we simulate by destroying the current scene.
            Destroy(); // Destroy current scene controller
            _world.Resume();
         
        }
    }
}
