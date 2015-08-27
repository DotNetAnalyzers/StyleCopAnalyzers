namespace StyleCop.Analyzers.Test.ReadabilityRules
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.ReadabilityRules;
    using TestHelper;
    using Xunit;

    public class SA1127UnitTests : CodeFixVerifier
    {
        public static IEnumerable<object[]> GetNullTests()
        {
            yield return new object[] { $"class Foo\r\n{{\r\n}}" };
            yield return new object[] { $"struct Foo\r\n{{\r\n}}" };
            yield return new object[] { $"class Foo\r\n{{\r\n    private void Method() {{}}\r\n}}" };
        }

        public static IEnumerable<object[]> GetTypeDeclarationTests()
        {
            yield return new object[] { $"class Foo<T> where T : class {{}}", $"class Foo<T>\r\n    where T : class {{}}", 14 };
            yield return new object[] { $"interface Foo<T> where T : class {{}}", $"interface Foo<T>\r\n    where T : class {{}}", 18 };
            yield return new object[] { $"struct Foo<T> where T : class {{}}", $"struct Foo<T>\r\n    where T : class {{}}", 15 };
        }

        public static IEnumerable<object[]> GetMethodDeclarationTests()
        {
            yield return new object[] { $"private void Method<T>() where T : class {{ }}", $"private void Method<T>()\r\n    where T : class {{ }}", 30 };
            yield return new object[] { $"private string Method<T>() where T : class => typeof(T).Name;", $"private string Method<T>()\r\n    where T : class\r\n    => typeof(T).Name;", 32 };
        }

        [Theory]
        [MemberData(nameof(GetNullTests))]
        public async Task TestNullScenariosAsync(string declaration)
        {
            await this.VerifyCSharpDiagnosticAsync(declaration, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(GetTypeDeclarationTests))]
        public async Task TestViolationWithTypeDeclarationAsync(string declaration, string fixedDeclaration, int column)
        {
            var testCode = $@"
{declaration}";
            var fixedCode = $@"
{fixedDeclaration}";

            var expected = this.CSharpDiagnostic().WithLocation(2, column);
            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(GetMethodDeclarationTests))]
        public async Task TestViolationWithMethodDeclarationAsync(string declaration, string fixedDeclaration, int column)
        {
            var testCode = $@"
class Foo
{{
    {declaration}
}}";
            var fixedCode = $@"
class Foo
{{
    {fixedDeclaration}
}}";
            var expected = this.CSharpDiagnostic().WithLocation(4, column);
            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task SuperSimpleCodeFixAsync()
        {
            var testCode = "class Foo<T> where T : class {{}}";
            var fixedCode = "class Foo<T>\r\n    where T : class\r\n{{}}";

            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestViolationWithClassAndTypeConstraintOnSingleLineAsync()
        {
            var testCode = $@"
class Foo<T> where T : class
{{
}}";
            var fixedCode = $@"
class Foo<T>
    where T : class
{{
}}";
            var expected = this.CSharpDiagnostic().WithLocation(2, 14);
            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestViolationWithClassAndTwoTypeConstraintsOnSingleLineAsync()
        {
            var testCode = $@"
class Foo<T, R> where T : class where R : T, new()
{{
}}";
            var fixedCode = $@"
class Foo<T, R>
    where T : class
    where R : T, new()
{{
}}";
            var expected = this.CSharpDiagnostic().WithLocation(2, 17);
            var expected2 = this.CSharpDiagnostic().WithLocation(2, 33);
            await this.VerifyCSharpDiagnosticAsync(testCode, new[] { expected, expected2 }, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestViolationWithClassAndTwoTypeConstraintsWithFirstOnSingleLineAsync()
        {
            var testCode = $@"
class Foo<T, R> where T : class
    where R : T, new()
{{
}}";
            var fixedCode = $@"
class Foo<T, R>
    where T : class
    where R : T, new()
{{
}}";
            var expected = this.CSharpDiagnostic().WithLocation(2, 17);
            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestViolationWithMethodAndThreeTypeConstraintsOnSingleLineAsync()
        {
            var testCode = $@"
class Foo
{{
    private void Method<T1, T2, T3>() where T1: class where T2: class where T3: class {{ }}
}}";
            var fixedCode = $@"
class Foo
{{
    private void Method<T1, T2, T3>()
        where T1 : class
        where T2 : class
        where T3 : class
    {{ }}
}}";
            var expected = this.CSharpDiagnostic().WithLocation(4, 39);
            var expected2 = this.CSharpDiagnostic().WithLocation(4, 55);
            var expected3 = this.CSharpDiagnostic().WithLocation(4, 71);
            await this.VerifyCSharpDiagnosticAsync(testCode, new[] { expected, expected2, expected3 }, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        protected override IEnumerable<DiagnosticAnalyzer> GetCSharpDiagnosticAnalyzers()
        {
            yield return new SA1127GenericTypeConstraintsMustBeOnOwnLine();
        }

        /// <inheritdoc/>
        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new SA1127CodeFixProvider();
        }
    }
}
