// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.NamingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.Helpers;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.NamingRules.SA1310FieldNamesMustNotContainUnderscore,
        StyleCop.Analyzers.NamingRules.SA1310CodeFixProvider>;

    public class SA1310UnitTests
    {
        [Theory]
        [InlineData("\n")]
        [InlineData("\r\n")]
        public async Task TestFieldWithUnderscoreAsync(string lineEnding)
        {
            var testCode = @"public class ClassName
{
    public string {|#0:name_bar|} = ""baz"";
}".ReplaceLineEndings(lineEnding);

            DiagnosticResult expected = Diagnostic().WithArguments("name_bar").WithLocation(0);

            var fixedCode = @"public class ClassName
{
    public string nameBar = ""baz"";
}".ReplaceLineEndings(lineEnding);

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestFieldStartingWithoutUnderscoreAsync()
        {
            var testCode = @"public class ClassName
{
    public string nameBar = ""baz"";
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestFieldWithUnderscorePlacedInsideNativeMethodsClassAsync()
        {
            var testCode = @"public class ClassNameNativeMethods
{
    internal string name_bar = ""baz"";
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestFieldWithUnderscorePlacedInsideNativeMethodsClassWithIncorrectNameAsync()
        {
            var testCode = @"public class ClassNameNativeMethodsClass
{
    internal string name_bar = ""baz"";
}";

            DiagnosticResult expected = Diagnostic().WithArguments("name_bar").WithLocation(3, 21);

            var fixedCode = @"public class ClassNameNativeMethodsClass
{
    internal string nameBar = ""baz"";
}";

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
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

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestUnderscoreOnlyVariableNameAsync()
        {
            var testCode = @"public class ClassNameNativeMethodsClass
{
    internal string _ = ""baz"";
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
