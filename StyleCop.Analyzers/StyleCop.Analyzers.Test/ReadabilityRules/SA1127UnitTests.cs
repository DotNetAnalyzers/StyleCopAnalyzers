// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.ReadabilityRules
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Analyzers.ReadabilityRules;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using TestHelper;
    using Xunit;

    public class SA1127UnitTests : CodeFixVerifier
    {
        public static IEnumerable<object[]> GetNullTests()
        {
            yield return new object[] { $"class Foo\r\n{{\r\n}}" };
            yield return new object[] { $"struct Foo\r\n{{\r\n}}" };
            yield return new object[] { $"interface Foo\r\n{{\r\n}}" };
            yield return new object[] { $"class Foo\r\n{{\r\n    private void Method() {{}}\r\n}}" };
        }

        public static IEnumerable<object[]> GetTypeDeclarationTests()
        {
            yield return new object[] { $"class Foo<T> where T : class {{}}", $"class Foo<T>\r\n    where T : class\r\n{{}}", 14 };
            yield return new object[] { $"interface Foo<T> where T : class {{}}", $"interface Foo<T>\r\n    where T : class\r\n{{}}", 18 };
            yield return new object[] { $"struct Foo<T> where T : class {{}}", $"struct Foo<T>\r\n    where T : class\r\n{{}}", 15 };
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
            var testCode = $"{declaration}";
            var fixedCode = $"{fixedDeclaration}";

            var expected = this.CSharpDiagnostic().WithLocation(1, column);
            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestViolationWithMethodDeclarationAsync()
        {
            var testCode = $@"
class Foo
{{
    private void Method<T>() where T : class {{ }}
}}";
            var fixedCode = $@"
class Foo
{{
    private void Method<T>()
        where T : class
    {{ }}
}}";
            var expected = this.CSharpDiagnostic().WithLocation(4, 30);
            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// This is a regression test for DotNetAnalyzers/StyleCopAnalyzers#1476:
        /// https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/1476
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestViolationWithObsoleteMethodDeclarationAsync()
        {
            var testCode = @"
class Foo
{
    [System.Obsolete]
    private void Method<T>() where T : class { }
}";
            var fixedCode = @"
class Foo
{
    [System.Obsolete]
    private void Method<T>()
        where T : class
    { }
}";
            var expected = this.CSharpDiagnostic().WithLocation(5, 30);
            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// This is a regression test for DotNetAnalyzers/StyleCopAnalyzers#1476:
        /// https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/1476
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestViolationWithMethodDeclarationMultiLineParametersAsync()
        {
            var testCode = @"
class Foo
{
    private void Method<T>(
        int a,
        int b) where T : class { }
}";
            var fixedCode = @"
class Foo
{
    private void Method<T>(
        int a,
        int b)
        where T : class
    { }
}";
            var expected = this.CSharpDiagnostic().WithLocation(6, 16);
            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestViolationWithExpressionBodiedMethodDeclarationAsync()
        {
            var testCode = $@"
class Foo
{{
    private string Method<T>() where T : class => typeof(T).Name;
}}";
            var fixedCode = $@"
class Foo
{{
    private string Method<T>()
        where T : class
        => typeof(T).Name;
}}";
            var expected = this.CSharpDiagnostic().WithLocation(4, 32);
            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestViolationWithOuterGenericClassAndInnerMethodAsync()
        {
            var testCode = $@"
using System;
class Foo<T> where T : class
{{
    T Method<T>()
    {{
        throw new NotImplementedException();
    }}
}}";
            var fixedCode = $@"
using System;
class Foo<T>
    where T : class
{{
    T Method<T>()
    {{
        throw new NotImplementedException();
    }}
}}";
            var expected = this.CSharpDiagnostic().WithLocation(3, 14);
            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestViolationWithInterfaceAndGenericMethodAsync()
        {
            var testCode = $@"
interface Foo
{{
    T GenericMethod<T>() where T : class;
}}";
            var fixedCode = $@"
interface Foo
{{
    T GenericMethod<T>()
        where T : class;
}}";
            var expected = this.CSharpDiagnostic().WithLocation(4, 26);
            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestViolationWithInterfaceAndGenericMethodTwoConstraintsAsync()
        {
            var testCode = $@"
interface Foo
{{
    T GenericMethod<T, R>() where T : class where R : new();
}}";
            var fixedCode = $@"
interface Foo
{{
    T GenericMethod<T, R>()
        where T : class
        where R : new();
}}";
            var expected = new DiagnosticResult[]
            {
                this.CSharpDiagnostic().WithLocation(4, 29),
                this.CSharpDiagnostic().WithLocation(4, 45)
            };
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
        public async Task TestViolationWithClassAndTwoTypeConstraintsAndFirstOnSingleLineAsync()
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
    private void Method<T1, T2, T3>() where T1 : class where T2 : class where T3 : class {{ }}
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
            var expected2 = this.CSharpDiagnostic().WithLocation(4, 56);
            var expected3 = this.CSharpDiagnostic().WithLocation(4, 73);
            await this.VerifyCSharpDiagnosticAsync(testCode, new[] { expected, expected2, expected3 }, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestViolationWithMethodAndCommentTriviaAtEndOfLineAsync()
        {
            var testCode = $@"
using System;
class Foo
{{
    T GenericMethod<T>() where T : class // constrain this to just classes
    {{
        throw new NotImplementedException();
    }}
}}";
            var fixedCode = $@"
using System;
class Foo
{{
    T GenericMethod<T>()
        where T : class // constrain this to just classes
    {{
        throw new NotImplementedException();
    }}
}}";
            var expected = this.CSharpDiagnostic().WithLocation(5, 26);
            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestViolationWithTwoConstraintsAndCommaTriviaAsync()
        {
            var testCode = $@"
class Foo<T, R> where T : class // constraint1
    where R : T, new() // constraint2
{{
}}";
            var fixedCode = $@"
class Foo<T, R>
    where T : class // constraint1
    where R : T, new() // constraint2
{{
}}";
            var expected = this.CSharpDiagnostic().WithLocation(2, 17);
            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
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
