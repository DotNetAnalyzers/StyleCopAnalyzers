// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp7.ReadabilityRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using StyleCop.Analyzers.Test.ReadabilityRules;
    using Xunit;

    public class SA1101CSharp7UnitTests : SA1101UnitTests
    {
        /// <summary>
        /// Verifies that a value tuple is handled properly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [WorkItem(2534, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/2534")]
        public async Task TestValueTupleAsync()
        {
            var testCode = @"public class Foo
{
    protected (bool a, bool b) Bar()
    {
        return (a: true, b: false);
    }
}
";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
