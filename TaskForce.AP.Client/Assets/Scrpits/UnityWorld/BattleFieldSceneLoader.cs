using System;
using TaskForce.AP.Client.Core;
using TaskForce.AP.Client.Core.BattleFieldScene;
using TaskForce.AP.Client.Core.BattleFieldScene.Skills;
using TaskForce.AP.Client.UnityWorld.AssetData;
using TaskForce.AP.Client.UnityWorld.BattleFieldScene;
using TaskForce.AP.Client.UnityWorld.View;
using TaskForce.AP.Client.UnityWorld.View.BattleFieldScene;
using UnityEngine;

namespace TaskForce.AP.Client.UnityWorld
{
    public class BattleFieldSceneLoader
    {
        private readonly Screen _screen;
        private readonly UserDataStore _userDataStore;
        private readonly GameDataStore _gameDataStore;
        private readonly Core.Random _random;
        private readonly Time _time;
        private readonly TextStore _textStore;
        private readonly AssetLoader _assetLoader;
        private readonly Core.ILogger _logger;
        private readonly Action _onGoToLobbyEvent;

        public BattleFieldSceneLoader(Screen screen, GameDataStore gameDataStore,
            Core.Random random, Time time, TextStore textStore,
            AssetLoader assetLoader, Core.ILogger logger, UserDataStore userDataStore,
            Action onGoToLobbyEvent)
        {
            _screen = screen;
            _gameDataStore = gameDataStore;
            _random = random;
            _time = time;
            _textStore = textStore;
            _assetLoader = assetLoader;
            _logger = logger;
            _userDataStore = userDataStore;
            _onGoToLobbyEvent = onGoToLobbyEvent;
        }

