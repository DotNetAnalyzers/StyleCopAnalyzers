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
        StyleCop.Analyzers.ReadabilityRules.SA1122UseStringEmptyForEmptyStrings,
        StyleCop.Analyzers.ReadabilityRules.SA1122CodeFixProvider>;

    public partial class SA1122CSharp11UnitTests : SA1122CSharp10UnitTests
    {
        /// <summary>
        /// Verifies that an empty UTF-8 string literal is not reported, because it is not a <see cref="string"/>.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [WorkItem(3996, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3996")]
        public async Task TestEmptyUtf8StringLiteralAsync()
        {
            var testCode = @"using System;

class TestClass
{
    void TestMethod()
    {
        ReadOnlySpan<byte> value = """"u8;
    }
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that an empty string used as a constant pattern for a <see cref="System.ReadOnlySpan{T}"/> of
        /// <see cref="char"/> is not reported, because patterns require constants.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [WorkItem(4002, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4002")]
        public async Task TestEmptyStringPatternOnSpanAsync()
        {
            var testCode = @"using System;

class TestClass
{
    int TestMethod(ReadOnlySpan<char> value)
    {
        if (value is """")
        {
            return 0;
        }

        return value switch
        {
            """" => 1,
            ""a"" => 2,
            _ => 3,
        };
    }
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
