namespace DotHttpTest.Tests.Unit.ParserTests.RequestLoader
{
    [UnitTest]
    [TestClass]
    public class DynamicVariableTests
    {
        [TestMethod]
        public void DotHttpRequestLoader_WithRandomIntDynamicVariable_ReplacedWithValue0To1000()
        {
            // Arrange
            // Act
            var request = DotHttpRequestLoader.ParseRequest(File.ReadAllLines("TestData/Requests/Get/get_with_randomInt_variable.http"));

            // Assert
            request.Should().NotBeNull();
            request.Url.Should().NotBeNull();
            var uri = new Uri(request.Url!.ToString());
            var port = uri.Port;
            Assert.IsGreaterThanOrEqualTo(0, port);
            Assert.IsLessThanOrEqualTo(1000, port);
        }

        [TestMethod]
        public void DotHttpRequestLoader_WithUuidDynamicVariable_ReplacedWithDynamicVariant()
        {
            // Arrange
            // Act
            var request = DotHttpRequestLoader.ParseRequest(File.ReadAllLines("TestData/Requests/Get/get_with_uuid_variable.http"));

            // Assert
            request.Should().NotBeNull();
            request.Url.Should().NotBeNull();
            var uri = new Uri(request.Url!.ToString());
            var uuid = uri!.AbsolutePath.Trim('/');
            var success = Guid.TryParse(uuid, out Guid _);
            Assert.IsTrue(success, "Failed to parse $uuid variable into Guid");
        }
    }
}
