using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace DotHttpTest.Parser
{
    internal class VariableLoaderState
    {
        private readonly ClientOptions mOptions;
        private Dictionary<string, string> mLocalVariables = new Dictionary<string, string>();

        public VariableLoaderState(ClientOptions options)
        {
            mOptions = options;
        }

        /// <summary>
        /// Manual variable assignment from a line
        /// </summary>
        /// <param name="line"></param>
        public void ParseVariableAssignmentLine(string line)
        {
            var p = line.IndexOf('=');
            if (p == -1) return;

            var name = line.Substring(0, p).TrimStart('@').Trim();
            var val = line.Substring(p+1).Trim();
            SetLocalVariable(name, val);
        }

        public ExpressionList ReplaceDataVariables(string data)
        {
            var expressions = new ExpressionList();
            string startPattern = "{{";
            string endPattern = "}}";

            int prevPos = 0;
            while (true)
            {
                var startPos = data.IndexOf(startPattern, prevPos);
                if(startPos == -1)
                {
                    expressions.Add(new Text(data.Substring(prevPos)));
                    break;
                } 
                else
                {
                    var endPos = data.IndexOf(endPattern, startPos);
                    if(endPos == -1)
                    {
                        // There is no end
                        expressions.Add(new Text(data.Substring(prevPos)));
                        break;
                    }

                    // We have found a variable
                    var variableEndPos = endPos + endPattern.Length;

                    var beforeVariable = data.Substring(prevPos, startPos - prevPos);
                    expressions.Add(new Text(beforeVariable));

                    // Extract the variable
                    var variable = data.Substring(startPos, variableEndPos - startPos);
                    var variableName = variable.TrimStart('{').TrimEnd('}');
                    expressions.Add(GetVariable(variableName));    

                    // Continue after the variable
                    prevPos = variableEndPos;
                }
            }


            return expressions;
        }

        /// <summary>
        /// Tries to get a varioable value. Local variables have precedence
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Variable GetVariable(string name)
        {
            if (mLocalVariables.TryGetValue(name, out string? val))
            {
                return new StringVariable()
                {
                    Value = val
                };
            }

            if (mOptions.VariableProviders != null)
            {
                foreach (var variableProvider in mOptions.VariableProviders)
                {
                    Variable? variable = variableProvider.GetVariableValue(name);
                    if (variable != null)
                    {
                        return variable;
                    }
                }
            }
            throw new ArgumentException($"There is no variable named '{name}' found");
        }
        public void SetLocalVariable(string name, string val)
        {
            mLocalVariables[name] = val;
        }
    }
}
