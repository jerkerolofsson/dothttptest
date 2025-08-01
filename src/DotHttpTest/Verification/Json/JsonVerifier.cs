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
        public Task VerifyAsync(DotHttpResponse response, VerificationCheckResult result)
        {
            var check = result.Check;
            var selector = check.PropertyId;

            var bytes = response.ContentBytes;
            if (bytes == null || bytes.Length == 0)
            {
                result.Error = "json: Response body is empty";
                result.IsSuccess = false;
                return Task.CompletedTask;
            }
            var text = Encoding.UTF8.GetString(bytes); // Todo: We need to check the encoding here..
            JObject? root = null;
            try
            {
                root = JObject.Parse(text);
            }
            catch (Exception)
            {
                // Root object may be an array, wrap it in an object so we can use SelectToken on it
                var array = JArray.Parse(text);
                root = new JObject();
                root.Add(new JProperty("Array", array));

                if (selector == ".")
                {
                    selector = "$.Array";
                }
                else
                {
                    selector = "$.Array" + selector;
                }
            }

            if (root == null)
            {
                result.Error = $"json: Failed to parse json. root object is null";
                result.IsSuccess = false;
                return Task.CompletedTask;
            }

            JToken? token;
            try
            {
                token = root.SelectToken(selector);
            }
            catch (Exception ex)
            {
                result.Error = $"json: Error reading token from selector: {selector}: {ex.Message}\njson:\n{text}";
                result.IsSuccess = false;
                return Task.CompletedTask;
            }
            result.IsSuccess = VerifyJsonToken(result, check, selector, text, token);
            return Task.CompletedTask;
        }

        internal static bool VerifyJsonToken(VerificationCheckResult result, VerificationCheck check, string selector, string actualJsonString, JToken? token)
        {
            if (token == null)
            {
                if (check.Operation == VerificationOperation.NotExists)
                {
                    result.IsSuccess = true;
                    return true;
                }

                result.Error = $"json: Not found: {selector}\njson:\n{actualJsonString}";
                return false;
            }
            result.ActualValue = token.ToString();
            try
            {
                if (!CompareValue(token, result.ActualValue, check.ExpectedValue, check.Operation, out var actualValue))
                {
                    if (actualValue is null)
                    {
                        result.Error = $"json: Expected: '{check.ExpectedValue}' for {check.Operation}-Check but got '{token.ToString()}'\njson:\n{actualJsonString}";
                    }
                    else
                    {
                        result.ActualValue = actualValue;
                        result.Error = $"json: Expected: '{check.ExpectedValue}' for {check.Operation}-Check but got '{actualValue}'\njson:\n{actualJsonString}";
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                result.Error = $"json: {ex.Message}";
                return false;
            }
            result.IsSuccess = true;
            result.Error = null;
            return true;
        }
    }
}
