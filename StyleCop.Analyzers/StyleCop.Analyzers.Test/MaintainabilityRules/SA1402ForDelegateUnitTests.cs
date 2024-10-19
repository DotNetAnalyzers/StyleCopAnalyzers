﻿// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.Test.MaintainabilityRules
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Settings.ObjectModel;
    using Xunit;
    using Xunit.Sdk;

    public class SA1402ForDelegateUnitTests : SA1402ForNonBlockDeclarationUnitTestsBase
    {
        public override string Keyword => "delegate";

        [Fact]
        public async Task TestOneElementAsync()
        {
            var testCode = @"public delegate void Foo();";

            await VerifyCSharpDiagnosticAsync(testCode, this.GetSettings(), DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestTwoElementsAsync()
        {
            var testCode = @"public delegate void Foo();
public delegate void Bar();
";

            var fixedCode = new[]
            {
                ("/0/Test0.cs", @"public delegate void Foo();
"),
                ("Bar.cs", @"public delegate void Bar();
"),
            };

            DiagnosticResult expected = Diagnostic().WithLocation(2, 22);
            await VerifyCSharpFixAsync(testCode, this.GetSettings(), expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [InlineData(FileNamingConvention.StyleCop)]
        [InlineData(FileNamingConvention.Metadata)]
        public async Task TestTwoGenericElementsAsync(object namingConvention)
        {
            var testCode = @"public delegate void Foo();
public delegate void Bar<T1, T2, T3>(T1 x, T2 y, T3 z);
";

            var expectedName = (FileNamingConvention)namingConvention switch
            {
                FileNamingConvention.StyleCop => "Bar{T1,T2,T3}.cs",
                FileNamingConvention.Metadata => "Bar`3.cs",
                _ => throw new NotImplementedException(),
            };

            var fixedCode = new[]
            {
                ("/0/Test0.cs", @"public delegate void Foo();
"),
                (expectedName, @"public delegate void Bar<T1, T2, T3>(T1 x, T2 y, T3 z);
"),
            };

            DiagnosticResult expected = Diagnostic().WithLocation(2, 22);
            await VerifyCSharpFixAsync(testCode, this.GetSettings((FileNamingConvention)namingConvention), expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestTwoElementsWithRuleDisabledAsync()
        {
            this.SettingsConfiguration = SA1402SettingsConfiguration.ConfigureAsNonTopLevelType;

            var testCode = @"public delegate void Foo();
public delegate void Bar();
";

            await VerifyCSharpDiagnosticAsync(testCode, this.GetSettings(), DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestTwoElementsWithDefaultRuleConfigurationAsync()
        {
            this.SettingsConfiguration = SA1402SettingsConfiguration.KeepDefaultConfiguration;

            var testCode = @"public delegate void Foo();
public delegate void Bar();
";

            await VerifyCSharpDiagnosticAsync(testCode, this.GetSettings(), DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
                ("/0/Test0.cs", @"public delegate void Foo();
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

            await VerifyCSharpFixAsync(testCode, this.GetSettings(), expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestPreferFilenameTypeAsync()
        {
            var testCode = @"public delegate void Foo();
public delegate void Test0();
";

            var fixedCode = new[]
            {
                ("/0/Test0.cs", $@"public delegate void Test0();
"),
                ("Foo.cs", $@"public delegate void Foo();
"),
            };

            DiagnosticResult expected = Diagnostic().WithLocation(1, 22);
            await VerifyCSharpFixAsync(testCode, this.GetSettings(), expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
