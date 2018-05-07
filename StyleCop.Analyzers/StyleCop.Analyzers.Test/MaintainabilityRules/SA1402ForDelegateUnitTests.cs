// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.MaintainabilityRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using TestHelper;
    using Xunit;

    public class SA1402ForDelegateUnitTests : SA1402ForNonBlockDeclarationUnitTestsBase
    {
        public override string Keyword => "delegate";

        [Fact]
        public async Task TestOneElementAsync()
        {
            var testCode = @"public delegate void Foo();";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestTwoElementsAsync()
        {
            var testCode = @"public delegate void Foo();
public delegate void Bar();
";

            var fixedFileNames = new[] { "Test0.cs", "Bar.cs" };
            var fixedCode = new[]
            {
                @"public delegate void Foo();
",
                @"public delegate void Bar();
",
            };

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(2, 22);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(new[] { testCode }, fixedCode, newFileNames: fixedFileNames, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestTwoGenericElementsAsync()
        {
            var testCode = @"public delegate void Foo();
public delegate void Bar<T1, T2, T3>(T1 x, T2 y, T3 z);
";

            var fixedFileNames = new[] { "Test0.cs", "Bar.cs" };
            var fixedCode = new[]
            {
                @"public delegate void Foo();
",
                @"public delegate void Bar<T1, T2, T3>(T1 x, T2 y, T3 z);
",
            };

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(2, 22);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(new[] { testCode }, fixedCode, newFileNames: fixedFileNames, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestTwoElementsWithRuleDisabledAsync()
        {
            this.SettingsConfiguration = SA1402SettingsConfiguration.ConfigureAsNonTopLevelType;

            var testCode = @"public delegate void Foo();
public delegate void Bar();
";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestTwoElementsWithDefaultRuleConfigurationAsync()
        {
            this.SettingsConfiguration = SA1402SettingsConfiguration.KeepDefaultConfiguration;

            var testCode = @"public delegate void Foo();
public delegate void Bar();
";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestThreeElementsAsync()
        {
            var testCode = @"public delegate void Foo();
public delegate void Bar();
public delegate void FooBar();
";

            var fixedFileNames = new[] { "Test0.cs", "Bar.cs", "FooBar.cs" };
            var fixedCode = new[]
            {
                @"public delegate void Foo();
",
                @"public delegate void Bar();
",
                @"public delegate void FooBar();
",
            };

            DiagnosticResult[] expected =
            {
                this.CSharpDiagnostic().WithLocation(2, 22),
                this.CSharpDiagnostic().WithLocation(3, 22),
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(new[] { testCode }, fixedCode, newFileNames: fixedFileNames, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestPreferFilenameTypeAsync()
        {
            var testCode = @"public delegate void Foo();
public delegate void Test0();
";

            var fixedFileNames = new[] { "Test0.cs", "Foo.cs" };
            var fixedCode = new[]
            {
                $@"public delegate void Test0();
",
                $@"public delegate void Foo();
",
            };

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(1, 22);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(new[] { testCode }, fixedCode, newFileNames: fixedFileNames, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }
    }
}
