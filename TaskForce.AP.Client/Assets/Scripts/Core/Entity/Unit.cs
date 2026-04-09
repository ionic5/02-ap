using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using TaskForce.AP.Client.Core;
using TaskForce.AP.Client.Core.GameData;

namespace TaskForce.AP.Client.Core.Entity
{
    public class Unit
    {
        public event EventHandler ExpChangedEvent;
        public event EventHandler RequireExpChangedEvent;
        public event EventHandler LevelUpEvent;
        public event EventHandler DiedEvent;
        public event EventHandler<SkillAddedEventArgs> SkillAddedEvent;

        private readonly GameDataStore _gameDataStore;
        private readonly GameData.Unit _unitData;

        private int _level;
        private int _exp;
        private int _hp;
        private string _unitLogicID;
        private bool _isPlayerSide;
        private Vector2 _position;
        private string _defaultSkillID;

        private readonly AttributeStore _attributeStore;
        private readonly List<ISkill> _skills;
        private readonly IEnumerable<GameData.LevelAttribute> _levelAttributes;
        private readonly List<IModifyAttributeEffect> _modifyAttributeEffects;

        private int _maxSkillCount;
        private int _skillCountLimit;

        public Unit(GameData.Unit unitData, GameDataStore gameDataStore, IEnumerable<LevelAttribute> levelAttributes)
        {
            _unitData = unitData;
            _gameDataStore = gameDataStore;

            _skills = new List<ISkill>();

            _levelAttributes = levelAttributes;
            _attributeStore = new AttributeStore();
            _modifyAttributeEffects = new List<IModifyAttributeEffect>();
        }

        public Variant GetAttribute(string id)
        {
            return _attributeStore.Get(id);
        }

        public int GetHP()
        {
            return _hp;
        }

        public void ApplyDamage(int damage)
        {
            var hp = GetHP();
            hp -= damage;
            if (hp < 0)
                hp = 0;

            SetHP(hp);

            if (hp == 0)
                DiedEvent?.Invoke(this, EventArgs.Empty);
        }

        public bool IsAlive()
        {
            return GetHP() > 0;
        }

        public bool IsDead()
        {
            return !IsAlive();
        }

        public bool IsPlayerSide()
        {
            return _isPlayerSide;
        }

        public string GetUnitID()
        {
            return _unitData.ID;
        }

        public string GetUnitBodyID()
        {
            return _unitData.BodyID;
        }

        public string GetUnitLogicID()
        {
            return _unitLogicID;
        }

        public void AddExp(int exp)
        {
            _exp += exp;

            var requireExp = GetRequireExp();
            while (_exp >= requireExp)
            {
                _exp -= requireExp;

                SetLevel(_level + 1);
                LevelUpEvent?.Invoke(this, EventArgs.Empty);
                requireExp = _gameDataStore.GetRequireExp(_level);
                RequireExpChangedEvent?.Invoke(this, EventArgs.Empty);
            }

            ExpChangedEvent?.Invoke(this, EventArgs.Empty);
        }

        public int GetExp()
        {
            return _exp;
        }

        public int GetRequireExp()
        {
            return _gameDataStore.GetRequireExp(_level);
        }

        public int GetLevel()
        {
            return _level;
        }

        public void SetUnitLogicID(string id)
        {
            _unitLogicID = id;
        }

        public void SetPlayerSide(bool isPlayerSide = true)
        {
            _isPlayerSide = isPlayerSide;
        }

        public void SetLevel(int level)
        {
            _level = level;

            UpdateAttributes();
        }

        public void SetHP(int hp)
        {
            _hp = hp;
        }

        public void AddSkill(ISkill skill)
        {
            if (_skills.Any(s => s.GetSkillID() == skill.GetSkillID()))
                return;

            if (_maxSkillCount > 0 && _skills.Count >= _maxSkillCount)
                return;

            _skills.Add(skill);
            skill.OnAddedToUnit(this);
            SkillAddedEvent?.Invoke(this, new SkillAddedEventArgs { SkillID = skill.GetSkillID() });
        }

