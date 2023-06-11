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
    public class RequestMethodTests
    {
        [TestMethod]
        public void DotHttpRequestLoader_WithGetRequestWithoutVersion_RequestMethodIsGet()
        {
            // Arrange
            // Act
            var request = DotHttpRequestLoader.ParseRequest(File.ReadAllLines("TestData/Requests/Get/get_no_http_version.http"));

            // Assert
            request.Should().NotBeNull().And.HaveMethod(HttpMethod.Get);
        }

        [TestMethod]
        public void DotHttpRequestLoader_WithGetRequest_RequestMethodIsGet()
        {
            // Arrange
            // Act
            var request = DotHttpRequestLoader.ParseRequest(File.ReadAllLines("TestData/Requests/Get/get_no_content.http"));

            // Assert
            request.Should().NotBeNull().And.HaveMethod(HttpMethod.Get);
        }

        [TestMethod]
        public void DotHttpRequestLoader_WithPostRequest_RequestMethodIsPost()
        {
            // Arrange
            // Act
            var request = DotHttpRequestLoader.ParseRequest(File.ReadAllLines("TestData/Requests/Post/post_no_content.http"));

            // Assert
            request.Should().NotBeNull().And.HaveMethod(HttpMethod.Post);
        }

    }
}
