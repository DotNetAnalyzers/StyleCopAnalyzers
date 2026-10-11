// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp12.SpacingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp11.SpacingRules;
    using Xunit;
    using static StyleCop.Analyzers.SpacingRules.SA1023DereferenceAndAccessOfSymbolsMustBeSpacedCorrectly;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.SpacingRules.SA1023DereferenceAndAccessOfSymbolsMustBeSpacedCorrectly,
        StyleCop.Analyzers.SpacingRules.TokenSpacingCodeFixProvider>;

    public partial class SA1023CSharp12UnitTests : SA1023CSharp11UnitTests
    {
        /// <summary>
        /// Verifies that a pointer type at the end of a using alias directive is not reported.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [WorkItem(4011, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4011")]
        public async Task TestPointerTypeInUsingAliasAsync()
        {
            var testCode = @"using unsafe Pointer = int*;
using unsafe PointerToPointer = int**;
using unsafe PointerArray = int*[];

unsafe class TestClass
{
    Pointer field1;
    PointerToPointer field2;
    PointerArray field3;
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a space between a pointer type and the semicolon ending a using alias directive is reported.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [WorkItem(4011, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4011")]
        public async Task TestPointerTypeFollowedBySpaceInUsingAliasAsync()
        {
            var testCode = @"using unsafe Pointer = int{|#0:*|} ;

unsafe class TestClass
{
    Pointer field;
}
";

            var fixedCode = @"using unsafe Pointer = int*;

unsafe class TestClass
{
    Pointer field;
}
";

            var expected = Diagnostic(DescriptorNotFollowed).WithLocation(0);
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
