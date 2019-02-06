// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.NamingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.Helpers;
    using TestHelper;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.NamingRules.SA1300ElementMustBeginWithUpperCaseLetter,
        StyleCop.Analyzers.NamingRules.RenameToUpperCaseCodeFixProvider>;

    [UseCulture("en-US")]
    public class SA1300UnitTests
    {
        [Fact]
        public async Task TestUpperCaseNamespaceAsync()
        {
            var testCode = @"namespace Test
{ 

}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            DiagnosticResult expected = Diagnostic().WithArguments("test").WithLocation(1, 11);
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
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
                Diagnostic().WithArguments("test").WithLocation(1, 11),
                Diagnostic().WithArguments("foo").WithLocation(1, 16),
                Diagnostic().WithArguments("bar").WithLocation(1, 20),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestUpperCaseClassAsync()
        {
            var testCode = @"public class Test
{ 

}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            DiagnosticResult expected = Diagnostic().WithArguments("test").WithLocation(1, 14);
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestLowerCaseClassWithConflictAsync()
        {
            var testCode = @"public class test
{
}

public class Test { }";
            var fixedCode = @"public class Test1
{
}

public class Test { }";

            DiagnosticResult expected = Diagnostic().WithArguments("test").WithLocation(1, 14);
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestUpperCaseInterfaceAsync()
        {
            var testCode = @"public interface Test
{

}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestLowerCaseInterfaceAsync()
        {
            var testCode = @"public interface test
{

}";

            // Reported as SA1302
            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestUpperCaseStructAsync()
        {
            var testCode = @"public struct Test 
{ 

}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            DiagnosticResult expected = Diagnostic().WithArguments("test").WithLocation(1, 15);
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestLowerCaseStructWithConflictAsync()
        {
            var testCode = @"public struct test
{
}

public class Test { }";
            var fixedCode = @"public struct Test1
{
}

public class Test { }";

            DiagnosticResult expected = Diagnostic().WithArguments("test").WithLocation(1, 15);
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestUpperCaseEnumAsync()
        {
            var testCode = @"public enum Test 
{ 

}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            DiagnosticResult expected = Diagnostic().WithArguments("test").WithLocation(1, 13);
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestLowerCaseEnumWithConflictAsync()
        {
            var testCode = @"public enum test
{
}

public class Test { }";
            var fixedCode = @"public enum Test1
{
}

public class Test { }";

            DiagnosticResult expected = Diagnostic().WithArguments("test").WithLocation(1, 13);
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestLowerCaseEnumWithMemberMatchingTargetNameAsync()
        {
            var testCode = @"public enum test
{
    Test
}";
            var fixedCode = @"public enum Test
{
    Test
}";

            DiagnosticResult expected = Diagnostic().WithArguments("test").WithLocation(1, 13);
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestUpperCaseEnumMemberAsync()
        {
            var testCode = @"public enum Test
{
    Member
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestLowerCaseEnumMemberAsync()
        {
            var testCode = @"public enum Test
{
    member
}";
            var fixedCode = @"public enum Test
{
    Member
}";

            DiagnosticResult expected = Diagnostic().WithArguments("member").WithLocation(3, 5);
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestLowerCaseEnumMemberWithConflictAsync()
        {
            var testCode = @"public enum Test
{
    member,
    Member
}";
            var fixedCode = @"public enum Test
{
    Member1,
    Member
}";

            DiagnosticResult expected = Diagnostic().WithArguments("member").WithLocation(3, 5);
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestLowerCaseEnumMemberWithTwoConflictsAsync()
        {
            var testCode = @"public enum Test
{
    member,
    Member,
    Member1,
}";
            var fixedCode = @"public enum Test
{
    Member2,
    Member,
    Member1,
}";

            DiagnosticResult expected = Diagnostic().WithArguments("member").WithLocation(3, 5);
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestLowerCaseEnumMemberWithNumberAndConflictAsync()
        {
            var testCode = @"public enum Test
{
    member1,
    Member1
}";
            var fixedCode = @"public enum Test
{
    Member11,
    Member1
}";

            DiagnosticResult expected = Diagnostic().WithArguments("member1").WithLocation(3, 5);
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestUpperCaseDelegateAsync()
        {
            var testCode = @"public class TestClass
{ 
public delegate void Test();
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            DiagnosticResult expected = Diagnostic().WithArguments("test").WithLocation(3, 22);
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestLowerCaseDelegateWithConflictAsync()
        {
            var testCode = @"public class Test1
{
public delegate void test();

public int Test => 0;
}";
            var fixedCode = @"public class Test1
{
public delegate void Test2();

public int Test => 0;
}";

            DiagnosticResult expected = Diagnostic().WithArguments("test").WithLocation(3, 22);
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
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

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            DiagnosticResult expected = Diagnostic().WithArguments("testEvent").WithLocation(5, 23);
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestLowerCaseEventWithConflictAsync()
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

    public int TestEvent => 0;
}";
            var fixedCode = @"public class TestClass
{
    public delegate void Test();
    Test _testEvent;
    public event Test TestEvent1
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

    public int TestEvent => 0;
}";

            DiagnosticResult expected = Diagnostic().WithArguments("testEvent").WithLocation(5, 23);
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestUpperCaseEventFieldAsync()
        {
            var testCode = @"public class TestClass
{
    public delegate void Test();
    public event Test TestEvent;
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            DiagnosticResult expected = Diagnostic().WithArguments("testEvent").WithLocation(4, 23);
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestLowerCaseEventFieldWithConflictAsync()
        {
            var testCode = @"public class TestClass
{
    public delegate void Test();
    public event Test testEvent;
    public event Test TestEvent;
}";
            var fixedCode = @"public class TestClass
{
    public delegate void Test();
    public event Test TestEvent1;
    public event Test TestEvent;
}";

            DiagnosticResult expected = Diagnostic().WithArguments("testEvent").WithLocation(4, 23);
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
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

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            DiagnosticResult expected = Diagnostic().WithArguments("test").WithLocation(3, 13);
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestLowerCaseMethodWithConflictAsync()
        {
            // Conflict resolution does not attempt to examine overloaded methods.
            var testCode = @"public class TestClass
{
public void test()
{
}

public int Test(int value) => value;
}";
            var fixedCode = @"public class TestClass
{
public void Test1()
{
}

public int Test(int value) => value;
}";

            DiagnosticResult expected = Diagnostic().WithArguments("test").WithLocation(3, 13);
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestUpperCasePropertyAsync()
        {
            var testCode = @"public class TestClass
{
public string Test { get; set; }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            DiagnosticResult expected = Diagnostic().WithArguments("test").WithLocation(3, 15);
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestLowerCasePropertyWithConflictAsync()
        {
            var testCode = @"public class TestClass
{
public string test { get; set; }
public string Test => string.Empty;
}";
            var fixedCode = @"public class TestClass
{
public string Test1 { get; set; }
public string Test => string.Empty;
}";

            DiagnosticResult expected = Diagnostic().WithArguments("test").WithLocation(3, 15);
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestUpperCasePublicFieldAsync()
        {
            var testCode = @"public class TestClass
{
public string Test;
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestUpperCaseInternalFieldAsync()
        {
            var testCode = @"public class TestClass
{
internal string Test;
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestUpperCaseConstFieldAsync()
        {
            var testCode = @"public class TestClass
{
const string Test = ""value"";
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestUpperCaseProtectedReadOnlyFieldAsync()
        {
            var testCode = @"public class TestClass
{
protected readonly string Test;
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestLowerCaseProtectedFieldAsync()
        {
            var testCode = @"public class TestClass
{
protected string Test;
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestLowerCaseReadOnlyFieldAsync()
        {
            var testCode = @"public class TestClass
{
readonly string test;
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestLowerCasePublicFieldAsync()
        {
            var testCode = @"public class TestClass
{
public string test;
}";

            // Handled by SA1307
            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestLowerCaseInternalFieldAsync()
        {
            var testCode = @"public class TestClass
{
internal string test;
}";

            // Handled by SA1307
            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestLowerCaseConstFieldAsync()
        {
            var testCode = @"public class TestClass
{
const string test = ""value"";
}";

            // Reported as SA1303
            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestNativeMethodsExceptionAsync()
        {
            var testCode = @"public class TestNativeMethods
{
public string test;
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestLowerCaseProtectedReadOnlyFieldAsync()
        {
            var testCode = @"public class TestClass
{
protected readonly string test;
}";

            // Handled by SA1304
            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
                Diagnostic().WithLocation(25, 26).WithArguments("foo"),
                Diagnostic().WithLocation(26, 25).WithArguments("bar"),
                Diagnostic().WithLocation(27, 47).WithArguments("fooBar"),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
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

    public string iInterface { get; }
}

public interface IInterface
{
    void foo();
    int bar { get; }
    event System.EventHandler fooBar;
    string iInterface { get; }
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

    public string IInterface { get; }
}

public interface IInterface
{
    void Foo();
    int Bar { get; }
    event System.EventHandler FooBar;
    string IInterface { get; }
}";

            var expected = new[]
            {
                Diagnostic().WithLocation(26, 10).WithArguments("foo"),
                Diagnostic().WithLocation(27, 9).WithArguments("bar"),
                Diagnostic().WithLocation(28, 31).WithArguments("fooBar"),
                Diagnostic().WithLocation(29, 12).WithArguments("iInterface"),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(1935, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/1935")]
        public async Task TestUnderscoreExclusionAsync()
        {
            var testCode = @"public enum TestEnum
{
    _12clock,
    _12Clock,
    _tick,
    _Tock,
}
";

            var fixedCode = @"public enum TestEnum
{
    _12clock,
    _12Clock,
    Tick,
    Tock,
}
";

            DiagnosticResult[] expected =
            {
                Diagnostic().WithLocation(5, 5).WithArguments("_tick"),
                Diagnostic().WithLocation(6, 5).WithArguments("_Tock"),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
