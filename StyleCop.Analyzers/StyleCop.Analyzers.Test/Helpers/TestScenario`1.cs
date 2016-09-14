// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.Helpers
{
    using System;
    using Xunit.Abstractions;

    internal sealed class TestScenario<TSetting> : IXunitSerializable
    {
        private static readonly Func<string, string> SameDisplayNameFunction = c => c;

        public TestScenario()
        {
        }

        internal TestScenario(string testCode, params ExpectedViolation<TSetting>[] expectedViolations)
            : this(testCode, SameDisplayNameFunction, expectedViolations)
        {
        }

        internal TestScenario(string testCode, Func<string, string> displayNameFunction, params ExpectedViolation<TSetting>[] expectedViolations)
        {
            this.TestCode = testCode;
            this.DisplayName = displayNameFunction.Invoke(testCode);
            this.ExpectedViolations = expectedViolations;
        }

        internal string TestCode { get; private set;  }

        internal string DisplayName { get; private set; }

        internal ExpectedViolation<TSetting>[] ExpectedViolations { get; private set; }

        public override string ToString()
        {
            return this.DisplayName;
        }

        void IXunitSerializable.Serialize(IXunitSerializationInfo info)
        {
            info.AddValue(nameof(this.TestCode), this.TestCode);
            info.AddValue(nameof(this.DisplayName), this.DisplayName);
            info.AddValue(nameof(this.ExpectedViolations), this.ExpectedViolations);
        }

        void IXunitSerializable.Deserialize(IXunitSerializationInfo info)
        {
            this.TestCode = info.GetValue<string>(nameof(this.TestCode));
            this.DisplayName = info.GetValue<string>(nameof(this.DisplayName));
            this.ExpectedViolations = info.GetValue<ExpectedViolation<TSetting>[]>(nameof(this.ExpectedViolations));
        }
    }
}
