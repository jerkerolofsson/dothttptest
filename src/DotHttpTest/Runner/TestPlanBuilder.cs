﻿using DotHttpTest.Builders;
using DotHttpTest.Runner.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotHttpTest.Runner
{
    /// <summary>
    /// Creates a test plan in the following structure
    /// Stage 1
    ///  + Request 1
    ///  + Request 2
    /// Stage 2
    ///  + Request 3
    ///  + Request 4
    ///  
    /// If a request does not define any stage, it will be added to the stages defined by the previous request.
    /// 
    /// Example 1:
    /// 
    /// Request 1 declares two stages (A,B)
    /// Request 2 declares no stages
    /// 
    /// Stage A:
    /// + Request 1
    /// + Request 2
    /// 
    /// Stage B:
    /// + Request 1
    /// + Request 2
    /// 
    /// 
    /// Example 1:
    /// 
    /// Request 1 declares one stage (A)
    /// Request 2 declares one stage (B)
    /// 
    /// Stage A:
    /// + Request 1
    /// 
    /// Stage B:
    /// + Request 2
    /// 
    /// </summary>
    /// 
    public class TestPlanBuilder
    {
        private List<DotHttpRequest> mRequests = new();
        private TestPlan mTestPlan = new TestPlan();

        /// <summary>
        /// Loads a .http plan from a string
        /// </summary>
        /// <param name="text"></param>
        /// <param name="requestConfigurator"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public TestPlanBuilder LoadHttpText(string text, Action<DotHttpRequestBuilder> requestConfigurator, ClientOptions? options = null)
        {
            options ??= ClientOptions.DefaultOptions();

            var requests = DotHttpRequest.Parse(text.Split('\n'), options);
            foreach (var request in requests)
            {
                var requestBuilder = new DotHttpRequestBuilder(request);
                requestConfigurator(requestBuilder);
            }
            mRequests.AddRange(requests);
            return this;
        }

        /// <summary>
        /// Loads a .http plan from a stream
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="requestConfigurator"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public TestPlanBuilder LoadHttpFile(Stream stream, Action<DotHttpRequestBuilder> requestConfigurator, ClientOptions? options = null)
        {
            options ??= ClientOptions.DefaultOptions();

            var requests = DotHttpRequest.FromStream(stream, options);
            foreach (var request in requests)
            {
                var requestBuilder = new DotHttpRequestBuilder(request);
                requestConfigurator(requestBuilder);
            }
            mRequests.AddRange(requests);
            return this;
        }

        /// <summary>
        /// Sets the name of the test plan
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public TestPlanBuilder SetName(string name)
        {
            mTestPlan.Name = name;
            return this;
        }

        /// Loads a .http plan from a file
        /// </summary>
        /// <param name="httpFilePath"></param>
        /// <param name="requestConfigurator"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public TestPlanBuilder LoadHttpFile(string httpFilePath, Action<DotHttpRequestBuilder> requestConfigurator, ClientOptions? options = null)
        {
            if(!File.Exists(httpFilePath))
            {
                throw new FileNotFoundException("File not found: {httpFilePath}");
            }

            options ??= ClientOptions.DefaultOptions();

            mTestPlan.Name = Path.GetFileNameWithoutExtension(httpFilePath);
            var requests = DotHttpRequest.FromFile(httpFilePath, options);
            foreach(var request in requests) 
            {
                var requestBuilder = new DotHttpRequestBuilder(request);
                requestConfigurator(requestBuilder);
            }
            mRequests.AddRange(requests);
            return this;
        }
        public TestPlanBuilder LoadHttpFile(string httpFilePath, ClientOptions? options = null)
        {
            if (!File.Exists(httpFilePath))
            {
                throw new FileNotFoundException("File not found: {httpFilePath}");
            }

            options ??= ClientOptions.DefaultOptions();

            mTestPlan.Name = Path.GetFileNameWithoutExtension(httpFilePath);
            var requests = DotHttpRequest.FromFile(httpFilePath, options);
            mRequests.AddRange(requests);
            return this;
        }

        public TestPlanBuilder Add(IEnumerable<DotHttpRequest> requests)
        {
            mRequests.AddRange(requests);
            return this;
        }

        public TestPlan Build()
        {

            List<TestPlanStage> currentStages = new();

            foreach(var request in mRequests)
            {
                if(request.HasStages)
                {
                    currentStages = new List<TestPlanStage>();
                    foreach(var stage in request.Stages) 
                    {
                        var testPlanStage = new TestPlanStage(stage);
                        mTestPlan.Stages.Add(testPlanStage);
                        currentStages.Add(testPlanStage);
                    }
                }

                if (currentStages.Count == 0)
                {
                    // If no stage is defined, this is a a request that is declared
                    // before any stage, then create an anonymous stage
                    var testPlanStage = new TestPlanStage(new StageAttributes()
                    {
                        Duration = TimeSpan.Zero,
                        Target = 0,
                        Name = request.RequestName
                    });

                    if(request.TestId is not null)
                    {
                        testPlanStage.Attributes.TestId = request.TestId;
                    }

                    mTestPlan.Stages.Add(testPlanStage);
                    currentStages.Add(testPlanStage);
                }

                // Add this request to the currently defined stages
                foreach (var currentStage in currentStages)
                {
                    currentStage.Requests.Add(request);
                }
            }

            return mTestPlan;
        }
    }
}
