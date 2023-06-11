using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotHttpTest.Tests.Unit.ParserTests.RequestLoader
{
    [TestCategory("UnitTests")]
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
            var port = request.Url!.Port;
            Assert.IsTrue(port >= 0);
            Assert.IsTrue(port <= 1000);
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
            var uuid = request.Url!.AbsolutePath.Trim('/');
            var success = Guid.TryParse(uuid, out Guid _);
            Assert.IsTrue(success, "Failed to parse $uuid variable into Guid");
        }
    }
}
