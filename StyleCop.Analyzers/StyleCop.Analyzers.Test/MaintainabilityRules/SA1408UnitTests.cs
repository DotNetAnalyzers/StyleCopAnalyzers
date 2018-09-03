// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.MaintainabilityRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using TestHelper;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.MaintainabilityRules.SA1408ConditionalExpressionsMustDeclarePrecedence,
        StyleCop.Analyzers.MaintainabilityRules.SA1407SA1408CodeFixProvider>;

    public class SA1408UnitTests
    {
        [Fact]
        public async Task TestOrAsync()
        {
            var testCode = @"public class Foo
{
    public void Bar()
    {
        bool x = true || false || true || false;
    }
}";
            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestAndAsync()
        {
            var testCode = @"public class Foo
{
    public void Bar()
    {
        bool x = true && false && true && false;
    }
}";
            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestOrAndAndAsync()
        {
            var testCode = @"public class Foo
{
    public void Bar()
    {
        bool x = true || false && true;
    }
}";
            DiagnosticResult expected = Diagnostic().WithLocation(5, 26);

            var fixedCode = @"public class Foo
{
    public void Bar()
    {
        bool x = true || (false && true);
    }
}";

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestAndAndOrAsync()
        {
            var testCode = @"public class Foo
{
    public void Bar()
    {
        bool x = true && false || true;
    }
}";
            DiagnosticResult expected = Diagnostic().WithLocation(5, 18);

            var fixedCode = @"public class Foo
{
    public void Bar()
    {
        bool x = (true && false) || true;
    }
}";

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestOrAndAndParenthesizedAsync()
        {
            var testCode = @"public class Foo
{
    public void Bar()
    {
        bool x = (true || false) && true;
    }
}";
            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestOrAndEqualsParenthesizedAsync()
        {
            var testCode = @"public class Foo
{
    public void Bar()
    {
        bool x = true || (false == true);
    }
}";
            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestAndAndEqualsAsync()
        {
            var testCode = @"public class Foo
{
    public void Bar()
    {
        bool x = true && false == true;
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestAndAndOrParenthesizedAsync()
        {
            var testCode = @"public class Foo
{
    public void Bar()
    {
        bool x = (true && false) || true;
    }
}";
            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestMultipleViolationsAsync()
        {
            var testCode = @"public class Foo
{
    public void Bar()
    {
        bool x = true && false || true && false;
    }
}";
            DiagnosticResult[] expected =
                {
                    Diagnostic().WithLocation(5, 18),
                    Diagnostic().WithLocation(5, 35),
                };

            var fixedCode = @"public class Foo
{
    public void Bar()
    {
        bool x = (true && false) || (true && false);
    }
}";

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestCodeFixAsync()
        {
            var testCode = @"public class Foo
{
    public void Bar()
    {
        bool b = true, c = true;
        bool x = true && false || true && false;
        bool y = true || x && b && c;
        // the following test makes sure the code fix doesn't alter spacing
        bool z = b ? true&&true||false :false;
    }
}";
            var fixedCode = @"public class Foo
{
    public void Bar()
    {
        bool b = true, c = true;
        bool x = (true && false) || (true && false);
        bool y = true || (x && b && c);
        // the following test makes sure the code fix doesn't alter spacing
        bool z = b ? (true&&true)||false :false;
    }
}";

            DiagnosticResult[] expected =
                {
                    Diagnostic().WithLocation(6, 18),
                    Diagnostic().WithLocation(6, 35),
                    Diagnostic().WithLocation(7, 26),
                    Diagnostic().WithLocation(9, 22),
                };

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
