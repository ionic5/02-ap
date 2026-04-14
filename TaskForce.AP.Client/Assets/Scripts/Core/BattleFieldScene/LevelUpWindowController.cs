using System;
using System.Collections.Generic;
using System.Linq;
using TaskForce.AP.Client.Core.Entity;
using TaskForce.AP.Client.Core.View.BattleFieldScene.LevelUpWindow;
using TaskForce.AP.Client.Core.View.BattleFieldScene.Windows;

namespace TaskForce.AP.Client.Core.BattleFieldScene
{
    public class LevelUpWindowController
    {
        private readonly ILevelUpWindow _window;
        private readonly TextStore _textStore;
        private readonly Entity.Unit _unit;
        private readonly int _level;
        private readonly IAdvertisementPlayer _advertisementPlayer;
        private readonly GameDataStore _gameDataStore;
        private readonly Random _random;
        private readonly Func<string, Entity.ISkill> _createSkillEntity;
        private readonly Action _onClosed;
        private readonly List<ISkillPanel> _panels;
        private List<ISkill> _skills;
        private int _index;

        public LevelUpWindowController(ILevelUpWindow window, Entity.Unit unit, int level, TextStore textStore,
            IAdvertisementPlayer advertisementPlayer, GameDataStore gameDataStore, Random random,
            Func<string, Entity.ISkill> createSkillEntity, Action onClosed = null)
        {
            _window = window;
            _unit = unit;
            _level = level;
            _textStore = textStore;
            _advertisementPlayer = advertisementPlayer;
            _gameDataStore = gameDataStore;
            _random = random;
            _createSkillEntity = createSkillEntity;
            _onClosed = onClosed;
            _panels = new List<ISkillPanel>();
        }

        public void Start()
        {
            _window.SetLevel(_level);

            RefreshSkillPanels();

            _window.OKButtonClickedEvent += OnOKButtonClickedEvent;
            _window.RerollButtonClickedEvent += OnRerollButtonClicked;
            _window.ClosedEvent += OnWindowClosed;
        }

        private void RefreshSkillPanels()
        {
            foreach (var panel in _panels)
                panel.ClickedEvent -= OnSkillPanelClicked;
            _panels.Clear();
            _window.ClearSkillPanels();

            _skills = new List<ISkill>();
            if (_unit.GetMaxSkillCount() > 0 && _unit.GetSkills().Count >= _unit.GetMaxSkillCount())
            {
                var ownedSkills = _unit.GetSkills().ToArray();
                _random.Shuffle(ownedSkills);
                foreach (var owned in ownedSkills.Take(3))
                {
                    var skill = _createSkillEntity.Invoke(owned.GetSkillID());
                    skill.SetLevel(owned.GetLevel() + 1);
                    _skills.Add(skill);
                }
            }
            else
            {
                var skillIDs = _gameDataStore.GetLevelUpRewardSkills().Select(entry => entry.SkillID).ToArray();
                _random.Shuffle(skillIDs);
                foreach (var skillID in skillIDs.Take(3))
                {
                    var skill = _createSkillEntity.Invoke(skillID);
                    var existing = _unit.GetSkill(skillID);
                    var level = existing != null ? existing.GetLevel() + 1 : 1;
                    skill.SetLevel(level);
                    _skills.Add(skill);
                }
            }

            foreach (var skill in _skills)
            {
                var panel = _window.AddSkillPanel();
                var icon = panel.GetIcon();
                icon.SetIcon(skill.GetIconID());
                icon.SetLevel(skill.GetLevel());

                panel.SetName(skill.GetName());
                panel.SetDescription(skill.GetDescription());
                panel.SetActiveNewMark(skill.GetLevel() == 1);
                panel.SetSelected(false);
                panel.ClickedEvent += OnSkillPanelClicked;
                _panels.Add(panel);
            }

            _index = 0;
            UpdateSelectedPanel();
        }

        private void OnSkillPanelClicked(object sender, SkillPanelClickedEventArgs e)
        {
            _index = _panels.IndexOf(e.Panel);

            UpdateSelectedPanel();
        }

        private void UpdateSelectedPanel()
        {
            for (var i = 0; i < _panels.Count; i++)
            {
                var panel = _panels.ElementAt(i);
                panel.SetSelected(i == _index);
            }
        }

        private void OnRerollButtonClicked(object sender, EventArgs args)
        {
            if (!_advertisementPlayer.CanPlayRewardedAdvertisement())
                return;

            _advertisementPlayer.PlayRewardedAdvertisement(() => { RefreshSkillPanels(); }, null);
        }

        private void OnWindowClosed(object sender, EventArgs args)
        {
            foreach (var panel in _panels)
                panel.ClickedEvent -= OnSkillPanelClicked;
            _panels.Clear();

            _window.OKButtonClickedEvent -= OnOKButtonClickedEvent;
            _window.RerollButtonClickedEvent -= OnRerollButtonClicked;
            _window.ClosedEvent -= OnWindowClosed;

            _onClosed?.Invoke();
        }

        private void OnOKButtonClickedEvent(object sender, EventArgs args)
        {
            var newSKill = _skills.ElementAt(_index);
            var skill = _unit.GetSkill(newSKill.GetSkillID());
            if (skill != null)
                skill.LevelUp();
            else
                _unit.AddSkill(newSKill);

            _window.Close();
        }
    }
}
