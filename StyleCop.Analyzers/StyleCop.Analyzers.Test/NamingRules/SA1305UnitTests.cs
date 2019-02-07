// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.NamingRules
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using TestHelper;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopDiagnosticVerifier<StyleCop.Analyzers.NamingRules.SA1305FieldNamesMustNotUseHungarianNotation>;

    public class SA1305UnitTests
    {
        public static IEnumerable<object[]> CommonPrefixes
        {
            get
            {
                yield return new object[] { "as" };
                yield return new object[] { "at" };
                yield return new object[] { "by" };
                yield return new object[] { "do" };
                yield return new object[] { "go" };
                yield return new object[] { "if" };
                yield return new object[] { "in" };
                yield return new object[] { "is" };
                yield return new object[] { "it" };
                yield return new object[] { "no" };
                yield return new object[] { "of" };
                yield return new object[] { "on" };
                yield return new object[] { "or" };
                yield return new object[] { "to" };
            }
        }

        [Fact]
        public async Task TestValidFieldNamesAreNotReportedAsync()
        {
            var testCode = @" public class TestClass
{
    string bar, Car, fooBar, x, yz;
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestValidVariableNamesAreNotReportedAsync()
        {
            var testCode = @" public class TestClass
{
    public void TestMethod()
    {
        string bar, Car, fooBar, x, yz;
    }
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestInvalidFieldNamesAreReportedAsync()
        {
            var testCode = @" public class TestClass
{
    string baR, caRe, daRE, fAre;
}
";

            DiagnosticResult[] expected =
            {
                Diagnostic().WithLocation(3, 12).WithArguments("field", "baR"),
                Diagnostic().WithLocation(3, 17).WithArguments("field", "caRe"),
                Diagnostic().WithLocation(3, 23).WithArguments("field", "daRE"),
                Diagnostic().WithLocation(3, 29).WithArguments("field", "fAre"),
            };

            await VerifyCSharpDiagnosticAsync(testCode, expected).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestInvalidVariableNamesAreReportedAsync()
        {
            var testCode = @" public class TestClass
{
    public void TestMethod()
    {
        string baR, caRe, daRE, fAre;
    }
}
";

            DiagnosticResult[] expected =
            {
                Diagnostic().WithLocation(5, 16).WithArguments("variable", "baR"),
                Diagnostic().WithLocation(5, 21).WithArguments("variable", "caRe"),
                Diagnostic().WithLocation(5, 27).WithArguments("variable", "daRE"),
                Diagnostic().WithLocation(5, 33).WithArguments("variable", "fAre"),
            };

            await VerifyCSharpDiagnosticAsync(testCode, expected).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestEventFieldsAreNotReportedAsync()
        {
            var testCode = @" public interface ITestInterface
{
    event System.Action abC;
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(CommonPrefixes))]
        public async Task TestAllowedCommonPrefixesAsync(string prefix)
        {
            var testCode = $@" public class TestClass
{{
    string {prefix}R;
}}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(CommonPrefixes))]
        public async Task TestAllowedCommonPrefixesWhenDisabledAsync(string prefix)
        {
            var customTestSettings = @"
{
  ""settings"": {
    ""namingRules"": {
      ""allowCommonHungarianPrefixes"": false,
      ""allowedHungarianPrefixes"": [ ]
    }
  }
}
";

            var testCode = $@" public class TestClass
{{
    string {prefix}R;
}}
";

            await new CSharpTest
            {
                TestCode = testCode,
                ExpectedDiagnostics =
                {
                    Diagnostic().WithLocation(3, 12).WithArguments("field", $"{prefix}R"),
                },
                Settings = customTestSettings,
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestExcludedPrefixesAreNotReportedAsync()
        {
            var customTestSettings = @"
{
  ""settings"": {
    ""namingRules"": {
      ""allowCommonHungarianPrefixes"": false,
      ""allowedHungarianPrefixes"": [ ""ba"", ""ca"", ""da"", ""f"" ]
    }
  }
}
";

            var testCode = @" public class TestClass
{
    string baR, caRe, daRE, fAre;
}
";

            await new CSharpTest
            {
                TestCode = testCode,
                Settings = customTestSettings,
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestParameterInInterfaceMethodParameterDeclarationAsync()
        {
            var testCode = @"
public interface TypeName
{
    void MethodName(bool abX);
}";

            DiagnosticResult[] expected =
            {
                Diagnostic().WithArguments("parameter", "abX").WithLocation(4, 26),
            };

            await VerifyCSharpDiagnosticAsync(testCode, expected).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestParameterInClassMethodAsync()
        {
            var testCode = @"
public class TypeName
{
    public void MethodName(bool abX)
    {
    }
}";

            DiagnosticResult[] expected =
            {
                Diagnostic().WithArguments("parameter", "abX").WithLocation(4, 33),
            };

            await VerifyCSharpDiagnosticAsync(testCode, expected).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestParameterInNativeClassMethodAsync()
        {
            var testCode = @"
public class TypeNameNativeMethods
{
    public void MethodName(bool abX)
    {
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestParameterInImplementedInterfaceMethodDeclarationAsync()
        {
            var testCode = @"
public interface Interface
{
    void MethodName(bool x);
}

public class Class : Interface
{
    public void MethodName(bool abX)
    {
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestParameterInIndirectlyImplementedInterfaceMethodDeclarationAsync()
        {
            var testCode = @"
public interface Interface1
{
    void MethodName(bool x);
}

public interface Interface2 : Interface1
{
}

public class Class : Interface2
{
    public void MethodName(bool abX)
    {
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestParameterInOverriddenMethodDeclarationAsync()
        {
            var testCode = @"
public class BaseClass
{
    public virtual void MethodName(bool x)
    {
    }
}

public class SubClass : BaseClass
{
    public override void MethodName(bool abX)
    {
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestParameterInConstructorDeclarationAsync()
        {
            var testCode = @"
public class TypeName
{
    public TypeName(string abX)
    {
    }
}";

            DiagnosticResult[] expected =
            {
                Diagnostic().WithArguments("parameter", "abX").WithLocation(4, 28),
            };

            await VerifyCSharpDiagnosticAsync(testCode, expected).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestParameterInIndexerDeclarationAsync()
        {
            var testCode = @"
public class TypeName
{
    public int this[int abX]
    {
        get { return 0; }
    }
}";

            DiagnosticResult[] expected =
            {
                Diagnostic().WithArguments("parameter", "abX").WithLocation(4, 25),
            };

            await VerifyCSharpDiagnosticAsync(testCode, expected).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestParameterInImplementedInterfaceIndexerMethodDeclarationAsync()
        {
            var testCode = @"
public interface Interface
{
    int this[int x] { get; }
}

public class Class : Interface
{
    public int this[int abX]
    {
        get { return 0; }
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestParameterInIndirectlyImplementedInterfaceIndexerDeclarationAsync()
        {
            var testCode = @"
public interface Interface1
{
    int this[int x] { get; }
}

public interface Interface2 : Interface1
{
}

public class Class : Interface2
{
    public int this[int abX]
    {
        get { return 0; }
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestParameterInOverriddenIndexerDeclarationAsync()
        {
            var testCode = @"
public class BaseClass
{
    public virtual int this[int x]
    {
        get { return 0; }
    }
}

public class SubClass : BaseClass
{
    public override int this[int abX]
    {
        get { return 0; }
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestParameterInLambdaDeclarationAsync()
        {
            var testCode = @"
using System;
public class TypeName
{
    public void Method()
    {
        Func<float, float> y = (float abX) => abX;
    }
}";

            DiagnosticResult[] expected =
            {
                Diagnostic().WithArguments("parameter", "abX").WithLocation(7, 39),
            };

            await VerifyCSharpDiagnosticAsync(testCode, expected).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestParameterInGlobalDelegateDeclarationAsync()
        {
            var testCode = @"
public delegate void Delegate(double abX);
";

            DiagnosticResult[] expected =
            {
                Diagnostic().WithArguments("parameter", "abX").WithLocation(2, 38),
            };

            await VerifyCSharpDiagnosticAsync(testCode, expected).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestParameterInInnerDelegateDeclarationAsync()
        {
            var testCode = @"
public class TypeName
{
    public delegate void Delegate(double abX);
}";

            DiagnosticResult[] expected =
            {
                Diagnostic().WithArguments("parameter", "abX").WithLocation(4, 42),
            };

            await VerifyCSharpDiagnosticAsync(testCode, expected).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestVariableInCatchDeclarationAsync()
        {
            var testCode = @"
using System;
public class TypeName
{
    public void MethodName()
    {
        try
        {
        }
        catch (Exception exA)
        {
        }
    }
}";

            DiagnosticResult[] expected =
            {
                Diagnostic().WithArguments("variable", "exA").WithLocation(10, 26),
            };

            await VerifyCSharpDiagnosticAsync(testCode, expected).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestVariableInForEachStatementAsync()
        {
            var testCode = @"public class TypeName
{
    public void MethodName()
    {
        foreach (var abX in new int[0])
        {
        }
    }
}";

            DiagnosticResult[] expected =
            {
                Diagnostic().WithArguments("variable", "abX").WithLocation(5, 22),
            };

            await VerifyCSharpDiagnosticAsync(testCode, expected).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestVariableInFromClauseAsync()
        {
            var testCode = @"
using System.Linq;
public class TypeName
{
    public void MethodName()
    {
        var result =
            from abX in new int[0]
            select abX;
    }
}";

            DiagnosticResult[] expected =
            {
                Diagnostic().WithArguments("variable", "abX").WithLocation(8, 18),
            };

            await VerifyCSharpDiagnosticAsync(testCode, expected).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestVariableInQueryContinuationAsync()
        {
            var testCode = @"
using System.Linq;
public class TypeName
{
    public void MethodName()
    {
        var result =
            from x in new int[0]
            select x into abY
            select abY;
    }
}";

            DiagnosticResult[] expected =
            {
                Diagnostic().WithArguments("variable", "abY").WithLocation(9, 27),
            };

            await VerifyCSharpDiagnosticAsync(testCode, expected).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestVariableInLetClauseAsync()
        {
            var testCode = @"
using System.Linq;
public class TypeName
{
    public void MethodName()
    {
        var result =
            from x in new int[0]
            let abY = x
            select abY;
    }
}";

            DiagnosticResult[] expected =
            {
                Diagnostic().WithArguments("variable", "abY").WithLocation(9, 17),
            };

            await VerifyCSharpDiagnosticAsync(testCode, expected).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestVariableInJoinClauseAsync()
        {
            var testCode = @"
using System.Linq;
public class TypeName
{
    public void MethodName()
    {
        var result =
            from x in new int[0]
            join abY in new int[0] on x equals abY
            select x;
    }
}";

            DiagnosticResult[] expected =
            {
                Diagnostic().WithArguments("variable", "abY").WithLocation(9, 18),
            };

            await VerifyCSharpDiagnosticAsync(testCode, expected).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestVariableInJoinIntoClauseAsync()
        {
            var testCode = @"
using System.Linq;
public class TypeName
{
    public void MethodName()
    {
        var result =
            from x in new int[0]
            join y in new int[0] on x equals y into abZ
            select abZ;
    }
}";

            DiagnosticResult[] expected =
            {
                Diagnostic().WithArguments("variable", "abZ").WithLocation(9, 53),
            };

            await VerifyCSharpDiagnosticAsync(testCode, expected).ConfigureAwait(false);
        }
    }
}
