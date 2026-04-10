using System;
using TaskForce.AP.Client.Core.View.Scenes;
using UnityEngine;

namespace TaskForce.AP.Client.UnityWorld.View.Scenes
{
    public class TitleScene : Scene, ITitleScene
    {
        [SerializeField]
        private GameObject _startButton;
        [SerializeField]
        private SoundPlayer soundPlayer;
        
        public SoundPlayer SoundPlayer => soundPlayer;
        
        public event EventHandler StartButtonClickedEvent;

        private void Awake()
        {
            _startButton.SetActive(false);
        }

        public void OnStartButtonClicked()
        {
            StartButtonClickedEvent?.Invoke(this, EventArgs.Empty);
        }

        public void SetStartButtonVisible(bool visible)
        {
            _startButton.SetActive(visible);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            StartButtonClickedEvent = null;
        }
    }
}
