namespace DotHttpTest.Tests.Unit.ParserTests.RequestLoader
{
    [UnitTest]
    [TestClass]
    public class MetadataTests
    {
        [TestMethod]
        public void DotHttpRequestLoader_WithIntellijName_RequestNameIsSet()
        {
            // Arrange

            // Act
            var requests = DotHttpRequestLoader.ParseRequests(File.ReadAllLines("TestData/Requests/Get/get_with_intellij_name.http"));

            // Assert
            Assert.HasCount(1, requests);
            Assert.IsNotNull(requests[0].RequestName);
            Assert.AreEqual("GetIndex", requests[0].RequestName);
        }

    }
}
