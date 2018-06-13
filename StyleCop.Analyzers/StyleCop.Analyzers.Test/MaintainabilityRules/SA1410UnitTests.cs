// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.MaintainabilityRules
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.MaintainabilityRules;
    using TestHelper;
    using Xunit;

    public class SA1410UnitTests : CodeFixVerifier
    {
        [Fact]
        public async Task TestMissingParenthesisAsync()
        {
            var testCode = @"public class Foo
{
    public void Bar()
    {
        System.Func<int> getRandomNumber = delegate { return 3; };
    }
}";
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestNonEmptyParameterListAsync()
        {
            var testCode = @"public class Foo
{
    public void Bar()
    {
        System.Func<int, int> getNumber = delegate (int i) { return i; };
    }
}";
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestEmptyParameterListAsync()
        {
            var testCode = @"public class Foo
{
    public void Bar()
    {
        System.Func<int> getRandomNumber = delegate() { return 3; };
    }
}";
            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(5, 52);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestCodeFixAsync()
        {
            var oldSource = @"public class Foo
{
    public void Bar()
    {
        System.Func<int> getRandomNumber = delegate() { return 3; };
    }
}";

            var newSource = @"public class Foo
{
    public void Bar()
    {
        System.Func<int> getRandomNumber = delegate { return 3; };
    }
}";

            await this.VerifyCSharpFixAsync(oldSource, newSource, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestCodeFixDoesNotRemoveExteriorTriviaAsync()
        {
            var oldSource = @"public class Foo
{
    public void Bar()
    {
        System.Func<int> getRandomNumber = delegate/*Foo*/(/*Bar*/)/*Foo*/ { return 3; };
    }
}";

            var newSource = @"public class Foo
{
    public void Bar()
    {
        System.Func<int> getRandomNumber = delegate/*Foo*//*Bar*//*Foo*/ { return 3; };
    }
}";

            await this.VerifyCSharpFixAsync(oldSource, newSource, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verify that no diagnostic is produced when removing the parentheses from the delegate statement
        /// would result in ambiguity on which method overload to call.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [WorkItem(2572, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/2572")]
        public async Task TestMethodOverloadAmbiguityAsync()
        {
            var testCode = @"public class TestClass
{
    public delegate bool Delegate1();
    public delegate bool Delegate2(int x);

    public void TestMethod()
    {
        var thread = new System.Threading.Thread(delegate()
        {
            // ...
        });

        this.TestMethod2(delegate() { return true; });
        this.TestMethod3(delegate() { return true; });
    }

    public void TestMethod2(Delegate1 d)
    {
    }

    public void TestMethod2(Delegate2 d)
    {
    }
}

public static class TestClassExtensions
{
    public static void TestMethod3(this TestClass obj, TestClass.Delegate1 d)
    {
    }

    public static void TestMethod3(this TestClass obj, TestClass.Delegate2 d)
    {
    }
}
";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verify that the ambiguity detection is specific enough.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [WorkItem(2572, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/2572")]
        public async Task TestMethodOverloadNonAmbiguityAsync()
        {
            var testCode = @"public class TestClass
{
    public delegate bool Delegate1();
    public delegate bool Delegate2(int x);

    public void TestMethod()
    {
        this.TestMethod2(delegate() { return true; });
        this.TestMethod3(0, delegate() { return true; });
        this.TestMethod4(delegate() { return true; }, 1);
        this.TestMethod5(delegate() { return true; });
        this.TestMethod6(delegate() { return true; });
    }

    public void TestMethod2(Delegate1 d)
    {
    }

    public void TestMethod2(int d)
    {
    }

    public void TestMethod3(int x, Delegate1 d)
    {
    }

    public void TestMethod3(double x, Delegate2 d)
    {
    }

    public void TestMethod4(Delegate1 d, int x)
    {
    }

    public void TestMethod4(Delegate2 d, double x)
    {
    }

    public void TestMethod5(Delegate1 d)
    {
    }

    public void TestMethod5(Delegate2 d, double x)
    {
    }

    public void TestMethod6(Delegate1 d)
    {
    }
}

public static class TestClassExtensions
{
    public static void TestMethod6(this TestClass obj, TestClass.Delegate2 d)
    {
    }
}
";

            DiagnosticResult[] expectedResults =
            {
                this.CSharpDiagnostic().WithLocation(8, 34),
                this.CSharpDiagnostic().WithLocation(9, 37),
                this.CSharpDiagnostic().WithLocation(10, 34),
                this.CSharpDiagnostic().WithLocation(11, 34),
                this.CSharpDiagnostic().WithLocation(12, 34),
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expectedResults, CancellationToken.None).ConfigureAwait(false);
        }

        protected override IEnumerable<DiagnosticAnalyzer> GetCSharpDiagnosticAnalyzers()
        {
            yield return new SA1410RemoveDelegateParenthesisWhenPossible();
        }

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new SA1410SA1411CodeFixProvider();
        }
    }
}
