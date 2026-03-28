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
        private readonly IEnumerable<ISkill> _skills;
        private readonly TextStore _textStore;
        private readonly Entity.Unit _unit;
        private readonly List<ISkillPanel> _panels = new List<ISkillPanel>();
        private int _index;

        public LevelUpWindowController(ILevelUpWindow window, IEnumerable<Entity.ISkill> skills,
            Entity.Unit unit, TextStore textStore)
        {
            _window = window;
            _skills = skills;
            _unit = unit;
            _textStore = textStore;
        }

        public void Start()
        {
            _window.SetLevel(_unit.GetLevel());

            foreach (var skill in _skills)
            {
                var panel = _window.AddSkillPanel();
                var icon = panel.GetIcon();
                icon.SetIcon(skill.GetIconID());
                icon.SetLevel(skill.GetLevel());

                panel.SetName(skill.GetName());
                //panel.SetDescription(_textStore.GetText(skill.GetDescTextID()));
                panel.ClickedEvent += OnSkillPanelClicked;
                _panels.Add(panel);
            }

            _window.OKButtonClickedEvent += OnOKButtonClickedEvent;
            _window.ClosedEvent += OnWindowClosed;
        }

        private void OnSkillPanelClicked(object sender, SkillPanelClickedEventArgs e)
        {
            _index = _panels.IndexOf(e.Panel);
        }

        private void OnWindowClosed(object sender, EventArgs args)
        {
            foreach (var panel in _panels)
                panel.ClickedEvent -= OnSkillPanelClicked;
            _panels.Clear();

            _window.OKButtonClickedEvent -= OnOKButtonClickedEvent;
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
