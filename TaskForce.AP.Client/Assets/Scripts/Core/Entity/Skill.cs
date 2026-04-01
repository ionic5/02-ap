using System;
using System.Collections.Generic;
using System.Linq;
using TaskForce.AP.Client.Core.GameData;

namespace TaskForce.AP.Client.Core.Entity
{
    public abstract class Skill : ISkill
    {
        public event EventHandler<LevelChangedEventArgs> LevelChangedEvent;
        private readonly GameData.Skill _skillData;
        private readonly TextStore _textStore;
        private Unit _owner;
        private int _level;
        private readonly AttributeStore _attributeStore;
        private readonly IEnumerable<GameData.BaseAttribute> _baseAttributes;
        private readonly IEnumerable<GameData.LevelAttribute> _levelAttributes;
        private readonly IEnumerable<GameData.SkillDescription> _skillDescriptions;

        public Skill(GameData.Skill skillData, TextStore textStore,
            IEnumerable<GameData.BaseAttribute> baseAttributes, IEnumerable<LevelAttribute> levelAttributes,
            IEnumerable<GameData.SkillDescription> skillDescriptions)
        {
            _skillData = skillData;
            _textStore = textStore;
            _attributeStore = new AttributeStore();
            _baseAttributes = baseAttributes;
            _levelAttributes = levelAttributes;
            _skillDescriptions = skillDescriptions;
        }

        public string GetSkillID()
        {
            return _skillData.ID;
        }

        public virtual void SetLevel(int value)
        {
            _level = value;

            UpdateAttributes();
            LevelChangedEvent?.Invoke(this, new LevelChangedEventArgs(_skillData.ID, _level));
        }

        private void UpdateAttributes()
        {
            _attributeStore.Clear();

            foreach (var entry in _baseAttributes)
                _attributeStore.Set(entry.AttributeID, entry.Value);
            SetLevelAttributes();
        }

        private void SetLevelAttributes()
        {
            var closestGroup = _levelAttributes.GroupBy(e => e.Level)
                .Where(g => g.Key <= _level)
                .OrderByDescending(g => g.Key).FirstOrDefault();
            if (closestGroup != null)
                foreach (var entry in closestGroup)
                    _attributeStore.Set(entry.AttributeID, entry.Value);
        }

        public virtual void OnAddedToUnit(Unit unit)
        {
            _owner = unit;
        }

        protected Unit GetOwner()
        {
            return _owner;
        }

        public virtual Variant GetAttribute(string attributeID)
        {
            return _attributeStore.Get(attributeID);
        }

        public string GetIconID()
        {
            return _skillData.IconID;
        }

        public string GetName()
        {
            return _textStore.GetText(_skillData.NameTextID);
        }

        public int GetLevel()
        {
            return _level;
        }

        public virtual void LevelUp()
        {
            _level++;
            SetLevel(_level);
        }

        public string GetDescription()
        {
            var entry = _skillDescriptions
                .Where(e => e.Level <= _level)
                .OrderByDescending(e => e.Level)
                .FirstOrDefault();
            if (entry == null)
                return string.Empty;
            return string.Format(_textStore.GetText(entry.TextID), entry.Parameters);
        }
    }
}
