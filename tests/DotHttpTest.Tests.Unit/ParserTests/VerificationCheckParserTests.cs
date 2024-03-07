using DotHttpTest.Verification.Models;
using DotHttpTest.Verification.Parser;
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
    public class VerificationCheckParserTests
    {
        [TestMethod]
        public void ParseVerificationCheck_WithoutOperator_DefaultsToEquals()
        {
            // Act
            var check = VerificationCheckParser.Parse("http status-code 200");

            // Assert
            Assert.AreEqual("http", check.VerifierId);
            Assert.AreEqual(VerificationOperation.Equals, check.Operation);
        }

        [TestMethod]
        public void ParseVerificationCheck_WithoutGreaterThan_OperatorIsSet()
        {
            // Act
            var check = VerificationCheckParser.Parse("http status-code > 500");

            // Assert
            Assert.AreEqual(VerificationOperation.Greater, check.Operation);
        }

        [TestMethod]
        public void ParseVerificationCheck_WithNotExistsOperator_OperatorIsSet()
        {
            // Act
            var check = VerificationCheckParser.Parse("headers cookie not exists");

            // Assert
            Assert.AreEqual(VerificationOperation.NotExists, check.Operation);
        }
        [TestMethod]
        public void ParseVerificationCheck_WithoutGreaterOrEquals_OperatorIsSet()
        {
            // Act
            var check = VerificationCheckParser.Parse("http status-code >= 500");

            // Assert
            Assert.AreEqual(VerificationOperation.GreaterOrEquals, check.Operation);
        }

        [TestMethod]
        public void ParseVerificationCheck_WithoutExpectedValue_OperatorIsSet()
        {
            // Act
            var check = VerificationCheckParser.Parse("http headers[content-type] exists");

            // Assert
            Assert.AreEqual(VerificationOperation.Exists, check.Operation);
        }

        [TestMethod]
        public void ParseVerificationCheck_WithoutOperator_ExpectedValueParsedCorrectly()
        {
            // Act
            var check = VerificationCheckParser.Parse("http status-code 200");

            // Assert
            Assert.AreEqual("200", check.ExpectedValue);
        }
        [TestMethod]
        public void ParseVerificationCheck_WithEqualsOperatorAndSpaces_OperatorParsedCorrectly()
        {
            // Act
            var check = VerificationCheckParser.Parse("http status-code == 200");

            // Assert
            Assert.AreEqual(VerificationOperation.Equals, check.Operation);
        }

        [TestMethod]
        public void ParseVerificationCheck_WithEqualsOperatorWithoutSpaces_OperatorParsedCorrectly()
        {
            // Act
            var check = VerificationCheckParser.Parse("http status-code==200");

            // Assert
            Assert.AreEqual(VerificationOperation.Equals, check.Operation);
        }

        [TestMethod]
        public void ParseVerificationCheck_WithIsOperator_OperatorParsedCorrectly()
        {
            // Act
            var check = VerificationCheckParser.Parse("http status-code is 200");

            // Assert
            Assert.AreEqual(VerificationOperation.Equals, check.Operation);
        }

        [TestMethod]
        public void ParseVerificationCheck_WithIsOperator_ExpectedValueParsedCorrectly()
        {
            // Act
            var check = VerificationCheckParser.Parse("http status-code is 200");

            // Assert
            Assert.AreEqual("200", check.ExpectedValue);
        }

        [TestMethod]
        public void ParseVerificationCheck_WithRegexOperator_ExpectedValueParsedCorrectly()
        {
            // Act
            var check = VerificationCheckParser.Parse("http status-code regex (200|202)");

            // Assert
            Assert.AreEqual("(200|202)", check.ExpectedValue);
        }

        [TestMethod]
        public void ParseVerificationCheck_WithRegexOperator_OperatorParsedCorrectly()
        {
            // Act
            var check = VerificationCheckParser.Parse("http status-code regex (200|202)");

            // Assert
            Assert.AreEqual(VerificationOperation.RegexMatch, check.Operation);
        }


        [TestMethod]
        public void ParseVerificationCheck_WithJsonCheckWithoutOperator_OperatorIsEquals()
        {
            // Act
            var check = VerificationCheckParser.Parse("json $.Count 1");

            // Assert
            Assert.AreEqual("1", check.ExpectedValue);
            Assert.AreEqual("json", check.VerifierId);
            Assert.AreEqual(VerificationOperation.Equals, check.Operation);
        }

        [TestMethod]
        public void ParseVerificationCheck_WithJsonCheckWithOperator_OperatorIsEquals()
        {
            // Act
            var check = VerificationCheckParser.Parse("json $.Count == 1");

            // Assert
            Assert.AreEqual("1", check.ExpectedValue);
            Assert.AreEqual("json", check.VerifierId);
            Assert.AreEqual(VerificationOperation.Equals, check.Operation);
        }

        [TestMethod]
        public void ParseVerificationCheck_WithJsonCheckString_OperatorIsEquals()
        {
            // Act
            var check = VerificationCheckParser.Parse("json $.Count == A B");

            // Assert
            Assert.AreEqual("A B", check.ExpectedValue);
            Assert.AreEqual("json", check.VerifierId);
            Assert.AreEqual(VerificationOperation.Equals, check.Operation);
        }
    }
}
