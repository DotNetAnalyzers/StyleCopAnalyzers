// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp9.ReadabilityRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp8.ReadabilityRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.ReadabilityRules.SA1101PrefixLocalCallsWithThis,
        StyleCop.Analyzers.ReadabilityRules.SA1101CodeFixProvider>;

    public partial class SA1101CSharp9UnitTests : SA1101CSharp8UnitTests
    {
        [Fact]
        [WorkItem(3201, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3201")]
        public async Task TestRecordWithExpressionAsync()
        {
            var testCode = @"public class Test
{
    public record A
    {
        public string Prop { get; init; }
    }

    public A UpdateA(A value)
    {
        return value with { Prop = ""newValue"" };
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(3976, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3976")]
        public async Task TestForeachWithExtensionEnumeratorAsync()
        {
            var testCode = @"
using System.Collections.Generic;

public class TestClass
{
    private readonly ExtensionEnumerable values = new();

    public void Test()
    {
        foreach (var value in [|values|])
        {
        }
    }
}

public class ExtensionEnumerable
{
}

public struct ExtensionEnumerator
{
    public int Current => 0;

    public bool MoveNext() => false;
}

public static class ExtensionEnumerableExtensions
{
    public static ExtensionEnumerator GetEnumerator(this ExtensionEnumerable enumerable) => new();
}
";

            var fixedCode = @"
using System.Collections.Generic;

public class TestClass
{
    private readonly ExtensionEnumerable values = new();

    public void Test()
    {
        foreach (var value in this.values)
        {
        }
    }
}

public class ExtensionEnumerable
{
}

public struct ExtensionEnumerator
{
    public int Current => 0;

    public bool MoveNext() => false;
}

public static class ExtensionEnumerableExtensions
{
    public static ExtensionEnumerator GetEnumerator(this ExtensionEnumerable enumerable) => new();
}
";

            await VerifyCSharpFixAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(3973, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3973")]
        public async Task TestStaticLambdaAccessingStaticMemberAsync()
        {
            var testCode = @"public class TestClass
{
    private static int value;

    public void TestMethod()
    {
        System.Action action = static () =>
        {
            value++;
        };
    }
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
