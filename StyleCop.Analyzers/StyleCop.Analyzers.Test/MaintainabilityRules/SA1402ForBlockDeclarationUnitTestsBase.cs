// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.MaintainabilityRules
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using StyleCop.Analyzers.MaintainabilityRules;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using TestHelper;
    using Xunit;

    public abstract class SA1402ForBlockDeclarationUnitTestsBase : FileMayOnlyContainTestBase
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
                @"%1 Foo<T1>
{
}
",
                @"%1 Bar<T2, T3>
{
}",
            };

            testCode = testCode.Replace("%1", this.Keyword);
            fixedCode = fixedCode.Select(c => c.Replace("%1", this.Keyword)).ToArray();

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(4, this.Keyword.Length + 2);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(new[] { testCode }, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
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

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
                @"%1 Foo
{
}
",
                @"%1 Bar
{
}",
            };

            testCode = testCode.Replace("%1", this.Keyword);
            fixedCode = fixedCode.Select(c => c.Replace("%1", this.Keyword)).ToArray();

            if (this.IsConfiguredAsTopLevelTypeByDefault)
            {
                DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(4, this.Keyword.Length + 2);

                await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
                await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
                await this.VerifyCSharpFixAsync(new[] { testCode }, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
            }
            else
            {
                await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            var fixedFileNames = new[] { "Test0.cs", "Bar.cs" };
            var fixedCode = new[]
            {
                $@"public partial {this.Keyword} Foo
{{
}}
",
                $@"public partial {this.Keyword} Bar
{{

}}",
            };

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(4, 17 + this.Keyword.Length);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(new[] { testCode }, fixedCode, newFileNames: fixedFileNames, cancellationToken: CancellationToken.None).ConfigureAwait(false);
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

            var fixedFileNames = new[] { "Test0.cs", "Foo.cs" };
            var fixedCode = new[]
            {
                $@"public {this.Keyword} Test0
{{
}}",
                $@"public {this.Keyword} Foo
{{
}}
",
            };

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(1, 9 + this.Keyword.Length);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(new[] { testCode }, fixedCode, newFileNames: fixedFileNames, cancellationToken: CancellationToken.None).ConfigureAwait(false);
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

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        protected override string GetSettings()
        {
            return this.SettingsConfiguration.GetSettings(this.Keyword);
        }

        protected override IEnumerable<DiagnosticAnalyzer> GetCSharpDiagnosticAnalyzers()
        {
            yield return new SA1402FileMayOnlyContainASingleType();
        }

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new SA1402CodeFixProvider();
        }
    }
}
