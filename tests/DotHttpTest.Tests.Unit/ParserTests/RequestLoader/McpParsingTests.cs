using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotHttpTest.Tests.Unit.ParserTests.RequestLoader
{
    [UnitTest]
    [TestClass]
    public class McpParsingTests
    {
        [TestMethod]
        public void DotHttpRequestLoader_WithMcpListRequest_MethodCorrect()
        {
            string mcpText = """
                INVOKE http://localhost:32768 MCP
                """;

            // Arrange
            // Act
            var dotRequest = DotHttpRequestLoader.ParseRequest(mcpText);

            // Assert
            dotRequest.Should().NotBeNull();
            Assert.IsNotNull(dotRequest.Method);
            dotRequest.Method.ToString(null, null).Should().Be("INVOKE");
        }
    }
}
