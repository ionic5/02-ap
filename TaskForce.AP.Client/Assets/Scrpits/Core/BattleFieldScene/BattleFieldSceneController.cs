using System;
using System.Collections.Generic;
using System.Linq;
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
        private readonly GameDataStore _gameDataStore;
        private readonly Func<Entity.Unit, IUnit> _createPlayerUnit;
        private readonly Core.Random _random;
        private readonly ILogger _logger;
        private readonly Func<string, Entity.Unit> _createUnitEntity;
        private readonly Func<Entity.Unit, string, int, Entity.ISkill> _createSkillEntity;
        private readonly Core.Timer _timer;
        private readonly Action _onRestartGame;
        private readonly BattleLog _battleLog;

        private bool _isDestroyed;
        private IUnit _unit;
        private Entity.Unit _unitEntity;

        public BattleFieldSceneController(IBattleFieldScene scene, IWorld world, IFollowCamera followCamera,
            WindowOpener windowOpener, Func<Entity.Unit, IUnit> createUnit,
            GameDataStore gameDataStore, Random random, ILogger logger,
            Func<Entity.Unit, string, int, Entity.ISkill> createSkillEntity,
            Func<string, Entity.Unit> createUnitEntity, Core.Timer timer,
            Action onRestartGame, BattleLog battleLog)
        {
            _scene = scene;
            _world = world;
            _followCamera = followCamera;
            _createPlayerUnit = createUnit;
            _windowOpener = windowOpener;
            _isDestroyed = false;
            _gameDataStore = gameDataStore;
            _random = random;
            _logger = logger;
            _createSkillEntity = createSkillEntity;
            _createUnitEntity = createUnitEntity;
            _timer = timer;
            _onRestartGame = onRestartGame;
            _battleLog = battleLog;
        }

        public void Update()
        {
            _scene.SetBattleTime(_battleLog.BattleTime);
            _scene.SetKillCount(_battleLog.KillCount);
        }

        public void Start()
        {
            _unitEntity = _createUnitEntity.Invoke("WARRIOR_0");

            _unit = _createPlayerUnit(_unitEntity);
            _unit.SetPosition(_world.GetPlayerUnitSpawnPosition());

            UpdateLevel();
            UpdateRequireExp();
            UpdateExp();

            _unit.RequireExpChangedEvent += OnRequireExpChnagedEvent;
            _unit.ExpChangedEvent += OnExpChnagedEvent;
            _unit.LevelUpEvent += OnLevelUpEvent;
            _unit.DestroyEvent += OnUnitDestroyEvent;
            _unitEntity.DiedEvent += OnUnitEntityDiedEvent; // Subscribe to Core.Entity.Unit's DiedEvent
            _unit.DeathAnimationCompletedEvent += OnUnitDeathAnimationCompletedEvent; // Subscribe to Unity Unit's death animation completion

            _followCamera.SetTarget(_unit);

            _scene.DestroyEvent += OnDestroySceneEvent;
            _scene.PauseButtonClickedEvent += OnPauseButtonClickedEvent;
        }

        private void OnPauseButtonClickedEvent(object sender, EventArgs e)
        {
            _windowOpener.OpenSettingWindow();
        }

        private void OnLevelUpEvent(object sender, EventArgs e)
        {
            UpdateLevel();

            var skillIDs = _gameDataStore.GetLevelUpRewardSkills().Select(entry => entry.SkillID).ToArray();
            _random.Shuffle(skillIDs);
            var newSkills = new List<Entity.ISkill>();

            foreach (var skillID in skillIDs.Take(3))
            {
                var skill = _unitEntity.GetSkill(skillID);
                var level = skill != null ? skill.GetLevel() + 1 : 1;
                newSkills.Add(_createSkillEntity.Invoke(_unitEntity, skillID, level));
            }

            _windowOpener.OpenPerkSelectionWindow(_unitEntity, newSkills);
        }

        private void OnExpChnagedEvent(object sender, EventArgs e)
        {
            UpdateExp();
        }

        private void OnRequireExpChnagedEvent(object sender, EventArgs e)
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

            _unit.RequireExpChangedEvent -= OnRequireExpChnagedEvent;
            _unit.ExpChangedEvent -= OnExpChnagedEvent;
            _unit.LevelUpEvent -= OnLevelUpEvent;
            _unit.DestroyEvent -= OnUnitDestroyEvent;
            _unitEntity.DiedEvent -= OnUnitEntityDiedEvent; // Unsubscribe from Core.Entity.Unit's DiedEvent
            _unit.DeathAnimationCompletedEvent -= OnUnitDeathAnimationCompletedEvent; // Unsubscribe from Unity Unit's death animation completion
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
            _windowOpener.OpenDeathWindow(OnRestartGame, OnReviveUnit);
        }

        private void OnRestartGame()
        {
            _logger.Info("Player chose to Restart Game."); // Changed to Info
            // In a real game, this would typically involve reloading the current scene
            // or transitioning to a game over screen and then restarting.
            // For now, we simulate by destroying the current scene.
            Destroy(); // Destroy current scene controller
            _world.Resume();
            _onRestartGame?.Invoke(); // Trigger full scene reload
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
                        _timer.Start(0, 3.0f, () => {
                            if (_unit != null && !_unit.IsDead())
                                _unit.SetInvincible(false);
                        });
                    }); // [REVIVE_EFFECT_TEST]
                }
            }
            else
            {
                _logger.Info("AdMob ad failed or was skipped. Proceeding to restart game."); // Changed to Info
                OnRestartGame(); // If ad fails, restart the game
            }
        }
    }
}
