using DotHttpTest.Models;

namespace DotHttpTest.Tests.Integration
{
    /// <summary>
    /// Note, requires docker container "kennethreitz/httpbin:latest"
    /// 
    /// docker run kennethreitz/httpbin:latest
    /// 
    /// </summary>
    [TestCategory("IntegrationTests")]
    [TestClass]
    public class HttpBinTests
    {
        private static readonly ClientOptions mOptions = ClientOptions.DefaultOptions();
        [TestMethod]
        public async Task SendHttpFileAsync_WithHttpBinGetUrl_Returns200()
        {
            // Arrange
            var client = new DotHttpClient(mOptions);

            // Act
            await foreach(var response in client.SendHttpFileAsync("TestData/get_http_bin.http"))
            {
                // Assert
                response.Should().NotBeNull();
                response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
            }
        }

        [TestMethod]
        public async Task SendAsync_WithHttpBinGetUrl_Returns200()
        {
            using var client = new DotHttpClient();
            var requests = client.LoadFile("TestData/get_http_bin.http");
            var request = requests.First();

            var response = await client.SendAsync(request);

            // Assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        }
    }
}