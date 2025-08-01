using DotHttpTest.Parser;
using DotHttpTest.Runner.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DotHttpTest.Converters;

namespace DotHttpTest.Tests.Unit.ParserTests.RequestLoader
{
    [UnitTest]
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
        public void DotHttpRequestLoader_WithHeaderWithSpaceInValue_HeaderIsSetInRequestObject()
        {
            // Arrange
            // Act
            var dotRequest = DotHttpRequestLoader.ParseRequest(File.ReadAllLines("TestData/Requests/Get/get_with_header_with_space_in_value.http"));

            // Assert
            dotRequest.Should().NotBeNull().And.ContainHeaderWithValue("x-test", "Hello World");
        }


        [TestMethod]
        public void ToHttpRequestMessage_WithHeaderWithSpaceInValue_HeaderIsSetInRequestObject()
        {
            // Arrange
            // Act
            var dotRequest = DotHttpRequestLoader.ParseRequest(File.ReadAllLines("TestData/Requests/Get/get_with_header_with_space_in_value.http"));

            // Assert
            var httpRequestMessage = dotRequest.ToHttpRequestMessage(null, null);
            httpRequestMessage.Should().NotBeNull();
            httpRequestMessage.Headers.Should().NotBeNullOrEmpty();
            httpRequestMessage.Headers.Count().Should().Be(1);
            httpRequestMessage.Headers.First().Key.Should().Be("x-test");

            var values = httpRequestMessage.Headers.First().Value.ToList();
            values.Count.Should().Be(1);
            values[0].Should().Be("Hello World");
        }

        [TestMethod]
        public void ToDto_WithHeaderWithSpaceIOnValue_HeaderIsSetInRequestObject()
        {
            // Arrange
            // Act
            var dotRequest = DotHttpRequestLoader.ParseRequest(File.ReadAllLines("TestData/Requests/Get/get_with_header_with_space_in_value.http"));

            // Assert
            var dto = dotRequest.ToHttpRequestMessage(null, null).ToDto();
            dto.Should().NotBeNull();
            dto.Headers.Should().NotBeNullOrEmpty();
            dto.Headers.Count().Should().Be(1);
            dto.Headers.First().Key.Should().Be("x-test");

            var values = dto.Headers.First().Value.ToList();
            values.Count.Should().Be(1);
            values[0].Should().Be("Hello World");
        }

        [TestMethod]
        public void HaveContentMediaType_ThrowsExceptionWhenWrongMediaType()
        {
            // Arrange
            // Act
            var dotRequest = DotHttpRequestLoader.ParseRequest(File.ReadAllLines("TestData/Requests/Post/post_with_content_type_header.http"));

            // Assert
            Assert.Throws<FluentAssertions.Execution.AssertionFailedException>(() =>
            {
                dotRequest.Should().NotBeNull().And.HaveContentMediaType("text/plain");
            });
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
