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

    public class SA1314UnitTests : CodeFixVerifier
    {
        [Fact]
        public async Task TestTypeParameterDoesNotStartWithTAsync()
        {
            var testCode = @"
public interface IFoo<Key>
{
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(2, 23);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);

            var fixedCode = @"
public interface IFoo<TKey>
{
}";

            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
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

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(2, 18);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);

            var fixedCode = @"
public class Foo<TKey>
{
    void Test()
    {
        var key = typeof(TKey);
    }
}
";

            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestTypeParameterStartsWithLowerTAsync()
        {
            var testCode = @"
public interface IFoo<tKey>
{
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(2, 23);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);

            var fixedCode = @"
public interface IFoo<TtKey>
{
}";

            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
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

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(4, 22);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);

            var fixedCode = @"
public class Bar
{
    public class Foo<TKey>
    {
    }
}";

            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestTypeParameterDoesStartWithTAsync()
        {
            var testCode = @"public interface IFoo<TKey>
{
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(2, 18);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
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

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(4, 22);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
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

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(4, 22);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
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

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(6, 18);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
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

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(4, 21);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);

            var fixedCode = @"
public class Foo
{
    public void Bar<TBaz>()
    {
    }
}";

            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
        }

        protected override IEnumerable<DiagnosticAnalyzer> GetCSharpDiagnosticAnalyzers()
        {
            yield return new SA1314TypeParameterNamesMustBeginWithT();
        }

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new SA1314CodeFixProvider();
        }
    }
}
