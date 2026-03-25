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

        private readonly string[] labels = {"Level", 
                                            "Kill Count", 
                                            "Survival Time"};

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
            SetSurvivalTimeText("");
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

        public void SetSurvivalTimeText(string time)
        {
            if (_survivalTimeText != null)
                _survivalTimeText.text = UpdateUI(LabelType.SurvivalTime, time);
        }

       public string UpdateUI(LabelType type, string value)
        {
            // Enum을 int로 형변환해서 인덱스로 사용
            string label = labels[(int)type]; 

            return $"{label}: {value}";
        }
    }
}