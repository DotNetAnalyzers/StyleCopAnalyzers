// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp7.ReadabilityRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.ReadabilityRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.ReadabilityRules.SA1101PrefixLocalCallsWithThis,
        StyleCop.Analyzers.ReadabilityRules.SA1101CodeFixProvider>;

    public class SA1101CSharp7UnitTests : SA1101UnitTests
    {
        /// <summary>
        /// Verifies that a value tuple is handled properly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [WorkItem(2534, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/2534")]
        public async Task TestValueTupleAsync()
        {
            var testCode = @"public class Foo
{
    protected (bool a, bool b) Bar()
    {
        return (a: true, b: false);
    }
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(2845, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/2845")]
        public async Task TestPropertyWithExpressionBodiedAccessorAsync()
        {
            var testCode = @"
public class Foo
{
    private int bar;

    public int Bar
    {
        get => bar;
        set => bar = value;
    }
}
";

            var fixedCode = @"
public class Foo
{
    private int bar;

    public int Bar
    {
        get => this.bar;
        set => this.bar = value;
    }
}
";

            var expected = new[]
            {
                Diagnostic().WithLocation(8, 16),
                Diagnostic().WithLocation(9, 16),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(2845, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/2845")]
        public async Task TestIndexerWithExpressionBodiedAccessorAsync()
        {
            var testCode = @"
public class Foo<T>
{
   private T[] arr = new T[100];

   public T this[int i]
   {
      get => arr[i];
      set => arr[i] = value;
   }
}
";

            var fixedCode = @"
public class Foo<T>
{
   private T[] arr = new T[100];

   public T this[int i]
   {
      get => this.arr[i];
      set => this.arr[i] = value;
   }
}
";

            var expected = new[]
            {
                Diagnostic().WithLocation(8, 14),
                Diagnostic().WithLocation(9, 14),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(3018, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3018")]
        public async Task TestGenericLocalFunctionAsync()
        {
            var testCode = @"
public class TestClass
{
    private int foobar = 1;

    public int Foo()
    {
        int Quux<T>() => this.foobar;
        return Quux<int>();
    }
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
