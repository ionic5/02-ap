using System;
using System.Collections.Generic;

namespace TaskForce.AP.Client.Core
{
    public class FormulaCalculator
    {
        private readonly Dictionary<string, Func<IReadOnlyDictionary<string, float>, float[], float>> _formulas;
        private readonly ILogger _logger;

        public FormulaCalculator(ILogger logger)
        {
            _formulas = new Dictionary<string, Func<IReadOnlyDictionary<string, float>, float[], float>>();
            _logger = logger;
        }

        public void RegisterFormula(string id, Func<IReadOnlyDictionary<string, float>, float[], float> formula)
        {
            _formulas[id] = formula;
        }

        public float Calculate(string calculationType, IReadOnlyDictionary<string, float> coefficients, params float[] variables)
        {
            if (!_formulas.TryGetValue(calculationType, out var formula))
            {
                _logger.Warn($"[FormulaCalculator] Formula '{calculationType}' not found.");
                return variables.Length > 0 ? variables[0] : 0f;
            }

            return formula(coefficients, variables);
        }
    }
}
