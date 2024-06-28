using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotHttpTest.Tests.Unit.ParserTests.RequestLoader
{
    [TestCategory("UnitTests")]
    [TestClass]
    public class BodyTests
    {
        [TestMethod]
        public void DotHttpRequestLoader_WithBody_BodyBytesAreRead()
        {
            // Arrange
            // Act
            var dotRequest = DotHttpRequestLoader.ParseRequest(File.ReadAllLines("TestData/Requests/Post/post_with_content.http"));

            // Assert
            dotRequest.Should().NotBeNull();
            dotRequest.Body.Should().NotBeNull();

            var bytes = dotRequest.Body.ToByteArray(Encoding.UTF8, null, null);

            bytes.Should().NotBeEmpty();
            bytes.Length.Should().Be(5);
        }

        [TestMethod]
        public void DotHttpRequestLoader_WithTwoRequests_BodyBytesAreReadCorrectly()
        {
            // Arrange
            // Act
            var dotRequests = DotHttpRequestLoader.ParseRequests(File.ReadAllLines("TestData/Requests/Post/two_requests_with_content.http"));

            // Assert
            dotRequests.Count.Should().Be(2);

            var text1 = dotRequests[0].GetBodyAsText();
            var text2 = dotRequests[1].GetBodyAsText();
            text1.Should().StartWith("hello");
            text2.Should().StartWith("world");
        }
    }
}
