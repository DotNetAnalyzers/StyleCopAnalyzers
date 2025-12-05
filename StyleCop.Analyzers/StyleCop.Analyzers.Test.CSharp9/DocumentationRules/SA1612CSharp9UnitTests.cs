// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp9.DocumentationRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp8.DocumentationRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.CustomDiagnosticVerifier<StyleCop.Analyzers.DocumentationRules.SA1612ElementParameterDocumentationMustMatchElementParameters>;

    public partial class SA1612CSharp9UnitTests : SA1612CSharp8UnitTests
    {
        [Fact]
        [WorkItem(3977, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3977")]
        public async Task TestLambdaDiscardParametersAsync()
        {
            var testCode = @"
/// <summary>Test class.</summary>
public class TestClass
{
    /// <summary>Test method.</summary>
    public void TestMethod()
    {
        System.Func<int, int, int> handler = (_, _) => 0;
    }
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(3971, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3971")]
        public async Task TestPartialMethodDeclarationWithMismatchedParameterDocumentationAsync()
        {
            var testCode = @"
/// <summary>
/// Tests a partial method.
/// </summary>
public partial class TestClass
{
    /// <summary>Declaration.</summary>
    /// <param name=""{|#0:value|}"">Value.</param>
    public partial void TestMethod(int other);

    public partial void TestMethod(int other)
    {
    }
}";

            var expected = Diagnostic().WithLocation(0).WithArguments("value");

            await VerifyCSharpDiagnosticAsync(testCode, new[] { expected }, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
