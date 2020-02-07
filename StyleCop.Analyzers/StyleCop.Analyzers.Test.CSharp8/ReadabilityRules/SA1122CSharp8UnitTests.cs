// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp8.ReadabilityRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp7.ReadabilityRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.ReadabilityRules.SA1122UseStringEmptyForEmptyStrings,
        StyleCop.Analyzers.ReadabilityRules.SA1122CodeFixProvider>;

    public class SA1122CSharp8UnitTests : SA1122CSharp7UnitTests
    {
        /// <summary>
        /// Verifies the analyzer will properly handle an empty string in a switch expression.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [WorkItem(3028, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3028")]
        public async Task TestEmptyStringInSwitchExpressionAsync()
        {
            string testCode = @"
public class TestClass
{
    public void TestMethod(string condition)
    {
        _ = [|""""|] switch
        {
        """" when condition == [|""""|] =>
            0,
        ("""" + ""a"") when condition == [|""""|] =>
            0,
        };
    }
}
";
            string fixedCode = @"
public class TestClass
{
    public void TestMethod(string condition)
    {
        _ = string.Empty switch
        {
        """" when condition == string.Empty =>
            0,
        ("""" + ""a"") when condition == string.Empty =>
            0,
        };
    }
}
";

            await VerifyCSharpFixAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(3028, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3028")]
        public async Task TestEmptyStringInTuplePatternAsync()
        {
            string testCode = @"
public class TestClass
{
    public bool TestMethod((string, string) condition)
    {
        return condition is ("""", null);
    }
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(3028, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3028")]
        public async Task TestEmptyStringInRecursivePatternAsync()
        {
            string testCode = @"
using System.Collections.Generic;
public class TestClass
{
    public bool TestMethod(KeyValuePair<string, string> condition)
    {
        return condition is { Key: """" };
    }
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
