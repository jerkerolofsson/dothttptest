using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotHttpTest.Verification.Http
{
    public class ValueVerifier
    {
        protected bool CompareValue(object value, string? expectedValue, VerificationOperation operation)
        {
            if(value is null)
            {
                return false;
            }

            switch(operation)
            {
                case VerificationOperation.Exists:
                    return true;

                case VerificationOperation.Greater:
                    return ToDouble(value.ToString()) > ToDouble(expectedValue);
                case VerificationOperation.GreaterOrEquals:
                    return ToDouble(value.ToString()) >= ToDouble(expectedValue);
                case VerificationOperation.Less:
                    return ToDouble(value.ToString()) < ToDouble(expectedValue);
                case VerificationOperation.LessOrEquals:
                    return ToDouble(value.ToString()) <= ToDouble(expectedValue);

                case VerificationOperation.Equals:
                    if (value != null && expectedValue is not null)
                    {
                        return value.ToString()!.Equals(expectedValue);
                    }
                    return false;
                case VerificationOperation.NotEquals:
                    if (value != null && expectedValue is not null)
                    {
                        return ! (value.ToString()!.Equals(expectedValue));
                    }
                    return false;
                default:
                    //throw new Exception($"Unknown operation: {operation}");
                    return false;
            }
        }

        private double ToDouble(string? value)
        {
            if(value is null)
            {
                return double.MinValue;
            }
            double.TryParse(value, System.Globalization.NumberStyles.Float, CultureInfo.InvariantCulture, out double val);
            return val;
        }
    }
}
