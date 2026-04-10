using System;
using System.Threading;
using TaskForce.AP.Client.Core;
using TaskForce.AP.Client.Core.TitleScene;
using TaskForce.AP.Client.UnityWorld.LobbyScene;
using TaskForce.AP.Client.UnityWorld.TitleScene;
using UnityEngine;

namespace TaskForce.AP.Client.UnityWorld
{
    public class Starter : MonoBehaviour
    {
        [SerializeField]
        private Screen _screen;
        [SerializeField]
        private View.Scenes.TitleScene _titleScene;
        [SerializeField]
        private AdvertisementPlayer _advertisementPlayer;
        [SerializeField]
        private Loop _loop;
        [SerializeField]
        private GameObject _persistantObject;

        private SynchronizationContext _synchronizationContext;

        private static bool _initialized = false;

        private void Start()
        {
            if (_initialized)
            {
                Destroy(gameObject);
                return;
            }
            _initialized = true;

            _synchronizationContext = SynchronizationContext.Current;

            DontDestroyOnLoad(_persistantObject);

            StartAsync();
        }

        private async void StartAsync()
        {
            var userDataStore = new UserDataStore();
            var application = new EditorApplication();
            var logger = new DebugLogger(application);
            var time = new Time();
            var random = new Core.Random();
            var assetPathStore = new AssetPathStore(logger);
            var gameDataStore = new GameDataStore();
            var textStore = new TextStore(logger);

            var csvReader = new CsvReader(logger);
            var assetLoader = new AssetLoader(assetPathStore, logger);
            var csvLoader = new CsvLoader(csvReader, assetLoader);
            var assetPathLoader = new AssetPathLoader(csvLoader);
            var gameDataLoader = new GameDataLoader(csvLoader);
            var textStoreLoader = new TextStoreLoader(csvLoader);

            await assetPathLoader.Load(assetPathStore);
            await gameDataLoader.Load(gameDataStore);
            await textStoreLoader.Load("ko", textStore);

            var userDataLoader = new UserDataLoader(userDataStore);
            userDataLoader.Load();
            _loop.Add(userDataLoader);

            _advertisementPlayer.Logger = logger;
            _advertisementPlayer.RetryTimer = new Core.Timer(time, _loop);
            _advertisementPlayer.RewardedRetryTimer = new Core.Timer(time, _loop);
            _advertisementPlayer.SynchronizationContext = _synchronizationContext;
            _advertisementPlayer.Initialize();

            _screen.AssetLoader = assetLoader;
            _screen.Logger = logger;

            Destroy(gameObject);

            LobbySceneLoader lobbySceneLoader = null;
            BattleFieldSceneLoader battleFieldSceneLoader = null;
            TitleSceneLoader titleSceneLoader = null;

            Action goToLobbyAction = () => lobbySceneLoader.Load();
            Action goToBattleAction = () => battleFieldSceneLoader.Load();

            titleSceneLoader = new TitleSceneLoader(_screen, goToLobbyAction);
            lobbySceneLoader = new LobbySceneLoader(_screen, gameDataStore, random, time, textStore, assetLoader, logger, userDataStore, goToBattleAction, _advertisementPlayer);
            battleFieldSceneLoader = new BattleFieldSceneLoader(_screen, gameDataStore, random, time, textStore, assetLoader, logger, userDataStore, goToLobbyAction, _advertisementPlayer);

            var soundPlayer = _titleScene.SoundPlayer;
            new MockSoundPlayer(soundPlayer, userDataStore, logger);
            
            var titleSceneCtrl = new TitleSceneController(_titleScene, goToLobbyAction);
            titleSceneCtrl.Start();
        }
    }
}
