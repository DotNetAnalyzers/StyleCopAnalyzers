// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.Helpers
{
    using System.Collections.Generic;

    internal sealed class TestScenario<TSetting>
    {
        internal TestScenario(string testCode, params ExpectedViolation<TSetting>[] expectedViolations)
        {
            this.TestCode = testCode;
            this.ExpectedViolations = expectedViolations;
        }

        internal string TestCode { get; }

        internal IEnumerable<ExpectedViolation<TSetting>> ExpectedViolations { get; }

        public override string ToString()
        {
            return this.TestCode;
        }
    }
}
