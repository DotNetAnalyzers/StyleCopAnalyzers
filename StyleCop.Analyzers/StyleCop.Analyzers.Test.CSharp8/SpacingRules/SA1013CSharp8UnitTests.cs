// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp8.SpacingRules
{
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.Testing;

    using StyleCop.Analyzers.Test.CSharp7.SpacingRules;

    using Xunit;

    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.SpacingRules.SA1013ClosingBracesMustBeSpacedCorrectly,
        StyleCop.Analyzers.SpacingRules.TokenSpacingCodeFixProvider>;

    public class SA1013CSharp8UnitTests : SA1013CSharp7UnitTests
    {
        /// <summary>
        /// Verifies the behavior of closing braces in case patterns.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        /// <seealso cref="SA1024CSharp8UnitTests.TestColonAfterClosingBraceInPatternAsync"/>
        [Fact]
        [WorkItem(3053, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3053")]
        public async Task TestSpacingAroundClosingBraceInPatternAsync()
        {
            const string testCode = @"using System;

public class Foo
{
    public void TestMethod(object value)
    {
        switch (value)
        {
        // The space before ':' is not checked
        case ArgumentException { Message: { } message } :
            break;

        // The space before 'message' is checked
        case Exception { Message: { }message }:
            break;
        }
    }
}";
            const string fixedCode = @"using System;

public class Foo
{
    public void TestMethod(object value)
    {
        switch (value)
        {
        // The space before ':' is not checked
        case ArgumentException { Message: { } message } :
            break;

        // The space before 'message' is checked
        case Exception { Message: { } message }:
            break;
        }
    }
}";

            var expected = Diagnostic().WithSpan(14, 37, 14, 38).WithArguments(string.Empty, "followed");
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Validates that a closing brace followed by a null-forgiving operator does not require a space.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [WorkItem(3172, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3172")]
        public async Task TestCloseBraceWithNullForgivingOperatorAsync()
        {
            const string testCode = @"#nullable enable
public class Foo
{
    public void TestMethod()
    {
        var test = new[]
        {
            new { Value = ""a"" }!,
            new { Value = ""b"" } !,
        };
    }
}
";

            await VerifyCSharpDiagnosticAsync(LanguageVersion.CSharp8, testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
