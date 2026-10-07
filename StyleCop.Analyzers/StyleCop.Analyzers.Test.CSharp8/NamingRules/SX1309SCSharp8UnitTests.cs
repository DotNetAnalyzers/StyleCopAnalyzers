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
        StyleCop.Analyzers.NamingRules.SX1309SStaticFieldNamesMustBeginWithUnderscore,
        StyleCop.Analyzers.NamingRules.SX1309CodeFixProvider>;

    public partial class SX1309SCSharp8UnitTests : SX1309SCSharp7UnitTests
    {
        [Fact]
        [WorkItem(3001, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3001")]
        public async Task TestReadonlyMembersDoNotTriggerAsync()
        {
            var testCode = @"public struct TestStruct
{
    public readonly void Method() { }
    public int Property { readonly get; set; }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(3001, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3001")]
        public async Task TestPrivateStaticFieldRequiresUnderscoreAsync()
        {
            var testCode = @"public struct TestStruct
{
    private static int {|#0:value|};
    public readonly void Method() { }
}";

            var fixedCode = @"public struct TestStruct
{
    private static int _value;
    public readonly void Method() { }
}";

            DiagnosticResult expected = Diagnostic().WithLocation(0).WithArguments("value");

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(3001, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3001")]
        public async Task TestStaticReadonlyFieldIgnoredAsync()
        {
            var testCode = @"public struct TestStruct
{
    private static readonly int value = 0;
    public readonly void Method() { }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
