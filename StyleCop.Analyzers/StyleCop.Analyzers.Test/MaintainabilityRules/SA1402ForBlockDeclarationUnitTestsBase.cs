// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.MaintainabilityRules
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Analyzers.MaintainabilityRules;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using TestHelper;
    using Xunit;

    public abstract class SA1402ForBlockDeclarationUnitTestsBase : FileMayOnlyContainTestBase
    {
        public override bool SupportsCodeFix => true;

        private bool ConfigureAsNonTopLevelType { get; set; } = false;

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
}"
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
            this.ConfigureAsNonTopLevelType = true;

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

            var fixedCode = new[]
            {
                $@"public partial {this.Keyword} Foo
{{
}}
",
                $@"public partial {this.Keyword} Bar
{{

}}"
            };

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(4, 17 + this.Keyword.Length);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(new[] { testCode }, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
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
                $@"public {this.Keyword} Test0
{{
}}",
                $@"public {this.Keyword} Foo
{{
}}
"
            };

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(1, 9 + this.Keyword.Length);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(new[] { testCode }, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
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
            var keywords = new List<string> { "class", "interface", "struct", "enum", "delegate" };
            if (this.ConfigureAsNonTopLevelType)
            {
                keywords.Remove(this.Keyword);
            }

            var keywordsStr = string.Join(", ", keywords.Select(x => "\"" + x + "\""));

            var settings = $@"
{{
  ""settings"": {{
    ""maintainabilityRules"": {{
      ""topLevelTypes"": [{keywordsStr}]
    }}
  }}
}}";

            return settings;
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
