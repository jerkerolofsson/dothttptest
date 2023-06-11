using DotHttpTest.Runner;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotHttpTest.Tests.Unit.RunnerTests
{
    [TestCategory("UnitTests")]
    [TestClass]
    public class TestPlanBuilderTests
    {
        [TestMethod]
        public void BuildTestPlan_WithRequestWithoutStage_AnonymousStageCreated()
        {
            // Arrange
            var dotRequests = DotHttpRequestLoader.ParseRequests(File.ReadAllLines("TestData/Requests/Get/get_no_content.http"));

            // Act
            var testPlan = new TestPlanBuilder().Add(dotRequests).Build();

            // Assert
            Assert.AreEqual(1, testPlan.Stages.Count);
        }

        [TestMethod]
        public void BuildTestPlan_WithOneRequestWithoutStage_AnonymousStageContainsOneRequest()
        {
            // Arrange
            var dotRequests = DotHttpRequestLoader.ParseRequests(File.ReadAllLines("TestData/Requests/Get/get_no_content.http"));

            // Act
            var testPlan = new TestPlanBuilder().Add(dotRequests).Build();

            // Assert
            Assert.AreEqual(1, testPlan.Stages[0].Requests.Count);
        }

        [TestMethod]
        public void BuildTestPlan_With3StagesAnd1Request_3StagesAreCreatedWith1RequestEach()
        {
            // Arrange
            var dotRequests = DotHttpRequestLoader.ParseRequests(File.ReadAllLines("TestData/Requests/Get/get_with_3_stages.http"));

            // Act
            var testPlan = new TestPlanBuilder().Add(dotRequests).Build();

            // Assert
            Assert.AreEqual(3, testPlan.Stages.Count);
            foreach (var stage in testPlan.Stages)
            {
                Assert.AreEqual(1, stage.Requests.Count);
            }
        }

        [TestMethod]
        public void BuildTestPlan_With1StagesAnd2Request_1StagesIsCreatedWith2Requests()
        {
            // Arrange
            var dotRequests = DotHttpRequestLoader.ParseRequests(File.ReadAllLines("TestData/Requests/Get/get_with_1_stage_2_requests.http"));

            // Act
            var testPlan = new TestPlanBuilder().Add(dotRequests).Build();

            // Assert
            Assert.AreEqual(1, testPlan.Stages.Count);
            foreach (var stage in testPlan.Stages)
            {
                Assert.AreEqual(2, stage.Requests.Count);
            }
        }


        [TestMethod]
        public void BuildTestPlan_With2StageGroups_2StagesAreCreatedWithTheirOwnRequest()
        {
            // Arrange
            var dotRequests = DotHttpRequestLoader.ParseRequests(File.ReadAllLines("TestData/Requests/Get/get_with_2_stage_groups.http"));

            // Act
            var testPlan = new TestPlanBuilder().Add(dotRequests).Build();

            // Assert
            Assert.AreEqual(2, testPlan.Stages.Count);
            Assert.AreEqual(1, testPlan.Stages[0].Requests.Count);
            Assert.AreEqual(1, testPlan.Stages[1].Requests.Count);
            Assert.AreEqual("request1", testPlan.Stages[0].Requests[0].RequestName);
            Assert.AreEqual("request2", testPlan.Stages[1].Requests[0].RequestName);
        }
    }
}
