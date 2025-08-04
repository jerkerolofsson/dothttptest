
using DotHttpTest.Verification.Json;
using ModelContextProtocol.Protocol;
using Newtonsoft.Json.Linq;
using System.Text.Json;

namespace DotHttpTest.Verification.Mcp
{
    /// <summary>
    /// Various MCP verification
    /// </summary>
    [ResponseVerifier("mcp")]
    internal class McpVerifier : ValueVerifier, IVerifier
    {
        public Task VerifyAsync(DotHttpResponse response, VerificationCheckResult result)
        {
            result.IsSuccess = VerifyContent(response, result);
            if (!result.IsSuccess)
            {
                if (string.IsNullOrEmpty(result.Error))
                {
                    result.Error = $"Check {result.Check} failed";
                }
            }
            return Task.CompletedTask;
        }

        private bool VerifyContent(DotHttpResponse response, VerificationCheckResult result)
        {
            var check = result.Check;
            switch(check.PropertyId)
            {
                case "success":
                    return VerifySuccess(response, result);
                case "failure":
                    return VerifyFailure(response, result);
                case "textContent":
                    return VerifyTextContent(response, result);
                default:
                    throw new ArgumentException($"Unknown property: {check.PropertyId}");
            }

        }

        private bool VerifyFailure(DotHttpResponse response, VerificationCheckResult result)
        {
            var success = VerifySuccess(response, result);
            if (success)
            {
                result.Error = "MCP tool call was successful, but expected a failure";
                return false;
            }
            return true;
        }

        private bool VerifySuccess(DotHttpResponse response, VerificationCheckResult result)
        {
            if(response.CallToolResult?.IsError is null || response.CallToolResult?.IsError == false)
            {
                return true;
            }
            result.Error = $"MCP tool call failed. CallToolResult.IsError={response.CallToolResult?.IsError}";
            return false;
        }

        private bool VerifyTextContent(DotHttpResponse response, VerificationCheckResult result)
        {
            var check = result.Check;
            if(check.Operation == VerificationOperation.NotExists)
            {
                var exists = response.CallToolResult != null && response.CallToolResult.Content.Any(x => x is TextContentBlock);
                if(exists)
                {
                    result.Error = "Expected response without TextContentBlock but a TextContentBlock was found";
                    return false;
                }
                return true;
            }
            else
            {
                var textContent = response.CallToolResult?.Content?.FirstOrDefault(x => x is TextContentBlock) as TextContentBlock;
                if(textContent is null)
                {
                    result.Error = "Expected response with TextContentBlock but no TextContentBlock was found";
                    return false;
                }

                var actual = textContent.Text;
                var success = CompareValue(actual, check.ExpectedValue, check.Operation);

                if(!success)
                {
                    result.Error = $"Expected: '{check.ExpectedValue}' for {check.Operation}-Check but got '{actual}'";
                }

                return success;
            }
        }
    }
}
