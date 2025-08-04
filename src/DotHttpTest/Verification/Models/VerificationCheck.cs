using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotHttpTest.Verification.Models
{
    public record class VerificationCheck
    {
        public VerificationCheck(string verifierId, string propertyId, VerificationOperation operation, string? expectedValue)
        {
            VerifierId = verifierId;
            PropertyId = propertyId;
            Operation = operation;
            ExpectedValue = expectedValue;
        }

        /// <summary>
        /// ID of module to do verification, for example 'http'
        /// </summary>
        public string VerifierId { get; set; }

        /// <summary>
        /// Property to be checked, for example 'status-code'
        /// </summary>
        public string PropertyId { get; set; }

        /// <summary>
        /// How to compare the value
        /// </summary>
        public VerificationOperation Operation { get; set; }

        /// <summary>
        /// Value to be checked
        /// </summary>
        public string? ExpectedValue { get; set; }

        
    }
}
