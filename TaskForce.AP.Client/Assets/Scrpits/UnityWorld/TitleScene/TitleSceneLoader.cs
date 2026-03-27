using System;
using TaskForce.AP.Client.Core.TitleScene;

namespace TaskForce.AP.Client.UnityWorld.TitleScene
{
    public class TitleSceneLoader
    {
        private readonly Screen _screen;
        private readonly Action _onGoToLobby;

        public TitleSceneLoader(Screen screen, Action onGoToLobby)
        {
            _screen = screen;
            _onGoToLobby = onGoToLobby;
        }

        public async void Load()
        {
            await _screen.ShowLoadingBlind();
            await _screen.DestroyLastScene();

            var instance = await _screen.AttachNewScene(SceneID.TitleScene);

            var scene = instance.GetComponent<View.Scenes.TitleScene>();

            var sceneCtrl = new TitleSceneController(scene, _onGoToLobby);
            sceneCtrl.Start();

            _screen.HideLoadingBlind();
        }
    }
}
