using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using TaskForce.AP.Client.Core.GameData;

namespace TaskForce.AP.Client.Core
{
    public class GameDataStore
    {
        private readonly List<SkillBaseAttribute> _skillBaseAttributes;
        private readonly List<StageEnemyUnit> _stageEnemyUnits;
        private readonly List<Stage> _stages;
        private readonly List<Unit> _units;
        private readonly List<NonPlayerUnitLogic> _nonPlayerUnitLogics;
        private readonly List<ModifyAttributeEffect> _modifyAttributeEffects;
        private readonly List<UnitDefaultSkill> _unitDefaultSkills;
        private readonly List<Skill> _skills;
        private readonly List<LevelUpRewardSkill> _levelUpRewardSkills;
        private readonly List<ModifyAttributeSkill> _modifyAttributeSkills;
        private readonly List<UnitDefaultActiveSkill> _unitDefaultActiveSkill;
        private readonly List<GameData.LevelAttribute> _levelAttributes;
        private readonly List<GameData.BaseAttribute> _baseAttributes;
        private readonly List<SkillLevelAttribute> _skillLevelAttributes;
        private readonly List<LevelCoefficient> _levelCoefficients;
        private readonly List<RequireExp> _requireExps;
        private readonly List<SoulExp> _soulExps;
        private readonly List<SkillDescription> _skillDescriptions;
        private float _soulDropRate;
        private int _maxEnergy;

        private Dictionary<string, Skill> _skillsByID;
        private Dictionary<string, IEnumerable<ModifyAttributeSkill>> _modifyAttributeSkillsBySkillID;

        public GameDataStore()
        {
            _skillBaseAttributes = new List<SkillBaseAttribute>();
            _modifyAttributeEffects = new List<ModifyAttributeEffect>();
            _stageEnemyUnits = new List<StageEnemyUnit>();
            _stages = new List<Stage>();
            _units = new List<Unit>();
            _nonPlayerUnitLogics = new List<NonPlayerUnitLogic>();
            _unitDefaultSkills = new List<UnitDefaultSkill>();
            _skills = new List<Skill>();
            _levelUpRewardSkills = new List<LevelUpRewardSkill>();
            _levelAttributes = new List<GameData.LevelAttribute>();
            _modifyAttributeSkills = new List<ModifyAttributeSkill>();
            _modifyAttributeSkillsBySkillID = new Dictionary<string, IEnumerable<ModifyAttributeSkill>>();
            _unitDefaultActiveSkill = new List<UnitDefaultActiveSkill>();
            _baseAttributes = new List<GameData.BaseAttribute>();
            _skillLevelAttributes = new List<SkillLevelAttribute>();
            _levelCoefficients = new List<LevelCoefficient>();
            _requireExps = new List<RequireExp>();
            _soulExps = new List<SoulExp>();
            _skillDescriptions = new List<SkillDescription>();
        }

        public void Bake()
        {
            _skillsByID = _skills.ToDictionary(entry => entry.ID);
            _modifyAttributeSkillsBySkillID = _modifyAttributeSkills.GroupBy(entry => entry.SkillID).ToDictionary(
                group => group.Key,
                group => group.AsEnumerable());
        }

        public void AddRequireExp(GameData.RequireExp entry)
        {
            _requireExps.Add(entry);
        }

        public void AddSoulExp(GameData.SoulExp entry)
        {
            _soulExps.Add(entry);
        }

        public void AddLevelAttribute(GameData.LevelAttribute entry)
        {
            _levelAttributes.Add(entry);
        }

        public void AddBaseAttribute(GameData.BaseAttribute entry)
        {
            _baseAttributes.Add(entry);
        }

        public void AddLevelUpRewardSkill(LevelUpRewardSkill entry)
        {
            _levelUpRewardSkills.Add(entry);
        }

        public void AddSkill(Skill entry)
        {
            _skills.Add(entry);
        }

        public void AddSkillLevelAtrribute(SkillLevelAttribute entry)
        {
            _skillLevelAttributes.Add(entry);
        }

        public void AddUnitDefaultSkill(UnitDefaultSkill entry)
        {
            _unitDefaultSkills.Add(entry);
        }

        public void AddNonPlayerUnitLogic(NonPlayerUnitLogic entry)
        {
            _nonPlayerUnitLogics.Add(entry);
        }

        public void AddUnit(Unit entry)
        {
            _units.Add(entry);
        }

        public void AddStage(Stage entry)
        {
            _stages.Add(entry);
        }

        public void AddStageEnemyUnit(StageEnemyUnit entry)
        {
            _stageEnemyUnits.Add(entry);
        }

