// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.MaintainabilityRules
{
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.MaintainabilityRules;
    using TestHelper;
    using Xunit;

#pragma warning disable xUnit1000 // Test classes must be public
    internal abstract class SA1402ForBlockDeclarationUnitTestsBase : FileMayOnlyContainTestBase<SA1402FileMayOnlyContainASingleType, SA1402CodeFixProvider>
#pragma warning restore xUnit1000 // Test classes must be public
    {
        public override bool SupportsCodeFix => true;

        protected SA1402SettingsConfiguration SettingsConfiguration { get; set; } = SA1402SettingsConfiguration.ConfigureAsTopLevelType;

        protected abstract bool IsConfiguredAsTopLevelTypeByDefault { get; }

        [Fact]
        public async Task TestTwoGenericElementsAsync()
        {
            var testCode = @"%1 Foo<T1>
{
}
%1 Bar<T2, T3>
{
}";

            var fixedCode = new[]
            {
                ("Test0.cs", @"%1 Foo<T1>
{
}
"),
                ("Test1.cs", @"%1 Bar<T2, T3>
{
}"),
            };

            testCode = testCode.Replace("%1", this.Keyword);
            fixedCode = fixedCode.Select(c => (c.Item1, c.Item2.Replace("%1", this.Keyword))).ToArray();

            DiagnosticResult expected = Diagnostic().WithLocation(4, this.Keyword.Length + 2);
            await VerifyCSharpFixAsync(testCode, this.GetSettings(), expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestTwoElementsWithRuleDisabledAsync()
        {
            this.SettingsConfiguration = SA1402SettingsConfiguration.ConfigureAsNonTopLevelType;

            var testCode = @"%1 Foo
{
}
%1 Bar
{
}";

            testCode = testCode.Replace("%1", this.Keyword);

            await VerifyCSharpDiagnosticAsync(testCode, this.GetSettings(), DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestTwoElementsWithDefaultRuleConfigurationAsync()
        {
            this.SettingsConfiguration = SA1402SettingsConfiguration.KeepDefaultConfiguration;

            var testCode = @"%1 Foo
{
}
%1 Bar
{
}";

            var fixedCode = new[]
            {
                ("Test0.cs", @"%1 Foo
{
}
"),
                ("Test1.cs", @"%1 Bar
{
}"),
            };

            testCode = testCode.Replace("%1", this.Keyword);
            fixedCode = fixedCode.Select(c => (c.Item1, c.Item2.Replace("%1", this.Keyword))).ToArray();

            if (this.IsConfiguredAsTopLevelTypeByDefault)
            {
                DiagnosticResult expected = Diagnostic().WithLocation(4, this.Keyword.Length + 2);
                await VerifyCSharpFixAsync(testCode, this.GetSettings(), expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
            }
            else
            {
                await VerifyCSharpDiagnosticAsync(testCode, this.GetSettings(), DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            }
        }

        [Fact]
        public async Task TestPartialTypesAsync()
        {
            var testCode = $@"public partial {this.Keyword} Foo
{{
}}
public partial {this.Keyword} Foo
{{

}}";

            await VerifyCSharpDiagnosticAsync(testCode, this.GetSettings(), DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestDifferentPartialTypesAsync()
        {
            var testCode = $@"public partial {this.Keyword} Foo
{{
}}
public partial {this.Keyword} Bar
{{

}}";

            var fixedCode = new[]
            {
                ("Test0.cs", $@"public partial {this.Keyword} Foo
{{
}}
"),
                ("Bar.cs", $@"public partial {this.Keyword} Bar
{{

}}"),
            };

            DiagnosticResult expected = Diagnostic().WithLocation(4, 17 + this.Keyword.Length);
            await VerifyCSharpFixAsync(testCode, this.GetSettings(), expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestPreferFilenameTypeAsync()
        {
            var testCode = $@"public {this.Keyword} Foo
{{
}}
public {this.Keyword} Test0
{{
}}";

            var fixedCode = new[]
            {
                ("Test0.cs", $@"public {this.Keyword} Test0
{{
}}"),
                ("Foo.cs", $@"public {this.Keyword} Foo
{{
}}
"),
            };

            DiagnosticResult expected = Diagnostic().WithLocation(1, 9 + this.Keyword.Length);
            await VerifyCSharpFixAsync(testCode, this.GetSettings(), expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestNestedTypesAsync()
        {
            var testCode = $@"public class Foo
{{
    public {this.Keyword} Bar
    {{
    
    }}
}}";

            await VerifyCSharpDiagnosticAsync(testCode, this.GetSettings(), DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        protected override string GetSettings()
        {
            return this.SettingsConfiguration.GetSettings(this.Keyword);
        }
    }
}
