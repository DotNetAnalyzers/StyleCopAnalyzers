// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.NamingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using TestHelper;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.NamingRules.SA1309FieldNamesMustNotBeginWithUnderscore,
        StyleCop.Analyzers.NamingRules.SA1309CodeFixProvider>;

    public class SA1309UnitTests
    {
        [Fact]
        public async Task TestFieldStartingWithAnUnderscoreAsync()
        {
            var testCode = @"public class Foo
{
    public string _bar = ""baz"";
}";

            DiagnosticResult expected = Diagnostic().WithArguments("_bar").WithLocation(3, 19);

            var fixedCode = @"public class Foo
{
    public string bar = ""baz"";
}";

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestFieldStartingWithLetterAsync()
        {
            var testCode = @"public class Foo
{
    public string bar = ""baz"";
}";

            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestFieldStartingAnUnderscorePlacedInsideNativeMethodsClassAsync()
        {
            var testCode = @"public class FooNativeMethods
{
    internal string _bar = ""baz"";
}";

            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestFieldStartingAnUnderscorePlacedInsideNativeMethodsClassWithIncorrectNameAsync()
        {
            var testCode = @"public class FooNativeMethodsClass
{
    internal string _bar = ""baz"";
}";

            DiagnosticResult expected = Diagnostic().WithArguments("_bar").WithLocation(3, 21);

            var fixedCode = @"public class FooNativeMethodsClass
{
    internal string bar = ""baz"";
}";

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestFieldStartingAnUnderscorePlacedInsideOuterNativeMethodsClassAsync()
        {
            var testCode = @"public class FooNativeMethods
{
    public class Foo
    {
        public string _bar = ""baz"";
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestUnderscoreOnlyVariableNameAsync()
        {
            var testCode = @"public class FooNativeMethodsClass
{
    internal string _ = ""baz"";
}";

            DiagnosticResult expected = Diagnostic().WithArguments("_").WithLocation(3, 21);

            // no changes will be made
            var fixedCode = testCode;
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// This is a regression test for DotNetAnalyzers/StyleCopAnalyzers#627.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        /// <seealso href="https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/627">#627: Code Fixes For Naming
        /// Rules SA1308 and SA1309 Do Not Always Fix The Name Entirely</seealso>
        [Fact]
        public async Task TestFieldStartingWithMultipleUnderscoresAsync()
        {
            var testCode = @"public class Foo
{
    public string ____bar = ""baz"";
}";

            DiagnosticResult expected = Diagnostic().WithArguments("____bar").WithLocation(3, 19);

            var fixedCode = @"public class Foo
{
    public string bar = ""baz"";
}";

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        // This a regression test for issue #2360.
        [Fact]
        public async Task VerifyThatAFieldStartingWithAnUnderscoreAndADigitIsNotAffectedByCodeFixAsync()
        {
            var testCode = @"public class Foo
{
    private string _1bar = ""baz"";
}";

            DiagnosticResult expected = Diagnostic().WithArguments("_1bar").WithLocation(3, 20);

            // no changes will be made
            var fixedCode = testCode;
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task VerifyThatAFieldStartingWithUnderscoreAndFollowedByKeywordTriggersDiagnosticAndIsCorrectedByCodefixAsync()
        {
            var testCode = @"public class Foo
{
    private string _int = ""baz"";
}";

            var fixedCode = @"public class Foo
{
    private string @int = ""baz"";
}";

            DiagnosticResult expected = Diagnostic().WithArguments("_int").WithLocation(3, 20);
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
