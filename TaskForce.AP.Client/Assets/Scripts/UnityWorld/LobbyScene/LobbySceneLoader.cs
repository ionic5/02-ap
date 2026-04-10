using System;
using TaskForce.AP.Client.Core;
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
        private readonly Core.IAdvertisementPlayer _advertisementPlayer;

        private event Action _battleFieldSceneLoadEvent;

        public LobbySceneLoader(Screen screen, GameDataStore gameDataStore,
            Core.Random random, Time time, TextStore textStore,
            AssetLoader assetLoader, Core.ILogger logger, UserDataStore userDataStore,
            Action battleFieldSceneLoadEvent, Core.IAdvertisementPlayer advertisementPlayer)
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
            _advertisementPlayer = advertisementPlayer;
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

            var windowStack = scene.WindowStack;
            scene.Logger = _logger;
            
            var soundPlayer = scene.SoundPlayer;
            var mockSoundPlayer = new MockSoundPlayer(soundPlayer, _userDataStore, _logger);
            var winOpener = new WindowOpener(windowStack, _textStore, mockSoundPlayer, _logger, _advertisementPlayer);

            scene.AssetLoader = _assetLoader;

            var sceneCtrl = new LobbySceneController(scene, winOpener, _gameDataStore,
                _logger, _userDataStore, _textStore, _battleFieldSceneLoadEvent);
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
