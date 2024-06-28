using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotHttpTest.Providers.State
{
    /// <summary>
    /// Provides variables from the dothttp state
    /// </summary>
    internal class StateVariableProvider : IVariableProvider
    {
        public Variable? GetVariableValue(string variableName)
        {
            switch (variableName)
            {
                case "stageIndex":
                    return new StageIndexVariable();
                case "iteration":
                    return new StageWorkerLoopCountVariable();
                default:
                    return null;
            }
        }
    }
}
