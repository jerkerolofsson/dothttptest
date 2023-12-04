using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotHttpTest.Builders
{
    public class DotHttpRequestBuilder
    {
        private DotHttpRequest mRequest;

        internal DotHttpRequestBuilder(DotHttpRequest request)
        {
            mRequest = request;
        }

        /// <summary>
        /// Sets a delay which will occur after each request
        /// </summary>
        /// <param name="delay"></param>
        /// <returns></returns>
        public DotHttpRequestBuilder SetDelayAfterRequest(TimeSpan delay)
        {
            mRequest.DelayAfterRequest = delay;
            return this;
        }

        /// <summary>
        /// Removes all stages configured for the request
        /// </summary>
        /// <returns></returns>
        public DotHttpRequestBuilder ClearStages()
        {
            mRequest.ClearStages();
            return this;
        }

        /// <summary>
        /// Adds a stage to the request
        /// </summary>
        /// <param name="stage"></param>
        /// <returns></returns>
        public DotHttpRequestBuilder AddStage(StageAttributes stage)
        {
            mRequest.AddStage(stage);
            return this;
        }
    }
}
