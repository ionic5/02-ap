using System.Collections.Generic;

namespace TaskForce.AP.Client.Core.Entity
{
    public class ModifyAttributeEffect : IModifyAttributeEffect
    {
        private int _applyOrder;
        private string _targetAttributeID;
        private string _calculationType;
        private Dictionary<string, float> _coefficients;
        private readonly FormulaCalculator _formulaCalculator;

        public ModifyAttributeEffect(int applyOrder,
            string targetAttributeID, string calculationType, Dictionary<string, float> coefficients,
            FormulaCalculator formulaCalculator)
        {
            _applyOrder = applyOrder;
            _targetAttributeID = targetAttributeID;
            _calculationType = calculationType;
            _coefficients = coefficients;
            _formulaCalculator = formulaCalculator;
        }

        public void Apply(AttributeStore store)
        {
            var x = _formulaCalculator.Calculate(_calculationType, _coefficients, store.Get(_targetAttributeID).AsFloat());
            store.Set(_targetAttributeID, new Attribute(x));
        }

        public bool CanMerge(IModifyAttributeEffect effect)
        {
            if (effect is not ModifyAttributeEffect)
                return false;

            var other = effect as ModifyAttributeEffect;
            if (_applyOrder != other._applyOrder)
                return false;
            if (_calculationType != other._calculationType)
                return false;
            if (_targetAttributeID != other._targetAttributeID)
                return false;

            return true;
        }

        public int GetApplyOrder()
        {
            return _applyOrder;
        }

        public void Merge(IModifyAttributeEffect effect)
        {
            if (effect is not ModifyAttributeEffect)
                return;

            var other = effect as ModifyAttributeEffect;
            foreach (var entry in _coefficients)
            {
                var key = entry.Key;
                _coefficients[key] += other._coefficients[key];
            }
        }

        public IModifyAttributeEffect Clone()
        {
            return new ModifyAttributeEffect(_applyOrder,
                _targetAttributeID, _calculationType, _coefficients,
                _formulaCalculator);
        }
    }
}
