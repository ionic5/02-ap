using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

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

        private int _level;
        private int _exp;
        private int _hp;
        private string _unitBodyID;
        private string _unitLogicID;
        private bool _isPlayerSide;
        private Vector2 _position;
        private string _defaultSkillID;
        private readonly AttributeMediator _attributeMediator;

        private readonly Dictionary<string, IActiveSkill> _skills;

        public Unit(GameDataStore gameDataStore, AttributeMediator attributeMediator)
        {
            _gameDataStore = gameDataStore;

            _skills = new Dictionary<string, IActiveSkill>();
            _attributeMediator = attributeMediator;
        }

        public void SetAttributeGrowthFormulas(IEnumerable<GameData.GrowthFormula> formulas)
        {
            _attributeMediator.SetGrowthFormulas(formulas);
        }

        public void SetBaseAttributes(IReadOnlyDictionary<string, Attribute> baseAttributes)
        {
            _attributeMediator.SetBaseAttributes(baseAttributes);
        }

        public Attribute GetAttribute(string id)
        {
            return _attributeMediator.GetAttribute(id);
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

        public string GetUnitBodyID()
        {
            return _unitBodyID;
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

        public void SetUnitBodyID(string id)
        {
            _unitBodyID = id;
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
            _attributeMediator.SetLevel(_level);
        }

        public void SetHP(int hp)
        {
            _hp = hp;
        }

        public void AddSkill(IActiveSkill skill)
        {
            if (_skills.ContainsKey(skill.GetSkillID()))
                return;

            _skills.Add(skill.GetSkillID(), skill);
            SkillAddedEvent?.Invoke(this, new SkillAddedEventArgs { SkillID = skill.GetSkillID() });
        }

        public void AddModifyAttributeEffects(IEnumerable<IModifyAttributeEffect> effects)
        {
            _attributeMediator.AddModifyAttributeEffects(effects);
        }

        public void AddModifyAttributeEffect(IModifyAttributeEffect effect)
        {
            _attributeMediator.AddModifyAttributeEffect(effect);
        }

        /// <summary>
        /// Retrieves a skill currently owned by the unit.
        /// </summary>
        /// <param name="skillID">The unique identifier of the skill to find.</param>
        /// <returns>The <see cref="IActiveSkill"/> instance if found; otherwise, <c>null</c>.</returns>
        public IActiveSkill GetSkill(string skillID)
        {
            return _skills.TryGetValue(skillID, out var skill) ? skill : null;
        }

        public IReadOnlyCollection<IActiveSkill> GetSkills()
        {
            return _skills.Values;
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
            if (!_skills.Any(entry => entry.Value.GetSkillID() == skillID))
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

        internal void RemoveModifyAttributeEffects(List<IModifyAttributeEffect> effects)
        {
            throw new NotImplementedException();
        }
    }
}
