// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.NamingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using TestHelper;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.NamingRules.SA1314TypeParameterNamesMustBeginWithT,
        StyleCop.Analyzers.NamingRules.SA1314CodeFixProvider>;

    public class SA1314UnitTests
    {
        [Fact]
        public async Task TestTypeParameterDoesNotStartWithTAsync()
        {
            var testCode = @"
public interface IFoo<Key>
{
}";

            DiagnosticResult expected = Diagnostic().WithLocation(2, 23);

            var fixedCode = @"
public interface IFoo<TKey>
{
}";

            await VerifyCSharpFixAsync(testCode, expected, fixedCode).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestTypeParameterDoesNotStartWithTPlusParameterUsedAsync()
        {
            var testCode = @"
public class Foo<Key>
{
    void Test()
    {
        var key = typeof(Key);
    }
}
";

            DiagnosticResult expected = Diagnostic().WithLocation(2, 18);

            var fixedCode = @"
public class Foo<TKey>
{
    void Test()
    {
        var key = typeof(TKey);
    }
}
";

            await VerifyCSharpFixAsync(testCode, expected, fixedCode).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestTypeParameterStartsWithLowerTAsync()
        {
            var testCode = @"
public interface IFoo<tKey>
{
}";

            DiagnosticResult expected = Diagnostic().WithLocation(2, 23);

            var fixedCode = @"
public interface IFoo<TtKey>
{
}";

            await VerifyCSharpFixAsync(testCode, expected, fixedCode).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestInnerTypeParameterDoesNotStartWithTAsync()
        {
            var testCode = @"
public class Bar
{
    public class Foo<Key>
    {
    }
}";

            DiagnosticResult expected = Diagnostic().WithLocation(4, 22);

            var fixedCode = @"
public class Bar
{
    public class Foo<TKey>
    {
    }
}";

            await VerifyCSharpFixAsync(testCode, expected, fixedCode).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestTypeParameterDoesStartWithTAsync()
        {
            var testCode = @"public interface IFoo<TKey>
{
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestInnerTypeParameterDoesStartWithTAsync()
        {
            var testCode = @"
public class Bar
{
    public class Foo<TKey>
    {
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestTypeParameterDoesNotStartWithTWithMemberMatchingTargetTypeAsync()
        {
            string testCode = @"
public class Foo<Key>
{
    Key Bar { get; }
}";

            string fixedCode = @"
public class Foo<TKey>
{
    TKey Bar { get; }
}";

            DiagnosticResult expected = Diagnostic().WithLocation(2, 18);
            await VerifyCSharpFixAsync(testCode, expected, fixedCode).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestNestedTypeParameterDoesNotStartWithTWithConflictAsync()
        {
            string testCode = @"
public class Outer<TKey>
{
    public class Foo<Key>
    {
    }
}";
            string fixedCode = @"
public class Outer<TKey>
{
    public class Foo<TKey1>
    {
    }
}";

            DiagnosticResult expected = Diagnostic().WithLocation(4, 22);
            await VerifyCSharpFixAsync(testCode, expected, fixedCode).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestNestedTypeParameterDoesNotStartWithTWithMemberConflictAsync()
        {
            string testCode = @"
public class Outer<TKey>
{
    public class Foo<Key>
    {
        Key Bar { get; }
    }
}";
            string fixedCode = @"
public class Outer<TKey>
{
    public class Foo<TKey1>
    {
        TKey1 Bar { get; }
    }
}";

            DiagnosticResult expected = Diagnostic().WithLocation(4, 22);
            await VerifyCSharpFixAsync(testCode, expected, fixedCode).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestTypeParameterDoesNotStartWithTAndTypeConflictAsync()
        {
            string testCode = @"
public class TFoo
{
}

public class Bar<Foo>
{
}";
            string fixedCode = @"
public class TFoo
{
}

public class Bar<TFoo1>
{
}";

            DiagnosticResult expected = Diagnostic().WithLocation(6, 18);
            await VerifyCSharpFixAsync(testCode, expected, fixedCode).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestTypeParameterInMethodSignatureDoesNotStartWithTAsync()
        {
            var testCode = @"
public class Foo
{
    public void Bar<Baz>()
    {
    }
}";

            DiagnosticResult expected = Diagnostic().WithLocation(4, 21);

            var fixedCode = @"
public class Foo
{
    public void Bar<TBaz>()
    {
    }
}";

            await VerifyCSharpFixAsync(testCode, expected, fixedCode).ConfigureAwait(false);
        }
    }
}
