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
        StyleCop.Analyzers.OrderingRules.SA1202ElementsMustBeOrderedByAccess,
        StyleCop.Analyzers.OrderingRules.ElementOrderCodeFixProvider>;

    public partial class SA1202CSharp14UnitTests : SA1202CSharp13UnitTests
    {
        [Fact]
        [WorkItem(4023, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4023")]
        public async Task TestPublicMethodAfterPrivateMethodInExtensionBlockAsync()
        {
            var testCode = @"
public static class TestClass
{
    extension(string source)
    {
        private int GetLength1() => source.Length;

        public int {|#0:GetLength2|}() => source.Length;
    }
}
";

            var fixedCode = @"
public static class TestClass
{
    extension(string source)
    {
        public int GetLength2() => source.Length;

        private int GetLength1() => source.Length;
    }
}
";

            var expected = Diagnostic().WithLocation(0).WithArguments("public", "private");
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(4023, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4023")]
        public async Task TestPublicPropertyAfterInternalPropertyInExtensionBlockAsync()
        {
            var testCode = @"
public static class TestClass
{
    extension(string source)
    {
        internal int Length1 => source.Length;

        public int {|#0:Length2|} => source.Length;
    }
}
";

            var fixedCode = @"
public static class TestClass
{
    extension(string source)
    {
        public int Length2 => source.Length;

        internal int Length1 => source.Length;
    }
}
";

            var expected = Diagnostic().WithLocation(0).WithArguments("public", "internal");
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(4023, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4023")]
        public async Task TestImplicitlyPrivateMemberBeforePublicMemberInExtensionBlockAsync()
        {
            var testCode = @"
public static class TestClass
{
    extension(string source)
    {
        int GetLength1() => source.Length;

        public int {|#0:GetLength2|}() => source.Length;
    }
}
";

            var fixedCode = @"
public static class TestClass
{
    extension(string source)
    {
        public int GetLength2() => source.Length;

        int GetLength1() => source.Length;
    }
}
";

            var expected = Diagnostic().WithLocation(0).WithArguments("public", "private");
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(4023, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4023")]
        public async Task TestExtensionBlockMembersOrderedByAccessAsync()
        {
            var testCode = @"
public static class TestClass
{
    extension(string source)
    {
        public int GetLength1() => source.Length;

        internal int GetLength2() => source.Length;

        private int GetLength3() => source.Length;
    }
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(4023, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4023")]
        public async Task TestPropertyAfterExtensionBlockDeclarationAsync()
        {
            string testCode = @"
public static class TestClass
{
    extension(string source)
    {
    }

    public static int TestProperty => 0;
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(4023, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4023")]
        public async Task TestPropertyBeforeExtensionBlockDeclarationAsync()
        {
            string testCode = @"
public static class TestClass
{
    public static int TestProperty => 0;

    extension(string source)
    {
    }
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(4023, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4023")]
        public async Task TestFieldBeforeExtensionBlockDeclarationAsync()
        {
            string testCode = @"
public static class TestClass
{
    private static int TestField;

    extension(string source)
    {
    }
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(4023, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4023")]
        public async Task TestFieldAfterExtensionBlockDeclarationAsync()
        {
            string testCode = @"
public static class TestClass
{
    extension(string source)
    {
    }

    private static int TestField;
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(4023, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4023")]
        public async Task TestTwoExtensionBlockDeclarationsAsync()
        {
            string testCode = @"
public static class TestClass
{
    extension(string source)
    {
    }

    extension(string source)
    {
    }
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
