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

    public class SA1128UnitTests : CodeFixVerifier
    {
        public static IEnumerable<object[]> GetNullTests()
        {
            yield return new object[] { $"class Foo\r\n{{\r\n}}" };
            yield return new object[] { $"class Foo\r\n{{\r\n    public Foo() {{}}\r\n}}" };
            yield return new object[] { $"class Foo\r\n{{\r\n    public Foo(int bar) {{}}\r\n}}" };
        }

        [Theory]
        [MemberData(nameof(GetNullTests))]
        public async Task TestNullScenariosAsync(string declaration)
        {
            await this.VerifyCSharpDiagnosticAsync(declaration, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestViolationWithBaseInitializerOnSameLineAsync()
        {
            var testCode = @"
public class TypeName
{
    public TypeName() : base()
    {
    }
}";
            var fixedCode = @"
public class TypeName
{
    public TypeName()
        : base()
    {
    }
}";
            var expected = this.CSharpDiagnostic().WithLocation(4, 23);
            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestViolationWithThisInitializerOnSameLineAsync()
        {
            var testCode = @"
public class TypeName
{
    public TypeName() : this(0)
    {
    }

    public TypeName(int value)
    {
    }
}";
            var fixedCode = @"
public class TypeName
{
    public TypeName()
        : this(0)
    {
    }

    public TypeName(int value)
    {
    }
}";
            var expected = this.CSharpDiagnostic().WithLocation(4, 23);
            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestViolationWithColonOnDifferentLineAsync()
        {
            var testCode = @"
public class TypeName
{
    public TypeName() 
        : 
        this(0)
    {
    }

    public TypeName(int value)
    {
    }
}";
            var fixedCode = @"
public class TypeName
{
    public TypeName()
        : this(0)
    {
    }

    public TypeName(int value)
    {
    }
}";
            var expected = this.CSharpDiagnostic().WithLocation(5, 9);
            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestViolationWithColonOnSameLineAsync()
        {
            var testCode = @"
public class TypeName
{
    public TypeName() : 
        this(0)
    {
    }

    public TypeName(int value)
    {
    }
}";
            var fixedCode = @"
public class TypeName
{
    public TypeName()
        : this(0)
    {
    }

    public TypeName(int value)
    {
    }
}";
            var expected = this.CSharpDiagnostic().WithLocation(4, 23);
            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestViolationWithCommentsAsync()
        {
            var testCode = @"
public class TypeName
{
    public TypeName() /* c1 */ : /* c2 */ this(0) /* c3 */
    {
    }

    public TypeName(int value)
    {
    }
}";
            var fixedCode = @"
public class TypeName
{
    public TypeName() /* c1 */
        : /* c2 */ this(0) /* c3 */
    {
    }

    public TypeName(int value)
    {
    }
}";
            var expected = this.CSharpDiagnostic().WithLocation(4, 32);
            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        protected override IEnumerable<DiagnosticAnalyzer> GetCSharpDiagnosticAnalyzers()
        {
            yield return new SA1128ConstructorInitializerMustBeOnOwnLine();
        }

        /// <inheritdoc/>
        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new SA1128CodeFixProvider();
        }
    }
}
