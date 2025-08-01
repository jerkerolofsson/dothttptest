using DotHttpTest.Runner;

namespace DotHttpTest.Tests.Unit.ParserTests
{
    [TestClass]
    [UnitTest]
    public class RequestProtocolDetectorTests
    {
        [TestMethod]
        [DataRow("MCP")]
        public void Detect_WithMcp_ProtocolIsMcp(string version)
        {
            // Arrange
            string mcpText = $"""
                INVOKE http://localhost:32768 {version}
                """;
            var dotRequest = DotHttpRequestLoader.ParseRequest(mcpText);

            // Act
            var protocol = RequestProtocolDetector.Detect(dotRequest, null, null);

            // Assert
            protocol.Should().Be(Models.RequestProtocol.Mcp);
        }

        [TestMethod]
        [DataRow("HTTP/1.1")]
        [DataRow("HTTP/1.0")]
        [DataRow("HTTP")]
        [DataRow("")]
        [DataRow(null)]
        public void Detect_WithEmpty_ProtocolIsHttp(string? version)
        {
            // Arrange
            string requestText = $"""
                GET http://localhost:32768 {version}
                """;
            var dotRequest = DotHttpRequestLoader.ParseRequest(requestText);

            // Act
            var protocol = RequestProtocolDetector.Detect(dotRequest, null, null);

            // Assert
            protocol.Should().Be(Models.RequestProtocol.Http);
        }
    }
}
