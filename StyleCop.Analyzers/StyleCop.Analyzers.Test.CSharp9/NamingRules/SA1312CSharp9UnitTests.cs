// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp9.NamingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp8.NamingRules;
    using StyleCop.Analyzers.Test.Helpers;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.NamingRules.SA1312VariableNamesMustBeginWithLowerCaseLetter,
        StyleCop.Analyzers.NamingRules.RenameToLowerCaseCodeFixProvider>;

    public partial class SA1312CSharp9UnitTests : SA1312CSharp8UnitTests
    {
        [Fact]
        [WorkItem(3976, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3976")]
        public async Task TestForeachVariableWithExtensionEnumeratorAsync()
        {
            var testCode = @"
using System;

public class TestClass
{
    public void TestMethod()
    {
        foreach (var [|Item|] in new ExtensionEnumerable())
        {
            Console.WriteLine(Item);
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
using System;

public class TestClass
{
    public void TestMethod()
    {
        foreach (var item in new ExtensionEnumerable())
        {
            Console.WriteLine(item);
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
