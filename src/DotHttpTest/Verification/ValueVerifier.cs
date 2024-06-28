using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotHttpTest.Verification
{
    public class ValueVerifier
    {
        protected bool CompareValue(object value, string? expectedValue, VerificationOperation operation)
        {
            if (value is null)
            {
                if (operation == VerificationOperation.NotExists)
                {
                    return true;
                }

                return false;
            }

            switch (operation)
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

                case VerificationOperation.DateEquals:
                    {
                        if (!DateTime.TryParse(expectedValue, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out DateTime expectedDateTime))
                        {
                            return false;
                        }
                        if (!DateTime.TryParse(value.ToString(), CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out DateTime valueDateTime))
                        {
                            return false;
                        }
                        return valueDateTime.ToUniversalTime().Equals(expectedDateTime.ToUniversalTime());
                    }

                case VerificationOperation.DateNotEquals:
                    {
                        if (!DateTime.TryParse(expectedValue, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out DateTime expectedDateTime))
                        {
                            return false;
                        }
                        if (!DateTime.TryParse(value.ToString(), CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out DateTime valueDateTime))
                        {
                            return false;
                        }
                        return !valueDateTime.ToUniversalTime().Equals(expectedDateTime.ToUniversalTime());
                    }

                case VerificationOperation.Equals:
                    if (value != null && expectedValue is not null)
                    {
                        if (value.GetType() == typeof(double))
                        {
                            if (!double.TryParse(expectedValue, NumberStyles.Float, CultureInfo.InvariantCulture, out double expectedDoubleValue))
                            {
                                return false;
                            }
                            return value.Equals(expectedDoubleValue);
                        }

                        return value.ToString()!.Equals(expectedValue);
                    }
                    return false;
                case VerificationOperation.NotEquals:
                    if (value != null && expectedValue is not null)
                    {
                        return !value.ToString()!.Equals(expectedValue);
                    }
                    return false;
                default:
                    //throw new Exception($"Unknown operation: {operation}");
                    return false;
            }
        }

        private double ToDouble(string? value)
        {
            if (value is null)
            {
                return double.MinValue;
            }
            double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out double val);
            return val;
        }
    }
}
