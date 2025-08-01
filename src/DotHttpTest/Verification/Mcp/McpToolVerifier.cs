
using DotHttpTest.Verification.Json;
using ModelContextProtocol.Protocol;
using Newtonsoft.Json.Linq;
using System.Text.Json;

namespace DotHttpTest.Verification.Mcp
{
    [ResponseVerifier("tool")]
    internal class McpToolVerifier : ValueVerifier, IVerifier
    {
        public Task VerifyAsync(DotHttpResponse response, VerificationCheckResult result)
        {
            result.IsSuccess = VerifyToolContent(response, result);
            return Task.CompletedTask;
        }

        private bool VerifyToolContent(DotHttpResponse response, VerificationCheckResult result)
        {
            var check = result.Check;
            if (response.ContentBytes is null || response.ContentBytes.Length == 0)
            {
                result.Error = "tool: Response body is empty";
                return false;
            }

            var json = Encoding.UTF8.GetString(response.ContentBytes);
            var tools = JsonSerializer.Deserialize<Tool[]>(json, ModelContextProtocol.McpJsonUtilities.DefaultOptions);
            if (tools is null)
            {
                result.Error = "tool: Failed to deserialize tools";
                return false;
            }

            var toolName = check.PropertyId;

            if (toolName.Contains('.'))
            {
                var items = toolName.Split('.');

                // To
                toolName = items[0];
                var section = items[1];
                switch(section)
                {
                    case "inputSchema":
                        if(items.Length == 2)
                        {
                            result.Error = $"tool: Expected format 'toolName.properties.propertyName'";
                            return false;
                        }
                        return CheckInputSchema(result, check, tools, toolName, "$." + string.Join('.', items.Skip(2)));
                    case "annotations":
                        if (items.Length == 2)
                        {
                            result.Error = $"tool: Expected format 'toolName.annotations.propertyName'";
                            return false;
                        }
                        return CheckAnnotations(result, check, tools, toolName, "$." + string.Join('.', items.Skip(2)));

                    case "meta":
                        if (items.Length == 2)
                        {
                            result.Error = $"tool: Expected format 'toolName.meta.propertyName'";
                            return false;
                        }
                        return CheckMeta(result, check, tools, toolName, "$." + string.Join('.', items.Skip(2)));

                    case "outputSchema":
                        if (items.Length == 2)
                        {
                            result.Error = $"tool: Expected format 'toolName.outputSchema.propertyName'";
                            return false;
                        }
                        return CheckOutputSchema(result, check, tools, toolName, "$." + string.Join('.', items.Skip(2)));

                    case "description":
                        if (items.Length != 2)
                        {
                            result.Error = $"tool: Expected format 'toolName.description'";
                            return false;
                        }
                        return CheckDescription(result, check, tools, toolName);
                    case "title":
                        if (items.Length != 2)
                        {
                            result.Error = $"tool: Expected format 'toolName.title'";
                            return false;
                        }
                        return CheckTitle(result, check, tools, toolName);

                    default:
                        result.Error = $"tool: Unsupported section '{section}' in tool name '{toolName}'";
                        return false;
                }
            }

            return CheckToolName(result, check, tools, toolName);
        }

        private bool CheckAnnotations(VerificationCheckResult result, VerificationCheck check, Tool[] tools, string toolName, string selector)
        {
            var tool = tools.Where(x => x.Name == toolName).FirstOrDefault();

            if (tool is null)
            {
                result.Error = $"The tool '{toolName}' was found";
                return false;
            }

            var json = JsonSerializer.Serialize(tool.InputSchema, ModelContextProtocol.McpJsonUtilities.DefaultOptions);
            var root = JObject.Parse(json);
            var token = root.SelectToken(selector);
            return JsonVerifier.VerifyJsonToken(result, check, selector, json, token);
        }
        private bool CheckTitle(VerificationCheckResult result, VerificationCheck check, Tool[] tools, string toolName)
        {
            var tool = tools.Where(x => x.Name == toolName).FirstOrDefault();

            if (tool is null)
            {
                result.Error = $"The tool '{toolName}' was found";
                return false;
            }

            return CompareValue(tool.Title, check.ExpectedValue, check.Operation);
        }
        private bool CheckDescription(VerificationCheckResult result, VerificationCheck check, Tool[] tools, string toolName)
        {
            var tool = tools.Where(x => x.Name == toolName).FirstOrDefault();

            if (tool is null)
            {
                result.Error = $"The tool '{toolName}' was found";
                return false;
            }

            return CompareValue(tool.Description, check.ExpectedValue, check.Operation);
        }
        private bool CheckMeta(VerificationCheckResult result, VerificationCheck check, Tool[] tools, string toolName, string selector)
        {
            var tool = tools.Where(x => x.Name == toolName).FirstOrDefault();

            if (tool is null)
            {
                result.Error = $"The tool '{toolName}' was found";
                return false;
            }

            var json = JsonSerializer.Serialize(tool.Meta, ModelContextProtocol.McpJsonUtilities.DefaultOptions);
            var root = JObject.Parse(json);
            var token = root.SelectToken(selector);
            return JsonVerifier.VerifyJsonToken(result, check, selector, json, token);
        }
        private bool CheckOutputSchema(VerificationCheckResult result, VerificationCheck check, Tool[] tools, string toolName, string selector)
        {
            var tool = tools.Where(x => x.Name == toolName).FirstOrDefault();

            if (tool is null)
            {
                result.Error = $"The tool '{toolName}' was found";
                return false;
            }

            var json = JsonSerializer.Serialize(tool.OutputSchema, ModelContextProtocol.McpJsonUtilities.DefaultOptions);
            var root = JObject.Parse(json);
            var token = root.SelectToken(selector);
            return JsonVerifier.VerifyJsonToken(result, check, selector, json, token);
        }
        private bool CheckInputSchema(VerificationCheckResult result, VerificationCheck check, Tool[] tools, string toolName, string selector)
        {
            var tool = tools.Where(x => x.Name == toolName).FirstOrDefault();

            if (tool is null)
            {
                result.Error = $"The tool '{toolName}' was found";
                return false;
            }

            var json = JsonSerializer.Serialize(tool.InputSchema, ModelContextProtocol.McpJsonUtilities.DefaultOptions);
            var root = JObject.Parse(json);
            var token = root.SelectToken(selector);
            return JsonVerifier.VerifyJsonToken(result, check, selector, json, token);
        }

        private static bool CheckToolName(VerificationCheckResult result, VerificationCheck check, Tool[] tools, string toolName)
        {
            var tool = tools.Where(x => x.Name == toolName).FirstOrDefault();
            if (check.Operation == VerificationOperation.NotExists)
            {
                if (tool is not null)
                {
                    result.Error = $"The tool '{toolName}' was found";
                    return false;
                }
                return true;
            }
            if (check.Operation == VerificationOperation.Exists)
            {
                var sampleTools = string.Join(",", tools.Select(x => x.Name).Take(3));
                if (tool is null)
                {
                    result.Error = $"The tool '{toolName}' was not found. {tools.Length} tools was found, including: {sampleTools}";
                    return false;
                }

                // Tool was found
                return true;
            }

            result.Error = $"The operation : {check.Operation} is not supported";
            return false;
        }
    }
}
