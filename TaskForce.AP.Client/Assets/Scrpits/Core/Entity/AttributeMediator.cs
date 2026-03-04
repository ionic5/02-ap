using System.Collections.Generic;
using System.Linq;

namespace TaskForce.AP.Client.Core.Entity
{
    public class AttributeMediator
    {
        private readonly GameDataStore _gameDataStore;
        private readonly Core.ILogger _logger;
        private readonly AttributeStore _baseAttributeStore;
        private readonly AttributeStore _attributeStore;
        private readonly List<IModifyAttributeEffect> _modifyAttributeEffects;
        private readonly List<GameData.GrowthFormula> _growthFormulas;
        private int _level;

        public AttributeMediator(GameDataStore gameDataStore, ILogger logger)
        {
            _attributeStore = new AttributeStore();
            _modifyAttributeEffects = new List<IModifyAttributeEffect>();
            _baseAttributeStore = new AttributeStore();
            _growthFormulas = new List<GameData.GrowthFormula>();
            _gameDataStore = gameDataStore;
            _logger = logger;
            _level = 1;

            UpdateAttributes();
        }

        public void SetLevel(int level)
        {
            _level = level;

            UpdateAttributes();
        }

        public void SetBaseAttributes(IReadOnlyDictionary<string, Attribute> attributes)
        {
            _baseAttributeStore.Clear();
            foreach (var entry in attributes)
                _baseAttributeStore.Set(entry.Key, entry.Value);

            UpdateAttributes();
        }

        public void SetGrowthFormulas(IEnumerable<GameData.GrowthFormula> formulas)
        {
            _growthFormulas.Clear();
            _growthFormulas.AddRange(formulas);
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

        public Attribute GetAttribute(string id)
        {
            return _attributeStore.Get(id);
        }

        private void UpdateAttributes()
        {
            _baseAttributeStore.CopyTo(_attributeStore);

            foreach (var formula in _growthFormulas)
            {
                var value = ApplyGrowth(formula.FormulaID, _attributeStore.Get(formula.TargetID), _level);
                _attributeStore.Set(formula.TargetID, value);
            }

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

            foreach (var entry in mergedEffects)
                entry.Apply(_attributeStore);
        }

        private Attribute ApplyGrowth(string formulaID, Attribute attribute, int level)
        {
            var formula = _gameDataStore.GetFormulas().FirstOrDefault(entry => entry.ID == formulaID);
            if (formula == null)
            {
                _logger.Warn($"Failed to find formula for id ({formulaID}) base stat returned.");
                return attribute;
            }

            var coeffs = _gameDataStore.GetCoefficients().Where(entry => entry.FormulaID == formulaID);
            var a = coeffs.ElementAtOrDefault(0).Value;
            return new Attribute(attribute.AsFloat() + a * (1 - level));
        }
    }
}
