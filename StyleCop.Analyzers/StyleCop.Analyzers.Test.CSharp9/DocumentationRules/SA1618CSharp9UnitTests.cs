// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp9.DocumentationRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp8.DocumentationRules;
    using StyleCop.Analyzers.Test.Helpers;
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

        [Theory]
        [MemberData(nameof(CommonMemberData.TypeKeywordsWhichSupportPrimaryConstructors), MemberType = typeof(CommonMemberData))]
        [WorkItem(4006, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4006")]
        public async Task TestGenericTypeWithPrimaryConstructorWithoutTypeParameterDocumentationAsync(string typeKeyword)
        {
            var testCode = $@"/// <summary>The type.</summary>
public {typeKeyword} TestType<{{|#0:T|}}>(T X);";

            var expected = this.GetExpectedResultTestGenericTypeWithPrimaryConstructorWithoutTypeParameterDocumentation();
            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        protected virtual DiagnosticResult[] GetExpectedResultTestGenericTypeWithPrimaryConstructorWithoutTypeParameterDocumentation()
        {
            return new[]
            {
                // Diagnostic issued twice because of https://github.com/dotnet/roslyn/issues/53136
                Diagnostic().WithLocation(0).WithArguments("T"),
                Diagnostic().WithLocation(0).WithArguments("T"),
            };
        }
    }
}
