// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.Test.MaintainabilityRules
{
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.MaintainabilityRules;
    using Xunit;

    public abstract class SA1402ForBlockDeclarationUnitTestsBase : FileMayOnlyContainTestBase
    {
        public override bool SupportsCodeFix => true;

        protected override DiagnosticAnalyzer Analyzer => new SA1402FileMayOnlyContainASingleType();

        protected override CodeFixProvider CodeFix => new SA1402CodeFixProvider();

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
                ("/0/Test0.cs", @"%1 Foo<T1>
{
}
"),
                ("Bar{T2,T3}.cs", @"%1 Bar<T2, T3>
{
}"),
            };

            testCode = testCode.Replace("%1", this.Keyword);
            fixedCode = fixedCode.Select(c => (c.Item1, c.Item2.Replace("%1", this.Keyword))).ToArray();

            DiagnosticResult expected = this.Diagnostic().WithLocation(4, this.Keyword.Length + 2);
            await this.VerifyCSharpFixAsync(testCode, this.GetSettings(), expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
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

            await this.VerifyCSharpDiagnosticAsync(testCode, this.GetSettings(), DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
                ("/0/Test0.cs", @"%1 Foo
{
}
"),
                ("Bar.cs", @"%1 Bar
{
}"),
            };

            testCode = testCode.Replace("%1", this.Keyword);
            fixedCode = fixedCode.Select(c => (c.Item1, c.Item2.Replace("%1", this.Keyword))).ToArray();

            if (this.IsConfiguredAsTopLevelTypeByDefault)
            {
                DiagnosticResult expected = this.Diagnostic().WithLocation(4, this.Keyword.Length + 2);
                await this.VerifyCSharpFixAsync(testCode, this.GetSettings(), expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
            }
            else
            {
                await this.VerifyCSharpDiagnosticAsync(testCode, this.GetSettings(), DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            await this.VerifyCSharpDiagnosticAsync(testCode, this.GetSettings(), DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
                ("/0/Test0.cs", $@"public partial {this.Keyword} Foo
{{
}}
"),
                ("Bar.cs", $@"public partial {this.Keyword} Bar
{{

}}"),
            };

            DiagnosticResult expected = this.Diagnostic().WithLocation(4, 17 + this.Keyword.Length);
            await this.VerifyCSharpFixAsync(testCode, this.GetSettings(), expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
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
                ("/0/Test0.cs", $@"public {this.Keyword} Test0
{{
}}"),
                ("Foo.cs", $@"public {this.Keyword} Foo
{{
}}
"),
            };

            DiagnosticResult expected = this.Diagnostic().WithLocation(1, 9 + this.Keyword.Length);
            await this.VerifyCSharpFixAsync(testCode, this.GetSettings(), expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
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

            await this.VerifyCSharpDiagnosticAsync(testCode, this.GetSettings(), DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        protected override string GetSettings()
        {
            return this.SettingsConfiguration.GetSettings(this.Keyword);
        }
    }
}
