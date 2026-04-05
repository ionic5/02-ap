using System;
using TaskForce.AP.Client.Core;
using TaskForce.AP.Client.Core.BattleFieldScene;
using TaskForce.AP.Client.Core.LobbyScene;
using TaskForce.AP.Client.UnityWorld.BattleFieldScene;
using WindowOpener = TaskForce.AP.Client.Core.LobbyScene.WindowOpener;
using UnityEngine;

namespace TaskForce.AP.Client.UnityWorld.LobbyScene
{
    public class LobbySceneLoader
    {
        private readonly Screen _screen;
        private readonly UserDataStore _userDataStore;
        private readonly GameDataStore _gameDataStore;
        private readonly Core.Random _random;
        private readonly Time _time;
        private readonly TextStore _textStore;
        private readonly AssetLoader _assetLoader;
        private readonly Core.ILogger _logger;

        private event Action _battleFieldSceneLoadEvent;

        public LobbySceneLoader(Screen screen, GameDataStore gameDataStore,
            Core.Random random, Time time, TextStore textStore,
            AssetLoader assetLoader, Core.ILogger logger, UserDataStore userDataStore,
            Action battleFieldSceneLoadEvent)
        {
            _screen = screen;
            _gameDataStore = gameDataStore;
            _random = random;
            _time = time;
            _textStore = textStore;
            _assetLoader = assetLoader;
            _logger = logger;
            _userDataStore = userDataStore;
            _battleFieldSceneLoadEvent = battleFieldSceneLoadEvent;
        }

        public async void Load()
        {
            UnityEngine.Time.timeScale = 1f;
            AudioListener.pause = false;

            await _screen.ShowLoadingBlind();
            await _screen.DestroyLastScene();

            var instance = await _screen.AttachNewScene(SceneID.LobbyScene);

            var scene = instance.GetComponent<View.Scenes.LobbyScene>();

            var loop = scene.Loop;
            var world = scene.World;

            var windowStack = scene.WindowStack;

            // TODO: 실제 SoundPlayer 구현체로 교체 필요
            var mockSoundPlayer = new MockSoundPlayer();
            var winOpener = new WindowOpener(windowStack, world, _textStore, mockSoundPlayer, _logger);

            scene.AssetLoader = _assetLoader;
            scene.Logger = _logger;

            // TODO: JW: pause panel 기능 검토
            // var pausePanel = scene.PausePanel;
            // var pausePanelCtrl = new PausePanelController(pausePanel, world);
            // pausePanelCtrl.Start();

            var sceneCtrl = new LobbySceneController(scene, world, winOpener, _gameDataStore,
                _logger, _userDataStore, _battleFieldSceneLoadEvent);
            sceneCtrl.Start();
            loop.Add(sceneCtrl);

            EventHandler<DestroyEventArgs> hdlr = null;
            hdlr = (sender, args) =>
            {
                loop.Remove(sceneCtrl);
                scene.DestroyEvent -= hdlr;
            };
            scene.DestroyEvent += hdlr;

            _screen.HideLoadingBlind();
        }
    }
}
