// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp11.OrderingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using StyleCop.Analyzers.Test.CSharp10.OrderingRules;
    using StyleCop.Analyzers.Test.Verifiers;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.OrderingRules.SA1206DeclarationKeywordsMustFollowOrder,
        StyleCop.Analyzers.OrderingRules.SA1206CodeFixProvider>;

    public class SA1206CSharp11CodeFixProviderUnitTests : SA1206CSharp10CodeFixProviderUnitTests
    {
        [Fact]
        [WorkItem(3589, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3589")]
        public async Task VerifyFileKeywordReorderingInClassDeclarationAsync()
        {
            var testCode = $"static unsafe {{|#0:file|}} class TestClass {{}}";
            var fixedTestCode = $"file static unsafe class TestClass {{}}";

            var expected = Diagnostic().WithLocation(0).WithArguments("file", "unsafe");
            await VerifyCSharpFixAsync(testCode, expected, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(3527, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3527")]
        public async Task VerifyRequiredKeywordReorderingInPropertiesAndFieldsAsync()
        {
            var testCode = @"
internal struct SomeStruct
{
    required {|#0:public|} int Prop { get; set; }
    required {|#1:public|} int Field;
}";

            var fixedCode = @"
internal struct SomeStruct
{
    public required int Prop { get; set; }
    public required int Field;
}";

            await new CSharpTest()
            {
                ReferenceAssemblies = GenericAnalyzerTest.ReferenceAssembliesNet70,
                TestCode = testCode,
                FixedCode = fixedCode,
                ExpectedDiagnostics =
                {
                    Diagnostic().WithLocation(0).WithArguments("public", "required"),
                    Diagnostic().WithLocation(1).WithArguments("public", "required"),
                },
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }
    }
}
