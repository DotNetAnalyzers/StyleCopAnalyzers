// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp14.LayoutRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp13.LayoutRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.LayoutRules.SA1500BracesForMultiLineStatementsMustNotShareLine,
        StyleCop.Analyzers.LayoutRules.SA1500CodeFixProvider>;

    public partial class SA1500CSharp14UnitTests : SA1500CSharp13UnitTests
    {
        /// <summary>
        /// Verifies that no diagnostics are reported for the valid properties defined in this test.
        /// </summary>
        /// <remarks>
        /// <para>These are valid for SA1500 only, some will report other diagnostics.</para>
        /// </remarks>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestPropertyDefaultValueAssignmentValidAsync()
        {
            var testCode = @"using System;

public class Foo
{
    public bool Property1
    {
        get { return field; }
        set { field = value; }
    } = default;
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
