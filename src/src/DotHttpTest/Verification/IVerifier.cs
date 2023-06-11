﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotHttpTest.Verification
{
    public interface IVerifier
    {
        void Verify(DotHttpResponse response, VerificationCheckResult result);
    }
}
