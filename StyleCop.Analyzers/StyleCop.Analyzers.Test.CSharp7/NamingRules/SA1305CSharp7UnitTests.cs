// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.Test.CSharp7.NamingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.NamingRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopDiagnosticVerifier<StyleCop.Analyzers.NamingRules.SA1305FieldNamesMustNotUseHungarianNotation>;

    public class SA1305CSharp7UnitTests : SA1305UnitTests
    {
        [Fact]
        public async Task TestInvalidVariableDesignatorNamesAreReportedAsync()
        {
            var testCode = @" public class TestClass
{
    public void TestMethod()
    {
        var (baR, caRe, daRE, fAre) = (1, 2, 3, 4);
    }
}
";

            DiagnosticResult[] expected =
            {
                Diagnostic().WithLocation(5, 14).WithArguments("variable", "baR"),
                Diagnostic().WithLocation(5, 19).WithArguments("variable", "caRe"),
                Diagnostic().WithLocation(5, 25).WithArguments("variable", "daRE"),
                Diagnostic().WithLocation(5, 31).WithArguments("variable", "fAre"),
            };

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestParameterInLocalFunctionAsync()
        {
            var testCode = @"
public class TypeName
{
    public void MethodName()
    {
        void LocalFunction(bool abX)
        {
        }
    }
}";

            DiagnosticResult[] expected =
            {
                Diagnostic().WithArguments("parameter", "abX").WithLocation(6, 33),
            };

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestParameterInNativeClassLocalFunctionAsync()
        {
            var testCode = @"
public class TypeNameNativeMethods
{
    public void MethodName()
    {
        void LocalFunction(bool abX)
        {
        }
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
