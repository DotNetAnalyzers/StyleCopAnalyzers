// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp14.OrderingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp13.OrderingRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.OrderingRules.SA1204StaticElementsMustAppearBeforeInstanceElements,
        StyleCop.Analyzers.OrderingRules.ElementOrderCodeFixProvider>;

    public partial class SA1204CSharp14UnitTests : SA1204CSharp13UnitTests
    {
        [Fact]
        [WorkItem(4023, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4023")]
        public async Task TestStaticMethodAfterInstanceMethodInExtensionBlockAsync()
        {
            var testCode = @"
public static class TestClass
{
    extension(string source)
    {
        public int GetLength() => source.Length;

        public static int {|#0:GetZero|}() => 0;
    }
}
";

            var fixedCode = @"
public static class TestClass
{
    extension(string source)
    {
        public static int GetZero() => 0;

        public int GetLength() => source.Length;
    }
}
";

            var expected = Diagnostic().WithLocation(0);
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(4023, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4023")]
        public async Task TestStaticPropertyAfterInstancePropertyInExtensionBlockAsync()
        {
            var testCode = @"
public static class TestClass
{
    extension(string source)
    {
        public int Length2 => source.Length;

        public static int {|#0:Zero|} => 0;
    }
}
";

            var fixedCode = @"
public static class TestClass
{
    extension(string source)
    {
        public static int Zero => 0;

        public int Length2 => source.Length;
    }
}
";

            var expected = Diagnostic().WithLocation(0);
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(4023, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4023")]
        public async Task TestExtensionBlockStaticMembersFirstAsync()
        {
            var testCode = @"
public static class TestClass
{
    extension(string source)
    {
        public static int GetZero() => 0;

        public int GetLength() => source.Length;
    }
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
