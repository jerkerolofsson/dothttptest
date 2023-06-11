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
    public class MetadataTests
    {
        [TestMethod]
        public void DotHttpRequestLoader_WithIntellijName_RequestNameIsSet()
        {
            // Arrange

            // Act
            var requests = DotHttpRequestLoader.ParseRequests(File.ReadAllLines("TestData/Requests/Get/get_with_intellij_name.http"));

            // Assert
            Assert.AreEqual(1, requests.Count);
            Assert.IsNotNull(requests[0].RequestName);
            Assert.AreEqual("GetIndex", requests[0].RequestName);
        }

    }
}
