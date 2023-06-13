using DotHttpTest.Models;
using DotHttpTest.Runner;
using DotHttpTest.Tests.Integration.Models;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace DotHttpTest.Tests.Integration
{
    /// <summary>
    /// Note, requires docker container "kennethreitz/httpbin:latest"
    /// 
    /// docker run -p 13080:80 kennethreitz/httpbin:latest
    /// 
    /// </summary>
    [TestCategory("IntegrationTests")]
    [TestClass]
    public class HttpBinTests
    {
        private static readonly ClientOptions mOptions = ClientOptions.DefaultOptions();

        private HttpClient CreateHttpClient()
        {
            var handler = new HttpClientHandler();
            handler.Proxy = null;
            handler.UseProxy = false;
            return new HttpClient(handler);
        }

        [TestMethod]
        public async Task SendAsync_WithHttpBinPostUrl_Returns200()
        {
            // Arrange
            using var client = new DotHttpClient(CreateHttpClient(), mOptions);
            var requests = client.LoadFile("TestData/post_http_bin.http");
            var request = requests.First();

            // Act
            var response = await client.SendAsync(request, null);

            // Assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        }

        [TestMethod]
        public async Task SendAsync_WithHttpBinPostUrl_ResponseBodyContainsEchoOfRequest()
        {
            // Arrange
            using var client = new DotHttpClient(CreateHttpClient(), mOptions);
            var requests = client.LoadFile("TestData/post_http_bin.http");
            var request = requests.First();

            // Act
            var response = await client.SendAsync(request, null);
            var body = await response.Content.ReadAsStringAsync();

            // Assert
            var binResponse = JsonConvert.DeserializeObject<HttpBinPostResponse>(body);
            binResponse.Should().NotBeNull();
            binResponse!.json.Should().NotBeNull();
            binResponse!.json!.ContainsKey("key1").Should().BeTrue();
            binResponse!.json!["key1"].Should().Be("val1");
        }

        [TestMethod]
        public async Task Run_WithJsonCheck_CheckPassed()
        {
            // Arrange
            var runner = new TestPlanRunnerOptionsBuilder()
                .LoadHttpFile("TestData/post_http_bin_with_json_check.http")
                .ConfigureClientOptions((client) =>
                {
                    client.WithHttpClientFactory(this.CreateHttpClient);
                })
                .Build();

            // Act
            var status = await runner.RunAsync();

            // Assert
            Assert.AreEqual(0, status.FailedChecks.Count);
        }


        [TestMethod]
        public async Task Run_WithJsonArrayLookup_CheckPassed()
        {
            // Arrange
            var runner = new TestPlanRunnerOptionsBuilder()
                .LoadHttpFile("TestData/post_with_json_array_lookup_in_requesturi.http")
                .ConfigureClientOptions((client) =>
                {
                    client.WithHttpClientFactory(this.CreateHttpClient);
                })
                .Build();

            // Act
            var status = await runner.RunAsync();

            // Assert
            Assert.AreEqual(0, status.FailedChecks.Count);
        }
        [TestMethod]
        public async Task Run_WithJsonVariableInRequestUri_CheckPassed()
        {
            // Arrange
            var runner = new TestPlanRunnerOptionsBuilder()
                .LoadHttpFile("TestData/get_with_json_variable_in_requesturi.http")
                .ConfigureClientOptions((client) =>
                {
                    client.WithHttpClientFactory(this.CreateHttpClient);
                })
                .Build();

            // Act
            var status = await runner.RunAsync();

            // Assert
            Assert.AreEqual(0, status.FailedChecks.Count);
        }

        [TestMethod]
        public async Task Run_WithJsonNotEqualsCheck_CheckPassed()
        {
            // Arrange
            var runner = new TestPlanRunnerOptionsBuilder()
                .LoadHttpFile("TestData/post_http_bin_with_json_not_eq.http")
                .ConfigureClientOptions((client) =>
                {
                    client.WithHttpClientFactory(this.CreateHttpClient);
                })
                .Build();

            // Act
            var status = await runner.RunAsync();

            // Assert
            Assert.AreEqual(0, status.FailedChecks.Count);
        }
        [TestMethod]
        public async Task Run_WithJsonVariable_ForwardedFromResponseToNextRequest()
        {
            // Arrange
            var runner = new TestPlanRunnerOptionsBuilder()
                .LoadHttpFile("TestData/post_http_bin_with_late_bound_json_variable.http")
                .ConfigureClientOptions((client) =>
                {
                    client.WithHttpClientFactory(this.CreateHttpClient);
                })
                .Build();

            // Act
            var status = await runner.RunAsync();

            // Assert
            Assert.AreEqual(0, status.FailedChecks.Count);
        }

        [TestMethod]
        public async Task SendAsync_WithHttpBinGetUrl_Returns200()
        {
            // Arrange
            using var client = new DotHttpClient(CreateHttpClient(), mOptions);
            var requests = client.LoadFile("TestData/get_http_bin.http");
            var request = requests.First();

            // Act
            var response = await client.SendAsync(request, null);

            // Assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        }
    }
}