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

        public void ParseVariableAssignmentLine(string line)
        {
            var p = line.IndexOf('=');
            if (p == -1) return;

            var name = line.Substring(0, p).TrimStart('@').Trim();
            var val = line.Substring(p+1).Trim();
            SetLocalVariable(name, val);
        }

        public string ReplaceDataVariables(string data)
        {
            var sb = new StringBuilder();
            string startPattern = "{{";
            string endPattern = "}}";

            int prevPos = 0;
            while (true)
            {
                var startPos = data.IndexOf(startPattern, prevPos);
                if(startPos == -1)
                {
                    sb.Append(data.Substring(prevPos));
                    break;
                } 
                else
                {
                    var endPos = data.IndexOf(endPattern, startPos);
                    if(endPos == -1)
                    {
                        // There is no end
                        sb.Append(data.Substring(prevPos));
                        break;
                    }

                    // We have found a variable
                    var variableEndPos = endPos + endPattern.Length;

                    var beforeVariable = data.Substring(prevPos, startPos - prevPos);
                    sb.Append(beforeVariable);

                    // Extract the variable
                    var variable = data.Substring(startPos, variableEndPos - startPos);
                    var variableName = variable.TrimStart('{').TrimEnd('}');
                    sb.Append(GetVariable(variableName));    

                    // Continue after the variable
                    prevPos = variableEndPos;
                }
            }


            return sb.ToString();
        }

        /// <summary>
        /// Tries to get a varioable value. Local variables have precedence
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public string GetVariable(string name)
        {
            if (mLocalVariables.TryGetValue(name, out string? val))
            {
                return val;
            }

            if (mOptions.VariableProviders != null)
            {
                foreach (var variableProvider in mOptions.VariableProviders)
                {
                    val = variableProvider.GetVariableValue(name);
                    if (val != null)
                    {
                        break;
                    }
                }
            }
            if(val == null)
            {
                throw new ArgumentException($"There is no variable named '{name}' found");
            }
            return val;
        }
        public void SetLocalVariable(string name, string val)
        {
            mLocalVariables[name] = val;
        }
    }
}
