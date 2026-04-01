using System;
using TaskForce.AP.Client.Core.View.BattleFieldScene.Windows;
using TMPro;
using UnityEngine;

namespace TaskForce.AP.Client.UnityWorld.View.BattleFieldScene.Windows
{
    public class DeathWindow : Window, IDeathWindow
    {
        public enum LabelType
        {
            Level,
            Kills,
            SurvivalTime
        };

        [SerializeField]
        private TMPro.TMP_Text _levelText;
        [SerializeField]
        private TMPro.TMP_Text _killsText;
        [SerializeField]
        private TMPro.TMP_Text _survivalTimeText;
        
        public event EventHandler RestartClickedEvent;
        public event EventHandler ReviveClickedEvent;

        private void Awake()
        {
            SetLevelText(0);
            SetKillsText(0);
            SetSurvivalTimeText(0f);
        }

        public void OnClickRestart()
        {
            RestartClickedEvent?.Invoke(this, EventArgs.Empty);
        }

        public void OnClickRevive()
        {
            ReviveClickedEvent?.Invoke(this, EventArgs.Empty);
        }

        public void SetLevelText(int level)
        {
            if (_levelText != null)
                _levelText.text = UpdateUI(LabelType.Level, level.ToString());
        }

        public void SetKillsText(int kills)
        {
            if (_killsText != null)
                _killsText.text = UpdateUI(LabelType.Kills, kills.ToString());
        }

        public void SetSurvivalTimeText(float time)
        {
            if (_survivalTimeText != null)
                _survivalTimeText.text = UpdateTime(LabelType.SurvivalTime, time);
        }

       public string UpdateUI(LabelType type, string value)
        {
            return $"{value}";
        }

        public string UpdateTime(LabelType type, float battleTime)
        {
            int totalSeconds = (int)battleTime;
            return $"{totalSeconds / 60:D2}:{totalSeconds % 60:D2}";
        }
    }
}