using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DotHttpTest.Verification
{
    public class VerifierFactory
    {
        private readonly List<IVerifier> mVerifiers = new();
        private readonly Dictionary<string, IVerifier> mVerifierMap = new();

        public VerifierFactory(ClientOptions options)
        {
            mVerifiers.Add(new Json.JsonVerifier());
            mVerifiers.Add(new Http.HttpVerifier());
            mVerifiers.Add(new Http.HttpHeaderVerifier());
            BuildLookupTable();
        }

        private void BuildLookupTable()
        {
            foreach (var verifier in mVerifiers)
            {
                var type = verifier.GetType();
                var attribute = type.GetCustomAttribute<ResponseVerifierAttribute>();

                ArgumentNullException.ThrowIfNull(attribute, $"The type {type} is not decorated with the ResponseVerifierAttribute");
                mVerifierMap[attribute.Name] = verifier;
            }
        }

        internal async Task VerifyAsync(DotHttpResponse response)
        {
            await VerifyChecksAsync(response);
            CreateMetrics(response);
        }

        private void CreateMetrics(DotHttpResponse response)
        {
            if (response.Results == null)
            {
                return;
            }

            var numPassed = response.Results.Where(x => x.IsSuccess).Count();
            var numFailed = response.Results.Where(x => !x.IsSuccess).Count();
            response.Metrics.ChecksPassed.Increment(numPassed);
            response.Metrics.ChecksFailed.Increment(numFailed);
        }

        private async Task VerifyChecksAsync(DotHttpResponse response)
        {
            if (response.Request == null)
            {
                return;
            }

            response.Results = new();
            foreach (var verificationCheck in response.Request.VerificationChecks)
            {
                var result = new VerificationCheckResult(response.Request, verificationCheck);
                response.Results.Add(result);
                if (mVerifierMap.TryGetValue(verificationCheck.VerifierId, out IVerifier? verifier))
                {
                    if (verifier != null)
                    {
                        await verifier.VerifyAsync(response, result);
                    }
                }
                else
                {
                    result.IsSuccess = false;
                    result.Error = $"Verifier {verificationCheck.VerifierId} not found";
                }
            }
        }
    }
}
