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
        private GameObject _background;
        [SerializeField]
        private GameObject _selectedMark;
        [SerializeField]
        private TMP_Text _descriptionText;
        [SerializeField]
        private TMP_Text _nameText;
        [SerializeField]
        private SkillIcon _skillIcon;
        [SerializeField]
        private GameObject _newMark;

        public SkillIcon SkillIcon => _skillIcon;

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

        public void SetActiveNewMark(bool active)
        {
            _newMark.SetActive(active);
        }

        public void SetSelected(bool isSelected)
        {
            _background.SetActive(!isSelected);
            _selectedMark.SetActive(isSelected);
        }
    }
}
