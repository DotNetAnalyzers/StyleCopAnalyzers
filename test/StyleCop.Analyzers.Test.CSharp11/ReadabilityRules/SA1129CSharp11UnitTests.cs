// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp11.ReadabilityRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp10.ReadabilityRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.ReadabilityRules.SA1129DoNotUseDefaultValueTypeConstructor,
        StyleCop.Analyzers.ReadabilityRules.SA1129CodeFixProvider>;

    public partial class SA1129CSharp11UnitTests : SA1129CSharp10UnitTests
    {
        [Fact]
        [WorkItem(3969, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3969")]
        public async Task VerifyNativeSizedIntegerCreationWithNet70Async()
        {
            var testCode = @"
using System;

class TestClass
{
    void TestMethod()
    {
        nint nativeInt = [|new nint()|];
        nuint nativeUInt = [|new nuint()|];
    }
}";

            var fixedCode = @"
using System;

class TestClass
{
    void TestMethod()
    {
        nint nativeInt = nint.Zero;
        nuint nativeUInt = nuint.Zero;
    }
}";

            await VerifyCSharpFixAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(3969, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3969")]
        public async Task VerifyNativeSizedIntegerCreationWithNet60Async()
        {
            var testCode = @"
using System;

class TestClass
{
    void TestMethod()
    {
        nint nativeInt = [|new nint()|];
        nuint nativeUInt = [|new nuint()|];
    }
}";

            var fixedCode = @"
using System;

class TestClass
{
    void TestMethod()
    {
        nint nativeInt = default(nint);
        nuint nativeUInt = default(nuint);
    }
}";

            await new CSharpTest
            {
                ReferenceAssemblies = ReferenceAssemblies.Net.Net60,
                TestCode = testCode,
                FixedCode = fixedCode,
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }
    }
}
