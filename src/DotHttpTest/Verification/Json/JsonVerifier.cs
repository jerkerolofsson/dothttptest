using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotHttpTest.Verification.Json
{
    [ResponseVerifier("json")]
    internal class JsonVerifier : ValueVerifier, IVerifier
    {
        public void Verify(DotHttpResponse response, VerificationCheckResult result)
        {
            var check = result.Check;
            var selector = check.PropertyId;

            var bytes = response.ContentBytes;
            if (bytes == null || bytes.Length == 0)
            {
                result.Error = "json: Response body is empty";
                result.IsSuccess = false;
                return;
            }
            var text = Encoding.UTF8.GetString(bytes); // Todo: We need to check the encoding here..
            var root = JObject.Parse(text);
            var token = root.SelectToken(selector);
            if (token == null)
            {
                if (check.Operation == VerificationOperation.NotExists)
                {
                    result.IsSuccess = true;
                    return;
                }

                result.Error = $"json: Not found: {selector}";
                result.IsSuccess = false;
                return;
            }

            if(!CompareValue(token.ToString(), check.ExpectedValue, check.Operation))
            {
                result.Error = $"json: Expected: {check.ExpectedValue} but got {token.ToString()}";
                result.IsSuccess = false;
                return;
            }
            result.IsSuccess = true;
            result.Error = null;
        }
    }
}
