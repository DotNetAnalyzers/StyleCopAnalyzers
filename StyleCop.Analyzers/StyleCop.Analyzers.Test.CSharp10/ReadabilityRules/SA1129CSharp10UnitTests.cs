// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.Test.CSharp10.ReadabilityRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp9.ReadabilityRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.ReadabilityRules.SA1129DoNotUseDefaultValueTypeConstructor,
        StyleCop.Analyzers.ReadabilityRules.SA1129CodeFixProvider>;

    public partial class SA1129CSharp10UnitTests : SA1129CSharp9UnitTests
    {
        /// <summary>
        /// Verifies that <c>new S()</c> is reported for a struct with field initializers and only a parameterized
        /// constructor. The field initializers only run in the declared constructor, so <c>new S()</c> creates the
        /// default value.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [WorkItem(3980, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3980")]
        public async Task VerifyStructWithFieldInitializerAndParameterizedConstructorAsync()
        {
            var testCode = @"struct S
{
    private int x = 1;

    public S(int x) { this.x = x; }

    internal static S F() => [|new S()|];
}
";

            var fixedCode = @"struct S
{
    private int x = 1;

    public S(int x) { this.x = x; }

    internal static S F() => default(S);
}
";

            await VerifyCSharpFixAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(3430, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3430")]
        public async Task VerifyParameterlessStructConstructorAsync()
        {
            var testCode = @"struct S
{
    public S() { }

    internal static S F1()
    {
        S s = new S();
        return s;
    }

    internal static S F2()
    {
        S s = new();
        return s;
    }

    internal static S F3() => new S();

    internal static S F4() => new();
}
";

            await VerifyCSharpFixAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, testCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(3430, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3430")]
        public async Task VerifyParameterlessStructConstructorInMetadataAsync()
        {
            await new CSharpTest
            {
                TestState =
                {
                    Sources =
                    {
                        @"class B
{
    internal static S F1()
    {
        S s = new S();
        return s;
    }

    internal static S F2()
    {
        S s = new();
        return s;
    }

    internal static S F3() => new S();

    internal static S F4() => new();
}
",
                    },
                    AdditionalProjects =
                    {
                        ["Reference"] =
                        {
                            Sources =
                            {
                                @"public struct S { public S() { } }",
                            },
                        },
                    },
                    AdditionalProjectReferences = { "Reference" },
                },
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }
    }
}
