// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.ReadabilityRules
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.ReadabilityRules.SA1128ConstructorInitializerMustBeOnOwnLine,
        StyleCop.Analyzers.ReadabilityRules.SA1128CodeFixProvider>;

    public class SA1128UnitTests
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
            await VerifyCSharpDiagnosticAsync(declaration, DiagnosticResult.EmptyDiagnosticResults).ConfigureAwait(false);
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
            var expected = Diagnostic().WithLocation(4, 23);
            await VerifyCSharpFixAsync(testCode, expected, fixedCode).ConfigureAwait(false);
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
            var expected = Diagnostic().WithLocation(4, 23);
            await VerifyCSharpFixAsync(testCode, expected, fixedCode).ConfigureAwait(false);
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
            var expected = Diagnostic().WithLocation(5, 9);
            await VerifyCSharpFixAsync(testCode, expected, fixedCode).ConfigureAwait(false);
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
            var expected = Diagnostic().WithLocation(4, 23);
            await VerifyCSharpFixAsync(testCode, expected, fixedCode).ConfigureAwait(false);
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
            var expected = Diagnostic().WithLocation(4, 32);
            await VerifyCSharpFixAsync(testCode, expected, fixedCode).ConfigureAwait(false);
        }
    }
}
