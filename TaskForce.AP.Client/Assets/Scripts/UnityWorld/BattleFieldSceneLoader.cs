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
        private readonly Core.IAdvertisementPlayer _advertisementPlayer;

        public BattleFieldSceneLoader(Screen screen, GameDataStore gameDataStore,
            Core.Random random, Time time, TextStore textStore,
            AssetLoader assetLoader, Core.ILogger logger, UserDataStore userDataStore,
            Action onGoToLobbyEvent, Core.IAdvertisementPlayer advertisementPlayer)
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
            _advertisementPlayer = advertisementPlayer;
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
            var fieldObjectFinder = new FieldObjectFinder();
            var playerUnitSpawnPosition = scene.PlayerUnitSpawnPosition;
            var followCamera = scene.FollowCamera;
            var expBar = scene.ExpBar;
            var levelText = scene.LevelText;
            Func<Timer> createTimer = () => new Timer(_time, loop);

            Func<FloatingTextAnimator> createFloatingTextAnimator = () => objFac.Create<UnityWorld.View.FloatingTextAnimator>(ObjectID.FloatingTextAnimator);

            objFac.Logger = _logger;
            objFac.CoreTime = _time; // ObjectFactory에 Core.Time 주입
            objFac.CoreLoop = loop; // ObjectFactory에 ILoop 주입

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
            var unitLogicFactory = new UnitLogicFactory(joystick, world, createTimer, loop, fieldObjectFinder, _gameDataStore, _logger, _userDataStore);
            var expOrbFactory = new ExpOrbFactory(() => objFac.Create<View.BattleFieldScene.ExpOrb>(ObjectID.ExpOrb));
            var skillEntityFactory = new TaskForce.AP.Client.Core.Entity.SkillFactory(_gameDataStore, _logger, _textStore, effectFactory);
            var unitEntityFactory = new Core.Entity.UnitFactory(_logger, _gameDataStore, skillEntityFactory.CreateSkill);
            var unitFactory = new UnitFactory(_random, createTimer, targetFinder,
                (id) => objFac.Create<View.BattleFieldScene.Unit>(id), _logger,
                skillFactory.Create, _gameDataStore, unitLogicFactory.Create, unitEntityFactory.CreateUnitEntity);

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
            skillFactory.AddCreator(Core.Entity.SkillID.Grenade, (skill) =>
            {
                return new GrenadeSkill(_random, new RepeatTimer(createTimer()),
                    createTimer(), (IUnit caster, int minDmg, int maxDmg, float explosionRadius) =>
                    {
                        var view = objFac.Create<Sheep>(ObjectID.Grenade);
                        return new Grenade(view, caster,
                        minDmg, maxDmg, explosionRadius, (IUnit caster, int minDmg, int maxDmg, float explosionRadius) =>
                        {
                            var view = objFac.Create<View.BattleFieldScene.Explosion>(ObjectID.Explosion0);
                            return new Core.BattleFieldScene.Skills.Explosion(view, caster, _random, minDmg, maxDmg, explosionRadius);
                        });
                    }, skill);
            });
            skillFactory.AddCreator(Core.Entity.SkillID.RpgMissile, (skill) =>
            {
                return new RpgMissileSkill(_random, new RepeatTimer(createTimer()),
                    createTimer(), (IUnit caster, int minDmg, int maxDmg, float explosionRadius) =>
                    {
                        var view = objFac.Create<Rpg>(ObjectID.Rpg);
                        return new RpgMissile(view, caster,
                            minDmg, maxDmg, explosionRadius, (IUnit caster, int minDmg, int maxDmg, float explosionRadius) =>
                            {
                                var view = objFac.Create<View.BattleFieldScene.Explosion>(ObjectID.Explosion0);
                                return new Core.BattleFieldScene.Skills.Explosion(view, caster, _random, minDmg, maxDmg, explosionRadius);
                            });
                    }, skill, _logger);
            });
            skillFactory.AddCreator(Core.Entity.SkillID.Landmine, (skill) =>
            {
                return new LandmineSkill(skill, createTimer(), (IUnit caster,
                    int minDmg, int maxDmg, float watchRadius, float explosionRadius, float expireTime) =>
                {
                    var view = objFac.Create<View.BattleFieldScene.PowderKeg>(ObjectID.Landmine);
                    return new Core.BattleFieldScene.Skills.Landmine(view, caster, createTimer(), minDmg, maxDmg,
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
            skillFactory.AddCreator(Core.Entity.SkillID.MeleeDagger, (skill) =>
            {
                return new Core.BattleFieldScene.Skills.MeleeDaggerSkill(createTimer, skill, _random, _logger);
            });
            skillFactory.AddCreator(Core.Entity.SkillID.MeleeBat, (skill) =>
            {
                return new Core.BattleFieldScene.Skills.MeleeBatSkill(createTimer, skill, _random, _logger);
            });
            skillFactory.AddCreator(Core.Entity.SkillID.PistolAttack, (skill) => // SkillID.Pistol -> SkillID.PistolAttack
            {
                // Core.BattleFieldScene.Skills.Bullet을 생성하는 람다 함수
                return new PistolSkill((caster, dmg, finder) =>
                {
                    var view = objFac.Create<View.BattleFieldScene.Bullet>(ObjectID.Bullet); // Unity View Bullet 생성
                    var bullet = new Core.BattleFieldScene.Skills.Bullet(_random, view, finder, caster, dmg);
                    
                    loop.Add(bullet); // Bullet을 루프에 추가하여 Update가 호출되도록 함
                    
                    // 파괴 시 루프에서 제거
                    EventHandler<DestroyEventArgs> onBulletDestroyed = null;
                    onBulletDestroyed = (s, e) =>
                    {
                        loop.Remove(bullet);
                        bullet.DestroyEvent -= onBulletDestroyed;
                    };
                    bullet.DestroyEvent += onBulletDestroyed;
                    
                    return bullet;
                }, targetFinder, skill);
            });

            skillFactory.AddCreator(Core.Entity.SkillID.SniperRifle, (skill) =>
            {
                return new SniperSkill((caster, dmg, finder) =>
                {
                    var view = objFac.Create<View.BattleFieldScene.Bullet>(ObjectID.SniperBullet);
                    var bullet = new Core.BattleFieldScene.Skills.Bullet(_random, view, finder, caster, dmg);
                    loop.Add(bullet);
                    EventHandler<DestroyEventArgs> onBulletDestroyed = null;
                    onBulletDestroyed = (s, e) =>
                    {
                        loop.Remove(bullet);
                        bullet.DestroyEvent -= onBulletDestroyed;
                    };
                    bullet.DestroyEvent += onBulletDestroyed;
                    return bullet;
                }, targetFinder, skill);
            });

            // 패시브 장비 7종 추가
            skillFactory.AddCreator(Core.Entity.SkillID.Gloves, (skill) => new PassiveSkill(skill));
            skillFactory.AddCreator(Core.Entity.SkillID.Armor, (skill) => new PassiveSkill(skill));
            skillFactory.AddCreator(Core.Entity.SkillID.Helmet, (skill) => new PassiveSkill(skill));
            skillFactory.AddCreator(Core.Entity.SkillID.Boots, (skill) => new PassiveSkill(skill));
            skillFactory.AddCreator(Core.Entity.SkillID.TacticalBackpack, (skill) => new PassiveSkill(skill));
            skillFactory.AddCreator(Core.Entity.SkillID.ArmorPiercingBullet, (skill) => new PassiveSkill(skill));
            skillFactory.AddCreator(Core.Entity.SkillID.TacticalManual, (skill) => new PassiveSkill(skill));

            var battleLog = new BattleLog();
            var battleLogRecorder = new BattleLogRecorder(battleLog, _time);
            loop.Add(battleLogRecorder);

            expOrbFactory.ExpOrbCreatedEvent += fieldObjectFinder.OnExpOrbCreatedEvent;
            fieldItemFactory.FieldItemCreatedEvent += fieldObjectFinder.OnFieldItemCreatedEvent;
            unitFactory.UnitCreatedEvent += targetFinder.OnTargetCreatedEvent;
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
            var winOpener = new WindowOpener(windowStack, world, _textStore, mockSoundPlayer, _logger, _advertisementPlayer,
                _gameDataStore, _random, skillEntityFactory.CreateSkill);

            var skillIconGrid = scene.SkillIconGrid;
            foreach (var icon in skillIconGrid.Icons)
            {
                icon.AssetLoader = _assetLoader;
                icon.Logger = _logger;
            }

            foreach (var panel in windowStack.LevelUpWindow.SkillPanels)
            {
                panel.SkillIcon.AssetLoader = _assetLoader;
                panel.SkillIcon.Logger = _logger;
            }

            var pausePanel = scene.PausePanel;
            var pausePanelCtrl = new PausePanelController(pausePanel, world);
            pausePanelCtrl.Start();

            var unitEntity = unitEntityFactory.CreateUnitEntity("WARRIOR_0");
            unitEntity.SetMaxSkillCount(5);
            unitEntity.SetSkillCountLimit(8);
            var unit = unitFactory.CreatePlayerUnit(unitEntity);
            unit.SetPosition(world.GetPlayerUnitSpawnPosition());

            var sceneCtrl = new BattleFieldSceneController(scene, world, followCamera, winOpener,
                _logger, createTimer(),
                _onGoToLobbyEvent, battleLog, _userDataStore, skillIconGrid,
                unit, unitEntity);
            sceneCtrl.Start();
            loop.Add(sceneCtrl);

            var stageHost = new StageHost(world, _gameDataStore, new Core.Timer(_time, loop),
                createTimer(), _logger, _random, unitFactory.CreateEnemyUnit);

            var fieldItemFactory = new FieldItemFactory((id) => objFac.Create<View.BattleFieldScene.FieldItem>(id), _gameDataStore, fieldObjectFinder, stageHost, _random);
            var fieldObjectDropHandler = new FieldObjectDropHandler(expOrbFactory, fieldItemFactory, _random, _gameDataStore);

            var bossStageHost = new Core.BattleFieldScene.BossStageHost(world, _gameDataStore,
                createTimer(), _logger, unitFactory.CreateEnemyUnit);

            stageHost.EnemyKilledEvent += fieldObjectDropHandler.OnEnemyKilled;
            bossStageHost.AllBossesKilledEvent += fieldObjectDropHandler.OnAllBossesKilled;

            EventHandler<Core.BattleFieldScene.DiedEventArgs> onEnemyKilledAddGold = (sender, args) =>
            {
                if (args.Killer == null || !args.Killer.IsPlayerSide())
                    return;
                _userDataStore.AddGold(1);
            };
            stageHost.EnemyKilledEvent += onEnemyKilledAddGold;

            var rootBoxFactory = new RootBoxFactory(
                () => objFac.Create<View.BattleFieldScene.RootBox>(ObjectID.RootBox), _gameDataStore);
            rootBoxFactory.RootBoxCreatedEvent += targetFinder.OnRootBoxCreatedEvent;
            EventHandler<CreatedEventArgs<Core.BattleFieldScene.RootBox>> onRootBoxCreated =
                (_, e) => e.CreatedObject.DiedEvent += fieldObjectDropHandler.OnRootBoxDied;
            rootBoxFactory.RootBoxCreatedEvent += onRootBoxCreated;
            var rootBoxSpawner = new RootBoxSpawner(
                rootBoxFactory, createTimer(), createTimer(), world, _gameDataStore, unit);

            EventHandler<DestroyEventArgs> hdlr = null;
            hdlr = (sender, args) =>
            {
                expOrbFactory.ExpOrbCreatedEvent -= fieldObjectFinder.OnExpOrbCreatedEvent;
                fieldItemFactory.FieldItemCreatedEvent -= fieldObjectFinder.OnFieldItemCreatedEvent;
                unitFactory.UnitCreatedEvent -= targetFinder.OnTargetCreatedEvent;
                unitFactory.UnitCreatedEvent -= battleLogRecorderHdlr;
                rootBoxFactory.RootBoxCreatedEvent -= targetFinder.OnRootBoxCreatedEvent;
                rootBoxFactory.RootBoxCreatedEvent -= onRootBoxCreated;
                stageHost.EnemyKilledEvent -= fieldObjectDropHandler.OnEnemyKilled;
                stageHost.EnemyKilledEvent -= onEnemyKilledAddGold;
                bossStageHost.AllBossesKilledEvent -= fieldObjectDropHandler.OnAllBossesKilled;

                loop.Remove(battleLogRecorder);
                loop.Remove(sceneCtrl);
                targetFinder.Destroy();
                fieldObjectFinder.Destroy();
                rootBoxSpawner.Destroy();

                scene.DestroyEvent -= hdlr;
            };
            scene.DestroyEvent += hdlr;

            stageHost.Start(1);

            var swarmGenerator = new Core.BattleFieldScene.SwarmGenerator(world, _gameDataStore,
                createTimer(), unitFactory.CreateEnemyUnit);

            swarmGenerator.Start();
            rootBoxSpawner.Start();

            bossStageHost.Start(1);

            _screen.HideLoadingBlind();
        }
    }
}
