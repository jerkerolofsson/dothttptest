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
    public class HttpHeaderTests
    {
        [TestMethod]
        public void DotHttpRequestLoader_WithAcceptHeader_HeaderIsSetInRequestObject()
        {
            // Arrange
            // Act
            var dotRequest = DotHttpRequestLoader.ParseRequest(File.ReadAllLines("TestData/Requests/Get/get_with_accept_header.http"));

            // Assert
            dotRequest.Should().NotBeNull().And.ContainHeaderWithValue("Accept", "application/json");

        }

        [TestMethod]
        public void DotHttpRequestLoader_WithContentType_ContentMediaTypeSetInRequestObject()
        {
            // Arrange
            // Act
            var dotRequest = DotHttpRequestLoader.ParseRequest(File.ReadAllLines("TestData/Requests/Post/post_with_content_type_header.http"));

            // Assert
            dotRequest.Should().NotBeNull().And.HaveContentMediaType("application/json");
        }
    }
}
