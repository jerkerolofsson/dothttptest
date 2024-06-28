using DotHttpTest.Models;
using DotHttpTest.Parser;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotHttpTest.Tests.Unit.ParserTests
{
    [TestCategory("UnitTests")]
    [TestClass]
    public class VariableLoaderStateTests
    {
        [TestMethod]
        public void ReplaceDataVariables_WithVariableInTheMiddle_VariableValueReplaced()
        {
            // Arrange
            var state = new VariableLoaderState(ClientOptions.DefaultOptions());
            state.ParseVariableAssignmentLine("@host=localhost");

            // Act
            var line = state.ReplaceDataVariables("GET http://{{host}}/index.html HTTP/1.1").ToString();

            // Assert
            Assert.AreEqual("GET http://localhost/index.html HTTP/1.1", line);
        }

        [TestMethod]
        public void ReplaceDataVariables_WithVariableInTheBeginning_VariableValueReplaced()
        {
            // Arrange
            var state = new VariableLoaderState(ClientOptions.DefaultOptions());
            state.ParseVariableAssignmentLine("@method=GET");

            // Act
            var line = state.ReplaceDataVariables("{{method}} http://localhost/index.html HTTP/1.1").ToString();

            // Assert
            Assert.AreEqual("GET http://localhost/index.html HTTP/1.1", line);
        }
        [TestMethod]
        public void ReplaceDataVariables_WithMultipleVariables_VariableValueReplaced()
        {
            // Arrange
            var state = new VariableLoaderState(ClientOptions.DefaultOptions());
            state.ParseVariableAssignmentLine("@method=GET");
            state.ParseVariableAssignmentLine("@url=http://localhost/index.html");

            // Act
            var line = state.ReplaceDataVariables("{{method}} {{url}} HTTP/1.1").ToString();

            // Assert
            Assert.AreEqual("GET http://localhost/index.html HTTP/1.1", line);
        }

        [TestMethod]
        public void ReplaceDataVariables_WithVariableInTheEnd_VariableValueReplaced()
        {
            // Arrange
            var state = new VariableLoaderState(ClientOptions.DefaultOptions());
            state.ParseVariableAssignmentLine("@version=HTTP/1.1");

            // Act
            var line = state.ReplaceDataVariables("GET http://localhost/index.html {{version}}").ToString();

            // Assert
            Assert.AreEqual("GET http://localhost/index.html HTTP/1.1", line);
        }

        [TestMethod]
        public void ParseVariableAssignmentLine_WithBasicVariable_CreatesVariable()
        {
            // Arrange
            var state = new VariableLoaderState(ClientOptions.DefaultOptions());

            // Act
            state.ParseVariableAssignmentLine("@key1=val1");

            // Assert
            Assert.AreEqual("val1", state.GetVariable("key1").ToString());
        }

        [TestMethod]
        public void ParseVariableAssignmentLine_WithSameKey_OverwritesVariable()
        {
            // Arrange
            var state = new VariableLoaderState(ClientOptions.DefaultOptions());

            // Act
            state.ParseVariableAssignmentLine("@key1=val1");
            state.ParseVariableAssignmentLine("@key1=val5");

            // Assert
            Assert.AreEqual("val5", state.GetVariable("key1").ToString());
        }
    }
}
