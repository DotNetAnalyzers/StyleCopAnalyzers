// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp10.ReadabilityRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp9.ReadabilityRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.ReadabilityRules.SA1130UseLambdaSyntax,
        StyleCop.Analyzers.ReadabilityRules.SA1130CodeFixProvider>;

    public partial class SA1130CSharp10UnitTests : SA1130CSharp9UnitTests
    {
        /// <summary>
        /// Verifies that the parameter types are kept when there is no target delegate type to infer them from, since
        /// the lambda would otherwise have no natural type.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [WorkItem(3985, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3985")]
        public async Task TestAnonymousMethodWithNaturalTypeAsync()
        {
            var testCode = @"using System;
public class TestClass
{
    public void TestMethod()
    {
        var a = [|delegate|](int x) { return x; };
        var b = [|delegate|](int x, int y) { return x + y; };
        var c = [|delegate|]() { };
        object d = [|delegate|](int x) { return x; };
        Delegate e = [|delegate|](int x) { return x; };
    }
}";

            var fixedCode = @"using System;
public class TestClass
{
    public void TestMethod()
    {
        var a = (int x) => { return x; };
        var b = (int x, int y) => { return x + y; };
        var c = () => { };
        object d = (int x) => { return x; };
        Delegate e = (int x) => { return x; };
    }
}";

            await VerifyCSharpFixAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the parameter types are kept when an anonymous method is passed as an argument for a
        /// <see cref="System.Delegate"/> parameter, and removed when it is passed for a specific delegate type.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [WorkItem(3985, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3985")]
        public async Task TestAnonymousMethodAsArgumentAsync()
        {
            var testCode = @"using System;
public class TestClass
{
    public void TestMethod()
    {
        Untyped([|delegate|](int x) { return x; });
        Typed([|delegate|](int arg) { return arg; });
    }

    private static void Untyped(Delegate d)
    {
    }

    private static void Typed(Func<int, int> f)
    {
    }
}";

            var fixedCode = @"using System;
public class TestClass
{
    public void TestMethod()
    {
        Untyped((int x) => { return x; });
        Typed(arg => { return arg; });
    }

    private static void Untyped(Delegate d)
    {
    }

    private static void Typed(Func<int, int> f)
    {
    }
}";

            await VerifyCSharpFixAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the parameter types are kept when an anonymous method is passed as an argument for a
        /// parameter whose type is a type parameter, because the type argument is inferred from the natural type.
        /// The parameter is named <c>arg</c>, like the parameter of <see cref="System.Func{T, TResult}"/>, because the
        /// analyzer only reports an argument if a lambda with the delegate's parameter names binds to the same method.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [WorkItem(3985, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3985")]
        public async Task TestAnonymousMethodAsGenericArgumentAsync()
        {
            var testCode = @"using System;
public class TestClass
{
    public void TestMethod()
    {
        Generic([|delegate|](int arg) { return arg; });
    }

    private static void Generic<T>(T value)
    {
    }
}";

            var fixedCode = @"using System;
public class TestClass
{
    public void TestMethod()
    {
        Generic((int arg) => { return arg; });
    }

    private static void Generic<T>(T value)
    {
    }
}";

            await VerifyCSharpFixAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
