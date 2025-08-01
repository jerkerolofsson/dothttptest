using ModelContextProtocol.Client;
using ModelContextProtocol.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DotHttpTest
{
    public partial class DotHttpClient
    {
        private async Task<DotHttpResponse> SendMcpAsync(DotHttpRequest request, TestStatus? status, StageWorkerState? stageWorkerState, CancellationToken cancellationToken)
        {
            if (request.Method is null)
            {
                throw new ArgumentNullException("Method cannot be null");
            }

            var method = request.Method.ToString(status, stageWorkerState);
            switch (method)
            {
                case "INVOKE":
                    return await InvokeMcpAsync(request, status, stageWorkerState, cancellationToken);
                case "LIST":
                    return await ListMcpAsync(request, status, stageWorkerState, cancellationToken);
                default:
                    throw new Exception($"Unknown method: {method}");
            }
        }

        private async Task<DotHttpResponse> InvokeMcpAsync(DotHttpRequest request, TestStatus? status, StageWorkerState? stageWorkerState, CancellationToken cancellationToken)
        {
            if (request.Url is null)
            {
                throw new ArgumentNullException("Method cannot be null");
            }
            var endpoint = request.Url.ToString(status, stageWorkerState);

            if (!endpoint.Contains('#'))
            {
                throw new ArgumentException("Expected tool name to be specified in request URL, e.g. http://mcpendpoint/mcp#name_of_tool");
            }
            var endpointComponents = endpoint.Split('#');
            var url = endpointComponents[0];
            var tool = endpointComponents[1];

            throw new NotImplementedException();
        }

        private async Task<DotHttpResponse> ListMcpAsync(DotHttpRequest request, TestStatus? status, StageWorkerState? stageWorkerState, CancellationToken cancellationToken)
        {
            IMcpClient client = await CreateMcpClientAsync(request, status, stageWorkerState);
            var metrics = new HttpRequestMetrics();

            // Print the list of tools available from the server.
            List<Tool> tools = [];
            foreach (var tool in await client.ListToolsAsync())
            {
                tools.Add(tool.ProtocolTool);
            }
            string json = JsonSerializer.Serialize(tools, ModelContextProtocol.McpJsonUtilities.DefaultOptions);

            var httpResponse = new HttpResponseMessage()
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };

            return new DotHttpResponse(httpResponse, metrics)
            {
                ContentBytes = Encoding.UTF8.GetBytes(json),
                Request = request
            };
        }

        private async Task<IMcpClient> CreateMcpClientAsync(DotHttpRequest request, TestStatus? status, StageWorkerState? stageWorkerState)
        {
            if (request.Url is null)
            {
                throw new ArgumentNullException("Method cannot be null");
            }
            var endpoint = request.Url.ToString(status, stageWorkerState);
            var clientTransport = new SseClientTransport(new SseClientTransportOptions
            {
                Endpoint = new Uri(endpoint)
            });

            var client = await McpClientFactory.CreateAsync(clientTransport);
            return client;
        }
    }
}
