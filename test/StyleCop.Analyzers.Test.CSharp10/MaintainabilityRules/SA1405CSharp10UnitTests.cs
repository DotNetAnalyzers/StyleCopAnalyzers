// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp10.MaintainabilityRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp9.MaintainabilityRules;
    using Xunit;

    public partial class SA1405CSharp10UnitTests : SA1405CSharp9UnitTests
    {
        /// <summary>
        /// Verifies that an interpolated string message is accepted when it binds to the
        /// <c>Debug.Assert(bool, ref AssertInterpolatedStringHandler)</c> overload added in .NET 6.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [WorkItem(3981, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3981")]
        public async Task TestInterpolatedStringHandlerMessageAsync()
        {
            var testCode = @"using System.Diagnostics;

public class Foo
{
    public void Bar(int x)
    {
        Debug.Assert(x > 0, $""x = {x}"");
    }
}
";

            await this.VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
