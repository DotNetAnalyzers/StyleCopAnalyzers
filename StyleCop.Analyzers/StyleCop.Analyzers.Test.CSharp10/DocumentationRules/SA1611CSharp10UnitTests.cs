// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp10.DocumentationRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp9.DocumentationRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.CustomDiagnosticVerifier<StyleCop.Analyzers.DocumentationRules.SA1611ElementParametersMustBeDocumented>;

    public partial class SA1611CSharp10UnitTests : SA1611CSharp9UnitTests
    {
        /// <summary>
        /// Verifies that a documented parameter with <c>[CallerArgumentExpression]</c> is not reported.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [WorkItem(3991, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3991")]
        public async Task TestDocumentedCallerArgumentExpressionParameterAsync()
        {
            var testCode = @"using System.Runtime.CompilerServices;

/// <summary>A class.</summary>
public class TestClass
{
    /// <summary>Checks a value.</summary>
    /// <param name=""value"">The value.</param>
    /// <param name=""expression"">The expression passed for <paramref name=""value""/>.</param>
    public static void Check(bool value, [CallerArgumentExpression(""value"")] string expression = null)
    {
    }
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
