// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp7.NamingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using StyleCop.Analyzers.Test.NamingRules;
    using TestHelper;
    using Xunit;

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
                this.CSharpDiagnostic().WithLocation(5, 14).WithArguments("variable", "baR"),
                this.CSharpDiagnostic().WithLocation(5, 19).WithArguments("variable", "caRe"),
                this.CSharpDiagnostic().WithLocation(5, 25).WithArguments("variable", "daRE"),
                this.CSharpDiagnostic().WithLocation(5, 31).WithArguments("variable", "fAre"),
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
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
                this.CSharpDiagnostic().WithArguments("parameter", "abX").WithLocation(6, 33),
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
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

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
