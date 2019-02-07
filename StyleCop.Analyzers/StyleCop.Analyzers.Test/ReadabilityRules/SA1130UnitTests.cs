﻿// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.ReadabilityRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Testing;
    using TestHelper;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.ReadabilityRules.SA1130UseLambdaSyntax,
        StyleCop.Analyzers.ReadabilityRules.SA1130CodeFixProvider>;

    public class SA1130UnitTests
    {
        private static readonly DiagnosticDescriptor CS1065 =
                   new DiagnosticDescriptor(nameof(CS1065), "Title", "Default values are not valid in this context.", "Category", DiagnosticSeverity.Error, AnalyzerConstants.EnabledByDefault);

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
            await VerifyCSharpFixAsync(testCode, expected, fixedCode).ConfigureAwait(false);
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

            await VerifyCSharpFixAsync(testCode, expected, fixedCode).ConfigureAwait(false);
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
            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults).ConfigureAwait(false);
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

            await VerifyCSharpFixAsync(testCode, expected, fixedCode).ConfigureAwait(false);
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
        Action a = delegate() { Console.WriteLine(); };
        Action b = delegate() { Console.WriteLine(); };
        Action<int> c = delegate(int x) { Console.WriteLine(); };
        Action<int, int> d = delegate(int x, int y) { Console.WriteLine(); };
        Action<int, int> e = delegate (int x, int y = 0) { Console.WriteLine(); };
        Action<int, int> f = delegate (int x, [Obsolete]int y) { Console.WriteLine(); };
        Action<int, int> g = delegate (int x, params int y) { Console.WriteLine(); };
        Action<int> h = delegate (int x, __arglist) { Console.WriteLine(); };
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
            var expected = new[]
            {
                Diagnostic().WithLocation(8, 20),
                Diagnostic().WithLocation(9, 20),
                Diagnostic().WithLocation(10, 25),
                Diagnostic().WithLocation(11, 30),
                Diagnostic().WithLocation(12, 30),
                Diagnostic(CS1065).WithLocation(12, 53),
                Diagnostic().WithLocation(13, 30),
                Diagnostic(CS7014).WithLocation(13, 47),
                Diagnostic().WithLocation(14, 30),
                Diagnostic(CS1670).WithLocation(14, 47),
                Diagnostic().WithLocation(15, 25),
                Diagnostic(CS1669).WithLocation(15, 42),
            };

            var expectedAfterFix = new[]
            {
                Diagnostic().WithLocation(12, 30),
                Diagnostic(CS1065).WithLocation(12, 53),
                Diagnostic().WithLocation(13, 30),
                Diagnostic(CS7014).WithLocation(13, 47),
                Diagnostic().WithLocation(14, 30),
                Diagnostic(CS1670).WithLocation(14, 47),
                Diagnostic().WithLocation(15, 25),
                Diagnostic(CS1669).WithLocation(15, 42),
            };

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
        Action action1 = /*a*/()/*b*/ => { };
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
            await VerifyCSharpFixAsync(testCode, expected, fixedCode).ConfigureAwait(false);
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

            await VerifyCSharpFixAsync(testCode, expected, fixedCode).ConfigureAwait(false);
        }
    }
}
