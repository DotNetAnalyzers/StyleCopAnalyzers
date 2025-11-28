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
        StyleCop.Analyzers.SpacingRules.SA1015ClosingGenericBracketsMustBeSpacedCorrectly,
        StyleCop.Analyzers.SpacingRules.TokenSpacingCodeFixProvider>;

    public partial class SA1015CSharp8UnitTests : SA1015CSharp7UnitTests
    {
        [Fact]
        [WorkItem(3302, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3302")]
        public async Task TestGenericTypePointerAsync()
        {
            const string testCode = @"using System;

public struct Foo<T>
{
    internal unsafe Foo<T [|>|] * Next1;
    internal unsafe Foo<T [|>|]* Next2;
}";
            const string fixedCode = @"using System;

public struct Foo<T>
{
    internal unsafe Foo<T> * Next1;
    internal unsafe Foo<T>* Next2;
}";

            await VerifyCSharpFixAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that nullable reference type annotations following closing generic brackets do not produce diagnostics.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [WorkItem(3006, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3006")]
        public async Task TestNullableReferenceTypeAnnotationsAsync()
        {
            const string testCode = @"#nullable enable
using System.Collections.Generic;

public class TestClass
{
    private List<string?>? names;
    private Dictionary<string, List<object?>?>? items;

    public IReadOnlyList<List<string?>?>? Property { get; }

    public void TestMethod()
    {
        List<(string? key, List<string?>? values)>? local = null;
        List<string?>?[]? jagged = null;
    }
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that nullable reference type constraints involving generic types are handled without diagnostics.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [WorkItem(3006, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3006")]
        public async Task TestNullableReferenceTypeConstraintsAsync()
        {
            const string testCode = @"#nullable enable
using System.Collections.Generic;

public class ConstraintClass<T> where T : class
{
    public void TestMethod<TValue>(TValue? value)
        where TValue : class, IEnumerable<T?>?
    {
        if (value is List<T?> items)
        {
            _ = items;
        }
    }
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
