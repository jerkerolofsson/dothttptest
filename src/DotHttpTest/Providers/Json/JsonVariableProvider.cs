using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotHttpTest.Providers.Json
{
    /// <summary>
    /// Provides a value from a json selector
    /// </summary>
    public class JsonVariableProvider : IVariableProvider
    {
        public Variable? GetVariableValue(string variableName)
        {
            var prefix = "$json";
            if (variableName.StartsWith(prefix))
            {
                // Generate a late-bound placeholder
                var selector = variableName.Substring(prefix.Length);
                return new JsonVariable("$" + selector);
            }
            return null;
        }
    }
}
