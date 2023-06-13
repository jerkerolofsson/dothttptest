using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotHttpTest.Verification.Http
{
    [ResponseVerifier("header")]
    public class HttpHeaderVerifier : ValueVerifier, IVerifier
    {
        public void Verify(DotHttpResponse response, VerificationCheckResult result)
        {
            var check = result.Check;
            string headerName = check.PropertyId;

            var headers = response.HttpResponse.Headers.Where(x => x.Key.Equals(headerName, StringComparison.InvariantCultureIgnoreCase)).ToList();
            if (headers.Count == 0)
            {
                if(check.Operation == VerificationOperation.NotExists)
                {
                    result.IsSuccess = true;
                    return;
                }

                result.Error = $"HTTP Header '{headerName}' was not found";
                result.IsSuccess = false;
            }
            else
            {
                result.Error = null;
                result.IsSuccess = false;

                foreach (var header in headers)
                {
                    foreach(var value in header.Value)
                    {
                        if(value != null)
                        {
                            result.ActualValue = value;
                            result.IsSuccess = CompareValue(value, check.ExpectedValue, check.Operation);
                            if (result.IsSuccess) break;
                        }
                    }
                }

                if (!result.IsSuccess && result.Error == null)
                {
                    result.Error = $"{result.Check.VerifierId} {result.Check.PropertyId} was {result.ActualValue}, expected {result.Check.Operation} {result.Check.ExpectedValue ?? ""} for {result.Request.RequestName} ({result.Request.Url})";
                }

            }
        }
    }
}
