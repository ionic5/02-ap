using TaskForce.AP.Client.Core.Entity;
using TaskForce.AP.Client.Core.View.BattleFieldScene;

namespace TaskForce.AP.Client.Core.BattleFieldScene
{
    public class SkillIconGridController
    {
        private readonly ISkillIconGrid _grid;
        private readonly Entity.Unit _unit;

        public SkillIconGridController(ISkillIconGrid grid, Entity.Unit unit)
        {
            _grid = grid;
            _unit = unit;
        }

        public void Start()
        {
            foreach (var skill in _unit.GetSkills())
                AddIcon(skill);

            _unit.SkillAddedEvent += OnSkillAdded;
            _grid.DestroyedEvent += OnGridDestroyed;
        }

        private void AddIcon(ISkill skill)
        {
            var icon = _grid.AddIcon();
            icon.SetIcon(skill.GetIconID());
            icon.SetLevel(skill.GetLevel());

            skill.LevelChangedEvent += OnLevelChanged;
        }

        private void OnGridDestroyed(object sender, System.EventArgs e)
        {
            _grid.DestroyedEvent -= OnGridDestroyed;
            _unit.SkillAddedEvent -= OnSkillAdded;

            foreach (var skill in _unit.GetSkills())
                skill.LevelChangedEvent -= OnLevelChanged;
        }

        private void OnSkillAdded(object sender, SkillAddedEventArgs args)
        {
            var skill = _unit.GetSkill(args.SkillID);
            if (skill == null)
                return;

            AddIcon(skill);
        }

        private void OnLevelChanged(object sender, LevelChangedEventArgs args)
        {
            var skills = _unit.GetSkills();
            for (var i = 0; i < skills.Count; i++)
            {
                if (skills[i].GetSkillID() != args.SkillID)
                    continue;

                if (_grid.IsIconExist(i))
                    _grid.GetIcon(i).SetLevel(args.Level);
                return;
            }
        }
    }
}
