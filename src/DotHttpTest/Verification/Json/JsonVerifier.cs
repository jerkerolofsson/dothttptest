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
            JObject? root = null;
            try
            {
                root = JObject.Parse(text);
            }
            catch(Exception ex)
            {
                // Root object may be an array, wrap it in an object so we can use SelectToken on it
                var array = JArray.Parse(text);
                root = new JObject();
                root.Add(new JProperty("Array", array));
                selector = "$.Array" + selector;
            }

            if (root == null)
            {
                result.Error = $"json: Failed to parse json. root object is null";
                result.IsSuccess = false;
                return;
            }

            JToken? token;
            try
            {
                token = root.SelectToken(selector);
            }
            catch(Exception ex)
            {
                result.Error = $"json: Error reading token from selector: {selector}: {ex.Message}";
                result.IsSuccess = false;
                return;
            }
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
            result.ActualValue = token.ToString();
            if (!CompareValue(result.ActualValue, check.ExpectedValue, check.Operation))
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
