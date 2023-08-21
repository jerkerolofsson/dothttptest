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

        public Variable? GetVariableValue(string variableName)
        {
            var val = Environment.GetEnvironmentVariable(variableName, mEnvironmentVariableTarget);
            if(val != null)
            {
                return new StringVariable() { Value = val };
            }
            return null;
        }
    }
}
