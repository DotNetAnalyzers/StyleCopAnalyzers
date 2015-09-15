// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.MaintainabilityRules
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.MaintainabilityRules;
    using TestHelper;
    using Xunit;

    public class SA1408UnitTests : CodeFixVerifier
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
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(5, 26);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);

            var fixedCode = @"public class Foo
{
    public void Bar()
    {
        bool x = true || (false && true);
    }
}";

            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
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
            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(5, 18);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);

            var fixedCode = @"public class Foo
{
    public void Bar()
    {
        bool x = (true && false) || true;
    }
}";

            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
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
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
                    this.CSharpDiagnostic().WithLocation(5, 18),
                    this.CSharpDiagnostic().WithLocation(5, 35)
                };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);

            var fixedCode = @"public class Foo
{
    public void Bar()
    {
        bool x = (true && false) || (true && false);
    }
}";

            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
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
                    this.CSharpDiagnostic().WithLocation(6, 18),
                    this.CSharpDiagnostic().WithLocation(6, 35),
                    this.CSharpDiagnostic().WithLocation(7, 26),
                    this.CSharpDiagnostic().WithLocation(9, 22),
                };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);

            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
        }

        protected override IEnumerable<DiagnosticAnalyzer> GetCSharpDiagnosticAnalyzers()
        {
            yield return new SA1408ConditionalExpressionsMustDeclarePrecedence();
        }

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new SA1407SA1408CodeFixProvider();
        }
    }
}