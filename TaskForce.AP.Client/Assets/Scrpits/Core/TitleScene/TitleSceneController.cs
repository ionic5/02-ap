using System;
using TaskForce.AP.Client.Core.View.Scenes;

namespace TaskForce.AP.Client.Core.TitleScene
{
    public class TitleSceneController
    {
        private readonly ITitleScene _scene;
        private readonly Action _onGoToLobby;

        public TitleSceneController(ITitleScene scene, Action onGoToLobby)
        {
            _scene = scene;
            _onGoToLobby = onGoToLobby;
        }

        public void Start()
        {
            _scene.SetStartButtonVisible(true);
            _scene.StartButtonClickedEvent += OnStartButtonClicked;
            _scene.DestroyEvent += OnDestroyScene;
        }

        private void OnStartButtonClicked(object sender, EventArgs e)
        {
            _scene.StartButtonClickedEvent -= OnStartButtonClicked;
            _scene.DestroyEvent -= OnDestroyScene;
            _onGoToLobby?.Invoke();
        }

        private void OnDestroyScene(object sender, DestroyEventArgs e)
        {
            _scene.StartButtonClickedEvent -= OnStartButtonClicked;
            _scene.DestroyEvent -= OnDestroyScene;
        }
    }
}
