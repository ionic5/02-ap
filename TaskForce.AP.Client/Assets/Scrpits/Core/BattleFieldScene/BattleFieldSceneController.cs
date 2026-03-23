using System;
using System.Collections.Generic;
using System.Linq;
using TaskForce.AP.Client.Core.View.BattleFieldScene;
using TaskForce.AP.Client.Core.View.Scenes;

namespace TaskForce.AP.Client.Core.BattleFieldScene
{
    public class BattleFieldSceneController
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

        private bool _isDestroyed;
        private IUnit _unit;
        private Entity.Unit _unitEntity;

        public BattleFieldSceneController(IBattleFieldScene scene, IWorld world, IFollowCamera followCamera,
            WindowOpener windowOpener, Func<Entity.Unit, IUnit> createUnit,
            GameDataStore gameDataStore, Random random, ILogger logger,
            Func<Entity.Unit, string, int, Entity.ISkill> createSkillEntity,
            Func<string, Entity.Unit> createUnitEntity)
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
            _unit.Destroy();
            _unit = null;
        }
    }
}
