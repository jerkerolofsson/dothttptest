using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotHttpTest.Providers
{
    /// <summary>
    /// Allows user-defined variables to be injected into a request
    /// </summary>
    public class VariableCollection : IVariableProvider
    {
        private readonly Dictionary<string, Variable> mVariables = new();

        public void SetVariableValue(string variableName, string value)
        {
            mVariables[variableName] = new Variable() { Value = value };
        }

        public Variable? GetVariableValue(string variableName)
        {
            mVariables.TryGetValue(variableName, out Variable? value);
            return value;
        }
            }
}
