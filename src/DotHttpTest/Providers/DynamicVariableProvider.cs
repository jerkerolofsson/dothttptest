using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotHttpTest.Providers
{
    /// <summary>
    /// Provides dynamic variables such as generating a UUID
    /// </summary>
    public class DynamicVariableProvider : IVariableProvider
    {
        public string? GetVariableValue(string variableName)
        {
            switch(variableName)
            {
                case "$uuid":
                    return Guid.NewGuid().ToString();
                case "$isoTimestamp":
                    return DateTime.UtcNow.ToString("o");
                case "$randomInt":
                    // Random value between 0 and 1000
                    return Random.Shared.Next(0,1001).ToString();
            }
            return null;
        }
    }
}
