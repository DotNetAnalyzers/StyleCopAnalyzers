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
        Analyzers.ReadabilityRules.SA1122UseStringEmptyForEmptyStrings,
        Analyzers.ReadabilityRules.SA1122CodeFixProvider>;

    public class SA1122CSharp7UnitTests : SA1122UnitTests
    {
        /// <summary>
        /// Verifies the analyzer will properly handle an empty string in a case label.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [WorkItem(3028, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3028")]
        public async Task TestEmptyStringInCaseLabelWithConditionAsync()
        {
            string testCode = @"
public class TestClass
{
    public void TestMethod(string condition)
    {
        switch (""Test string"")
        {
        case """" when condition == [|""""|]:
            break;
        case ("""" + ""a"") when condition == [|""""|]:
            break;
        }
    }
}
";
            string fixedCode = @"
public class TestClass
{
    public void TestMethod(string condition)
    {
        switch (""Test string"")
        {
        case """" when condition == string.Empty:
            break;
        case ("""" + ""a"") when condition == string.Empty:
            break;
        }
    }
}
";

            await VerifyCSharpFixAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(3028, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3028")]
        public async Task TestEmptyStringInSimplePatternAsync()
        {
            string testCode = @"
public class TestClass
{
    public bool TestMethod(string condition)
    {
        return condition is """";
    }
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
