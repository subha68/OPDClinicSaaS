namespace Helpers
{
    public class CalculationEngine
    {
        private static readonly HashSet<string> SupportedFunctions =
                        new(StringComparer.OrdinalIgnoreCase)
                        {
                            "SQRT",
                            "ABS",
                            "ROUND",
                            "MIN",
                            "MAX"
                        };

        // supports: * , + , - , * , / , parentheses , SQRT() 

        public List<string> ExtractVariables(string formula)
        {
            if (string.IsNullOrWhiteSpace(formula))
                return new List<string>();

            return System.Text.RegularExpressions.Regex
                .Matches(formula, @"\b[A-Za-z_][A-Za-z0-9_]*\b")
                .Cast<System.Text.RegularExpressions.Match>()
                .Select(m => m.Value.ToUpperInvariant())
                .Where(v => !SupportedFunctions.Contains(v))
                .Distinct()
                .OrderBy(v => v)
                .ToList();
        }        
        
        public decimal? Calculate(string formula, IDictionary<string, decimal> values)
        {
            if (string.IsNullOrWhiteSpace(formula))
                return null;

            if (values == null)
                return null;

            try
            {
                string expression = formula;

                /*
                 * Replace variable names with their values.
                 *
                 * Example:
                 *
                 * WEIGHT / ((HEIGHT / 100) * (HEIGHT / 100))
                 *
                 * becomes:
                 *
                 * 72 / ((175 / 100) * (175 / 100))
                 */
                foreach (var item in values)
                {
                    expression =
                        System.Text.RegularExpressions.Regex.Replace(
                            expression,
                            $@"\b{System.Text.RegularExpressions.Regex.Escape(item.Key)}\b",
                            item.Value.ToString(
                                System.Globalization.CultureInfo.InvariantCulture),
                            System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                }

                /*
                 * Any remaining identifier means that the formula
                 * references a value that was not supplied.
                 *
                 * SQRT is a supported function, so exclude it.
                 */
                string expressionWithoutFunctions =
                    System.Text.RegularExpressions.Regex.Replace(
                        expression,
                        @"\bSQRT\b",
                        "",
                        System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                if (System.Text.RegularExpressions.Regex.IsMatch(
                        expressionWithoutFunctions,
                        @"[A-Za-z_]"))
                {
                    return null;
                }

                return EvaluateExpression(expression);
            }
            catch
            {
                return null;
            }
        }


        private decimal? EvaluateExpression(string expression)
        {
            expression = expression.Trim();

            /*
             * Resolve SQRT(...) expressions.
             */
            while (true)
            {
                var match =
                    System.Text.RegularExpressions.Regex.Match(
                        expression,
                        @"SQRT\s*\(\s*([^()]*)\s*\)",
                        System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                if (!match.Success)
                    break;

                decimal? innerValue =
                    EvaluateExpression(
                        match.Groups[1].Value);

                if (!innerValue.HasValue)
                    return null;

                if (innerValue.Value < 0)
                    return null;

                decimal result =
                    (decimal)Math.Sqrt(
                        (double)innerValue.Value);

                expression =
                    expression.Substring(
                        0,
                        match.Index)
                    + result.ToString(
                        System.Globalization.CultureInfo.InvariantCulture)
                    + expression.Substring(
                        match.Index + match.Length);
            }

            /*
             * Resolve parentheses.
             */
            while (true)
            {
                var match =
                    System.Text.RegularExpressions.Regex.Match(
                        expression,
                        @"\(([^()]*)\)");

                if (!match.Success)
                    break;

                decimal? value =
                    EvaluateExpression(
                        match.Groups[1].Value);

                if (!value.HasValue)
                    return null;

                expression =
                    expression.Substring(
                        0,
                        match.Index)
                    + value.Value.ToString(
                        System.Globalization.CultureInfo.InvariantCulture)
                    + expression.Substring(
                        match.Index + match.Length);
            }

            return EvaluateSimpleExpression(expression);
        }


        private decimal? EvaluateSimpleExpression(string expression)
        {
            expression = expression.Trim();

            /*
             * Split addition and subtraction while preserving
             * the operator.
             */
            var parts =
                System.Text.RegularExpressions.Regex.Split(
                    expression,
                    @"(?=[+-])");

            decimal? result = null;

            foreach (var rawPart in parts)
            {
                string part = rawPart.Trim();

                if (string.IsNullOrWhiteSpace(part))
                    continue;

                char operation = '+';

                if (part[0] == '+' || part[0] == '-')
                {
                    operation = part[0];

                    part =
                        part.Substring(1).Trim();
                }

                decimal value =
                    EvaluateMultiplyDivide(part);

                if (!result.HasValue)
                {
                    result =
                        operation == '-'
                            ? -value
                            : value;
                }
                else if (operation == '-')
                {
                    result -= value;
                }
                else
                {
                    result += value;
                }
            }

            return result;
        }


        private decimal EvaluateMultiplyDivide(string expression)
        {
            var parts =
                System.Text.RegularExpressions.Regex.Split(
                    expression,
                    @"([*/])");

            decimal result =
                decimal.Parse(
                    parts[0].Trim(),
                    System.Globalization.CultureInfo.InvariantCulture);

            for (int i = 1; i < parts.Length; i += 2)
            {
                string operation = parts[i];

                decimal operand =
                    decimal.Parse(
                        parts[i + 1].Trim(),
                        System.Globalization.CultureInfo.InvariantCulture);

                if (operation == "*")
                {
                    result *= operand;
                }
                else
                {
                    if (operand == 0)
                        throw new DivideByZeroException();

                    result /= operand;
                }
            }

            return result;
        }
    }
}
