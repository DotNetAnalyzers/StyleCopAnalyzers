// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp8.SpacingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp7.SpacingRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.SpacingRules.SA1018NullableTypeSymbolsMustNotBePrecededBySpace,
        StyleCop.Analyzers.SpacingRules.SA1018CodeFixProvider>;

    public partial class SA1018CSharp8UnitTests : SA1018CSharp7UnitTests
    {
        /// <summary>
        /// Verifies that nullable reference type annotations are handled without diagnostics when spaced correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [WorkItem(3006, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3006")]
        public async Task TestNullableReferenceTypesNoDiagnosticsAsync()
        {
            const string testCode = @"#nullable enable

class TestClass
{
    private string? field;
    private string?[] array;
    private System.Collections.Generic.List<string?>? list;

    public string? Property { get; set; }

    public void TestMethod(string? parameter, out string? output)
    {
        output = parameter;
    }

    public (string? first, string? second) TupleMethod()
    {
        return (null, null);
    }
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that spacing diagnostics are reported for nullable reference types preceded by whitespace.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [WorkItem(3006, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3006")]
        public async Task TestNullableReferenceTypeSpacingAsync()
        {
            var testCode = @"#nullable enable

class TestClass
{
    string {|#0:?|} field1;
    string /* comment */ {|#1:?|} field2;
    string
{|#2:?|} field3;
    string
/* comment */
{|#3:?|} field4;
}
";

            var fixedCode = @"#nullable enable

class TestClass
{
    string? field1;
    string/* comment */? field2;
    string? field3;
    string/* comment */? field4;
}
";

            DiagnosticResult[] expected =
            {
                Diagnostic().WithLocation(0),
                Diagnostic().WithLocation(1),
                Diagnostic().WithLocation(2),
                Diagnostic().WithLocation(3),
            };
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
