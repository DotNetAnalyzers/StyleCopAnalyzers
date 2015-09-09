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

    public class SA1309UnitTests : CodeFixVerifier
    {
        [Fact]
        public async Task TestFieldStartingWithAnUnderscoreAsync()
        {
            var testCode = @"public class Foo
{
    public string _bar = ""baz"";
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithArguments("_bar").WithLocation(3, 19);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);

            var fixedCode = @"public class Foo
{
    public string bar = ""baz"";
}";

            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestFieldStartingWithLetterAsync()
        {
            var testCode = @"public class Foo
{
    public string bar = ""baz"";
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestFieldStartingAnUnderscorePlacedInsideNativeMethodsClassAsync()
        {
            var testCode = @"public class FooNativeMethods
{
    internal string _bar = ""baz"";
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestFieldStartingAnUnderscorePlacedInsideNativeMethodsClassWithIncorrectNameAsync()
        {
            var testCode = @"public class FooNativeMethodsClass
{
    internal string _bar = ""baz"";
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithArguments("_bar").WithLocation(3, 21);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);

            var fixedCode = @"public class FooNativeMethodsClass
{
    internal string bar = ""baz"";
}";

            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
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

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestUnderscoreOnlyVariableNameAsync()
        {
            var testCode = @"public class FooNativeMethodsClass
{
    internal string _ = ""baz"";
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithArguments("_").WithLocation(3, 21);
            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);

            // no changes will be made
            var fixedCode = testCode;
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
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

            DiagnosticResult expected = this.CSharpDiagnostic().WithArguments("____bar").WithLocation(3, 19);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);

            var fixedCode = @"public class Foo
{
    public string bar = ""baz"";
}";

            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
        }

        protected override IEnumerable<DiagnosticAnalyzer> GetCSharpDiagnosticAnalyzers()
        {
            yield return new SA1309FieldNamesMustNotBeginWithUnderscore();
        }

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new SA1309CodeFixProvider();
        }
    }
}
