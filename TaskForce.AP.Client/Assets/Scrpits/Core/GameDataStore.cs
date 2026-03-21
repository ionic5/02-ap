using System;
using System.Collections.Generic;
using System.Linq;
using TaskForce.AP.Client.Core.GameData;

namespace TaskForce.AP.Client.Core
{
    public class GameDataStore
    {
        private readonly List<SkillBaseAttribute> _skillBaseAttributes;
        private readonly List<Coefficient> _coefficients;
        private readonly List<StageEnemyUnit> _stageEnemyUnits;
        private readonly List<Stage> _stages;
        private readonly List<Unit> _units;
        private readonly List<NonPlayerUnitLogic> _nonPlayerUnitLogics;
        private readonly List<ModifyAttributeEffect> _modifyAttributeEffects;
        private readonly List<GrowthFormula> _growthFormulas;
        private readonly List<Formula> _formulas;
        private readonly List<SkillGrowthFormula> _skillGrowthFormulas;
        private readonly List<UnitDefaultSkill> _unitDefaultSkills;
        private readonly List<Skill> _skills;
        private readonly List<LevelUpRewardSkill> _levelUpRewardSkills;
        private readonly List<AttributeSet> _attributeSets;
        private readonly List<ModifyAttributeSkill> _modifyAttributeSkills;
        private readonly List<CoefficientFormulaSet> _coefficientFormulaSets;
        private readonly List<UnitDefaultActiveSkill> _unitDefaultActiveSkill;
        private readonly List<GameData.LevelAttribute> _levelAttributes;
        private readonly List<GameData.Attribute> _attributes;
        private readonly List<UnitAttribute> _unitAttributes;
        private float _soulDropRate;

        private Dictionary<string, Formula> _formulasByID;
        private ILookup<string, GrowthFormula> _growthFormulasByID;
        private Dictionary<string, Skill> _skillsByID;
        private Dictionary<string, Dictionary<string, float>> _coefficientsByFormulaID;
        private Dictionary<string, IEnumerable<ModifyAttributeSkill>> _modifyAttributeSkillsBySkillID;
        private Dictionary<string, Dictionary<string, Formula>> _coefficientFormulasBySetID;

        public GameDataStore()
        {
            _skillBaseAttributes = new List<SkillBaseAttribute>();
            _coefficients = new List<Coefficient>();
            _modifyAttributeEffects = new List<ModifyAttributeEffect>();
            _stageEnemyUnits = new List<StageEnemyUnit>();
            _stages = new List<Stage>();
            _units = new List<Unit>();
            _nonPlayerUnitLogics = new List<NonPlayerUnitLogic>();
            _growthFormulas = new List<GrowthFormula>();
            _formulas = new List<Formula>();
            _unitDefaultSkills = new List<UnitDefaultSkill>();
            _skills = new List<Skill>();
            _skillGrowthFormulas = new List<SkillGrowthFormula>();
            _levelUpRewardSkills = new List<LevelUpRewardSkill>();
            _coefficientFormulaSets = new List<CoefficientFormulaSet>();
            _attributeSets = new List<AttributeSet>();
            _levelAttributes = new List<GameData.LevelAttribute>();
            _modifyAttributeSkills = new List<ModifyAttributeSkill>();
            _modifyAttributeSkillsBySkillID = new Dictionary<string, IEnumerable<ModifyAttributeSkill>>();
            _coefficientFormulasBySetID = new Dictionary<string, Dictionary<string, Formula>>();
            _unitDefaultActiveSkill = new List<UnitDefaultActiveSkill>();
            _unitAttributes = new List<UnitAttribute>();
            _attributes = new List<GameData.Attribute>();
        }

        public void Bake()
        {
            _formulasByID = _formulas.ToDictionary(entry => entry.ID);
            _skillsByID = _skills.ToDictionary(entry => entry.ID);
            _coefficientsByFormulaID = _coefficients.GroupBy(entry => entry.FormulaID).ToDictionary(
                group => group.Key,
                group => group.ToDictionary(entry => entry.Key, entry => entry.Value));
            _growthFormulasByID = _growthFormulas.ToLookup(entry => entry.ID);
            _modifyAttributeSkillsBySkillID = _modifyAttributeSkills.GroupBy(entry => entry.SkillID).ToDictionary(
                group => group.Key,
                group => group.AsEnumerable());
            _coefficientFormulasBySetID = _coefficientFormulaSets.GroupBy(entry => entry.ID).ToDictionary(
                group => group.Key,
                group => group.ToDictionary(
                    entry => entry.TargetCoefficientKey,
                    entry => GetFormulaByID(entry.FormulaID)));
        }

        public void AddUnitAttribute(UnitAttribute entry)
        {
            _unitAttributes.Add(entry);
        }

        public void AddLevelAttribute(GameData.LevelAttribute entry)
        {
            _levelAttributes.Add(entry);
        }

        public void AddAttribute(GameData.Attribute entry)
        {
            _attributes.Add(entry);
        }

        public void AddLevelUpRewardSkill(LevelUpRewardSkill entry)
        {
            _levelUpRewardSkills.Add(entry);
        }

        public void AddSkill(Skill entry)
        {
            _skills.Add(entry);
        }

        public void AddSkillGrowthFormula(SkillGrowthFormula entry)
        {
            _skillGrowthFormulas.Add(entry);
        }

