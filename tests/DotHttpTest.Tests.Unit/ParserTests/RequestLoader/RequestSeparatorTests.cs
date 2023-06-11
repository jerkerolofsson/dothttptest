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
    public class RequestSeparatorTests
    {

        [TestMethod]
        public void DotHttpRequestLoader_WithTwoRequestsAndTrailingSeparator_TwoRequestIsReturned()
        {
            // Arrange
            // Act
            var requests = DotHttpRequestLoader.ParseRequests(File.ReadAllLines("TestData/Requests/Post/two_requests_with_content.http"));

            // Assert
            Assert.AreEqual(2, requests.Count);
        }
        [TestMethod]
        public void DotHttpRequestLoader_WithTwoRequestsInSingleFile_TwoRequestsAreReturned()
        {
            // Arrange
            // Act
            var requests = DotHttpRequestLoader.ParseRequests(File.ReadAllLines("TestData/Requests/Get/get_two_requests.http"));

            // Assert
            requests.Count.Should().Be(2);
        }

        [TestMethod]
        public void DotHttpRequestLoader_WithTwoRequestsInSingleFile_UrlCorrectInBothRequests()
        {
            // Arrange
            // Act
            var requests = DotHttpRequestLoader.ParseRequests(File.ReadAllLines("TestData/Requests/Get/get_two_requests.http"));

            // Assert
            requests.Count.Should().Be(2);
            requests[0].Url.Should().NotBeNull();
            requests[1].Url.Should().NotBeNull();
            requests[0].Url!.ToString().Should().Be("http://localhost/index.html");
            requests[1].Url!.ToString().Should().Be("http://localhost/other.html");
        }


        [TestMethod]
        public void DotHttpRequestLoader_WithSingleRequestFile_OneRequestIsReturned()
        {
            // Arrange
            // Act
            var requests = DotHttpRequestLoader.ParseRequests(File.ReadAllLines("TestData/Requests/Get/get_no_content.http"));

            // Assert
            Assert.AreEqual(1, requests.Count);
        }
    }
}
