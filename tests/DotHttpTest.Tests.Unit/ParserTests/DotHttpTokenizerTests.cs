using DotHttpTest.Parser;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotHttpTest.Tests.Unit.TokenizerTests
{
    [UnitTest]
    [TestClass]
    public class DotHttpTokenizerTests
    {
        public TestContext? TestContext { get; set; }

        [TestMethod]
        public void ParseRequestHeader_WithGet_UrlIsCorrect()
        {
            // Arrange

            // Act
            var tokens = DotHttpTokenizer.ParseRequestHeader(0, "GET http://localhost/index.html HTTP/1.1");

            // Assert
            var token = tokens.Where(x => x.Type == Parser.Models.HttpTokenType.Url).FirstOrDefault();
            Assert.IsNotNull(token);
            Assert.AreEqual("http://localhost/index.html", token.Data);
        }

        [TestMethod]
        public void ParseRequestHeader_WithGet_VersionIsCorrect()
        {
            // Arrange

            // Act
            var tokens = DotHttpTokenizer.ParseRequestHeader(0, "GET http://localhost/index.html HTTP/1.1");

            // Assert
            var token = tokens.Where(x => x.Type == Parser.Models.HttpTokenType.HttpVersion).FirstOrDefault();
            Assert.IsNotNull(token);
            Assert.AreEqual("HTTP/1.1", token.Data);
        }

        [TestMethod]
        public void ParseRequestHeader_WithGet_HttpMethodIsGet()
        {
            // Arrange

            // Act
            var tokens = DotHttpTokenizer.ParseRequestHeader(0, "GET http://localhost/index.html HTTP/1.1");

            // Assert
            var token = tokens.Where(x => x.Type == Parser.Models.HttpTokenType.Method).FirstOrDefault();
            Assert.IsNotNull(token);
            Assert.AreEqual("GET", token.Data);
        }

        [TestMethod]
        public void ParseRequestHeader_WithPost_HttpMethodIsPost()
        {
            // Arrange

            // Act
            var tokens = DotHttpTokenizer.ParseRequestHeader(0, "POST http://localhost/index.html HTTP/1.1");

            // Assert
            var token = tokens.Where(x => x.Type == Parser.Models.HttpTokenType.Method).FirstOrDefault();
            Assert.IsNotNull(token);
            Assert.AreEqual("POST", token.Data);
        }

        [TestMethod]
        public void IsVariableAssignment_WithHostVariable_ReturnsTrue()
        {
            // Arrange

            // Act
            var isVariableAssignment = DotHttpTokenizer.IsVariableAssignment("@hostname=localhost");

            // Assert
            Assert.IsTrue(isVariableAssignment);
        }

        [TestMethod]
        public void IsVariableAssignment_CommentLine_ReturnsFalse()
        {
            // Arrange

            // Act
            var isVariableAssignment = DotHttpTokenizer.IsVariableAssignment("# @hostname=localhost");

            // Assert
            Assert.IsFalse(isVariableAssignment);
        }

        [TestMethod]
        public void IsLineComment_WithLineSeparator_ReturnsFalse()
        {
            // Arrange

            // Act
            var isLineComment = DotHttpTokenizer.IsLineComment("###");

            // Assert
            Assert.IsFalse(isLineComment);
        }

        [TestMethod]
        public void IsLineComment_WithLeadingHash_ReturnsTrue()
        {
            // Arrange

            // Act
            var isLineComment = DotHttpTokenizer.IsLineComment("# Comment");

            // Assert
            Assert.IsTrue(isLineComment);
        }

        [TestMethod]
        public void IsLineComment_WithLeadingSlashes_ReturnsTrue()
        {
            // Arrange

            // Act
            var isLineComment = DotHttpTokenizer.IsLineComment("// Comment");

            // Assert
            Assert.IsTrue(isLineComment);
        }
    }
}
