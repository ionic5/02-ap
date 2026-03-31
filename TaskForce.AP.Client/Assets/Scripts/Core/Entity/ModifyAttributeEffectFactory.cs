using log4net.Core;
using System.Collections.Generic;
using System.Linq;
using TaskForce.AP.Client.Core.GameData;

namespace TaskForce.AP.Client.Core.Entity
{
    public class ModifyAttributeEffectFactory
    {
        private readonly GameDataStore _gameDataStore;
        private readonly FormulaCalculator _formulaCalculator;

        public ModifyAttributeEffectFactory(GameDataStore gameDataStore, FormulaCalculator formulaCalculator)
        {
            _gameDataStore = gameDataStore;
            _formulaCalculator = formulaCalculator;
        }

        public IModifyAttributeEffect Create(string effectID, int level)
        {
            var effectData = _gameDataStore.GetModifyAttributeEffects().FirstOrDefault(entry => entry.ID == effectID);

            var lvCoeffs = _gameDataStore.GetLevelCoefficients(effectData.LevelCoefficientID);
            var closestGroup = lvCoeffs.GroupBy(e => e.Level)
                .Where(g => g.Key <= level).OrderByDescending(g => g.Key).FirstOrDefault();
            var coeffs = new Dictionary<string, float>();
            if (closestGroup != null)
                foreach (var entry in closestGroup)
                    coeffs.Add(entry.Key, entry.Value);

            return new ModifyAttributeEffect(effectData.ApplyOrder, effectData.AttributeID,
                effectData.CalculationType, coeffs, _formulaCalculator);
        }
    }
}
