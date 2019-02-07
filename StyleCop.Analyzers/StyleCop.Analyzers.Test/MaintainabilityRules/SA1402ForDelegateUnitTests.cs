// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.MaintainabilityRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using TestHelper;
    using Xunit;

    public class SA1402ForDelegateUnitTests : SA1402ForNonBlockDeclarationUnitTestsBase
    {
        public override string Keyword => "delegate";

        [Fact]
        public async Task TestOneElementAsync()
        {
            var testCode = @"public delegate void Foo();";

            await VerifyCSharpDiagnosticAsync(testCode, this.GetSettings(), DiagnosticResult.EmptyDiagnosticResults).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestTwoElementsAsync()
        {
            var testCode = @"public delegate void Foo();
public delegate void Bar();
";

            var fixedCode = new[]
            {
                ("Test0.cs", @"public delegate void Foo();
"),
                ("Bar.cs", @"public delegate void Bar();
"),
            };

            DiagnosticResult expected = Diagnostic().WithLocation(2, 22);
            await VerifyCSharpFixAsync(testCode, this.GetSettings(), expected, fixedCode).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestTwoGenericElementsAsync()
        {
            var testCode = @"public delegate void Foo();
public delegate void Bar<T1, T2, T3>(T1 x, T2 y, T3 z);
";

            var fixedCode = new[]
            {
                ("Test0.cs", @"public delegate void Foo();
"),
                ("Bar.cs", @"public delegate void Bar<T1, T2, T3>(T1 x, T2 y, T3 z);
"),
            };

            DiagnosticResult expected = Diagnostic().WithLocation(2, 22);
            await VerifyCSharpFixAsync(testCode, this.GetSettings(), expected, fixedCode).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestTwoElementsWithRuleDisabledAsync()
        {
            this.SettingsConfiguration = SA1402SettingsConfiguration.ConfigureAsNonTopLevelType;

            var testCode = @"public delegate void Foo();
public delegate void Bar();
";

            await VerifyCSharpDiagnosticAsync(testCode, this.GetSettings(), DiagnosticResult.EmptyDiagnosticResults).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestTwoElementsWithDefaultRuleConfigurationAsync()
        {
            this.SettingsConfiguration = SA1402SettingsConfiguration.KeepDefaultConfiguration;

            var testCode = @"public delegate void Foo();
public delegate void Bar();
";

            await VerifyCSharpDiagnosticAsync(testCode, this.GetSettings(), DiagnosticResult.EmptyDiagnosticResults).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestThreeElementsAsync()
        {
            var testCode = @"public delegate void Foo();
public delegate void Bar();
public delegate void FooBar();
";

            var fixedCode = new[]
            {
                ("Test0.cs", @"public delegate void Foo();
"),
                ("Bar.cs", @"public delegate void Bar();
"),
                ("FooBar.cs", @"public delegate void FooBar();
"),
            };

            DiagnosticResult[] expected =
            {
                Diagnostic().WithLocation(2, 22),
                Diagnostic().WithLocation(3, 22),
            };

            await VerifyCSharpFixAsync(testCode, this.GetSettings(), expected, fixedCode).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestPreferFilenameTypeAsync()
        {
            var testCode = @"public delegate void Foo();
public delegate void Test0();
";

            var fixedCode = new[]
            {
                ("Test0.cs", $@"public delegate void Test0();
"),
                ("Foo.cs", $@"public delegate void Foo();
"),
            };

            DiagnosticResult expected = Diagnostic().WithLocation(1, 22);
            await VerifyCSharpFixAsync(testCode, this.GetSettings(), expected, fixedCode).ConfigureAwait(false);
        }
    }
}
