// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.MaintainabilityRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using TestHelper;
    using Xunit;

    public class SA1402ForEnumUnitTests : SA1402ForNonBlockDeclarationUnitTestsBase
    {
        public override string Keyword => "enum";

        [Fact]
        public async Task TestOneElementAsync()
        {
            var testCode = @"enum Foo
{
    A, B, C
}
";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestTwoElementsAsync()
        {
            var testCode = @"enum Foo
{
    A, B, C
}

enum Bar
{
    D, E
}
";

            var fixedCode = new[]
            {
                @"enum Foo
{
    A, B, C
}
",

                // There should be no leading whitespace here... Why are there?
                @"
enum Bar
{
    D, E
}
"
            };

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(6, 6);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(new[] { testCode }, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestTwoElementsWithRuleDisabledAsync()
        {
            this.ConfigureAsNonTopLevelType = true;

            var testCode = @"enum Foo
{
    A, B, C
}

enum Bar
{
    D, E
}
";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestThreeElementsAsync()
        {
            var testCode = @"enum Foo
{
    A, B, C
}

enum Bar
{
    D, E
}

enum FooBar
{
    F, G, H
}
";

            var fixedCode = new[]
            {
                @"enum Foo
{
    A, B, C
}
",

                // There should be no leading whitespace here... Why are there?
                @"
enum Bar
{
    D, E
}
",

                // There should be no leading whitespace here... Why are there?
                @"
enum FooBar
{
    F, G, H
}
"
            };

            DiagnosticResult[] expected =
            {
                this.CSharpDiagnostic().WithLocation(6, 6),
                this.CSharpDiagnostic().WithLocation(11, 6)
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(new[] { testCode }, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestPreferFilenameTypeAsync()
        {
            var testCode = @"enum Foo
{
    A, B, C
}

enum Test0
{
    D, E
}
";

            var fixedCode = new[]
            {
                // There should be no leading whitespace here... Why are there?
                @"
enum Test0
{
    D, E
}
",
                @"enum Foo
{
    A, B, C
}
"
            };

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(1, 6);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(new[] { testCode }, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }
    }
}
