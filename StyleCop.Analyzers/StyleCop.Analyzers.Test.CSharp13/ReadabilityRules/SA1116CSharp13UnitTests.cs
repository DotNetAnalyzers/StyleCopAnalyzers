// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp13.ReadabilityRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp12.ReadabilityRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.ReadabilityRules.SA1116SplitParametersMustStartOnLineAfterDeclaration,
        StyleCop.Analyzers.ReadabilityRules.SA1116CodeFixProvider>;

    public partial class SA1116CSharp13UnitTests : SA1116CSharp12UnitTests
    {
        [Fact]
        [WorkItem(4013, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4013")]
        public async Task TestSplitParamsCollectionParameterNotStartingOnNextLineAsync()
        {
            var testCode = @"
using System;

class Foo
{
    public Foo([|int a|],
        params ReadOnlySpan<int> s) { }
}";

            var fixedCode = @"
using System;

class Foo
{
    public Foo(
        int a,
        params ReadOnlySpan<int> s) { }
}";

            await VerifyCSharpFixAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(4013, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4013")]
        public async Task TestSplitParamsCollectionParameterOnNetFrameworkAsync()
        {
            // params ReadOnlySpan<T> needs runtime support that .NET Framework reference assemblies don't have, so the
            // snippet does not compile there. SA1116 still has to report and fix the parameter list.
            var testCode = @"
using System;

class Foo
{
    public Foo([|int a|],
        params ReadOnlySpan<int> s) { }
}";

            var fixedCode = @"
using System;

class Foo
{
    public Foo(
        int a,
        params ReadOnlySpan<int> s) { }
}";

            await new CSharpTest
            {
                TestCode = testCode,
                FixedCode = fixedCode,
                ReferenceAssemblies = ReferenceAssemblies.NetFramework.Net472.Default,
                CompilerDiagnostics = CompilerDiagnostics.None,
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }

        protected override DiagnosticResult[] GetExpectedResultTestPrimaryConstructorBaseListSplitArgumentsNotStartingOnNextLine()
        {
            return new[]
            {
                // Diagnostic previously issued twice because of https://github.com/dotnet/roslyn/issues/70488
                Diagnostic().WithLocation(0),
            };
        }
    }
}
