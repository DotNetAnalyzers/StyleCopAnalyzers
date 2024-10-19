﻿// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.Test.CSharp7.MaintainabilityRules
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Lightup;
    using StyleCop.Analyzers.Test.Helpers;
    using StyleCop.Analyzers.Test.MaintainabilityRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.MaintainabilityRules.SA1119StatementMustNotUseUnnecessaryParenthesis,
        StyleCop.Analyzers.MaintainabilityRules.SA1119CodeFixProvider>;

    public partial class SA1119CSharp7UnitTests : SA1119UnitTests
    {
        public static IEnumerable<object[]> Assignments
        {
            get
            {
                yield return new object[] { "= 1" };
                yield return new object[] { "+= 1" };
                yield return new object[] { "-= 1" };
                yield return new object[] { "*= 1" };
                yield return new object[] { "/= 1" };
                yield return new object[] { "%= 1" };
                yield return new object[] { "&= 1" };
                yield return new object[] { "|= 1" };
                yield return new object[] { "^= 1" };
                yield return new object[] { "<<= 1" };
                yield return new object[] { ">>= 1" };
                yield return new object[] { "++" };
                yield return new object[] { "--" };

                if (LightupHelpers.SupportsCSharp11)
                {
                    yield return new object[] { ">>>= 1" };
                }
            }
        }

        /// <summary>
        /// Verifies that extra parentheses in pattern matching is not reported.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        /// <seealso cref="SA1408CSharp7UnitTests.TestPatternMatchingAsync"/>
        [Fact]
        [WorkItem(2372, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/2398")]
        public async Task TestPatternMatchingAsync()
        {
            var testCode = @"public class Foo
{
    public void Bar()
    {
        if ((new object() is bool b) && b)
        {
            return;
        }
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(2372, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/2372")]
        public async Task TestNegatedPatternMatchingAsync()
        {
            var testCode = @"public class Foo
{
    public void Bar()
    {
        object obj = null;
        if (!(obj is string anythng))
        {
            // ...
        }
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestTupleDeconstructionAsync()
        {
            var testCode = @"public class Foo
{
    public void Bar()
    {
        var (a, (b, c), d) = (1, (2, (3)), 4);
    }
}";
            var fixedCode = @"public class Foo
{
    public void Bar()
    {
        var (a, (b, c), d) = (1, (2, 3), 4);
    }
}";

            DiagnosticResult[] expected =
            {
                Diagnostic(DiagnosticId).WithSpan(5, 38, 5, 41),
                Diagnostic(ParenthesesDiagnosticId).WithLocation(5, 38),
                Diagnostic(ParenthesesDiagnosticId).WithLocation(5, 40),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(Assignments))]
        [WorkItem(3712, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3712")]
        public async Task TestConditionalRefAssignmentAsync(string assignment)
        {
            var testCode = $@"public class Foo
{{
    public void Bar(bool b, ref int x, ref int y)
    {{
        (b ? ref x : ref y) {assignment};
    }}
}}";

            await VerifyCSharpDiagnosticAsync(LanguageVersionEx.CSharp7_2.OrLaterDefault(), testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
