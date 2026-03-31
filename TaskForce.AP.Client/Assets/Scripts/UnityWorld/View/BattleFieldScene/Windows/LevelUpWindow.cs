using System;
using System.Linq;
using TaskForce.AP.Client.Core.View.BattleFieldScene.LevelUpWindow;
using TaskForce.AP.Client.Core.View.BattleFieldScene.Windows;
using TaskForce.AP.Client.UnityWorld.View.BattleFieldScene.LevelUpWindow;
using TMPro;
using UnityEngine;

namespace TaskForce.AP.Client.UnityWorld.View.BattleFieldScene.Windows
{
    public class LevelUpWindow : Window, ILevelUpWindow
    {
        public event EventHandler OKButtonClickedEvent;
        public event EventHandler RerollButtonClickedEvent;

        [SerializeField]
        public SkillPanel[] SkillPanels;

        [SerializeField]
        private TMP_Text _levelText;

        private void Awake()
        {
            ClearSkillPanels();
        }

        public ISkillPanel AddSkillPanel()
        {
            for (var i = 0; i < SkillPanels.Count(); i++)
            {
                var panel = SkillPanels[i];
                if (panel.gameObject.activeSelf)
                    continue;

                panel.gameObject.SetActive(true);

                return panel;
            }

            return null;
        }

        public override void Clear()
        {
            base.Clear();

            ClearSkillPanels();

            OKButtonClickedEvent = null;
            RerollButtonClickedEvent = null;
        }

        public void ClearSkillPanels()
        {
            foreach (var panel in SkillPanels)
                panel.gameObject.SetActive(false);
        }

        public void OnOKButtonClicked()
        {
            OKButtonClickedEvent?.Invoke(this, EventArgs.Empty);
        }

        public void OnRerollButtonClicked()
        {
            RerollButtonClickedEvent?.Invoke(this, EventArgs.Empty);
        }

        public void SetLevel(int level)
        {
            _levelText.text = level.ToString();
        }
    }
}