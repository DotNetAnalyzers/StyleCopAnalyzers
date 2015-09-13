// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.SpacingRules
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.SpacingRules;
    using TestHelper;
    using Xunit;

    /// <summary>
    /// Unit tests for <see cref="SA1014OpeningGenericBracketsMustBeSpacedCorrectly"/>
    /// </summary>
    public class SA1014UnitTests : CodeFixVerifier
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

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
                this.CSharpDiagnostic().WithLocation(3, 25).WithArguments("preceded"),
                this.CSharpDiagnostic().WithLocation(3, 50).WithArguments("followed"),
                this.CSharpDiagnostic().WithLocation(7, 24).WithArguments("followed"),
                this.CSharpDiagnostic().WithLocation(7, 51).WithArguments("preceded"),
                this.CSharpDiagnostic().WithLocation(11, 25).WithArguments("preceded"),
                this.CSharpDiagnostic().WithLocation(11, 25).WithArguments("followed"),
                this.CSharpDiagnostic().WithLocation(11, 52).WithArguments("preceded"),
                this.CSharpDiagnostic().WithLocation(11, 52).WithArguments("followed")
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
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
                this.CSharpDiagnostic().WithLocation(6, 19).WithArguments("preceded"),
                this.CSharpDiagnostic().WithLocation(6, 27).WithArguments("preceded"),
                this.CSharpDiagnostic().WithLocation(7, 18).WithArguments("followed"),
                this.CSharpDiagnostic().WithLocation(7, 26).WithArguments("followed"),
                this.CSharpDiagnostic().WithLocation(8, 19).WithArguments("preceded"),
                this.CSharpDiagnostic().WithLocation(8, 19).WithArguments("followed"),
                this.CSharpDiagnostic().WithLocation(8, 28).WithArguments("preceded"),
                this.CSharpDiagnostic().WithLocation(8, 28).WithArguments("followed"),
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
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
                this.CSharpDiagnostic().WithLocation(10, 19).WithArguments("preceded"),
                this.CSharpDiagnostic().WithLocation(10, 27).WithArguments("preceded"),
                this.CSharpDiagnostic().WithLocation(10, 49).WithArguments("preceded"),
                this.CSharpDiagnostic().WithLocation(15, 18).WithArguments("followed"),
                this.CSharpDiagnostic().WithLocation(15, 26).WithArguments("followed"),
                this.CSharpDiagnostic().WithLocation(15, 48).WithArguments("followed"),
                this.CSharpDiagnostic().WithLocation(20, 19).WithArguments("preceded"),
                this.CSharpDiagnostic().WithLocation(20, 19).WithArguments("followed"),
                this.CSharpDiagnostic().WithLocation(20, 28).WithArguments("preceded"),
                this.CSharpDiagnostic().WithLocation(20, 28).WithArguments("followed"),
                this.CSharpDiagnostic().WithLocation(20, 51).WithArguments("preceded"),
                this.CSharpDiagnostic().WithLocation(20, 51).WithArguments("followed")
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
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
                this.CSharpDiagnostic().WithLocation(12, 27).WithArguments("preceded"),
                this.CSharpDiagnostic().WithLocation(13, 26).WithArguments("followed"),
                this.CSharpDiagnostic().WithLocation(14, 27).WithArguments("preceded"),
                this.CSharpDiagnostic().WithLocation(14, 27).WithArguments("followed"),
                this.CSharpDiagnostic().WithLocation(17, 21).WithArguments("preceded"),
                this.CSharpDiagnostic().WithLocation(18, 20).WithArguments("followed"),
                this.CSharpDiagnostic().WithLocation(19, 21).WithArguments("preceded"),
                this.CSharpDiagnostic().WithLocation(19, 21).WithArguments("followed")
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
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

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        protected override IEnumerable<DiagnosticAnalyzer> GetCSharpDiagnosticAnalyzers()
        {
            yield return new SA1014OpeningGenericBracketsMustBeSpacedCorrectly();
        }

        /// <inheritdoc/>
        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new TokenSpacingCodeFixProvider();
        }
    }
}
