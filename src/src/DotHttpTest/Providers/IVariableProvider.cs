using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotHttpTest.Providers
{
    /// <summary>
    /// Provides variable values from some source
    /// </summary>
    public interface IVariableProvider
    {
        Dictionary<string, string> ToDictionary();

        string? GetVariableValue(string variableName);
    }
}
