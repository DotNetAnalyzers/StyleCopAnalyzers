// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp9.ReadabilityRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Lightup;
    using StyleCop.Analyzers.Test.CSharp8.ReadabilityRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.ReadabilityRules.SA1129DoNotUseDefaultValueTypeConstructor,
        StyleCop.Analyzers.ReadabilityRules.SA1129CodeFixProvider>;

    public partial class SA1129CSharp9UnitTests : SA1129CSharp8UnitTests
    {
        /// <summary>
        /// Verifies that target type new expressions for value types will generate diagnostics.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [WorkItem(3277, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3277")]
        public async Task VerifyValueTypeWithTargetTypeNewAsync()
        {
            var testCode = @"struct S
{
    internal static S F()
    {
        S s = [|new()|];
        return s;
    }
}
";

            var fixedTestCode = @"struct S
{
    internal static S F()
    {
        S s = default(S);
        return s;
    }
}
";

            await VerifyCSharpFixAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(3969, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3969")]
        public async Task VerifyNativeSizedIntegerCreationAsync()
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

            var fixedTestCode = @"
using System;

class TestClass
{
    void TestMethod()
    {
        nint nativeInt = default(nint);
        nuint nativeUInt = default(nuint);
    }
}";

            // Force C# 9 language version even in later test scenarios
            await new CSharpTest(LanguageVersionEx.CSharp9)
            {
                TestCode = testCode,
                FixedCode = fixedTestCode,
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(3969, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3969")]
        public async Task VerifyNativeSizedIntegerDefaultParameterValuesAsync()
        {
            var testCode = @"
using System;

class TestClass
{
    void TestMethod(nint nativeInt = [|new nint()|], nuint nativeUInt = [|new nuint()|])
    {
    }
}";

            var fixedTestCode = @"
using System;

class TestClass
{
    void TestMethod(nint nativeInt = default(nint), nuint nativeUInt = default(nuint))
    {
    }
}";

            await VerifyCSharpFixAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
