// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp8.ReadabilityRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp7.ReadabilityRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopDiagnosticVerifier<StyleCop.Analyzers.ReadabilityRules.SA1125UseShorthandForNullableTypes>;

    public partial class SA1125CSharp8UnitTests : SA1125CSharp7UnitTests
    {
        /// <summary>
        /// Verifies that the rule continues to report diagnostics for value type nullable long forms when nullable annotations are enabled.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [WorkItem(3006, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3006")]
        public async Task TestNullableEnableWithLongFormValueTypesAsync()
        {
            const string testCode = @"#nullable enable

using System;

class TestClass<T>
    where T : struct
{
    private Nullable<int> field1;
    private System.Nullable<int> field2;
    private global::System.Nullable<T> field3;

    private string? referenceField;
}";

            var expectedDiagnostics = new[]
            {
                Diagnostic().WithLocation(8, 13),
                Diagnostic().WithLocation(9, 13),
                Diagnostic().WithLocation(10, 13),
            };

            await VerifyCSharpDiagnosticAsync(testCode, expectedDiagnostics, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that nullable reference type annotations are not reported as violations of SA1125.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [WorkItem(3006, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3006")]
        public async Task TestNullableReferenceAnnotationsNoDiagnosticsAsync()
        {
            const string testCode = @"#nullable enable

using System.Collections.Generic;

class TestClass<T>
    where T : class
{
    private string? field;
    private List<string?>? list;

    public T? Property { get; set; }

    public T? Method(T? parameter)
    {
        return parameter;
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
