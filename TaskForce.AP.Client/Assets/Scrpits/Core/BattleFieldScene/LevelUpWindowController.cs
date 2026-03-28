using System;
using System.Collections.Generic;
using System.Linq;
using TaskForce.AP.Client.Core.Entity;
using TaskForce.AP.Client.Core.View.BattleFieldScene.Windows;

namespace TaskForce.AP.Client.Core.BattleFieldScene
{
    public class LevelUpWindowController
    {
        private readonly ILevelUpWindow _window;
        private readonly IEnumerable<ISkill> _skills;
        private readonly TextStore _textStore;
        private readonly Entity.Unit _unit;

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
            foreach (var skill in _skills)
            {
                var panel = _window.AddSkillPanel();
                panel.SetName(skill.GetName());
                //panel.SetDescription(_textStore.GetText(skill.GetDescTextID()));
                var form = _textStore.GetText(TextID.LevelFormat0);
                panel.SetLevel(string.Format(form, skill.GetLevel()));
                //panel.SetIcon(skill.GetIconID());
            }

            _window.OKButtonClickedEvent += OnOKButtonClickedEvent;
        }

        public void OnOKButtonClickedEvent(object sender, EventArgs args)
        {
            var index = _window.GetSelectedSkillIndex();


            var newSKill = _skills.ElementAt(index);
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

            _window.OKButtonClickedEvent -= OnOKButtonClickedEvent;
            _window.Close();
        }
    }
}
