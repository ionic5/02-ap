using System;
using TaskForce.AP.Client.Core.View.Scenes;
using TaskForce.AP.Client.UnityWorld.BattleFieldScene;
using TaskForce.AP.Client.UnityWorld.View.BattleFieldScene;
using TMPro;
using UnityEngine;

namespace TaskForce.AP.Client.UnityWorld.View.Scenes
{
    public class BattleFieldScene : Scene, IBattleFieldScene
    {
        [SerializeField]
        private Loop _loop;
        [SerializeField]
        private global::Joystick _joystick;
        [SerializeField]
        private GameObject _playerUnitSpawnPosition;
        [SerializeField]
        public ObjectFactory ObjectFactory;
        [SerializeField]
        private FollowCamera _followCamera;
        [SerializeField]
        private World _world;
        [SerializeField]
        private GaugeBar _expBar;
        [SerializeField]
        private TMP_Text _levelText;
        [SerializeField]
        private View.BattleFieldScene.WindowStack _windowStack;
        [SerializeField]
        private PausePanel _pausePanel;
        [SerializeField]
        private TMP_Text _killCountText;
        [SerializeField]
        private TMP_Text _battleTimeText;

        public event EventHandler PauseButtonClickedEvent;

        public Loop Loop => _loop;
        public World World => _world;
        public Joystick Joystick => _joystick;
        public GameObject PlayerUnitSpawnPosition => _playerUnitSpawnPosition;
        public FollowCamera FollowCamera => _followCamera;
        public GaugeBar ExpBar => _expBar;
        public TMP_Text LevelText => _levelText;
        public View.BattleFieldScene.WindowStack WindowStack => _windowStack;
        public PausePanel PausePanel => _pausePanel;

        public void SetExp(int v)
        {
            _expBar.SetValue(v);
        }

        public void SetLevel(string v)
        {
            _levelText.text = v;
        }

        public void SetRequireExp(int v)
        {
            _expBar.SetMaxValue(v);
        }

        public void SetKillCount(int killCount)
        {
            var text = killCount.ToString();
            if (_killCountText.text == text)
                return;
            _killCountText.text = text;
        }

        public void SetBattleTime(float battleTime)
        {
            int totalSeconds = (int)battleTime;
            int minutes = totalSeconds / 60;
            int seconds = totalSeconds % 60;
            var text = $"{minutes:D2}:{seconds:D2}";
            if (_battleTimeText.text == text)
                return;
            _battleTimeText.text = text;
        }

        public void OnPauseButtonClicked()
        {
            PauseButtonClickedEvent?.Invoke(this, EventArgs.Empty);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            PauseButtonClickedEvent = null;
        }
    }
}
