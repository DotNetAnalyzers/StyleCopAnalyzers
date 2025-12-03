// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp9.LayoutRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp8.LayoutRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.LayoutRules.SA1501StatementMustNotBeOnASingleLine,
        StyleCop.Analyzers.LayoutRules.SA1501CodeFixProvider>;

    public partial class SA1501CSharp9UnitTests : SA1501CSharp8UnitTests
    {
        [Fact]
        [WorkItem(3976, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3976")]
        public async Task TestSingleLineForeachWithExtensionEnumeratorAsync()
        {
            var testCode = @"
using System;

public class TestClass
{
    public void TestMethod()
    {
        foreach (var value in new ExtensionEnumerable()) [|{|] Console.WriteLine(value); }
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
using System;

public class TestClass
{
    public void TestMethod()
    {
        foreach (var value in new ExtensionEnumerable())
        {
            Console.WriteLine(value);
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
    }
}
