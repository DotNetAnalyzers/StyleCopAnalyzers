// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp9.DocumentationRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using StyleCop.Analyzers.Test.CSharp8.DocumentationRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.CustomDiagnosticVerifier<StyleCop.Analyzers.DocumentationRules.SA1615ElementReturnValueMustBeDocumented>;

    public partial class SA1615CSharp9UnitTests : SA1615CSharp8UnitTests
    {
        [Fact]
        [WorkItem(3971, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3971")]
        public async Task TestPartialMethodDeclarationMissingReturnsDocumentationAsync()
        {
            var testCode = @"
/// <summary>
/// Tests a partial method.
/// </summary>
public partial class TestClass
{
    /// <summary>Declaration.</summary>
    public partial {|#0:int|} TestMethod(int value);

    public partial int TestMethod(int value) => value;
}";

            var expected = Diagnostic().WithLocation(0);

            await VerifyCSharpDiagnosticAsync(testCode, new[] { expected }, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
