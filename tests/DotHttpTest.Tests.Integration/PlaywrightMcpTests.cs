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
        private const string _server = "http://localhost:32768";

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

            // Act
            var testPlan = new TestPlanBuilder().Add(dotRequests).Build();

            var runnerOptions = new TestPlanRunnerOptions();
            var runner = new TestPlanRunner(testPlan, runnerOptions);

            var result = await runner.RunAsync();
            Assert.IsNotNull(result);
            if (result.FailedChecks.Count > 0)
            {
                var failures = string.Join(",", result.FailedChecks.Select(x => x.Error));
                Assert.Fail($"Failed checks: {failures}");
            }
            Assert.IsEmpty(result.FailedChecks);
            Assert.AreEqual(0, result.TestsFailed.Value);
        }


        [TestMethod]
        public async Task ListTools_VerifyExists()
        {
            // Arrange
            var request = $"""
                # @verify tool browser_close exists
                LIST {_server} MCP
                """;

            var dotRequests = DotHttpRequestLoader.ParseRequests(request);

            // Act
            var testPlan = new TestPlanBuilder().Add(dotRequests).Build();

            var runnerOptions = new TestPlanRunnerOptions();
            var runner = new TestPlanRunner(testPlan, runnerOptions);

            var result = await runner.RunAsync();
            Assert.IsNotNull(result);
            if(result.FailedChecks.Count > 0)
            {
                var failures = string.Join(",", result.FailedChecks.Select(x=>x.Error));
                Assert.Fail($"Failed checks: {failures}");
            }
            Assert.IsEmpty(result.FailedChecks);
            Assert.AreEqual(0, result.TestsFailed.Value);
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

            // Act
            var testPlan = new TestPlanBuilder().Add(dotRequests).Build();

            var runnerOptions = new TestPlanRunnerOptions();
            var runner = new TestPlanRunner(testPlan, runnerOptions);

            var result = await runner.RunAsync();
            Assert.IsNotNull(result);
            if (result.FailedChecks.Count > 0)
            {
                var failures = string.Join(",", result.FailedChecks.Select(x => x.Error));
                Assert.Fail($"Failed checks: {failures}");
            }
            Assert.IsEmpty(result.FailedChecks);
            Assert.AreEqual(0, result.TestsFailed.Value);
        }
    }
}
