// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.Test.CSharp7.LayoutRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.LayoutRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.LayoutRules.SA1515SingleLineCommentMustBePrecededByBlankLine,
        StyleCop.Analyzers.LayoutRules.SA1515CodeFixProvider>;

    public partial class SA1515CSharp7UnitTests : SA1515UnitTests
    {
        [Fact]
        public async Task TestCommentAfterCasePatternSwitchLabelAsync()
        {
            var testCode = @"
public class ClassName
{
    public void Method()
    {
        switch (new object())
        {
            case int x:
                // Single line comment after pattern-matching case statement is valid
                break;
        }
    }
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the analyzer does not fire in expression bodied property accessors.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [WorkItem(3550, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3550")]
        public async Task TestExpressionBodiedPropertyAccessorsAsync()
        {
            var testCode = @"
class TestClass
{
    public int TestProperty
    {
        get =>
            // A comment line
            42;

        set =>
            // A comment line
            _ = value;
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the analyzer does not fire in expression bodied indexer accessors.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [WorkItem(3550, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3550")]
        public async Task TestExpressionBodiedIndexerAccessorsAsync()
        {
            var testCode = @"
class TestClass
{
    public int this[int i]
    {
        get =>
            // A comment line
            42;

        set =>
            // A comment line
            _ = value;
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the analyzer does not fire in expression bodied event accessors.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [WorkItem(3550, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3550")]
        public async Task TestExpressionBodiedEventAccessorsAsync()
        {
            var testCode = @"
class TestClass
{
    public event System.Action TestEvent
    {
        add =>
            // A comment line
            _ = value;

        remove =>
            // A comment line
            _ = value;
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
