namespace DotHttpTest.Tests.Unit.ParserTests.RequestLoader
{
    [UnitTest]

    [TestClass]
    public class VariableTests
    {
        [TestMethod]
        public void DotHttpRequestLoader_WithVariableInVariable_VariableReplacementIsCorrect()
        {
            // Arrange
            // Act
            var request = DotHttpRequestLoader.ParseRequest(File.ReadAllLines("TestData/Requests/Get/get_with_variable_in_variable.http"));

            // Assert
            request.Should().NotBeNull().And.HaveRequestUri("https://httpbin.org/get");   
        }

        [TestMethod]
        public void DotHttpRequestLoader_WithVariableInUrl_UrlIsCorrect()
        {
            // Arrange
            // Act
            var request = DotHttpRequestLoader.ParseRequest(File.ReadAllLines("TestData/Requests/Get/get_with_variable_assignment.http"));

            // Assert
            request.Should().NotBeNull().And.HaveRequestUri("https://httpbin.org/get");
        }
    }
}
