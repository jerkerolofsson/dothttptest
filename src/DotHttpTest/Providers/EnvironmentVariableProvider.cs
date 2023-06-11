using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotHttpTest.Providers
{
    public class EnvironmentVariableProvider : IVariableProvider
    {
        private readonly EnvironmentVariableTarget mEnvironmentVariableTarget;

        public EnvironmentVariableProvider(EnvironmentVariableTarget environmentVariableTarget)
        {
            mEnvironmentVariableTarget = environmentVariableTarget;
        }

        public string? GetVariableValue(string variableName)
        {
            return Environment.GetEnvironmentVariable(variableName, mEnvironmentVariableTarget);
        }

        public Dictionary<string, string> ToDictionary()
        {
            var dict = new Dictionary<string, string>();
            var environmentVariables = Environment.GetEnvironmentVariables(mEnvironmentVariableTarget);
            foreach (var key in environmentVariables.Keys)
            {
                if(key != null)
                {
                    var val = environmentVariables[key];
                    if(val != null)
                    {
                        var keyString = key.ToString();
                        var valString = val.ToString();
                        if (keyString != null && valString != null)
                        {
                            dict[keyString] = valString;
                        }
                    }
                }
                
            }
            return dict;
        }
    }
}
