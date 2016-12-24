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

            var fixedFileNames = new[] { "Test0.cs", "Bar.cs" };
            var fixedCode = new[]
            {
                @"enum Foo
{
    A, B, C
}
",
                @"enum Bar
{
    D, E
}
",
            };

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(5, 6);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(new[] { testCode }, fixedCode, newFileNames: fixedFileNames, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestTwoElementsWithRuleDisabledAsync()
        {
            this.SettingsConfiguration = SA1402SettingsConfiguration.ConfigureAsNonTopLevelType;

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
        public async Task TestTwoElementsWithDefaultRuleConfigurationAsync()
        {
            this.SettingsConfiguration = SA1402SettingsConfiguration.KeepDefaultConfiguration;

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

            var fixedFileNames = new[] { "Test0.cs", "Bar.cs", "FooBar.cs" };
            var fixedCode = new[]
            {
                @"enum Foo
{
    A, B, C
}
",
                @"enum Bar
{
    D, E
}
",
                @"enum FooBar
{
    F, G, H
}
",
            };

            DiagnosticResult[] expected =
            {
                this.CSharpDiagnostic().WithLocation(5, 6),
                this.CSharpDiagnostic().WithLocation(9, 6),
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(new[] { testCode }, fixedCode, newFileNames: fixedFileNames, cancellationToken: CancellationToken.None).ConfigureAwait(false);
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

            var fixedFileNames = new[] { "Test0.cs", "Foo.cs" };
            var fixedCode = new[]
            {
                @"enum Test0
{
    D, E
}
",
                @"enum Foo
{
    A, B, C
}
",
            };

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(1, 6);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(new[] { testCode }, fixedCode, newFileNames: fixedFileNames, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }
    }
}
