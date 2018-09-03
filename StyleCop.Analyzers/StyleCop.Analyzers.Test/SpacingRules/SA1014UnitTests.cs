// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.SpacingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.SpacingRules;
    using TestHelper;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.SpacingRules.SA1014OpeningGenericBracketsMustBeSpacedCorrectly,
        StyleCop.Analyzers.SpacingRules.TokenSpacingCodeFixProvider>;

    /// <summary>
    /// Unit tests for <see cref="SA1014OpeningGenericBracketsMustBeSpacedCorrectly"/>.
    /// </summary>
    public class SA1014UnitTests
    {
        /// <summary>
        /// Verifies that the analyzer will properly handle valid opening generic brackets.
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
    }
}
";

            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the analyzer will properly handle invalid opening generic brackets in a class declaration.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestInvalidSpacingOfClassDeclarationAsync()
        {
            var testCode = @"using System.Collections.Generic;

public class TestClass1 <T> where T : IEnumerable< object>
{
}

public class TestClass2< T> where T : IEnumerable <object>
{
}

public class TestClass3 < T> where T : IEnumerable < object>
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
                Diagnostic().WithLocation(3, 25).WithArguments("preceded"),
                Diagnostic().WithLocation(3, 50).WithArguments("followed"),
                Diagnostic().WithLocation(7, 24).WithArguments("followed"),
                Diagnostic().WithLocation(7, 51).WithArguments("preceded"),
                Diagnostic().WithLocation(11, 25).WithArguments("preceded"),
                Diagnostic().WithLocation(11, 25).WithArguments("followed"),
                Diagnostic().WithLocation(11, 52).WithArguments("preceded"),
                Diagnostic().WithLocation(11, 52).WithArguments("followed"),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the analyzer will properly handle valid opening generic brackets in a property declaration.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestInvalidSpacingOfPropertyDeclarationAsync()
        {
            var testCode = @"using System;

public class TestClass
{
    public Action<Action<object>> TestProperty1 { get; set; }
    public Action <Action <object>> TestProperty2 { get; set; }
    public Action< Action< object>> TestProperty3 { get; set; }
    public Action < Action < object>> TestProperty4 { get; set; }
}
";

            var fixedCode = @"using System;

public class TestClass
{
    public Action<Action<object>> TestProperty1 { get; set; }
    public Action<Action<object>> TestProperty2 { get; set; }
    public Action<Action<object>> TestProperty3 { get; set; }
    public Action<Action<object>> TestProperty4 { get; set; }
}
";

            DiagnosticResult[] expected =
            {
                Diagnostic().WithLocation(6, 19).WithArguments("preceded"),
                Diagnostic().WithLocation(6, 27).WithArguments("preceded"),
                Diagnostic().WithLocation(7, 18).WithArguments("followed"),
                Diagnostic().WithLocation(7, 26).WithArguments("followed"),
                Diagnostic().WithLocation(8, 19).WithArguments("preceded"),
                Diagnostic().WithLocation(8, 19).WithArguments("followed"),
                Diagnostic().WithLocation(8, 28).WithArguments("preceded"),
                Diagnostic().WithLocation(8, 28).WithArguments("followed"),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the analyzer will properly handle valid opening generic brackets in a method declaration.
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

    public Action <Action <object>> TestMethod2 <T>()
    {
        throw new NotImplementedException();
    }

    public Action< Action< object>> TestMethod3< T>()
    {
        throw new NotImplementedException();
    }

    public Action < Action < object>> TestMethod4 < T>()
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

    public Action<Action<object>> TestMethod2<T>()
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
}
";

            DiagnosticResult[] expected =
            {
                Diagnostic().WithLocation(10, 19).WithArguments("preceded"),
                Diagnostic().WithLocation(10, 27).WithArguments("preceded"),
                Diagnostic().WithLocation(10, 49).WithArguments("preceded"),
                Diagnostic().WithLocation(15, 18).WithArguments("followed"),
                Diagnostic().WithLocation(15, 26).WithArguments("followed"),
                Diagnostic().WithLocation(15, 48).WithArguments("followed"),
                Diagnostic().WithLocation(20, 19).WithArguments("preceded"),
                Diagnostic().WithLocation(20, 19).WithArguments("followed"),
                Diagnostic().WithLocation(20, 28).WithArguments("preceded"),
                Diagnostic().WithLocation(20, 28).WithArguments("followed"),
                Diagnostic().WithLocation(20, 51).WithArguments("preceded"),
                Diagnostic().WithLocation(20, 51).WithArguments("followed"),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the analyzer will properly handle valid opening generic brackets in a method body.
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
        x = typeof(Action <,>);
        x = typeof(Action< ,>);
        x = typeof(Action < ,>);

        TestMethod1<object>();
        TestMethod1 <object>();
        TestMethod1< object>();
        TestMethod1 < object>();
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
        x = typeof(Action<,>);
        x = typeof(Action<,>);

        TestMethod1<object>();
        TestMethod1<object>();
        TestMethod1<object>();
        TestMethod1<object>();
    }
}
";

            DiagnosticResult[] expected =
            {
                Diagnostic().WithLocation(12, 27).WithArguments("preceded"),
                Diagnostic().WithLocation(13, 26).WithArguments("followed"),
                Diagnostic().WithLocation(14, 27).WithArguments("preceded"),
                Diagnostic().WithLocation(14, 27).WithArguments("followed"),
                Diagnostic().WithLocation(17, 21).WithArguments("preceded"),
                Diagnostic().WithLocation(18, 20).WithArguments("followed"),
                Diagnostic().WithLocation(19, 21).WithArguments("preceded"),
                Diagnostic().WithLocation(19, 21).WithArguments("followed"),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the analyzer will properly handle less than operators.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestLessThanOperatorNotReportedAsync()
        {
            var testCode = @"using System;

public class TestClass
{
    public void TestMethod()
    {
        var x = 1 < 3;
        x = 1 <= 3;
        if (0 < 1)
        {
        }
        if (0 <= 1)
        {
        }
    }
}
";

            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
