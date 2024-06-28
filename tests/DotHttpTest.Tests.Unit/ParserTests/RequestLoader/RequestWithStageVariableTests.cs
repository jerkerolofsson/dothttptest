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