        public void AddLevelCoefficient(LevelCoefficient entry)
        {
            _levelCoefficients.Add(entry);
        }

        public void AddSkillBaseAttribute(SkillBaseAttribute entry)
        {
            _skillBaseAttributes.Add(entry);
        }

        public void AddModifyAttributeEffect(ModifyAttributeEffect entry)
        {
            _modifyAttributeEffects.Add(entry);
        }

        public void SetSoulDropRate(float value)
        {
            _soulDropRate = value;
        }

        public float GetSoulDropRate()
        {
            return _soulDropRate;
        }
        
        public void SetMaxEnergy(int value)
        {
            _maxEnergy = value;
        }

        public int GetMaxEnergy()
        {
            return _maxEnergy;
        }

        public Skill GetSkillById(string id)
        {
            return _skillsByID.GetValueOrDefault(id);
        }

        public int GetSoulExp(int level)
        {
            var exp = _soulExps.Where(entry => entry.Level <= level).OrderByDescending(Entry => Entry.Level).FirstOrDefault();
            return exp.Exp;
        }

        public int GetRequireExp(int level)
        {
            var exp = _requireExps.Where(entry => entry.Level <= level).OrderByDescending(Entry => Entry.Level).FirstOrDefault();
            return exp.Exp;
        }

        public IEnumerable<ModifyAttributeSkill> GetModifyAttributeSkillEffects(string skillID)
        {
            return _modifyAttributeSkillsBySkillID.GetValueOrDefault(skillID);
        }

        public IEnumerable<BaseAttribute> GetSkillBaseAttributes(string skillID)
        {
            var attr = _skillBaseAttributes.FirstOrDefault(entry => entry.SkillID == skillID);
            if (attr == null)
                return Enumerable.Empty<BaseAttribute>();

            return _baseAttributes.Where(entry => entry.ID == attr.BaseAttributeID);
        }

        public IEnumerable<LevelAttribute> GetSkillLevelAttributes(string skillID)
        {
            var attr = _skillLevelAttributes.FirstOrDefault(entry => entry.SkillID == skillID);
            if (attr == null)
                return Enumerable.Empty<LevelAttribute>();

            return _levelAttributes.Where(entry => entry.ID == attr.LevelAttributeID);
        }

        public IEnumerable<ModifyAttributeEffect> GetModifyAttributeEffects()
        {
            return _modifyAttributeEffects;
        }

        public IEnumerable<Stage> GetStages()
        {
            return _stages;
        }

        public IEnumerable<StageEnemyUnit> GetStageEnemyUnits()
        {
            return _stageEnemyUnits;
        }

        public IEnumerable<Unit> GetUnits()
        {
            return _units;
        }

        public IEnumerable<NonPlayerUnitLogic> GetNonPlayerUnitLogics()
        {
            return _nonPlayerUnitLogics;
        }

        public IEnumerable<Skill> GetSkills()
        {
            return _skills;
        }

        public IEnumerable<UnitDefaultSkill> GetUnitDefaultSkills()
        {
            return _unitDefaultSkills;
        }

        public IEnumerable<LevelUpRewardSkill> GetLevelUpRewardSkills()
        {
            return _levelUpRewardSkills;
        }

        public void AddModifyAttributeSkill(ModifyAttributeSkill entry)
        {
            _modifyAttributeSkills.Add(entry);
        }

        public void AddUnitDefaultActiveSkill(UnitDefaultActiveSkill entry)
        {
            _unitDefaultActiveSkill.Add(entry);
        }

        public UnitDefaultActiveSkill GetUnitDefaultActiveSkillByUnitID(string unitID)
        {
            return _unitDefaultActiveSkill.FirstOrDefault(entry => entry.UnitID == unitID);
        }

        public IEnumerable<GameData.BaseAttribute> GetBaseAttributes(string baseAttributeID)
        {
            return _baseAttributes.Where(entry => entry.ID == baseAttributeID);
        }

        public IEnumerable<LevelAttribute> GetLevelAttributes(string levelAttributeID)
        {
            return _levelAttributes.Where(entry => entry.ID == levelAttributeID);
        }

        public IEnumerable<LevelCoefficient> GetLevelCoefficients(string lvCoeffID)
        {
            return _levelCoefficients.Where(entry => entry.ID == lvCoeffID);
        }

        public void AddSkillDescription(SkillDescription description)
        {
            _skillDescriptions.Add(description);
        }

        public IEnumerable<SkillDescription> GetSkillDescriptions(string skillID)
        {
            return _skillDescriptions.Where(entry => entry.SkillID == skillID);
        }
    }
}