        public async void Load()
        {
            UnityEngine.Time.timeScale = 1f;
            AudioListener.pause = false;

            await _screen.ShowLoadingBlind();
            await _screen.DestroyLastScene();

            var instance = await _screen.AttachNewScene(SceneID.BattleFieldScene);

            var scene = instance.GetComponent<View.Scenes.BattleFieldScene>();

            var objFac = scene.ObjectFactory;
            var loop = scene.Loop;
            var world = scene.World;
            var joystick = new BattleFieldScene.Joystick(scene.Joystick);
            var targetFinder = new BattleFieldScene.TargetFinder();
            var soulFinder = new SoulFinder();
            var playerUnitSpawnPosition = scene.PlayerUnitSpawnPosition;
            var followCamera = scene.FollowCamera;
            var expBar = scene.ExpBar;
            var levelText = scene.LevelText;
            Func<Timer> createTimer = () => new Timer(_time, loop);

            Func<FloatingTextAnimator> createFloatingTextAnimator = () => objFac.Create<UnityWorld.View.FloatingTextAnimator>(ObjectID.FloatingTextAnimator);

            objFac.Logger = _logger;

            Action<UnityWorld.Object> unitPrepareHdlr = (go) =>
            {
                var unit = go.GetComponent<UnityWorld.View.BattleFieldScene.Unit>();

                unit.gameObject.name = Guid.NewGuid().ToString();
                unit.CreateFloatingTextAnimator = createFloatingTextAnimator;
                unit.Timer = createTimer();
            };
            objFac.RegisterPrepareHandler("WARRIOR_0", unitPrepareHdlr);
            objFac.RegisterPrepareHandler("WARRIOR_1", unitPrepareHdlr);
            objFac.RegisterPrepareHandler("MONK", unitPrepareHdlr);

            world.Random = _random;

            var formulaCalculator = new FormulaCalculator(_logger);
            formulaCalculator.RegisterFormula("FORMULA_0", (coeffs, variables) => { return coeffs["a"]; });
            formulaCalculator.RegisterFormula("FORMULA_2", (coeffs, variables) => { return coeffs["a"] + variables[0]; });

            var effectFactory = new TaskForce.AP.Client.Core.Entity.ModifyAttributeEffectFactory(_gameDataStore, formulaCalculator);
            var skillFactory = new SkillFactory();
            var unitLogicFactory = new UnitLogicFactory(joystick, world, createTimer, loop, soulFinder, _gameDataStore, _logger);
            var soulFactory = new SoulFactory(() => objFac.Create<View.BattleFieldScene.Soul>(ObjectID.Soul));
            var dropHandler = new DropHandler(soulFactory, _random, _gameDataStore);
            var skillEntityFactory = new TaskForce.AP.Client.Core.Entity.SkillFactory(_gameDataStore, _logger, _textStore, effectFactory);
            var unitFactory = new UnitFactory(_random, createTimer, targetFinder,
                (id) => objFac.Create<View.BattleFieldScene.Unit>(id), _logger,
                skillFactory.Create, _gameDataStore, unitLogicFactory.Create, skillEntityFactory.CreateSkill);
            var unitEntityFactory = new Core.Entity.UnitFactory(_logger, _gameDataStore, skillEntityFactory.CreateSkill);

            skillFactory.AddCreator(Core.Entity.SkillID.Monk, (skill) =>
            {
                return new MonkSkill(skill, unitFactory.CreateNonPlayerUnit);
            });
            skillFactory.AddCreator(Core.Entity.SkillID.SheepMissile, (skill) =>
            {
                return new SheepMissileSkill(_random, new RepeatTimer(createTimer()),
                    createTimer(), (IUnit caster, int minDmg, int maxDmg) =>
                    {
                        var view = objFac.Create<Sheep>(ObjectID.SheepMissile);
                        return new SheepMissile(_random, view, caster, minDmg, maxDmg, targetFinder);
                    }, skill);
            });
            skillFactory.AddCreator(Core.Entity.SkillID.Dynamite, (skill) =>
            {
                return new DynamiteSkill(_random, new RepeatTimer(createTimer()),
                    createTimer(), (IUnit caster, int minDmg, int maxDmg, float explosionRadius) =>
                    {
                        var view = objFac.Create<Sheep>(ObjectID.Dynamite);
                        return new Dynamite(view, caster,
                        minDmg, maxDmg, explosionRadius, (IUnit caster, int minDmg, int maxDmg, float explosionRadius) =>
                        {
                            var view = objFac.Create<View.BattleFieldScene.Explosion>(ObjectID.Explosion0);
                            return new Core.BattleFieldScene.Skills.Explosion(view, caster, _random, minDmg, maxDmg, explosionRadius);
                        });
                    }, skill);
            });
            skillFactory.AddCreator(Core.Entity.SkillID.PowderKeg, (skill) =>
            {
                return new PowderKegSkill(skill, createTimer(), (IUnit caster,
                    int minDmg, int maxDmg, float watchRadius, float explosionRadius, float expireTime) =>
                {
                    var view = objFac.Create<View.BattleFieldScene.PowderKeg>(ObjectID.PowderKeg);
                    return new Core.BattleFieldScene.Skills.PowderKeg(view, caster, createTimer(), minDmg, maxDmg,
                        watchRadius, explosionRadius, expireTime, (IUnit caster, int minDmg, int maxDmg, float explosionRadius) =>
                        {
                            var view = objFac.Create<View.BattleFieldScene.Explosion>(ObjectID.Explosion1);
                            return new Core.BattleFieldScene.Skills.Explosion(view, caster, _random, minDmg, maxDmg, explosionRadius);
                        });
                });
            });
            skillFactory.AddCreator(Core.Entity.SkillID.Heal, (skill) =>
            {
                return new HealSkill(createTimer, () => objFac.Create<OneShotEffect>(ObjectID.HealEffect), skill);
            });
            skillFactory.AddCreator(Core.Entity.SkillID.MeleeAttack, (skill) =>
            {
                return new Core.BattleFieldScene.Skills.MeleeAttackSkill(createTimer, skill, _random);
            });

            var battleLog = new BattleLog();
            var battleLogRecorder = new BattleLogRecorder(battleLog, _time);
            loop.Add(battleLogRecorder);

            soulFactory.SoulCreatedEvent += soulFinder.OnSoulCreatedEvent;
            unitFactory.UnitCreatedEvent += targetFinder.OnTargetCreatedEvent;
            unitFactory.UnitCreatedEvent += dropHandler.OnUnitCreatedEvent;
            EventHandler<CreatedEventArgs<Core.BattleFieldScene.Unit>> battleLogRecorderHdlr = (sender, e) =>
            {
                if (e.CreatedObject.IsPlayerSide())
                    return;
                e.CreatedObject.DiedEvent += battleLogRecorder.OnUnitDied;
                e.CreatedObject.DestroyEvent += (s, args) => e.CreatedObject.DiedEvent -= battleLogRecorder.OnUnitDied;
            };
            unitFactory.UnitCreatedEvent += battleLogRecorderHdlr;

            var windowStack = scene.WindowStack;

            var window = windowStack.LevelUpWindow;
            var deathPopup = windowStack.deathWindow;

            // TODO: 실제 SoundPlayer 구현체로 교체 필요
            var mockSoundPlayer = new MockSoundPlayer();
            var winOpener = new WindowOpener(windowStack, world, _textStore, mockSoundPlayer, _logger);

            var skillIconGrid = scene.SkillIconGrid;
            foreach (var icon in skillIconGrid.Icons)
            {
                icon.AssetLoader = _assetLoader;
                icon.Logger = _logger;
            }

            var pausePanel = scene.PausePanel;
            var pausePanelCtrl = new PausePanelController(pausePanel, world);
            pausePanelCtrl.Start();

            var sceneCtrl = new BattleFieldSceneController(scene, world, followCamera, winOpener,
                unitFactory.CreatePlayerUnit, _gameDataStore, _random, _logger,
                skillEntityFactory.CreateSkillEntity,
                unitEntityFactory.CreateUnitEntity, createTimer(),
                _onGoToLobbyEvent, battleLog, _userDataStore, skillIconGrid);
            sceneCtrl.Start();
            loop.Add(sceneCtrl);

            EventHandler<DestroyEventArgs> hdlr = null;
            hdlr = (sender, args) =>
            {
                soulFactory.SoulCreatedEvent -= soulFinder.OnSoulCreatedEvent;
                unitFactory.UnitCreatedEvent -= targetFinder.OnTargetCreatedEvent;
                unitFactory.UnitCreatedEvent -= dropHandler.OnUnitCreatedEvent;
                unitFactory.UnitCreatedEvent -= battleLogRecorderHdlr;

                loop.Remove(battleLogRecorder);
                loop.Remove(sceneCtrl);
                targetFinder.Destroy();

                scene.DestroyEvent -= hdlr;
            };
            scene.DestroyEvent += hdlr;

            var spawner = new EnemyUnitSpawner(world, _gameDataStore, new Core.Timer(_time, loop),
                _logger, _random, unitFactory.CreateEnemyUnit);
            loop.Add(spawner);

            spawner.Start(1);

            _screen.HideLoadingBlind();
        }
    }
}
