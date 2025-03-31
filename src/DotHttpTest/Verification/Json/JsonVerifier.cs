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
            catch(Exception ex)
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
            catch(Exception ex)
            {
                result.Error = $"json: Error reading token from selector: {selector}: {ex.Message}\njson:\n{text}";
                result.IsSuccess = false;
                return Task.CompletedTask;
            }
            if (token == null)
            {
                if (check.Operation == VerificationOperation.NotExists)
                {
                    result.IsSuccess = true;
                    return Task.CompletedTask;
                }

                result.Error = $"json: Not found: {selector}\njson:\n{text}";
                result.IsSuccess = false;
                return Task.CompletedTask;
            }
            result.ActualValue = token.ToString();
            try
            {
                if (!CompareValue(token, result.ActualValue, check.ExpectedValue, check.Operation, out var actualValue))
                {
                    if (actualValue is null)
                    {
                        result.Error = $"json: Expected: {check.ExpectedValue} for operation {check.Operation} but got {token.ToString()}\njson:\n{text}";
                    }
                    else
                    {
                        result.ActualValue = actualValue;
                        result.Error = $"json: Expected: {check.ExpectedValue} for operation {check.Operation} but got {actualValue}\njson:\n{text}";
                    }
                    result.IsSuccess = false;
                    return Task.CompletedTask;
                }
            }
            catch(Exception ex)
            {
                result.Error = $"json: {ex.Message}";
                result.IsSuccess = false;
                return Task.CompletedTask;
            }
            result.IsSuccess = true;
            result.Error = null;
            return Task.CompletedTask;
        }
    }
}
