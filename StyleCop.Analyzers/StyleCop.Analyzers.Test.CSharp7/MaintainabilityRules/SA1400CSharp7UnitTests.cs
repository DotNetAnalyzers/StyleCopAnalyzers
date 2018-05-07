// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp7.MaintainabilityRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using StyleCop.Analyzers.Test.MaintainabilityRules;
    using Xunit;

    public class SA1400CSharp7UnitTests : SA1400UnitTests
    {
        /// <summary>
        /// Verifies that local functions, which do not support access modifiers, do not trigger SA1400.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestLocalFunctionAsync()
        {
            var testCode = @"
internal class ClassName
{
    public void MethodName()
    {
        void LocalFunction()
        {
        }
    }
}
";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
