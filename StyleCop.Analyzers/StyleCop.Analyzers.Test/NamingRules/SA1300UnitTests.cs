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

    public class SA1300UnitTests : CodeFixVerifier
    {
        [Fact]
        public async Task TestUpperCaseNamespaceAsync()
        {
            var testCode = @"namespace Test
{ 

}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestLowerCaseNamespaceAsync()
        {
            var testCode = @"namespace test
{ 

}";

            var fixedCode = @"namespace Test
{ 

}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithArguments("test").WithLocation(1, 11);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestLowerCaseComlicatedNamespaceAsync()
        {
            var testCode = @"namespace test.foo.bar
{

}";

            var fixedCode = @"namespace Test.Foo.Bar
{

}";

            DiagnosticResult[] expected = new[]
            {
                this.CSharpDiagnostic().WithArguments("test").WithLocation(1, 11),
                this.CSharpDiagnostic().WithArguments("test").WithLocation(1, 11),
                this.CSharpDiagnostic().WithArguments("test").WithLocation(1, 11),
                this.CSharpDiagnostic().WithArguments("foo").WithLocation(1, 16),
                this.CSharpDiagnostic().WithArguments("foo").WithLocation(1, 16),
                this.CSharpDiagnostic().WithArguments("foo").WithLocation(1, 16),
                this.CSharpDiagnostic().WithArguments("bar").WithLocation(1, 20),
                this.CSharpDiagnostic().WithArguments("bar").WithLocation(1, 20),
                this.CSharpDiagnostic().WithArguments("bar").WithLocation(1, 20)
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestUpperCaseClassAsync()
        {
            var testCode = @"public class Test
{ 

}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestLowerCaseClassAsync()
        {
            var testCode = @"public class test
{ 

}";
            var fixedCode = @"public class Test
{ 

}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithArguments("test").WithLocation(1, 14);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestUpperCaseInterfaceAsync()
        {
            var testCode = @"public interface Test
{

}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestLowerCaseInterfaceAsync()
        {
            var testCode = @"public interface test
{

}";

            // Reported as SA1302
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestUpperCaseStructAsync()
        {
            var testCode = @"public struct Test 
{ 

}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestLowerCaseStructAsync()
        {
            var testCode = @"public struct test 
{ 

}";
            var fixedCode = @"public struct Test 
{ 

}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithArguments("test").WithLocation(1, 15);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestUpperCaseEnumAsync()
        {
            var testCode = @"public enum Test 
{ 

}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestLowerCaseEnumAsync()
        {
            var testCode = @"public enum test 
{ 

}";
            var fixedCode = @"public enum Test 
{ 

}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithArguments("test").WithLocation(1, 13);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestUpperCaseDelegateAsync()
        {
            var testCode = @"public class TestClass
{ 
public delegate void Test();
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestLowerCaseDelegateAsync()
        {
            var testCode = @"public class TestClass
{ 
public delegate void test();
}";
            var fixedCode = @"public class TestClass
{ 
public delegate void Test();
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithArguments("test").WithLocation(3, 22);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestUpperCaseEventAsync()
        {
            var testCode = @"public class TestClass
{
    public delegate void Test();
    Test _testEvent;
    public event Test TestEvent
    {
        add
        {
            _testEvent += value;
        }
        remove
        {
            _testEvent -= value;
        }
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestLowerCaseEventAsync()
        {
            var testCode = @"public class TestClass
{
    public delegate void Test();
    Test _testEvent;
    public event Test testEvent
    {
        add
        {
            _testEvent += value;
        }
        remove
        {
            _testEvent -= value;
        }
    }
}";
            var fixedCode = @"public class TestClass
{
    public delegate void Test();
    Test _testEvent;
    public event Test TestEvent
    {
        add
        {
            _testEvent += value;
        }
        remove
        {
            _testEvent -= value;
        }
    }
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithArguments("testEvent").WithLocation(5, 23);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestUpperCaseEventFieldAsync()
        {
            var testCode = @"public class TestClass
{
    public delegate void Test();
    public event Test TestEvent;
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestLowerCaseEventFieldAsync()
        {
            var testCode = @"public class TestClass
{
    public delegate void Test();
    public event Test testEvent;
}";
            var fixedCode = @"public class TestClass
{
    public delegate void Test();
    public event Test TestEvent;
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithArguments("testEvent").WithLocation(4, 23);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestUpperCaseMethodAsync()
        {
            var testCode = @"public class TestClass
{
public void Test()
{
}
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestLowerCaseMethodAsync()
        {
            var testCode = @"public class TestClass
{
public void test()
{
}
}";
            var fixedCode = @"public class TestClass
{
public void Test()
{
}
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithArguments("test").WithLocation(3, 13);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestUpperCasePropertyAsync()
        {
            var testCode = @"public class TestClass
{
public string Test { get; set; }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestLowerCasePropertyAsync()
        {
            var testCode = @"public class TestClass
{
public string test { get; set; }
}";
            var fixedCode = @"public class TestClass
{
public string Test { get; set; }
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithArguments("test").WithLocation(3, 15);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestUpperCasePublicFieldAsync()
        {
            var testCode = @"public class TestClass
{
public string Test;
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestUpperCaseInternalFieldAsync()
        {
            var testCode = @"public class TestClass
{
internal string Test;
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestUpperCaseConstFieldAsync()
        {
            var testCode = @"public class TestClass
{
const string Test = ""value"";
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestUpperCaseProtectedReadOnlyFieldAsync()
        {
            var testCode = @"public class TestClass
{
protected readonly string Test;
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestLowerCaseProtectedFieldAsync()
        {
            var testCode = @"public class TestClass
{
protected string Test;
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestLowerCaseReadOnlyFieldAsync()
        {
            var testCode = @"public class TestClass
{
readonly string test;
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestLowerCasePublicFieldAsync()
        {
            var testCode = @"public class TestClass
{
public string test;
}";

            // Handled by SA1307
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestLowerCaseInternalFieldAsync()
        {
            var testCode = @"public class TestClass
{
internal string test;
}";

            // Handled by SA1307
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestLowerCaseConstFieldAsync()
        {
            var testCode = @"public class TestClass
{
const string test = ""value"";
}";

            // Reported as SA1303
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestNativeMethodsExceptionAsync()
        {
            var testCode = @"public class TestNativeMethods
{
public string test;
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestLowerCaseProtectedReadOnlyFieldAsync()
        {
            var testCode = @"public class TestClass
{
protected readonly string test;
}";

            // Handled by SA1304
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestLowerCaseOverriddenMembersAsync()
        {
            var testCode = @"public class TestClass : BaseClass
{
    public override int bar
    {
        get
        {
            return 0;
        }
    }

    public override event System.EventHandler fooBar
    {
        add { }
        remove { }
    }

    public override void foo()
    {

    }
}

public abstract class BaseClass
{
    public abstract void foo();
    public abstract int bar { get; }
    public abstract event System.EventHandler fooBar;
}";
            var fixedCode = @"public class TestClass : BaseClass
{
    public override int Bar
    {
        get
        {
            return 0;
        }
    }

    public override event System.EventHandler FooBar
    {
        add { }
        remove { }
    }

    public override void Foo()
    {

    }
}

public abstract class BaseClass
{
    public abstract void Foo();
    public abstract int Bar { get; }
    public abstract event System.EventHandler FooBar;
}";

            var expected = new[]
            {
                this.CSharpDiagnostic().WithLocation(25, 26).WithArguments("foo"),
                this.CSharpDiagnostic().WithLocation(26, 25).WithArguments("bar"),
                this.CSharpDiagnostic().WithLocation(27, 47).WithArguments("fooBar"),
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestLowerCaseInterfaceMembersAsync()
        {
            var testCode = @"public class TestClass : IInterface
{
    public int bar
    {
        get
        {
            return 0;
        }
    }

    public event System.EventHandler fooBar
    {
        add { }
        remove { }
    }

    public void foo()
    {

    }
}

public interface IInterface
{
    void foo();
    int bar { get; }
    event System.EventHandler fooBar;
}";
            var fixedCode = @"public class TestClass : IInterface
{
    public int Bar
    {
        get
        {
            return 0;
        }
    }

    public event System.EventHandler FooBar
    {
        add { }
        remove { }
    }

    public void Foo()
    {

    }
}

public interface IInterface
{
    void Foo();
    int Bar { get; }
    event System.EventHandler FooBar;
}";

            var expected = new[]
            {
                this.CSharpDiagnostic().WithLocation(25, 10).WithArguments("foo"),
                this.CSharpDiagnostic().WithLocation(26, 9).WithArguments("bar"),
                this.CSharpDiagnostic().WithLocation(27, 31).WithArguments("fooBar"),
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
        }

        protected override IEnumerable<DiagnosticAnalyzer> GetCSharpDiagnosticAnalyzers()
        {
            yield return new SA1300ElementMustBeginWithUpperCaseLetter();
        }

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new RenameToUpperCaseCodeFixProvider();
        }
    }
}