        public void SetMaxSkillCount(int count)
        {
            _maxSkillCount = count;
        }

        public int GetMaxSkillCount()
        {
            return _maxSkillCount;
        }

        public void SetSkillCountLimit(int limit)
        {
            _skillCountLimit = limit;
        }

        public int GetSkillCountLimit()
        {
            return _skillCountLimit;
        }

        public void AddModifyAttributeEffects(IEnumerable<IModifyAttributeEffect> effects)
        {
            _modifyAttributeEffects.AddRange(effects);

            UpdateAttributes();
        }

        public void AddModifyAttributeEffect(IModifyAttributeEffect effect)
        {
            _modifyAttributeEffects.Add(effect);

            UpdateAttributes();
        }

        /// <summary>
        /// Retrieves a skill currently owned by the unit.
        /// </summary>
        /// <param name="skillID">The unique identifier of the skill to find.</param>
        /// <returns>The <see cref="ISkill"/> instance if found; otherwise, <c>null</c>.</returns>
        public ISkill GetSkill(string skillID)
        {
            return _skills.FirstOrDefault(s => s.GetSkillID() == skillID);
        }

        public IReadOnlyList<ISkill> GetSkills()
        {
            return _skills;
        }

        public void SetPosition(Vector2 position)
        {
            _position = position;
        }

        public Vector2 GetPosition()
        {
            return _position;
        }

        public bool IsFullHP()
        {
            return _hp == GetAttribute(AttributeID.MaxHP).AsInt();
        }

        public void ApplyHeal(int healAmount)
        {
            _hp = Math.Min(_hp + healAmount, GetAttribute(AttributeID.MaxHP).AsInt());
        }

        public void SetDefaultSkill(string skillID)
        {
            if (!_skills.Any(s => s.GetSkillID() == skillID))
                return;

            _defaultSkillID = skillID;
        }

        public string GetDefaultSkillID()
        {
            return _defaultSkillID;
        }

        public void SetDead()
        {
            SetHP(0);
        }

        public void RemoveModifyAttributeEffects(IEnumerable<IModifyAttributeEffect> effects)
        {
            var effectsToRemove = new HashSet<IModifyAttributeEffect>(effects);
            _modifyAttributeEffects.RemoveAll(e => effectsToRemove.Contains(e));

            UpdateAttributes();
        }

        private List<IModifyAttributeEffect> GetMergedEffects()
        {
            var mergedEffects = new List<IModifyAttributeEffect>();
            foreach (var entry in _modifyAttributeEffects)
            {
                var existing = mergedEffects.FirstOrDefault(m => m.CanMerge(entry));
                if (existing == null)
                    mergedEffects.Add(entry.Clone());
                else
                    existing.Merge(entry);
            }
            mergedEffects.Sort((a, b) => a.GetApplyOrder().CompareTo(b.GetApplyOrder()));
            return mergedEffects;
        }

        public Variant ApplyGlobalModifier(string attributeID, Variant baseValue)
        {
            var tempStore = new AttributeStore();
            tempStore.Set(attributeID, baseValue);

            var mergedEffects = GetMergedEffects();
            foreach (var entry in mergedEffects)
                entry.Apply(tempStore);

            return tempStore.Get(attributeID);
        }

        public void UpdateAttributes()
        {
            _attributeStore.Clear();

            SetLevelAttributes();

            var mergedEffects = GetMergedEffects();
            foreach (var entry in mergedEffects)
                entry.Apply(_attributeStore);
        }

        private void SetLevelAttributes()
        {
            var byAttribute = _levelAttributes
                .Where(e => e.Level <= _level)
                .GroupBy(e => e.AttributeID);

            foreach (var group in byAttribute)
            {
                var closest = group.OrderByDescending(e => e.Level).First();
                _attributeStore.Set(closest.AttributeID, closest.Value);
            }
        }
    }
}
