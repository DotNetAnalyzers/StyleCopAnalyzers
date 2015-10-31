// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.ReadabilityRules
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.ReadabilityRules;
    using TestHelper;
    using Xunit;

    public class SX1101UnitTests : CodeFixVerifier
    {
        /// <summary>
        /// Verifies that this prefixes are detected and removed.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task VerifyThisPrefixesAreDetectedAndRemovedAsync()
        {
            var testCode = @"using System;

public class BaseTestClass
{
    protected int BaseTestField;

    protected int BaseTestProperty { get; set; }

    public event EventHandler BaseTestEvent;

    protected void BaseTestMethod()
    {
    }
}

public class TestClass : BaseTestClass
{
    protected int TestField;
    
    public int TestProperty { get; set; }

    public event EventHandler TestEvent;

    public void TestMethod1()
    {
    }

    public void TestMethod2()
    {
        if ((this.BaseTestProperty == this.TestProperty) && (this.BaseTestField == this.TestField))
        {
            this.BaseTestMethod();
        }
        else
        {
            this.TestMethod1();
        }

        var x = this.listeners;
        this.BaseTestEvent += (s, e) => { };
        var listeners = this.TestEvent;
        if (listeners != null)
        {
            listeners(this, null);
        }

        var y = this.listeners;
    }

    public int listeners;
}
";
            var fixedTestCode = @"using System;

public class BaseTestClass
{
    protected int BaseTestField;

    protected int BaseTestProperty { get; set; }

    public event EventHandler BaseTestEvent;

    protected void BaseTestMethod()
    {
    }
}

public class TestClass : BaseTestClass
{
    protected int TestField;
    
    public int TestProperty { get; set; }

    public event EventHandler TestEvent;

    public void TestMethod1()
    {
    }

    public void TestMethod2()
    {
        if ((BaseTestProperty == TestProperty) && (BaseTestField == TestField))
        {
            BaseTestMethod();
        }
        else
        {
            TestMethod1();
        }

        var x = this.listeners;
        BaseTestEvent += (s, e) => { };
        var listeners = TestEvent;
        if (listeners != null)
        {
            listeners(this, null);
        }

        var y = this.listeners;
    }

    public int listeners;
}
";

            DiagnosticResult[] expected =
            {
                this.CSharpDiagnostic().WithLocation(30, 14),
                this.CSharpDiagnostic().WithLocation(30, 39),
                this.CSharpDiagnostic().WithLocation(30, 62),
                this.CSharpDiagnostic().WithLocation(30, 84),
                this.CSharpDiagnostic().WithLocation(32, 13),
                this.CSharpDiagnostic().WithLocation(36, 13),
                this.CSharpDiagnostic().WithLocation(40, 9),
                this.CSharpDiagnostic().WithLocation(41, 25)
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a constructor call using the 'this(...)' will not produce any diagnostics.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task VerifyThatConstructorCallsDoNotProduceDiagnosticsAsync()
        {
            var testCode = @"
public class TestClass
{
    private int _x;

    public TestClass(int x)
    {
        _x = x;
    }

    public TestClass()
        : this(1)
    {
    }
}
";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that extension methods will not produce any diagnostics.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task VerifyThatExtensionMethodsDoNotProduceDiagnosticsAsync()
        {
            var testCode = @"
public static class TestClass
{
    public static void TestExtensionMethod(this System.AppDomain appDomain)
    {
    }
}
";
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies indexers will not produce any diagnostics.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task VerifyThatIndexersDoNotProduceDiagnosticsAsync()
        {
            var testCode = @"
public class TestClass
{
    public int this[int index]
    {
        get { return index; }
    }
}
";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a necessary this prefix will not produce any diagnostics.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task VerifyThatNecessaryPrefixWillNotProduceDiagnosticsAsync()
        {
            var testCode = @"
public class TestClass
{
    private int test;

    public TestClass(int test)
    {
        this.test = test;
    }

    public void TestMethod(int test)
    {
        this.test = test;
    }
}
";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a necessary field reference prefix will not produce any diagnostics.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task VerifyThatNecessaryFieldReferencePrefixWillNotProduceDiagnosticAsync()
        {
            var testCode = @"
public class TestClass
{
    private int test;

    public TestClass()
    {
        var test = this.test;
    }

    public void TestMethod()
    {
        var test = this.test;
    }
}
";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestExtensionMethodAsync()
        {
            var testCode = @"
class ClassName
{
    string P => string.Empty;

    public void Method()
    {
        int value = this.P();
    }
}

static class ClassNameExtensions
{
    public static int P(this ClassName instance) => 3;
}
";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestTypeArgumentCollisionAsync()
        {
            var testCode = @"
class BaseClass
{
    protected int Options => 3;
}

class DerivedClass : BaseClass
{
    void Method<Options>()
    {
        int options = this.Options;
    }
}
";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestPropertyValueKeywordCollisionAsync()
        {
            var testCode = @"
class ClassName
{
    int value;

    int Value
    {
        get { return value; }
        set { this.value = value; }
    }
}
";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestEscapedParameterCollisionAsync()
        {
            var testCode = @"
class ClassName
{
    int @object;

    ClassName(int @object)
    {
        this.@object = @object;
    }
}
";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestFieldUseBeforeVariableDefinitionAsync()
        {
            var testCode = @"
class ClassName
{
    object p;

    object Property
    {
        get
        {
            if (this.p != null) return this.p;
            object p = new object();
            return this.p = p;
        }
    }
}
";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        protected override IEnumerable<DiagnosticAnalyzer> GetCSharpDiagnosticAnalyzers()
        {
            yield return new SX1101DoNotPrefixLocalMembersWithThis();
        }

        /// <inheritdoc/>
        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new SX1101CodeFixProvider();
        }
    }
}
