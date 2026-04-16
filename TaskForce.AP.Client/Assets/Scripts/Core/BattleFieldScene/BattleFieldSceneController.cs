using System;
using System.Collections.Generic;
using System.Linq; // Add Linq
using TaskForce.AP.Client.Core.View.BattleFieldScene;
using TaskForce.AP.Client.Core.View.Scenes;

namespace TaskForce.AP.Client.Core.BattleFieldScene
{
    public class BattleFieldSceneController : IUpdatable
    {
        private readonly IBattleFieldScene _scene;
        private readonly IWorld _world;
        private readonly IFollowCamera _followCamera;
        private readonly WindowOpener _windowOpener;
        private readonly ILogger _logger;
        private readonly Core.Timer _timer;
        private readonly Action _onGoToLobbyEvent;
        private readonly BattleLog _battleLog;
        private readonly UserDataStore _userDataStore;
        private readonly View.BattleFieldScene.ISkillIconGrid _skillIconGrid;
        private readonly GameDataStore _gameDataStore; // Added GameDataStore

        private bool _isDestroyed;
        private IUnit _unit;
        private Entity.Unit _unitEntity;
        private readonly Queue<int> _levelUpQueue = new Queue<int>();
        private bool _isLevelUpWindowOpen;
        private Random _random;

        public BattleFieldSceneController(IBattleFieldScene scene, IWorld world, IFollowCamera followCamera,
            WindowOpener windowOpener, ILogger logger, Core.Timer timer,
            Action onGoToLobbyEvent, BattleLog battleLog, UserDataStore userDataStore,
            View.BattleFieldScene.ISkillIconGrid skillIconGrid,
            IUnit unit, Entity.Unit unitEntity, GameDataStore gameDataStore, Random random) // Added GameDataStore
        {
            _scene = scene;
            _world = world;
            _followCamera = followCamera;
            _windowOpener = windowOpener;
            _isDestroyed = false;
            _logger = logger;
            _timer = timer;
            _onGoToLobbyEvent = onGoToLobbyEvent;
            _battleLog = battleLog;
            _userDataStore = userDataStore;
            _skillIconGrid = skillIconGrid;
            _unit = unit;
            _unitEntity = unitEntity;
            _gameDataStore = gameDataStore;
            _random = random;
        }

        public void Update()
        {
            _scene.SetBattleTime(_battleLog.BattleTime);
            _scene.SetKillCount(_battleLog.KillCount);
            UpdateGold();
        }

        public void Start()
        {
            UpdateLevel();
            UpdateRequireExp();
            UpdateExp();
            UpdateGold();

            _unit.RequireExpChangedEvent += OnRequireExpChangedEvent;
            _unit.ExpChangedEvent += OnExpChangedEvent;
            _unit.LevelUpEvent += OnLevelUpEvent;
            _unit.DestroyEvent += OnUnitDestroyEvent;
            _unitEntity.DiedEvent += OnUnitEntityDiedEvent; // Subscribe to Core.Entity.Unit's DiedEvent
            _unit.DeathAnimationCompletedEvent += OnUnitDeathAnimationCompletedEvent; // Subscribe to Unity Unit's death animation completion
            _unit.HitEvent += OnUnitHitEvent;

            _followCamera.SetTarget(_unit);

            _skillIconGrid.SetIconSlots(_unitEntity.GetMaxSkillCount());

            var skillIconGridCtrl = new SkillIconGridController(_skillIconGrid, _unitEntity);
            skillIconGridCtrl.Start();

            _scene.DestroyEvent += OnDestroySceneEvent;
            _scene.PauseButtonClickedEvent += OnPauseButtonClickedEvent;
        }

        private void OnPauseButtonClickedEvent(object sender, EventArgs e)
        {
            _windowOpener.OpenSettingWindow(_onGoToLobbyEvent);
        }

        private void OnLevelUpEvent(object sender, EventArgs e)
        {
            UpdateLevel();
            
            // Update slots based on PlayerRank data for the new level
            int currentLevel = _unit.GetLevel();
            var rankData = _gameDataStore.GetPlayerRanks().FirstOrDefault(r => r.Rank == currentLevel);
            if (rankData != null)
            {
                _unitEntity.SetMaxSkillCount(rankData.SlotNum);
                _skillIconGrid.SetIconSlots(rankData.SlotNum);
            }

            _levelUpQueue.Enqueue(currentLevel);
            TryOpenNextLevelUpWindow();
        }

        private void TryOpenNextLevelUpWindow()
        {
            if (_isLevelUpWindowOpen) return;
            if (_levelUpQueue.Count == 0) return;

            var level = _levelUpQueue.Dequeue();
            _isLevelUpWindowOpen = true;
            _windowOpener.OpenLevelUpWindow(_unitEntity, level, OnLevelUpWindowClosed);
        }

