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
        private readonly IAdvertisementPlayer _advertisementPlayer;
        private readonly GameDataStore _gameDataStore;
        private readonly Random _random;
        private readonly Func<Entity.Unit, string, int, Entity.ISkill> _createSkillEntity;
        private readonly List<ISkillPanel> _panels = new List<ISkillPanel>();
        private List<ISkill> _skills;
        private int _index;

        public LevelUpWindowController(ILevelUpWindow window, Entity.Unit unit, TextStore textStore,
            IAdvertisementPlayer advertisementPlayer, GameDataStore gameDataStore, Random random,
            Func<Entity.Unit, string, int, Entity.ISkill> createSkillEntity)
        {
            _window = window;
            _unit = unit;
            _textStore = textStore;
            _advertisementPlayer = advertisementPlayer;
            _gameDataStore = gameDataStore;
            _random = random;
            _createSkillEntity = createSkillEntity;
        }

        public void Start()
        {
            _window.SetLevel(_unit.GetLevel());

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

            var skillIDs = _gameDataStore.GetLevelUpRewardSkills().Select(entry => entry.SkillID).ToArray();
            _random.Shuffle(skillIDs);
            _skills = new List<ISkill>();
            foreach (var skillID in skillIDs.Take(3))
            {
                var existing = _unit.GetSkill(skillID);
                var level = existing != null ? existing.GetLevel() + 1 : 1;
                _skills.Add(_createSkillEntity.Invoke(_unit, skillID, level));
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
                panel.ClickedEvent += OnSkillPanelClicked;
                _panels.Add(panel);
            }

            _index = 0;
        }

        private void OnSkillPanelClicked(object sender, SkillPanelClickedEventArgs e)
        {
            _index = _panels.IndexOf(e.Panel);
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
        }

        private void OnOKButtonClickedEvent(object sender, EventArgs args)
        {
            var newSKill = _skills.ElementAt(_index);
            var skill = _unit.GetSkill(newSKill.GetSkillID());
            if (skill != null)
            {
                skill.LevelUp();
            }
            else
            {
                newSKill.SetOwner(_unit);
                newSKill.AddToOwner();
            }

            _window.Close();
        }
    }
}
