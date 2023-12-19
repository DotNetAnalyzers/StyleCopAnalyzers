// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.Test.SpacingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.SpacingRules;
    using Xunit;
    using static StyleCop.Analyzers.SpacingRules.SA1015ClosingGenericBracketsMustBeSpacedCorrectly;
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
                Diagnostic(DescriptorNotPreceded).WithLocation(7, 27),
                Diagnostic(DescriptorNotPreceded).WithLocation(7, 58),
                Diagnostic(DescriptorFollowed).WithLocation(11, 26),
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
                Diagnostic(DescriptorNotPreceded).WithLocation(6, 33),
                Diagnostic(DescriptorNotPreceded).WithLocation(7, 34),
                Diagnostic(DescriptorFollowed).WithLocation(8, 33),
                Diagnostic(DescriptorNotPreceded).WithLocation(9, 33),
                Diagnostic(DescriptorNotPreceded).WithLocation(9, 35),
                Diagnostic(DescriptorFollowed).WithLocation(9, 35),
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
                Diagnostic(DescriptorNotPreceded).WithLocation(10, 33),
                Diagnostic(DescriptorNotPreceded).WithLocation(10, 51),
                Diagnostic(DescriptorNotPreceded).WithLocation(15, 34),
                Diagnostic(DescriptorFollowed).WithLocation(15, 34),
                Diagnostic(DescriptorNotFollowed).WithLocation(15, 48),
                Diagnostic(DescriptorFollowed).WithLocation(20, 33),
                Diagnostic(DescriptorNotPreceded).WithLocation(25, 33),
                Diagnostic(DescriptorNotPreceded).WithLocation(25, 35),
                Diagnostic(DescriptorFollowed).WithLocation(25, 35),
                Diagnostic(DescriptorNotPreceded).WithLocation(25, 50),
                Diagnostic(DescriptorNotFollowed).WithLocation(25, 50),
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
                Diagnostic(DescriptorNotPreceded).WithLocation(12, 29),

                // 13, 29 should be reported by SA1009
                Diagnostic(DescriptorNotPreceded).WithLocation(14, 29),

                // 14, 30 should be reported by SA1009
                Diagnostic(DescriptorNotPreceded).WithLocation(18, 28),
                Diagnostic(DescriptorNotFollowed).WithLocation(19, 27),
                Diagnostic(DescriptorNotPreceded).WithLocation(20, 28),
                Diagnostic(DescriptorNotFollowed).WithLocation(20, 28),
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

            var expected = this.GetExpectedResultMissingToken();

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(3312, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3312")]
        public async Task TestSpacingInIndexAsync()
        {
            var testCode = @"using System;
using System.Collections.Generic;

public class TestClass
{
    private Dictionary<object, object> values;

    public void TestMethod2(object input)
    {
        var x = values[input as List<int>];
        x = values[input as List<int {|#0:>|}];
        x = values[input as List<int{|#1:>|} ];
        x = values[input as List<int {|#2:>|} ];
    }
}
";

            var fixedCode = @"using System;
using System.Collections.Generic;

public class TestClass
{
    private Dictionary<object, object> values;

    public void TestMethod2(object input)
    {
        var x = values[input as List<int>];
        x = values[input as List<int>];
        x = values[input as List<int>];
        x = values[input as List<int>];
    }
}
";

            DiagnosticResult[] expected =
            {
                Diagnostic(DescriptorNotPreceded).WithLocation(0),
                Diagnostic(DescriptorNotFollowed).WithLocation(1),
                Diagnostic(DescriptorNotPreceded).WithLocation(2),
                Diagnostic(DescriptorNotFollowed).WithLocation(2),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        protected virtual DiagnosticResult[] GetExpectedResultMissingToken()
        {
            return new[]
            {
                DiagnosticResult.CompilerError("CS1003").WithMessage("Syntax error, '>' expected").WithLocation(7, 35),
            };
        }
    }
}
