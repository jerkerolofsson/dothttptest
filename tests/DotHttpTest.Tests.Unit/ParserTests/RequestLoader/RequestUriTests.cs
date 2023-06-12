using DotHttpTest.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotHttpTest.Tests.Unit.ParserTests.RequestLoader
{
    [TestCategory("UnitTests")]
    [TestClass]
    public class RequestUriTests
    {
        [TestMethod]
        public void DotHttpRequestLoader_WithGetRequestWithoutVersion_RequestUriIsCorrect()
        {
            // Arrange
            // Act
            var request = DotHttpRequestLoader.ParseRequest(File.ReadAllLines("TestData/Requests/Get/get_no_http_version.http"));

            // Assert
            request.Should().NotBeNull();
            request.Url.ToString().Should().Be("http://localhost/index.html");
        }

        [TestMethod]
        public void DotHttpRequestLoader_WithGetRequestWithVersion_RequestUriIsCorrect()
        {
            // Arrange
            // Act
            var request = DotHttpRequestLoader.ParseRequest(File.ReadAllLines("TestData/Requests/Get/get_no_content.http"));

            // Assert
            request.Should().NotBeNull();
            request.Url!.ToString().Should().Be("http://localhost/index.html");
        }
    }
}
