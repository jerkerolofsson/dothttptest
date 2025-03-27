using DotHttpTest.Providers.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotHttpTest.Providers.Random
{
    /// <summary>
    /// Expands to a random byte array
    /// </summary>
    internal class RandomBytesProvider : IVariableProvider
    {
        public Variable? GetVariableValue(string variableName)
        {
            var prefix = "$randomBytes";
            if (variableName.StartsWith(prefix))
            {
                // Generate a late-bound placeholder
                var lengthString = variableName.Substring(prefix.Length).Trim();
                if (int.TryParse(lengthString, out var length))
                {
                    return new RandomByteArray(length);
                }
            }
            return null;
        }
    }
}
