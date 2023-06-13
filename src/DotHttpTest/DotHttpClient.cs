using DotHttpTest.Converters;
using DotHttpTest.Parser;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DotHttpTest
{
    public class DotHttpClient : IDisposable
    {
        private readonly HttpClient mClient;
        private readonly ClientOptions mOptions;
        private VerifierFactory? mVerifierFactory;

        public DotHttpClient(ClientOptions? options = null)
        {
            mOptions = options ?? ClientOptions.DefaultOptions();
            mClient = mOptions.CreateHttpClient();
            ConfigureHttpClient(mClient, (client) =>
            {
                ConfigureHttpClientWithOptions(client);
            });
        }
        public DotHttpClient(HttpClient httpClient, ClientOptions? options = null)
        {
            mClient = httpClient;
            mOptions = options ?? ClientOptions.DefaultOptions();
            ConfigureHttpClient(mClient, (client) =>
            {
                ConfigureHttpClientWithOptions(client);
            });
        }
        public DotHttpClient(IHttpClientFactory httpClientFactory, string clientName, ClientOptions? options = null)
        {
            mClient = httpClientFactory.CreateClient(clientName);
            mOptions = options ?? ClientOptions.DefaultOptions();
            ConfigureHttpClient(mClient, (client) =>
            {
                ConfigureHttpClientWithOptions(client);
            });
        }

        private void ConfigureHttpClientWithOptions(HttpClient httpClient)
        {
            httpClient.Timeout = mOptions.Request.Timeout;
        }

        private void ConfigureHttpClient(HttpClient httpClient, Action<HttpClient> clientBuilder)
        {
            mVerifierFactory = new VerifierFactory(mOptions);
            clientBuilder(httpClient);
        }

        public List<DotHttpRequest> LoadFile(string filename)
        {
            return DotHttpRequest.FromFile(filename);
        }

        public async Task<DotHttpResponse> SendAsync(DotHttpRequest request, TestStatus? status, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var httpRequestMessage = request.ToHttpRequestMessage(status);

            // Create new HTTP metrics
            var metrics = new HttpRequestMetrics()
            {
                RequestUri = httpRequestMessage.RequestUri,
                Created = DateTime.UtcNow,
                Request = request
            };
            var stopwatch = Stopwatch.StartNew();

            // Send the request
            var httpResponse = await mClient.SendAsync(httpRequestMessage, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
            var sendingElapsed = stopwatch.Elapsed.TotalSeconds;
            metrics.HttpRequestSending.SetValue(sendingElapsed);
            metrics.StatusCode = httpResponse.StatusCode;

            var response = new DotHttpResponse(httpResponse, metrics);
            response.RequestUri = httpRequestMessage.RequestUri;
            response.Request = request;
            if (mOptions.Request.ReadContent)
            {
                var bytes = await httpResponse.Content.ReadAsByteArrayAsync();
                var elapsed = stopwatch.Elapsed.TotalSeconds;

                metrics.HttpRequestReceiving.SetValue(elapsed - sendingElapsed);
                metrics.HttpRequestDuration.SetValue(elapsed);
                response.ContentBytes = bytes;
            }

            if (mVerifierFactory != null)
            {
                mVerifierFactory.Verify(response);
            }

            return response;
        }

        public void Dispose()
        {
            mClient.Dispose();
        }
    }
}
