using System;
using System.Linq;
using TaskForce.AP.Client.Core.View.BattleFieldScene.SkillSelectionWindow;
using TaskForce.AP.Client.Core.View.BattleFieldScene.Windows;
using TaskForce.AP.Client.UnityWorld.View.BattleFieldScene.SkillSelectionWindow;
using UnityEngine;

namespace TaskForce.AP.Client.UnityWorld.View.BattleFieldScene.Windows
{
    public class SkillSelectionWindow : Window, ISkillSelectionWindow
    {
        public event EventHandler OKButtonClickedEvent;

        [SerializeField]
        public SkillPanel[] SkillPanels;

        private void Awake()
        {
            ResetSkillPanels();
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

        public int GetSelectedSkillIndex()
        {
            for (var i = 0; i < SkillPanels.Count(); i++)
            {
                if (SkillPanels[i].IsSelected())
                    return i;
            }
            return -1;
        }

        public override void Clear()
        {
            base.Clear();

            ResetSkillPanels();
        }

        private void ResetSkillPanels()
        {
            foreach (var panel in SkillPanels)
                panel.gameObject.SetActive(false);
            SkillPanels[0].SetSelected();
        }

        public void OnOKButtonClicked()
        {
            OKButtonClickedEvent?.Invoke(this, EventArgs.Empty);
        }
    }
}