using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotHttpTest.Verification.Http
{
    [ResponseVerifier("http")]
    public class HttpVerifier : ValueVerifier, IVerifier
    {
        public void Verify(DotHttpResponse response, VerificationCheckResult result)
        {
            var check = result.Check;
            switch(check.PropertyId)
            {
                case "status-code":
                    result.ActualValue = ((int)response.StatusCode).ToString();
                    result.IsSuccess = CompareValue((int)response.StatusCode, check.ExpectedValue, check.Operation);
                    break;
                case "duration":
                    double duration = response.Metrics.HttpRequestDuration.Value;
                    var millis = (int)(Math.Round(duration * 1000));
                    result.ActualValue = millis.ToString();
                    result.IsSuccess = CompareValue(millis, check.ExpectedValue, check.Operation);
                    break;
                default:
                    result.IsSuccess = false;
                    result.Error = $"Unknown property: {result.Check.PropertyId}";
                    return;
            }

            if(!result.IsSuccess && result.Error == null)
            {
                result.Error = $"{result.Check.VerifierId} {result.Check.PropertyId} was {result.ActualValue}, expected {result.Check.Operation} {result.Check.ExpectedValue ?? ""} for {result.Request.RequestName} {response.RequestUri}";
            }
        }
    }
}
