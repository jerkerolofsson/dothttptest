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
        private readonly Dictionary<string, string> mVariables = new();

        public void SetVariableValue(string variableName, string value)
        {
            mVariables[variableName] = value;
        }

        public string? GetVariableValue(string variableName)
        {
            mVariables.TryGetValue(variableName, out string? value);
            return value;
        }

        public Dictionary<string, string> ToDictionary()
        {
            return mVariables;
        }
    }
}
