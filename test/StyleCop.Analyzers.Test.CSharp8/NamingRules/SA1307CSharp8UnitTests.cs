// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp8.NamingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp7.NamingRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.NamingRules.SA1307AccessibleFieldsMustBeginWithUpperCaseLetter,
        StyleCop.Analyzers.NamingRules.RenameToUpperCaseCodeFixProvider>;

    public partial class SA1307CSharp8UnitTests : SA1307CSharp7UnitTests
    {
        [Fact]
        [WorkItem(3001, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3001")]
        public async Task TestReadonlyFieldInStructAsync()
        {
            var testCode = @"public struct TestStruct
{
    public readonly int {|#0:bar|};
    public readonly void Method() { }
}";

            var fixedCode = @"public struct TestStruct
{
    public readonly int Bar;
    public readonly void Method() { }
}";

            DiagnosticResult expected = Diagnostic().WithLocation(0).WithArguments("bar");

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(3001, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3001")]
        public async Task TestStaticReadonlyFieldInStructAsync()
        {
            var testCode = @"public struct TestStruct
{
    public static readonly int {|#0:bar|};
    public readonly void Method() { }
}";

            var fixedCode = @"public struct TestStruct
{
    public static readonly int Bar;
    public readonly void Method() { }
}";

            DiagnosticResult expected = Diagnostic().WithLocation(0).WithArguments("bar");

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(3001, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3001")]
        public async Task TestReadonlyMembersWithoutFieldsAsync()
        {
            var testCode = @"public struct TestStruct
{
    public readonly void Method() { }
    public int Property { readonly get; set; }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
