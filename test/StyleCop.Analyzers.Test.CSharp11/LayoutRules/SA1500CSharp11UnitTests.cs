// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp11.LayoutRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp10.LayoutRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.LayoutRules.SA1500BracesForMultiLineStatementsMustNotShareLine,
        StyleCop.Analyzers.LayoutRules.SA1500CodeFixProvider>;

    public partial class SA1500CSharp11UnitTests : SA1500CSharp10UnitTests
    {
        /// <summary>
        /// Verifies that no diagnostic is reported when the closing brace of a switch expression is followed by the
        /// closing brace of an interpolation, or by its format clause. From C# 11, the expression of an interpolation
        /// can span multiple lines in any interpolated string.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [WorkItem(3997, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3997")]
        public async Task TestSwitchExpressionInInterpolationAsync()
        {
            var testCode = @"
public class TestClass
{
    public string TestMethod(int x)
    {
        var a = $""{x switch
        {
            1 => ""one"",
            _ => ""other"",
        }}"";

        var b = $""{x switch
        {
            1 => 1.0,
            _ => 2.0,
        }:N2}"";

        var c = $""""""{x switch
        {
            1 => ""one"",
            _ => ""other"",
        }}"""""";

        return a + b + c;
    }
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
