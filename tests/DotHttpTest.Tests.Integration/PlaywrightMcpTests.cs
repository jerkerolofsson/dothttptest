using DotHttpTest.Runner;
using TestBucket.Traits.MSTest;

namespace DotHttpTest.Tests.Integration
{
    /// <summary>
    /// Tests using playwright mcp server
    /// </summary>
    [TestClass]
    [IntegrationTest]
    public class PlaywrightMcpTests
    {
        private const string _server = "http://localhost:33323/sse";

        [TestMethod]
        public async Task CallTool_WithHeader()
        {
            // Arrange
            var request = $$"""
                CALL {{_server}}#browser_navigate MCP
                X-API-KEY: 1234567890

                {
                    "url": "https://www.github.com"
                }
                """;

            var dotRequests = DotHttpRequestLoader.ParseRequests(request);

            // Act
            var testPlan = new TestPlanBuilder().Add(dotRequests).Build();

            var runnerOptions = new TestPlanRunnerOptions();
            var runner = new TestPlanRunner(testPlan, runnerOptions);

            var result = await runner.RunAsync();
            AssertNoFailure(result);
        }

        [TestMethod]
        public async Task CallTool_VerifyTextContent()
        {
            // Arrange
            var request = $$"""
                # @verify mcp success
                # @verify mcp textContent contains ### Ran Playwright code
                CALL {{_server}}#browser_navigate MCP

                {
                    "url": "https://www.github.com"
                }
                """;

            var dotRequests = DotHttpRequestLoader.ParseRequests(request);

            // Act
            var testPlan = new TestPlanBuilder().Add(dotRequests).Build();

            var runnerOptions = new TestPlanRunnerOptions();
            var runner = new TestPlanRunner(testPlan, runnerOptions);

            var result = await runner.RunAsync();
            AssertNoFailure(result);
        }
        [TestMethod]
        public async Task CallTool_WithParameters()
        {
            // Arrange
            var request = $$"""
                CALL {{_server}}#browser_navigate MCP

                {
                    "url": "https://www.github.com"
                }
                """;

            var dotRequests = DotHttpRequestLoader.ParseRequests(request);

            // Act
            var testPlan = new TestPlanBuilder().Add(dotRequests).Build();

            var runnerOptions = new TestPlanRunnerOptions();
            var runner = new TestPlanRunner(testPlan, runnerOptions);

            var result = await runner.RunAsync();
            AssertNoFailure(result);
        }

        private static void AssertNoFailure(Runner.Models.TestStatus result)
        {
            Assert.IsNotNull(result);
            if (result.FailedChecks.Count > 0)
            {
                var failures = string.Join(",", result.FailedChecks.Select(x => x.Error));
                Assert.Fail($"{result.FailedChecks.Count} failed checks: {failures}");
            }
            Assert.IsEmpty(result.FailedChecks);
            Assert.AreEqual(0, result.TestsFailed.Value);
        }

        [TestMethod]
        public async Task ListTools_VerifyParameterExists()
        {
            // Arrange
            var request = $"""
                # @verify tool browser_navigate.inputSchema.properties.url exists
                # @verify tool browser_navigate.inputSchema.properties.url.type == string
                LIST {_server} MCP
                """;

            var dotRequests = DotHttpRequestLoader.ParseRequests(request);
            var testPlan = new TestPlanBuilder().Add(dotRequests).Build();
            var runnerOptions = new TestPlanRunnerOptions();
            var runner = new TestPlanRunner(testPlan, runnerOptions);

            // Act
            var result = await runner.RunAsync();

            // Assert
            AssertNoFailure(result);
        }


        [TestMethod]
        public async Task ListTools_VerifyExists()
        {
            // Arrange
            var request = $"""
                # @verify tool browser_navigate exists
                LIST {_server} MCP
                """;

            var dotRequests = DotHttpRequestLoader.ParseRequests(request);
            var testPlan = new TestPlanBuilder().Add(dotRequests).Build();

            var runnerOptions = new TestPlanRunnerOptions();
            var runner = new TestPlanRunner(testPlan, runnerOptions);

            // Act
            var result = await runner.RunAsync();

            // Assert
            AssertNoFailure(result);

        }

        [TestMethod]
        public async Task ListToolsNot_VerifyExists()
        {
            // Arrange
            var request = $"""
                # @verify tool feed_the_dog not exists
                LIST {_server} MCP
                """;

            var dotRequests = DotHttpRequestLoader.ParseRequests(request);
            var testPlan = new TestPlanBuilder().Add(dotRequests).Build();

            var runnerOptions = new TestPlanRunnerOptions();
            var runner = new TestPlanRunner(testPlan, runnerOptions);

            // Act
            var result = await runner.RunAsync();

            // Assert
            AssertNoFailure(result);
        }
    }
}
