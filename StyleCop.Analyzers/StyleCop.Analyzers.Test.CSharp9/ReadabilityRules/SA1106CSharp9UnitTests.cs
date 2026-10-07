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
        StyleCop.Analyzers.ReadabilityRules.SA1106CodeMustNotContainEmptyStatements,
        StyleCop.Analyzers.ReadabilityRules.SA1106CodeFixProvider>;

    public partial class SA1106CSharp9UnitTests : SA1106CSharp8UnitTests
    {
        [Fact]
        [WorkItem(3267, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3267")]
        public async Task TestNoDiagnosticForEmptyRecordDeclarationAsync()
        {
            var testCode = @"public record Result(int Value);";

            await new CSharpTest()
            {
                ReferenceAssemblies = ReferenceAssemblies.Net.Net50,
                TestCode = testCode,
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(3976, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3976")]
        public async Task TestForeachWithExtensionEnumeratorAsync()
        {
            var testCode = @"
using System.Collections.Generic;

public class TestClass
{
    public void TestMethod()
    {
        foreach (var value in new ExtensionEnumerable())
            [|;|]
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
    public void TestMethod()
    {
        foreach (var value in new ExtensionEnumerable())
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
    }
}
