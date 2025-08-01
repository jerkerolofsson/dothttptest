using DotHttpTest.Models;
using DotHttpTest.Runner;
using DotHttpTest.Verification.Json;
using DotHttpTest.Verification.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotHttpTest.Tests.Unit.VerificationTests
{
    [UnitTest]
    [TestClass]
    public class JsonVerifierTests
    {
        [TestMethod]
        public async Task VerifyAsync_WithPropertyNotEquals_IsSuccess()
        {
            // Arrange
            var verifier = new JsonVerifier();
            var result = new VerificationCheckResult(new DotHttpRequest(), new VerificationCheck("json", "Name", VerificationOperation.NotEquals, "Kalle"));
            var response = new DotHttpResponse(new HttpResponseMessage(), new HttpRequestMetrics());
            response.ContentBytes = Encoding.UTF8.GetBytes("""
                {
                    "Name": "Andersson"
                }
                """);

            // Act
            await verifier.VerifyAsync(response, result);

            // Assert
            Assert.IsTrue(result.IsSuccess);
        }

        [TestMethod]
        public async Task VerifyAsync_WithPropertyEquals_IsSuccess()
        {
            // Arrange
            var verifier = new JsonVerifier();
            var result = new VerificationCheckResult(new DotHttpRequest(), new VerificationCheck("json", "Name", VerificationOperation.Equals, "Kalle"));
            var response = new DotHttpResponse(new HttpResponseMessage(), new HttpRequestMetrics());
            response.ContentBytes = Encoding.UTF8.GetBytes("""
                {
                    "Name": "Kalle"
                }
                """);

            // Act
            await verifier.VerifyAsync(response, result);

            // Assert
            Assert.IsTrue(result.IsSuccess);
        }


        [TestMethod]
        public async Task VerifyAsync_WithPropertyEquals_And_ValueNotEqual_IsNotSuccess()
        {
            // Arrange
            var verifier = new JsonVerifier();
            var result = new VerificationCheckResult(new DotHttpRequest(), new VerificationCheck("json", "Name", VerificationOperation.Equals, "Kalle"));
            var response = new DotHttpResponse(new HttpResponseMessage(), new HttpRequestMetrics());
            response.ContentBytes = Encoding.UTF8.GetBytes("""
                {
                    "Name": "Andersson"
                }
                """);

            // Act
            await verifier.VerifyAsync(response, result);

            // Assert
            Assert.IsFalse(result.IsSuccess);
        }

        [TestMethod]
        public async Task VerifyAsync_PropertyExists_IsSuccess()
        {
            // Arrange
            var verifier = new JsonVerifier();
            var result = new VerificationCheckResult(new DotHttpRequest(), new VerificationCheck("json", "Names", VerificationOperation.Exists, ""));
            var response = new DotHttpResponse(new HttpResponseMessage(), new HttpRequestMetrics());
            response.ContentBytes = Encoding.UTF8.GetBytes("""
                {
                    "Names": "Kalle"
                }
                """);

            // Act
            await verifier.VerifyAsync(response, result);

            // Assert
            Assert.IsTrue(result.IsSuccess, result.Error??"");
        }

        [TestMethod]
        public async Task VerifyAsync_ArrayExists_IsSuccess()
        {
            // Arrange
            var verifier = new JsonVerifier();
            var result = new VerificationCheckResult(new DotHttpRequest(), new VerificationCheck("json", "Names", VerificationOperation.Exists, ""));
            var response = new DotHttpResponse(new HttpResponseMessage(), new HttpRequestMetrics());
            response.ContentBytes = Encoding.UTF8.GetBytes("""
                {
                    "Names": []
                }
                """);

            // Act
            await verifier.VerifyAsync(response, result);

            // Assert
            Assert.IsTrue(result.IsSuccess, result.Error ?? "");
        }


        [TestMethod]
        public async Task VerifyAsync_ArrayPropertyContains_WithFirstItemMatch_IsSuccess()
        {
            // Arrange
            var verifier = new JsonVerifier();
            var result = new VerificationCheckResult(new DotHttpRequest(), new VerificationCheck("json", "Names", VerificationOperation.Contains, "Kalle"));
            var response = new DotHttpResponse(new HttpResponseMessage(), new HttpRequestMetrics());
            response.ContentBytes = Encoding.UTF8.GetBytes("""
                {
                    "Names": ["Kalle", "Anka"]
                }
                """);

            // Act
            await verifier.VerifyAsync(response, result);

            // Assert
            Assert.IsTrue(result.IsSuccess, result.Error ?? "");
        }

        [TestMethod]
        public async Task VerifyAsync_ArrayPropertyContains_WithRootArrayWithoutItem_IsNotSuccess()
        {
            // Arrange
            var verifier = new JsonVerifier();
            var result = new VerificationCheckResult(new DotHttpRequest(), new VerificationCheck("json", ".", VerificationOperation.Contains, "Goofy"));
            var response = new DotHttpResponse(new HttpResponseMessage(), new HttpRequestMetrics());
            response.ContentBytes = Encoding.UTF8.GetBytes("""
                [
                    "Kalle", 
                    "Anka"
                ]
                """);

            // Act
            await verifier.VerifyAsync(response, result);

            // Assert
            Assert.IsFalse(result.IsSuccess, result.Error ?? "");
        }


        [TestMethod]
        public async Task VerifyAsync_ArrayPropertyContains_WithRootArray_IsSuccess()
        {
            // Arrange
            var verifier = new JsonVerifier();
            var result = new VerificationCheckResult(new DotHttpRequest(), new VerificationCheck("json", ".", VerificationOperation.Contains, "Anka"));
            var response = new DotHttpResponse(new HttpResponseMessage(), new HttpRequestMetrics());
            response.ContentBytes = Encoding.UTF8.GetBytes("""
                [
                    "Kalle", 
                    "Anka"
                ]
                """);

            // Act
            await verifier.VerifyAsync(response, result);

            // Assert
            Assert.IsTrue(result.IsSuccess, result.Error ?? "");
        }


        [TestMethod]
        public async Task VerifyAsync_ArrayPropertyContains_WithSecondItemMatch_IsSuccess()
        {
            // Arrange
            var verifier = new JsonVerifier();
            var result = new VerificationCheckResult(new DotHttpRequest(), new VerificationCheck("json", "Names", VerificationOperation.Contains, "Anka"));
            var response = new DotHttpResponse(new HttpResponseMessage(), new HttpRequestMetrics());
            response.ContentBytes = Encoding.UTF8.GetBytes("""
                {
                    "Names": ["Kalle", "Anka"]
                }
                """);

            // Act
            await verifier.VerifyAsync(response, result);

            // Assert
            Assert.IsTrue(result.IsSuccess, result.Error ?? "");
        }

        [TestMethod]
        public async Task VerifyAsync_ArrayPropertyNotContains_WithNoItemMatch_IsSuccess()
        {
            // Arrange
            var verifier = new JsonVerifier();
            var result = new VerificationCheckResult(new DotHttpRequest(), new VerificationCheck("json", "Names", VerificationOperation.NotContains, "Goofy"));
            var response = new DotHttpResponse(new HttpResponseMessage(), new HttpRequestMetrics());
            response.ContentBytes = Encoding.UTF8.GetBytes("""
                {
                    "Names": ["Kalle", "Anka"]
                }
                """);

            // Act
            await verifier.VerifyAsync(response, result);

            // Assert
            Assert.IsTrue(result.IsSuccess, result.Error ?? "");
        }
        [TestMethod]
        public async Task VerifyAsync_ArrayPropertyContains_WithNoItemMatch_IsNotSuccess()
        {
            // Arrange
            var verifier = new JsonVerifier();
            var result = new VerificationCheckResult(new DotHttpRequest(), new VerificationCheck("json", "Names", VerificationOperation.Contains, "Goofy"));
            var response = new DotHttpResponse(new HttpResponseMessage(), new HttpRequestMetrics());
            response.ContentBytes = Encoding.UTF8.GetBytes("""
                {
                    "Names": ["Kalle", "Anka"]
                }
                """);

            // Act
            await verifier.VerifyAsync(response, result);

            // Assert
            Assert.IsFalse(result.IsSuccess, message:result.Error??"");
        }
    }
}
