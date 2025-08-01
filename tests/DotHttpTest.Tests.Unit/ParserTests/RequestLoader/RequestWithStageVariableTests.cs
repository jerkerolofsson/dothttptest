namespace DotHttpTest.Tests.Unit.ParserTests.RequestLoader
{
    [UnitTest]

    [TestClass]
    public class RequestWithStageVariableTests
    {
        [TestMethod]
        public void DotHttpRequestLoader_WithGetRequestWithoutVersion_RequestUriIsCorrect()
        {
            // Arrange
            // Act
            var request = DotHttpRequestLoader.ParseRequest(File.ReadAllLines("TestData/Requests/Get/get_with_stage_variable.http"));

            // Assert
            request.Should().NotBeNull();
        }
    }
}
