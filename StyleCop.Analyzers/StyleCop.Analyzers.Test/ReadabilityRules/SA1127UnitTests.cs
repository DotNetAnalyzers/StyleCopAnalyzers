// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.ReadabilityRules
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using TestHelper;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.ReadabilityRules.SA1127GenericTypeConstraintsMustBeOnOwnLine,
        StyleCop.Analyzers.ReadabilityRules.SA1127CodeFixProvider>;

    public class SA1127UnitTests
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
            await VerifyCSharpDiagnosticAsync(declaration, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(GetTypeDeclarationTests))]
        public async Task TestViolationWithTypeDeclarationAsync(string declaration, string fixedDeclaration, int column)
        {
            var testCode = $"{declaration}";
            var fixedCode = $"{fixedDeclaration}";

            var expected = Diagnostic().WithLocation(1, column);
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
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
            var expected = Diagnostic().WithLocation(4, 30);
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(1476, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/1476")]
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
            var expected = Diagnostic().WithLocation(5, 30);
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(1476, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/1476")]
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
            var expected = Diagnostic().WithLocation(6, 16);
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(1652, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/1652")]
        public async Task TestViolationWithMethodDeclarationAndXmlCommentsAsync()
        {
            var testCode = $@"
class Foo
{{
    /// <summary>Foo</summary>
    /// <typeparam name=""T"">The type.</typeparam>
    private void Method<T>() where T : class {{ }}
}}";
            var fixedCode = $@"
class Foo
{{
    /// <summary>Foo</summary>
    /// <typeparam name=""T"">The type.</typeparam>
    private void Method<T>()
        where T : class
    {{ }}
}}";
            var expected = Diagnostic().WithLocation(6, 30);
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(1652, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/1652")]
        public async Task TestViolationWithMethodDeclarationRegionDirectiveAsync()
        {
            var testCode = $@"
class Foo
{{
    #region Test
    private void Method<T>() where T : class {{ }}
    #endregion
}}";
            var fixedCode = $@"
class Foo
{{
    #region Test
    private void Method<T>()
        where T : class
    {{ }}
    #endregion
}}";
            var expected = Diagnostic().WithLocation(5, 30);
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
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
            var expected = Diagnostic().WithLocation(4, 32);
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
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
            var expected = Diagnostic().WithLocation(3, 14);
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
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
            var expected = Diagnostic().WithLocation(4, 26);
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
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
                Diagnostic().WithLocation(4, 29),
                Diagnostic().WithLocation(4, 45),
            };
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
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
            var expected = Diagnostic().WithLocation(2, 17);
            var expected2 = Diagnostic().WithLocation(2, 33);
            await VerifyCSharpFixAsync(testCode, new[] { expected, expected2 }, fixedCode, CancellationToken.None).ConfigureAwait(false);
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
            var expected = Diagnostic().WithLocation(2, 17);
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
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
            var expected = Diagnostic().WithLocation(4, 39);
            var expected2 = Diagnostic().WithLocation(4, 56);
            var expected3 = Diagnostic().WithLocation(4, 73);
            await VerifyCSharpFixAsync(testCode, new[] { expected, expected2, expected3 }, fixedCode, CancellationToken.None).ConfigureAwait(false);
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
            var expected = Diagnostic().WithLocation(5, 26);
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
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
            var expected = Diagnostic().WithLocation(2, 17);
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
