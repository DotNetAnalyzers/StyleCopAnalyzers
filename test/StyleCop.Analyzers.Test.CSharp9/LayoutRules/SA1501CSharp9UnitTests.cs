// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp9.LayoutRules
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp8.LayoutRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.LayoutRules.SA1501StatementMustNotBeOnASingleLine,
        StyleCop.Analyzers.LayoutRules.SA1501CodeFixProvider>;

    public partial class SA1501CSharp9UnitTests : SA1501CSharp8UnitTests
    {
        [Fact]
        [WorkItem(987, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/987")]
        public async Task TestTopLevelStatementAtEndOfFileAsync()
        {
            var testCode = @"if (true) {|#0:{|} System.Console.WriteLine(); }";
            var fixedCode = @"if (true)
{
    System.Console.WriteLine();
}" + "\r\n";

            var test = new CSharpTest
            {
                TestCode = testCode,
                FixedCode = fixedCode,
            };

            test.SolutionTransforms.Add((solution, projectId) =>
            {
                var project = solution.GetProject(projectId) ?? throw new InvalidOperationException("Test project was not found.");
                var compilationOptions = project.CompilationOptions as CSharpCompilationOptions
                    ?? throw new InvalidOperationException("Expected C# compilation options.");
                return solution.WithProjectCompilationOptions(projectId, compilationOptions.WithOutputKind(OutputKind.ConsoleApplication));
            });

            test.ExpectedDiagnostics.Add(Diagnostic().WithLocation(0));
            await test.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(3978, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3978")]
        public async Task TestLocalFunctionWithAttributeOnSingleLineAsync()
        {
            var testCode = @"using System;

class TestClass
{
    void Outer()
    {
        [Obsolete]
        void Local(){|#0:{|} int value = 0; }
    }
}
";

            var fixedCode = @"using System;

class TestClass
{
    void Outer()
    {
        [Obsolete]
        void Local()
        {
            int value = 0;
        }
    }
}
";

            await VerifyCSharpFixAsync(testCode, Diagnostic().WithLocation(0), fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

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
