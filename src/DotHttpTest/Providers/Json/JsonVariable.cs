using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotHttpTest.Providers.Json
{
    internal class JsonVariable : Variable
    {
        private string mSelector;

        public JsonVariable(string selector)
        {
            mSelector = selector;
        }

        public override string ToString()
        {
            throw new InvalidOperationException("$json variables must be used with TestPlanRunner");
        }

        public override string? ToString(TestStatus? status)
        {
            // Lookup from the response
            if (status == null)
            {
                throw new InvalidOperationException("$json variables must be used with TestPlanRunner");
            }
            if (status.PreviousResponse == null)
            {
                throw new InvalidOperationException("$json variables can only be used in the 2nd request as they refer to the response from the previous request");
            }

            var bytes = status.PreviousResponse.ContentBytes;
            if (bytes == null || bytes.Length == 0)
            {
                throw new InvalidOperationException("$json lookup failed as previous response did not contain any content");
            }
            var text = Encoding.UTF8.GetString(bytes); // Todo: We need to check the encoding here..
            var root = JObject.Parse(text);
            var token = root.SelectToken(mSelector);
            if(token == null)
            {
                return null;
            }

            return token.ToString();
        }
    }
}
