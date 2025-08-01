using DotHttpTest.Runner;

namespace DotHttpTest.Tests.Unit.RunnerTests
{
    [UnitTest]
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
            Assert.HasCount(1, testPlan.Stages);
        }

        [TestMethod]
        public void BuildTestPlan_WithOneRequestWithoutStage_AnonymousStageContainsOneRequest()
        {
            // Arrange
            var dotRequests = DotHttpRequestLoader.ParseRequests(File.ReadAllLines("TestData/Requests/Get/get_no_content.http"));

            // Act
            var testPlan = new TestPlanBuilder().Add(dotRequests).Build();

            // Assert
            Assert.HasCount(1, testPlan.Stages[0].Requests);
        }

        [TestMethod]
        public void BuildTestPlan_With3StagesAnd1Request_3StagesAreCreatedWith1RequestEach()
        {
            // Arrange
            var dotRequests = DotHttpRequestLoader.ParseRequests(File.ReadAllLines("TestData/Requests/Get/get_with_3_stages.http"));

            // Act
            var testPlan = new TestPlanBuilder().Add(dotRequests).Build();

            // Assert
            Assert.HasCount(3, testPlan.Stages);
            foreach (var stage in testPlan.Stages)
            {
                Assert.HasCount(1, stage.Requests);
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
            Assert.HasCount(1, testPlan.Stages);
            foreach (var stage in testPlan.Stages)
            {
                Assert.HasCount(2, stage.Requests);
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
            Assert.HasCount(2, testPlan.Stages);
            Assert.HasCount(1, testPlan.Stages[0].Requests);
            Assert.HasCount(1, testPlan.Stages[1].Requests);
            Assert.AreEqual("request1", testPlan.Stages[0].Requests[0].RequestName);
            Assert.AreEqual("request2", testPlan.Stages[1].Requests[0].RequestName);
        }
    }
}
