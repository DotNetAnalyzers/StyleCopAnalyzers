// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp14.ReadabilityRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp13.ReadabilityRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<StyleCop.Analyzers.ReadabilityRules.SA1137ElementsShouldHaveTheSameIndentation, StyleCop.Analyzers.ReadabilityRules.IndentationCodeFixProvider>;

    public partial class SA1137CSharp14UnitTests : SA1137CSharp13UnitTests
    {
        [Fact]
        [WorkItem(4023, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4023")]
        public async Task TestExtensionBlockMembersAsync()
        {
            var testCode = @"
public static class TestClass
{
    extension(string source)
    {
        public int Length1 => source.Length;
[|         |]public int Length2 => source.Length;
[|       |]public int Length3 => source.Length;
    }
}
";

            var fixedCode = @"
public static class TestClass
{
    extension(string source)
    {
        public int Length1 => source.Length;
        public int Length2 => source.Length;
        public int Length3 => source.Length;
    }
}
";

            await VerifyCSharpFixAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(4023, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4023")]
        public async Task TestExtensionBlockConstraintsAsync()
        {
            var testCode = @"
public static class TestClass
{
    extension<T1, T2, T3>((T1, T2, T3) source)
        where T1 : class
[|         |]where T2 : class
[|       |]where T3 : class
    {
    }
}
";

            var fixedCode = @"
public static class TestClass
{
    extension<T1, T2, T3>((T1, T2, T3) source)
        where T1 : class
        where T2 : class
        where T3 : class
    {
    }
}
";

            await VerifyCSharpFixAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
