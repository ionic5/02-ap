using System;
using TaskForce.AP.Client.Core.View.BattleFieldScene;
using TaskForce.AP.Client.Core.View.BattleFieldScene.LevelUpWindow;
using TMPro;
using UnityEngine;

namespace TaskForce.AP.Client.UnityWorld.View.BattleFieldScene.LevelUpWindow
{
    public class SkillPanel : MonoBehaviour, ISkillPanel
    {
        [SerializeField]
        private TMP_Text _descriptionText;
        [SerializeField]
        private TMP_Text _nameText;
        [SerializeField]
        private SkillIcon _skillIcon;

        public event EventHandler<SkillPanelClickedEventArgs> ClickedEvent;

        public void OnClicked()
        {
            ClickedEvent?.Invoke(this, new SkillPanelClickedEventArgs(this));
        }

        private void OnDestroy()
        {
            ClickedEvent = null;
        }

        public ISkillIcon GetIcon()
        {
            return _skillIcon;
        }

        public void SetDescription(string value)
        {
            _descriptionText.text = value;
        }

        public void SetName(string value)
        {
            _nameText.text = value;
        }
    }
}
