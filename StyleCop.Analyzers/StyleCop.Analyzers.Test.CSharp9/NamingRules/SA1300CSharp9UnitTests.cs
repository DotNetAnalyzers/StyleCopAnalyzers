// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp9.NamingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.CSharp;
    using StyleCop.Analyzers.Test.CSharp8.NamingRules;
    using StyleCop.Analyzers.Test.Verifiers;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.NamingRules.SA1300ElementMustBeginWithUpperCaseLetter,
        StyleCop.Analyzers.NamingRules.RenameToUpperCaseCodeFixProvider>;

    public class SA1300CSharp9UnitTests : SA1300CSharp8UnitTests
    {
        [Fact]
        public async Task TestPositionalRecord1Async()
        {
            var testCode = @"
public record {|#0:r|}(int A)
{
    public r(int a, int b)
        : this(A: a)
    {
    }
}
";

            var fixedCode = @"
public record R(int A)
{
    public R(int a, int b)
        : this(A: a)
    {
    }
}
";

            await new CSharpTest(LanguageVersion.CSharp9)
            {
                ReferenceAssemblies = GenericAnalyzerTest.ReferenceAssembliesNet50,
                TestCode = testCode,
                ExpectedDiagnostics =
                {
                    // /0/Test0.cs(2,15): warning SA1300: Element 'r' should begin with an uppercase letter
                    Diagnostic().WithLocation(0).WithArguments("r"),

                    // /0/Test0.cs(2,15): warning SA1300: Element 'r' should begin with an uppercase letter
                    Diagnostic().WithLocation(0).WithArguments("r"),
                },
                FixedCode = fixedCode,
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestPositionalRecord2Async()
        {
            var testCode = @"
public record R(int [|a|])
{
    public R(int a, int b)
        : this(a: a)
    {
    }
}
";

            var fixedCode = @"
public record R(int A)
{
    public R(int a, int b)
        : this(A: a)
    {
    }
}
";

            await new CSharpTest(LanguageVersion.CSharp9)
            {
                ReferenceAssemblies = GenericAnalyzerTest.ReferenceAssembliesNet50,
                TestCode = testCode,
                FixedCode = fixedCode,
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }
    }
}
