namespace DotHttpTest.Tests.Unit.ParserTests.RequestLoader
{
    [UnitTest]
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
            request.Url.Should().NotBeNull();
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
            request.Url.Should().NotBeNull();
            request.Url.ToString().Should().Be("http://localhost/index.html");
        }
    }
}
