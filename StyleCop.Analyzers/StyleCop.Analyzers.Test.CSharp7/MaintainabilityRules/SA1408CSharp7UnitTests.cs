// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp7.MaintainabilityRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using StyleCop.Analyzers.Test.MaintainabilityRules;
    using Xunit;

    public class SA1408CSharp7UnitTests : SA1408UnitTests
    {
        /// <summary>
        /// Verifies that a code fix for SA1119 in a pattern matching expression does not trigger SA1408.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        /// <seealso cref="SA1119CSharp7UnitTests.TestPatternMatchingAsync"/>
        [Fact]
        public async Task TestPatternMatchingAsync()
        {
            var testCode = @"public class Foo
{
    public void Bar()
    {
        if (new object() is bool b && b)
        {
            return;
        }
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
