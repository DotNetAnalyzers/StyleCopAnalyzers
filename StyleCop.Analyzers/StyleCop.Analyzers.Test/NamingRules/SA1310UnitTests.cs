// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.NamingRules
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.NamingRules;
    using TestHelper;
    using Xunit;

    public class SA1310UnitTests : CodeFixVerifier
    {
        [Fact]
        public async Task TestFieldWithUnderscoreAsync()
        {
            var testCode = @"public class ClassName
{
    public string name_bar = ""baz"";
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithArguments("name_bar").WithLocation(3, 19);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);

            var fixedCode = @"public class ClassName
{
    public string nameBar = ""baz"";
}";

            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestFieldStartingWithoutUnderscoreAsync()
        {
            var testCode = @"public class ClassName
{
    public string nameBar = ""baz"";
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [InlineData("m_")]
        [InlineData("s_")]
        [InlineData("t_")]
        public async Task TestFieldStartingWithSpecialPrefixAsync(string prefix)
        {
            var testCode = $@"public class ClassName
{{
    public string {prefix}nameBar = ""baz"";
}}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestFieldWithUnderscorePlacedInsideNativeMethodsClassAsync()
        {
            var testCode = @"public class ClassNameNativeMethods
{
    internal string name_bar = ""baz"";
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestFieldWithUnderscorePlacedInsideNativeMethodsClassWithIncorrectNameAsync()
        {
            var testCode = @"public class ClassNameNativeMethodsClass
{
    internal string name_bar = ""baz"";
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithArguments("name_bar").WithLocation(3, 21);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);

            var fixedCode = @"public class ClassNameNativeMethodsClass
{
    internal string nameBar = ""baz"";
}";

            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestFieldWithUnderscorePlacedInsideOuterNativeMethodsClassAsync()
        {
            var testCode = @"public class ClassNameNativeMethods
{
    public class Foo
    {
        public string name_bar = ""baz"";
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestUnderscoreOnlyVariableNameAsync()
        {
            var testCode = @"public class ClassNameNativeMethodsClass
{
    internal string _ = ""baz"";
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        protected override IEnumerable<DiagnosticAnalyzer> GetCSharpDiagnosticAnalyzers()
        {
            yield return new SA1310FieldNamesMustNotContainUnderscore();
        }

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new SA1310CodeFixProvider();
        }
    }
}
