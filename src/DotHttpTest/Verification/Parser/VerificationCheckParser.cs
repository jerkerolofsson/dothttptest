using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DotHttpTest.Verification.Parser
{
    internal class VerificationCheckParser
    {
        private class VerificationOperatorPattern
        {
            public string Regex { get; set; }
            public VerificationOperation Operation { get; set; }

            public VerificationOperatorPattern(string regex, VerificationOperation operation)
            {
                Regex = regex;
                Operation = operation;
            }   
        }

        private static List<VerificationOperatorPattern> mOperatorMap = new()
        {
            new ("(not exists)", VerificationOperation.NotExists ),

            new ("(is date)", VerificationOperation.DateEquals ),
            new ("(is not date)", VerificationOperation.DateNotEquals ),

            new ("(==|\\sis\\s)", VerificationOperation.Equals ),
            new ("(!=)", VerificationOperation.NotEquals ),
            new ("(exists)", VerificationOperation.Exists ),

            new ("(<=)", VerificationOperation.LessOrEquals ),
            new ("(>=)", VerificationOperation.GreaterOrEquals ),
            new ("(<)", VerificationOperation.Less ),
            new ("(>)", VerificationOperation.Greater ),

            new ("(\\sregex\\s)", VerificationOperation.RegexMatch )
        };

        internal static VerificationCheck Parse(string val)
        {
            var p1 = val.IndexOfAny(new char[] { ' ', '\t' });
            if (p1 == -1)
            {
                throw new Exceptions.HttpParsingException($"Failed to parse @verify attribute: {val}, expected white spaces to separate verifier module from check");
            }

            // @verify http status-code 200
            // @verify http status-code == 200

            var verifierId = val.Substring(0, p1).Trim();
            var remainder = val.Substring(p1).Trim();
            var propertyId = remainder;
            string? expectedValue = null;
            var matchedOperator = false;

            // See if the remainder is an operator, if not, use equals as default
            VerificationOperation operation = VerificationOperation.Equals;
            foreach (var pair in mOperatorMap)
            {
                var regex = new Regex(pair.Regex);
                if (regex.Match(remainder).Success)
                {
                    operation = pair.Operation;
                    matchedOperator = true;

                    var items = regex.Split(remainder);
                    expectedValue = null;
                    propertyId = items[0].Trim();

                    if (items.Length == 3)
                    {
                        expectedValue = items[2].Trim();
                    }
                    break;
                }
            }

            // No operator matched, see if it has a value
            if (!matchedOperator)
            {
                var p = remainder.IndexOf(' ');
                if(p > 0)
                {
                    propertyId = remainder.Substring(0, p).Trim();
                    expectedValue = remainder.Substring(p+1).Trim();
                }
            }

            return new VerificationCheck(verifierId, propertyId, operation, expectedValue);
        }
    }
}