        public void AddUnitDefaultSkill(UnitDefaultSkill entry)
        {
            _unitDefaultSkills.Add(entry);
        }

        public void AddFormula(Formula entry)
        {
            _formulas.Add(entry);
        }

        public void AddUnitAttributeGrowthFormula(GrowthFormula entry)
        {
            _growthFormulas.Add(entry);
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

        public void AddSkillAttribute(SkillBaseAttribute entry)
        {
            _skillBaseAttributes.Add(entry);
        }

        public void AddCoefficient(Coefficient entry)
        {
            _coefficients.Add(entry);
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

        public IReadOnlyDictionary<string, float> GetCoefficientByFormulaID(string id)
        {
            return _coefficientsByFormulaID[id];
        }

        public IEnumerable<GrowthFormula> GetGrowthFormulaByID(string id)
        {
            return _growthFormulasByID[id];
        }

        public Formula GetFormulaByID(string id)
        {
            return _formulasByID.GetValueOrDefault(id);
        }

        public Skill GetSkillById(string id)
        {
            return _skillsByID.GetValueOrDefault(id);
        }

        public int GetSoulExp(int level)
        {
            var factors = _coefficientsByFormulaID[CoefficientID.SoulExp].ToList();
            if (factors.Count == 0) return 0;

            float a = factors[0].Value;
            return (int)Math.Floor(a * level);
        }

        public int GetRequireExp(int level)
        {
            var factors = _coefficients.Where(entry => entry.FormulaID == CoefficientID.RequireExp);
            var a = factors.ElementAt(0).Value;
            var b = factors.ElementAt(1).Value;
            var c = factors.ElementAt(2).Value;

            return (int)Math.Floor(a * Math.Pow(level, 2) + b * level + c);
        }

        public IEnumerable<ModifyAttributeSkill> GetModifyAttributeSkillEffects(string skillID)
        {
            return _modifyAttributeSkillsBySkillID.GetValueOrDefault(skillID);
        }

        public IEnumerable<SkillBaseAttribute> GetSkillBaseAttributes()
        {
            return _skillBaseAttributes;
        }

        public IEnumerable<Coefficient> GetCoefficients()
        {
            return _coefficients;
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

        public IEnumerable<GrowthFormula> GetGrowthFormulas()
        {
            return _growthFormulas;
        }

        public IEnumerable<GrowthFormula> GetGrowthFormulas(string id)
        {
            return _growthFormulas.Where(entry => entry.ID == id);
        }

        public IEnumerable<Formula> GetFormulas()
        {
            return _formulas;
        }

        public IEnumerable<Skill> GetSkills()
        {
            return _skills;
        }

        public IEnumerable<UnitDefaultSkill> GetUnitDefaultSkills()
        {
            return _unitDefaultSkills;
        }

        public IEnumerable<SkillGrowthFormula> GetSkillGrowthFormulas()
        {
            return _skillGrowthFormulas;
        }

        public IEnumerable<GrowthFormula> GetSkillGrowthFormulas(string skillID)
        {
            var growthFormulaID = GetSkillGrowthFormulas().Where(entry => entry.SkillID == skillID).Select(entry => entry.GrowthFormulaID).FirstOrDefault();
            return GetGrowthFormulas(growthFormulaID);
        }

        public IReadOnlyDictionary<string, Attribute> GetSkillBaseAttributes(string skillID)
        {
            var result = new Dictionary<string, Attribute>();
            var attributes = GetSkillBaseAttributes().Where(entry => entry.SkillID == skillID);
            if (!attributes.Any())
                return result;
            foreach (var entry in attributes)
                result[entry.AttributeID] = entry.Value;
            return result;
        }

        public IEnumerable<LevelUpRewardSkill> GetLevelUpRewardSkills()
        {
            return _levelUpRewardSkills;
        }

        public void AddAttributeSet(AttributeSet entry)
        {
            _attributeSets.Add(entry);
        }

        public void AddModifyAttributeSkill(ModifyAttributeSkill entry)
        {
            _modifyAttributeSkills.Add(entry);
        }

        public void AddCoeffcientFomulaSet(CoefficientFormulaSet entry)
        {
            _coefficientFormulaSets.Add(entry);
        }

        public void AddUnitDefaultActiveSkill(UnitDefaultActiveSkill entry)
        {
            _unitDefaultActiveSkill.Add(entry);
        }

        public IReadOnlyDictionary<string, Formula> GetCoefficientFormulasBySetID(string setID)
        {
            return _coefficientFormulasBySetID[setID];
        }

        public UnitDefaultActiveSkill GetUnitDefaultActiveSkillByUnitID(string unitID)
        {
            return _unitDefaultActiveSkill.FirstOrDefault(entry => entry.UnitID == unitID);
        }

        public UnitAttribute GetUnitAttributeByUnitID(string unitID)
        {
            return _unitAttributes.FirstOrDefault(entry => entry.UnitID == unitID);
        }

        public IEnumerable<GameData.Attribute> GetAttributes(string attributeID)
        {
            return _attributes.Where(entry => entry.ID == attributeID);
        }

        public IEnumerable<LevelAttribute> GetLevelAttributes(string levelAttributeID)
        {
            return _levelAttributes.Where(entry => entry.ID == levelAttributeID);
        }
    }
}
