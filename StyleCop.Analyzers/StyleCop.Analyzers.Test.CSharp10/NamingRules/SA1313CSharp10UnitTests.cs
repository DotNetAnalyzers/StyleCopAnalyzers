// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp10.NamingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.CSharp;
    using StyleCop.Analyzers.Test.CSharp9.NamingRules;
    using StyleCop.Analyzers.Test.Verifiers;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.NamingRules.SA1313ParameterNamesMustBeginWithLowerCaseLetter,
        StyleCop.Analyzers.NamingRules.RenameToLowerCaseCodeFixProvider>;

    public class SA1313CSharp10UnitTests : SA1313CSharp9UnitTests
    {
        [Theory]
        [WorkItem(3384, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3384")]
        [InlineData("class")]
        [InlineData("struct")]
        public async Task TestRecordTypeAsync(string typeKind)
        {
            var testCode = $@"
public record {typeKind} R(int A)
{{
    public R(int [|A|], int [|B|])
        : this(A)
    {{
    }}
}}
";

            var fixedCode = $@"
public record {typeKind} R(int A)
{{
    public R(int a, int b)
        : this(a)
    {{
    }}
}}
";

            await new CSharpTest(LanguageVersion.CSharp10)
            {
                ReferenceAssemblies = GenericAnalyzerTest.ReferenceAssembliesNet60,
                TestCode = testCode,
                FixedCode = fixedCode,
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }
    }
}
