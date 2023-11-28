// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.Test.ReadabilityRules
{
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Testing;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.ReadabilityRules.SA1130UseLambdaSyntax,
        StyleCop.Analyzers.ReadabilityRules.SA1130CodeFixProvider>;

    public class SA1130UnitTests
    {
        [SuppressMessage("MicrosoftCodeAnalysisDesign", "RS1032:Define diagnostic message correctly", Justification = "The message here matches the compiler.")]
        private static readonly DiagnosticDescriptor CS1065 =
                   new DiagnosticDescriptor(nameof(CS1065), "Title", "Default values are not valid in this context.", "Category", DiagnosticSeverity.Error, AnalyzerConstants.EnabledByDefault);

        [SuppressMessage("MicrosoftCodeAnalysisDesign", "RS1032:Define diagnostic message correctly", Justification = "The message here matches the compiler.")]
        private static readonly DiagnosticDescriptor CS7014 =
                   new DiagnosticDescriptor(nameof(CS7014), "Title", "Attributes are not valid in this context.", "Category", DiagnosticSeverity.Error, AnalyzerConstants.EnabledByDefault);

        private static readonly DiagnosticDescriptor CS1670 =
                          new DiagnosticDescriptor(nameof(CS1670), "Title", "params is not valid in this context", "Category", DiagnosticSeverity.Error, AnalyzerConstants.EnabledByDefault);

        private static readonly DiagnosticDescriptor CS1669 =
                          new DiagnosticDescriptor(nameof(CS1669), "Title", "__arglist is not valid in this context", "Category", DiagnosticSeverity.Error, AnalyzerConstants.EnabledByDefault);

        [Fact]
        public async Task TestSimpleDelegateUseAsync()
        {
            var testCode = @"
using System;
public class TypeName
{
    public void Test()
    {
        Action action1 = delegate { };
        Action action2 = delegate() { };
        Action<int> action3 = delegate(int i) { };
    }
}";

            string fixedCode = @"
using System;
public class TypeName
{
    public void Test()
    {
        Action action1 = () => { };
        Action action2 = () => { };
        Action<int> action3 = i => { };
    }
}";

            var expected = new[]
            {
                Diagnostic().WithLocation(7, 26),
                Diagnostic().WithLocation(8, 26),
                Diagnostic().WithLocation(9, 31),
            };
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestDelegateUseAsMethodArgumentsAsync()
        {
            var testCode = @"
using System;
public class TypeName
{
    public void Test(Action argument)
    {

    }

    public void Test()
    {
        Test(delegate { });
    }
}";

            string fixedCode = @"
using System;
public class TypeName
{
    public void Test(Action argument)
    {

    }

    public void Test()
    {
        Test(() => { });
    }
}";

            var expected = Diagnostic().WithLocation(12, 14);

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestDelegateUseAsMethodArgumentsWithConflictingExpressionOverloadAsync()
        {
            var testCode = @"
using System;
using System.Linq.Expressions;
public class TypeName
{
    public void Test(Action argument)
    {

    }

    public void Test(Expression<Action> argument)
    {

    }

    public void Test()
    {
        Test(delegate { });
    }
}";
            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestDelegateUseAsMethodArgumentsWithNonConflictingExpressionOverloadAsync()
        {
            var testCode = @"
using System;
using System.Linq.Expressions;
public class TypeName
{
    public void Test(Action argument)
    {

    }

    public void Test(Expression<Func<int>> argument)
    {

    }

    public void Test()
    {
        Test(delegate { });
    }
}";

            var fixedCode = @"
using System;
using System.Linq.Expressions;
public class TypeName
{
    public void Test(Action argument)
    {

    }

    public void Test(Expression<Func<int>> argument)
    {

    }

    public void Test()
    {
        Test(() => { });
    }
}";
            var expected = Diagnostic().WithLocation(18, 14);

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestCodeFixSpecialCasesAsync()
        {
            var testCode = @"
using System;

internal class TypeName
{
    private void Method(object o)
    {
        Action a = [|delegate|]() { Console.WriteLine(); };
        Action b = [|delegate|]() { Console.WriteLine(); };
        Action<int> c = [|delegate|](int x) { Console.WriteLine(); };
        Action<int, int> d = [|delegate|](int x, int y) { Console.WriteLine(); };
        Action<int, int> e = [|delegate|] (int x, int y = 0) { Console.WriteLine(); };
        Action<int, int> f = [|delegate|] (int x, [Obsolete]int y) { Console.WriteLine(); };
        Action<int, int> g = [|delegate|] (int x, params int y) { Console.WriteLine(); };
        Action<int> h = [|delegate|] (int x, __arglist) { Console.WriteLine(); };
    }
}";

            var fixedCode = @"
using System;

internal class TypeName
{
    private void Method(object o)
    {
        Action a = () => { Console.WriteLine(); };
        Action b = () => { Console.WriteLine(); };
        Action<int> c = x => { Console.WriteLine(); };
        Action<int, int> d = (x, y) => { Console.WriteLine(); };
        Action<int, int> e = delegate (int x, int y = 0) { Console.WriteLine(); };
        Action<int, int> f = delegate (int x, [Obsolete]int y) { Console.WriteLine(); };
        Action<int, int> g = delegate (int x, params int y) { Console.WriteLine(); };
        Action<int> h = delegate (int x, __arglist) { Console.WriteLine(); };
    }
}";

            var expected = this.GetCompilerExpectedResultCodeFixSpecialCases();

            var expectedAfterFix = this.GetCompilerExpectedResultCodeFixSpecialCases()
                .Concat(new[]
                {
                    Diagnostic().WithLocation(12, 30),
                    Diagnostic().WithLocation(13, 30),
                    Diagnostic().WithLocation(14, 30),
                    Diagnostic().WithLocation(15, 25),
                });

            var test = new CSharpTest
            {
                TestCode = testCode,
                FixedCode = fixedCode,
                NumberOfFixAllIterations = 2,
            };

            test.ExpectedDiagnostics.AddRange(expected);
            test.RemainingDiagnostics.AddRange(expectedAfterFix);
            await test.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestSimpleDelegateUseWithTriviaAsync()
        {
            var testCode = @"
using System; using System.Collections.Generic;
public class TypeName
{
    public void Test()
    {
        Action action1 = /*a*/delegate/*b*/{ };
        Action action2 = /*a*/delegate/*b*/(/*c*/)/*d*/ { };
        Action<int> action3 = /*a*/delegate/*b*/(/*c*/int/*d*/i/*e*/)/*f*/{ };
        Action<List<int>> action4 = delegate(List</* c1 */ int> i) { };
    }
}";

            string fixedCode = @"
using System; using System.Collections.Generic;
public class TypeName
{
    public void Test()
    {
        Action action1 = /*a*/() =>/*b*/{ };
        Action action2 = /*a*//*b*/(/*c*/)/*d*/ => { };
        Action<int> action3 = /*a*//*b*//*c*//*d*/i/*e*//*f*/ => { };
        Action<List<int>> action4 = i => { };
    }
}";

            var expected = new[]
            {
                Diagnostic().WithLocation(7, 31),
                Diagnostic().WithLocation(8, 31),
                Diagnostic().WithLocation(9, 36),
                Diagnostic().WithLocation(10, 37),
            };
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verify that expansion of a delegate without parameters will generate a lambda with the necessary parameters.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [WorkItem(2593, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/2593")]
        public async Task VerifyThatDelegateExpansionWillNotGenerateInvalidCodeAsync()
        {
            var testCode = @"
using System;

namespace StyleCopDemo
{
    class Program
    {
        public delegate void TestDelegate(int arg1, double arg2);
        public static event EventHandler TestEvent;

        static void Main(string[] args)
        {
            EventHandler anon = delegate { Console.WriteLine(""hello""); };
            TestEvent += delegate { Console.WriteLine(""hello""); };
            TestEvent -= delegate { Console.WriteLine(""hello""); };
        }

        public static void TestMethod()
        {
            object sender = null;
            EventArgs e = EventArgs.Empty;

            EventHandler anon = delegate { Console.WriteLine(""hello""); };

            TestMethod2(delegate { Console.WriteLine(""hello""); });
        }

        public static void TestMethod2(TestDelegate testDelegate)
        {
        }
    }
}
";

            var fixedCode = @"
using System;

namespace StyleCopDemo
{
    class Program
    {
        public delegate void TestDelegate(int arg1, double arg2);
        public static event EventHandler TestEvent;

        static void Main(string[] args)
        {
            EventHandler anon = (sender, e) => { Console.WriteLine(""hello""); };
            TestEvent += (sender, e) => { Console.WriteLine(""hello""); };
            TestEvent -= (sender, e) => { Console.WriteLine(""hello""); };
        }

        public static void TestMethod()
        {
            object sender = null;
            EventArgs e = EventArgs.Empty;

            EventHandler anon = (sender1, e1) => { Console.WriteLine(""hello""); };

            TestMethod2((arg1, arg2) => { Console.WriteLine(""hello""); });
        }

        public static void TestMethod2(TestDelegate testDelegate)
        {
        }
    }
}
";

            DiagnosticResult[] expected =
            {
                Diagnostic().WithLocation(13, 33),
                Diagnostic().WithLocation(14, 26),
                Diagnostic().WithLocation(15, 26),
                Diagnostic().WithLocation(23, 33),
                Diagnostic().WithLocation(25, 25),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestOverloadResolutionFailureAsync()
        {
            var testCode = @"
using System;
using System.Linq.Expressions;
public class TypeName
{
    public void Test(string argument)
    {

    }

    public void Test()
    {
        Test(delegate { });
    }
}";

            var expected = DiagnosticResult.CompilerError("CS1660").WithLocation(13, 14);

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestParamsAsync()
        {
            var testCode = @"
using System;
public class TypeName
{
    public void Test(params Action[] argument)
    {

    }

    public void Test()
    {
        Test(delegate { }, delegate { });
    }
}";

            string fixedCode = @"
using System;
public class TypeName
{
    public void Test(params Action[] argument)
    {

    }

    public void Test()
    {
        Test(() => { }, () => { });
    }
}";

            var expected = new[]
            {
                Diagnostic().WithLocation(12, 14),
                Diagnostic().WithLocation(12, 28),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestDelegateUseAsConstructorArgumentsAsync()
        {
            var testCode = @"
using System;
public class TypeName
{
    public TypeName(Action argument)
    {

    }

    public void Test()
    {
        new TypeName(delegate { });
    }
}";
            string fixedCode = @"
using System;
public class TypeName
{
    public TypeName(Action argument)
    {

    }

    public void Test()
    {
        new TypeName(() => { });
    }
}";
            var expected = Diagnostic().WithLocation(12, 22);
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestDelegateUseAsConstructorArgumentsWithConflictingExpressionOverloadAsync()
        {
            var testCode = @"
using System;
using System.Linq.Expressions;
public class TypeName
{
    public TypeName(Action argument)
    {
     
    }
    
    public TypeName(Expression<Action> argument)
    {
    
    }
    
    public void Test()
    {
        new TypeName(delegate { });
    }
}";
            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestDelegateUseAsConstructorArgumentsWithNonConflictingExpressionOverloadAsync()
        {
            var testCode = @"
using System;
using System.Linq.Expressions;
public class TypeName
{
    public TypeName(Action argument)
    {

    }

    public TypeName(Expression<Func<int>> argument)
    {

    }

    public void Test()
    {
        new TypeName(delegate { });
    }
}";
            var fixedCode = @"
using System;
using System.Linq.Expressions;
public class TypeName
{
    public TypeName(Action argument)
    {

    }

    public TypeName(Expression<Func<int>> argument)
    {

    }

    public void Test()
    {
        new TypeName(() => { });
    }
}";
            var expected = Diagnostic().WithLocation(18, 22);
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestDelegateUseAsIndexerArgumentsAsync()
        {
            var testCode = @"
using System;
public class TypeName
{
    public int this[Action argument]
    {
        get { return 0; }
    }

    public void Test()
    {
        int _ = this[delegate { }];
    }
}";
            string fixedCode = @"
using System;
public class TypeName
{
    public int this[Action argument]
    {
        get { return 0; }
    }

    public void Test()
    {
        int _ = this[() => { }];
    }
}";
            var expected = Diagnostic().WithLocation(12, 22);
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestDelegateUseAsIndexerArgumentsWithConflictingExpressionOverloadAsync()
        {
            var testCode = @"
using System;
using System.Linq.Expressions;
public class TypeName
{
    public int this[Action argument]
    {
        get { return 0; }
    }
     public int this[Expression<Action> argument]
    {
        get { return 0; }
    }
     public void Test()
    {
        int _ = this[delegate { }];
    }
}";
            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestDelegateUseAsIndexerArgumentsWithNonConflictingExpressionOverloadAsync()
        {
            var testCode = @"
using System;
using System.Linq.Expressions;
public class TypeName
{
    public int this[Action argument]
    {
        get { return 0; }
    }

    public int this[Expression<Func<int>> argument]
    {
        get { return 0; }
    }

    public void Test()
    {
        int _ = this[delegate { }];
    }
}";
            var fixedCode = @"
using System;
using System.Linq.Expressions;
public class TypeName
{
    public int this[Action argument]
    {
        get { return 0; }
    }

    public int this[Expression<Func<int>> argument]
    {
        get { return 0; }
    }

    public void Test()
    {
        int _ = this[() => { }];
    }
}";
            var expected = Diagnostic().WithLocation(18, 22);
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestInvalidOverloadAsync()
        {
            var testCode = @"
using System;
using System.Linq.Expressions;
public unsafe class TypeName
{
    void Method(int* data) { throw null; }
    void Caller() => Method(delegate { });
}";
            var fixedCode = @"
using System;
using System.Linq.Expressions;
public unsafe class TypeName
{
    void Method(int* data) { throw null; }
    void Caller() => Method(delegate { });
}";
            var expected = DiagnosticResult.CompilerError("CS1660").WithLocation(7, 29);
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task VerifyThatCodeFixDoesNotCrashOnMissingEventSymbolAsync()
        {
            var testCode = @"
using System;

namespace StyleCopDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            TestEvent -= delegate { Console.WriteLine(""hello""); };
        }
    }
}
";

            DiagnosticResult[] expected =
            {
                Diagnostic().WithLocation(10, 26),
                DiagnosticResult.CompilerError("CS0103").WithLocation(10, 13),
            };

            await VerifyCSharpFixAsync(testCode, expected, testCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(2902, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/2902")]
        public async Task VerifyThatCodeFixDoesNotCrashOnDelegateReturnAsync()
        {
            var testCode = @"using System;
public class TestClass
{
    public static EventHandler TestMethod1()
    {
        return delegate
        {
        };
    }

    public static EventHandler TestMethod2() => delegate
    {
    };

    public static EventHandler TestProperty1
    {
        get
        {
            return delegate
            {
            };
        }
    }

    public static EventHandler TestProperty2 => delegate
    {
    };
}";

            var fixedCode = @"using System;
public class TestClass
{
    public static EventHandler TestMethod1()
    {
        return (sender, e) =>
        {
        };
    }

    public static EventHandler TestMethod2() => (sender, e) =>
    {
    };

    public static EventHandler TestProperty1
    {
        get
        {
            return (sender, e) =>
            {
            };
        }
    }

    public static EventHandler TestProperty2 => (sender, e) =>
    {
    };
}";

            DiagnosticResult[] expected =
            {
                Diagnostic().WithLocation(6, 16),
                Diagnostic().WithLocation(11, 49),
                Diagnostic().WithLocation(19, 20),
                Diagnostic().WithLocation(25, 49),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(2902, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/2902")]
        public async Task VerifyThatEventInitializersWorkAsExpectedAsync()
        {
            var testCode = @"using System;
public class TestClass
{
    public static event EventHandler StaticEvent = delegate { };
    public event EventHandler InstanceEvent = delegate { };
}
";

            var fixedCode = @"using System;
public class TestClass
{
    public static event EventHandler StaticEvent = (sender, e) => { };
    public event EventHandler InstanceEvent = (sender, e) => { };
}
";

            DiagnosticResult[] expected =
            {
                Diagnostic().WithLocation(4, 52),
                Diagnostic().WithLocation(5, 47),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(2902, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/2902")]
        public async Task VerifyInvalidCodeConstructionsAsync()
        {
            var testCode = @"using System;
public class TestClass
{
    public static EventHandler[] TestMethod() => [|delegate|] { };
}
";

            var expected = new[]
            {
                DiagnosticResult.CompilerError("CS1660").WithLocation(4, 50),
            };

            var test = new CSharpTest
            {
                TestCode = testCode,
                FixedCode = testCode,
            };

            test.ExpectedDiagnostics.AddRange(expected);
            test.RemainingDiagnostics.AddRange(expected);
            await test.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(2997, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/2997")]
        public async Task VerifyDelegateConstructionAsync()
        {
            var testCode = @"using System;
using System.Collections.Generic;

public class TestClass
{
    private Dictionary<string, Func<int>> items = new Dictionary<string, Func<int>>()
    {
        { ""a"", delegate { return 0; } },
        { ""b"", () => 1 },
    };
}
";

            var fixedCode = @"using System;
using System.Collections.Generic;

public class TestClass
{
    private Dictionary<string, Func<int>> items = new Dictionary<string, Func<int>>()
    {
        { ""a"", () => { return 0; } },
        { ""b"", () => 1 },
    };
}
";

            DiagnosticResult[] expected =
            {
                Diagnostic().WithLocation(8, 16),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(3279, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3279")]
        public async Task TestDelegateUsedAsSecondNamedArgumentAsync()
        {
            var testCode = @"
using System;
using System.Linq;
public class TypeName
{
    public void Test()
    {
        Test2(resolve: delegate
        {
            return """";
        });
    }

    private void Test2(string description = null, Func<object, string> resolve = null)
    {
        resolve(0);
    }
}";

            string fixedCode = @"
using System;
using System.Linq;
public class TypeName
{
    public void Test()
    {
        Test2(resolve: arg =>
        {
            return """";
        });
    }

    private void Test2(string description = null, Func<object, string> resolve = null)
    {
        resolve(0);
    }
}";

            var expected = new[]
            {
                Diagnostic().WithSpan(8, 24, 8, 32),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(3279, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3279")]
        public async Task VerifyThatUnknownNamedParameterWontCauseCrashAsync()
        {
            var testCode = @"
using System;
using System.Linq;
public class TypeName
{
    public void Test()
    {
        Test2(unknownParam: delegate
        {
            return """";
        });
    }

    private void Test2(string description = null, Func<object, string> resolve = null)
    {
        resolve(0);
    }
}";

            var expected = DiagnosticResult.CompilerError("CS1739")
                .WithMessage("The best overload for 'Test2' does not have a parameter named 'unknownParam'")
                .WithSpan(8, 15, 8, 27)
                .WithArguments("Test2", "unknownParam");

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [InlineData(
            "(Func<int>)[|delegate|] { return 1; }",
            "(Func<int>)(() => { return 1; })")]
        [InlineData(
            "(Func<int>)[|delegate|]() { return 1; }",
            "(Func<int>)(() => { return 1; })")]
        [InlineData(
            "(Func<int, int>)[|delegate|] { return 1; }",
            "(Func<int, int>)(arg => { return 1; })")]
        [InlineData(
            "(Func<int, int>)[|delegate|](int x) { return 1; }",
            "(Func<int, int>)(x => { return 1; })")]
        [InlineData(
            "(Func<int, int, int>)[|delegate|] { return 1; }",
            "(Func<int, int, int>)((arg1, arg2) => { return 1; })")]
        [InlineData(
            "(Func<int, int, int>)[|delegate|](int x, int y) { return 1; }",
            "(Func<int, int, int>)((x, y) => { return 1; })")]
        [WorkItem(3510, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3510")]
        public async Task TestDelegateUsedInCastAsync(string testExpression, string fixedExpression)
        {
            var testCode = $@"
using System;

public class TypeName
{{
    public void Test()
    {{
        var z = {testExpression};
    }}
}}";

            var fixedCode = $@"
using System;

public class TypeName
{{
    public void Test()
    {{
        var z = {fixedExpression};
    }}
}}";

            await VerifyCSharpFixAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        protected virtual DiagnosticResult[] GetCompilerExpectedResultCodeFixSpecialCases()
        {
            return new[]
            {
                Diagnostic(CS1065).WithLocation(12, 53),
                Diagnostic(CS7014).WithLocation(13, 47),
                Diagnostic(CS1670).WithLocation(14, 47),
                Diagnostic(CS1669).WithLocation(15, 42),
            };
        }
    }
}
