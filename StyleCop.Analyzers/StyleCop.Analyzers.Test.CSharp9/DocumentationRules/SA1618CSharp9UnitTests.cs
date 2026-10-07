// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp9.DocumentationRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using StyleCop.Analyzers.Test.CSharp8.DocumentationRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.CustomDiagnosticVerifier<StyleCop.Analyzers.DocumentationRules.SA1618GenericTypeParametersMustBeDocumented>;

    public partial class SA1618CSharp9UnitTests : SA1618CSharp8UnitTests
    {
        [Fact]
        [WorkItem(3971, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3971")]
        public async Task TestPartialMethodDeclarationMissingTypeParameterDocumentationAsync()
        {
            var testCode = @"
/// <summary>
/// Tests a partial method.
/// </summary>
public partial class TestClass
{
    /// <summary>Declaration.</summary>
    public partial void TestMethod<{|#0:T|}>();

    public partial void TestMethod<T>()
    {
    }
}";

            var expected = Diagnostic().WithLocation(0).WithArguments("T");

            await VerifyCSharpDiagnosticAsync(testCode, new[] { expected }, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