        private void OnLevelUpWindowClosed()
        {
            _isLevelUpWindowOpen = false;
            TryOpenNextLevelUpWindow();
        }

        private void OnExpChangedEvent(object sender, EventArgs e)
        {
            UpdateExp();
        }

        private void OnRequireExpChangedEvent(object sender, EventArgs e)
        {
            UpdateRequireExp();
        }

        private void UpdateLevel()
        {
            _scene.SetLevel(_unit.GetLevel().ToString());
        }

        private void UpdateExp()
        {
            _scene.SetExp(_unit.GetExp());
        }

        private void UpdateRequireExp()
        {
            _scene.SetRequireExp(_unit.GetRequireExp());
        }

        private void UpdateGold()
        {
            _scene.SetGold(_userDataStore.GetGold());
        }

        private void OnDestroySceneEvent(object sender, EventArgs e)
        {
            Destroy();
        }

        private void OnUnitDestroyEvent(object sender, DestroyEventArgs e)
        {
            Destroy();
        }

        private void Destroy()
        {
            if (_isDestroyed)
                return;
            _isDestroyed = true;

            _followCamera.UnsetTarget();

            _unit.RequireExpChangedEvent -= OnRequireExpChangedEvent;
            _unit.ExpChangedEvent -= OnExpChangedEvent;
            _unit.LevelUpEvent -= OnLevelUpEvent;
            _unit.DestroyEvent -= OnUnitDestroyEvent;
            _unitEntity.DiedEvent -= OnUnitEntityDiedEvent; // Unsubscribe from Core.Entity.Unit's DiedEvent
            _unit.DeathAnimationCompletedEvent -= OnUnitDeathAnimationCompletedEvent; // Unsubscribe from Unity Unit's death animation completion
            _unit.HitEvent -= OnUnitHitEvent;
            _unit.Destroy();
            _unit = null;
        }

        private void OnUnitEntityDiedEvent(object sender, EventArgs e)
        {
            // Core.Entity.Unit has died, trigger visual death for Unity MonoBehaviour
            if (_unit != null)
            {
                _unit.Die(); // Play death animation, will then invoke DeathAnimationCompletedEvent
            }
        }

        private void OnUnitDeathAnimationCompletedEvent(object sender, EventArgs e)
        {
            // Death animation finished, show the death popup
            _windowOpener.OpenDeathWindow(_unit.GetLevel(), _battleLog.KillCount, _battleLog.BattleTime, GoToLobby, OnReviveUnit);
        }

        private void OnUnitHitEvent(object sender, EventArgs e)
        {
            _unit.SetInvincible(true);
            float invincibleTime = _random.NextFloat(0.5f, 1f);
            
            _timer.Start(invincibleTime, () => {
                if (_unit != null && !_unit.IsDead())
                    _unit.SetInvincible(false);
            });
        }

        private void GoToLobby()
        {
            _logger.Info("Player chose to go to Lobby.");
            Destroy(); // Destroy current scene controller
            _world.Resume();
            _onGoToLobbyEvent?.Invoke(); // Trigger lobby scene load
        }

        private void OnReviveUnit()
        {
            _logger.Info("Player chose to Revive Unit (AdMob linked)."); // Changed to Info
            // Simulate Google AdMob ad
            bool adSuccess = true; // 100% chance for testing.

            if (adSuccess)
            {
                _logger.Info("AdMob ad watched successfully! Reviving unit."); // Changed to Info
                
                if (_unit != null)
                {
                    _unit.RecoverFullHP(); // Restore full HP and update UI
                    _unit.SetActive(true); // Reactivate GameObject
                    _unit.Wait(); // Reset unit's visual state to idle animation
                    
                    _unit.SetInvincible(true); // 깜빡이는 동안에도 공격을 받지 않도록 즉시 무적 적용
                    
                    _unit.PlayReviveEffect(() => {
                        _world.Resume(); // Resume game (여기서 몹들이 움직이기 시작함)
                        
                        // 3초 무적 상태 해제는 게임이 재개된 시점부터 3초 뒤에
                        _timer.Start(3.0f, () => {
                            if (_unit != null && !_unit.IsDead())
                                _unit.SetInvincible(false);
                        });
                    }); // [REVIVE_EFFECT_TEST]
                }
            }
            else
            {
                _logger.Info("AdMob ad failed or was skipped. Proceeding to restart game."); // Changed to Info
                GoToLobby(); // If ad fails, go to lobby
            }
        }
    }
}
