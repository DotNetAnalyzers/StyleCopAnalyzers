// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.SpacingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.SpacingRules;
    using TestHelper;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.SpacingRules.SA1015ClosingGenericBracketsMustBeSpacedCorrectly,
        StyleCop.Analyzers.SpacingRules.TokenSpacingCodeFixProvider>;

    /// <summary>
    /// Unit tests for <see cref="SA1015ClosingGenericBracketsMustBeSpacedCorrectly"/>.
    /// </summary>
    public class SA1015UnitTests
    {
        /// <summary>
        /// Verifies that the analyzer will properly handle valid closing generic brackets.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestValidSpacingOfOpenGenericBracketAsync()
        {
            var testCode = @"using System;
using System.Collections.Generic;

public class TestClass<T> where T : IEnumerable<object>
{
    public Action<IEnumerable<object>> TestProperty { get; set; }

    public void TestMethod<TModel>() {
        var x = typeof(Action<,,>);
        TestMethod<TModel>();
        TestMethod<TModel
            >();
    }
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the analyzer will properly handle invalid closing generic brackets in a class declaration.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestInvalidSpacingOfClassDeclarationAsync()
        {
            var testCode = @"using System.Collections.Generic;

public class TestClass1<T> where T : IEnumerable<object>
{
}

public class TestClass2<T > where T : IEnumerable<object >
{
}

public class TestClass3<T>where T : IEnumerable<object>
{
}
";

            var fixedCode = @"using System.Collections.Generic;

public class TestClass1<T> where T : IEnumerable<object>
{
}

public class TestClass2<T> where T : IEnumerable<object>
{
}

public class TestClass3<T> where T : IEnumerable<object>
{
}
";

            DiagnosticResult[] expected =
            {
                Diagnostic().WithLocation(7, 27).WithArguments(" not", "preceded"),
                Diagnostic().WithLocation(7, 58).WithArguments(" not", "preceded"),
                Diagnostic().WithLocation(11, 26).WithArguments(string.Empty, "followed"),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the analyzer will properly handle valid closing generic brackets in a property declaration.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestInvalidSpacingOfPropertyDeclarationAsync()
        {
            var testCode = @"using System;

public class TestClass
{
    public Action<Action<object>> TestProperty1 { get; set; }
    public Action<Action<object >> TestProperty2 { get; set; }
    public Action<Action<object> > TestProperty3 { get; set; }
    public Action<Action<object>>TestProperty4 { get; set; }
    public Action<Action<object > >TestProperty5 { get; set; }
}
";

            var fixedCode = @"using System;

public class TestClass
{
    public Action<Action<object>> TestProperty1 { get; set; }
    public Action<Action<object>> TestProperty2 { get; set; }
    public Action<Action<object>> TestProperty3 { get; set; }
    public Action<Action<object>> TestProperty4 { get; set; }
    public Action<Action<object>> TestProperty5 { get; set; }
}
";

            DiagnosticResult[] expected =
            {
                Diagnostic().WithLocation(6, 33).WithArguments(" not", "preceded"),
                Diagnostic().WithLocation(7, 34).WithArguments(" not", "preceded"),
                Diagnostic().WithLocation(8, 33).WithArguments(string.Empty, "followed"),
                Diagnostic().WithLocation(9, 33).WithArguments(" not", "preceded"),
                Diagnostic().WithLocation(9, 35).WithArguments(" not", "preceded"),
                Diagnostic().WithLocation(9, 35).WithArguments(string.Empty, "followed"),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the analyzer will properly handle valid closing generic brackets in a method declaration.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestInvalidSpacingInMethodDeclarationAsync()
        {
            var testCode = @"using System;

public class TestClass
{
    public Action<Action<object>> TestMethod1<T>()
    {
        throw new NotImplementedException();
    }

    public Action<Action<object >> TestMethod2 <T >()
    {
        throw new NotImplementedException();
    }

    public Action<Action<object> >TestMethod3<T> ()
    {
        throw new NotImplementedException();
    }

    public Action<Action<object>>TestMethod4<T>()
    {
        throw new NotImplementedException();
    }

    public Action<Action<object > >TestMethod5<T > ()
    {
        throw new NotImplementedException();
    }
}
";

            var fixedCode = @"using System;

public class TestClass
{
    public Action<Action<object>> TestMethod1<T>()
    {
        throw new NotImplementedException();
    }

    public Action<Action<object>> TestMethod2 <T>()
    {
        throw new NotImplementedException();
    }

    public Action<Action<object>> TestMethod3<T>()
    {
        throw new NotImplementedException();
    }

    public Action<Action<object>> TestMethod4<T>()
    {
        throw new NotImplementedException();
    }

    public Action<Action<object>> TestMethod5<T>()
    {
        throw new NotImplementedException();
    }
}
";

            DiagnosticResult[] expected =
            {
                Diagnostic().WithLocation(10, 33).WithArguments(" not", "preceded"),
                Diagnostic().WithLocation(10, 51).WithArguments(" not", "preceded"),
                Diagnostic().WithLocation(15, 34).WithArguments(" not", "preceded"),
                Diagnostic().WithLocation(15, 34).WithArguments(string.Empty, "followed"),
                Diagnostic().WithLocation(15, 48).WithArguments(" not", "followed"),
                Diagnostic().WithLocation(20, 33).WithArguments(string.Empty, "followed"),
                Diagnostic().WithLocation(25, 33).WithArguments(" not", "preceded"),
                Diagnostic().WithLocation(25, 35).WithArguments(" not", "preceded"),
                Diagnostic().WithLocation(25, 35).WithArguments(string.Empty, "followed"),
                Diagnostic().WithLocation(25, 50).WithArguments(" not", "preceded"),
                Diagnostic().WithLocation(25, 50).WithArguments(" not", "followed"),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the analyzer will properly handle valid closing generic brackets in a method body.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestInvalidSpacingInMethodBodyAsync()
        {
            var testCode = @"using System;

public class TestClass
{
    public void TestMethod1<T>()
    {
    }

    public void TestMethod2()
    {
        var x = typeof(Action<,>);
        x = typeof(Action<, >);
        x = typeof(Action<,> );
        x = typeof(Action<, > ); 
        x = typeof(Action<Action<object>, object>);

        TestMethod1<object>();
        TestMethod1<object >();
        TestMethod1<object> ();
        TestMethod1<object > ();

        var y = new Action<object>[] { };
        System.Action<object>.Equals(new object(), new object());
    }
}
";

            var fixedCode = @"using System;

public class TestClass
{
    public void TestMethod1<T>()
    {
    }

    public void TestMethod2()
    {
        var x = typeof(Action<,>);
        x = typeof(Action<,>);
        x = typeof(Action<,> );
        x = typeof(Action<,> ); 
        x = typeof(Action<Action<object>, object>);

        TestMethod1<object>();
        TestMethod1<object>();
        TestMethod1<object>();
        TestMethod1<object>();

        var y = new Action<object>[] { };
        System.Action<object>.Equals(new object(), new object());
    }
}
";

            DiagnosticResult[] expected =
            {
                Diagnostic().WithLocation(12, 29).WithArguments(" not", "preceded"),

                // 13, 29 should be reported by SA1009
                Diagnostic().WithLocation(14, 29).WithArguments(" not", "preceded"),

                // 14, 30 should be reported by SA1009
                Diagnostic().WithLocation(18, 28).WithArguments(" not", "preceded"),
                Diagnostic().WithLocation(19, 27).WithArguments(" not", "followed"),
                Diagnostic().WithLocation(20, 28).WithArguments(" not", "preceded"),
                Diagnostic().WithLocation(20, 28).WithArguments(" not", "followed"),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the analyzer will properly handle greater than operators.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestGreaterThanOperatorNotReportedAsync()
        {
            var testCode = @"using System;

public class TestClass
{
    public void TestMethod()
    {
        var x = 1 > 3;
        x = 1 >= 3;
        if (0 > 1)
        {
        }
        if (0 >= 1)
        {
        }
    }
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the analyzer will properly handle using aliases.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestUsingAliasAsync()
        {
            var testCode = @"using TestAction = System.Action<object>;";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the analyzer will properly handle nullables.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestNullableAsync()
        {
            var testCode = @"public struct TestStruct<T>
{
    public void TestMethod()
    {
        var x = typeof(TestStruct<int>?);
        TestStruct<int>? y;
    }
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestMissingTokenAsync()
        {
            string testCode = @"
using System;
class ClassName
{
    void Method()
    {
        Type x = typeof(Action<int);
    }
}
";

            DiagnosticResult[] expected =
            {
                DiagnosticResult.CompilerError("CS1003").WithMessage("Syntax error, '>' expected").WithLocation(7, 35),
            };

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
