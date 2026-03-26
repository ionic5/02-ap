using TaskForce.AP.Client.Core;
using TaskForce.AP.Client.UnityWorld.LobbyScene;
using UnityEngine;

namespace TaskForce.AP.Client.UnityWorld
{
    public class LobbyStarter : MonoBehaviour
    {
        [SerializeField]
        private Screen _screen;
        [SerializeField]
        private GameObject _loadingBlind;

        private void Start()
        {
            DontDestroyOnLoad(_screen.gameObject);
            DontDestroyOnLoad(_loadingBlind.gameObject);

            StartAsync();
        }

        private async void StartAsync()
        {
            // TODO: JW: 추후 정리
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
            //

            _screen.AssetLoader = assetLoader;
            _screen.Logger = logger;

            Destroy(gameObject);
            
            var loader = new LobbySceneLoader(_screen, gameDataStore, random, time, textStore, assetLoader, logger, userDataStore);
            loader.Load();
        }
    }
}