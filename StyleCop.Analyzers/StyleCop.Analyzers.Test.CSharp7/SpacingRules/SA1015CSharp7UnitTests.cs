// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.Test.CSharp7.SpacingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.SpacingRules;
    using Xunit;
    using static StyleCop.Analyzers.SpacingRules.SA1015ClosingGenericBracketsMustBeSpacedCorrectly;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.SpacingRules.SA1015ClosingGenericBracketsMustBeSpacedCorrectly,
        StyleCop.Analyzers.SpacingRules.TokenSpacingCodeFixProvider>;

    public class SA1015CSharp7UnitTests : SA1015UnitTests
    {
        /// <summary>
        /// Verifies spacing around a <c>&gt;</c> character in tuple types.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        /// <seealso cref="SA1001CSharp7UnitTests.TestClosingGenericBracketsInTupleTypesNotFollowedBySpaceAsync"/>
        /// <seealso cref="SA1009CSharp7UnitTests.TestClosingGenericBracketsInTupleTypesNotFollowedBySpaceAsync"/>
        [Fact]
        public async Task TestClosingGenericBracketsInTupleTypesNotPrecededBySpaceAsync()
        {
            const string testCode = @"using System;

public class Foo
{
    public void TestMethod()
    {
        (Func<int > , Func<int > ) value = (null, null);
    }
}";
            const string fixedCode = @"using System;

public class Foo
{
    public void TestMethod()
    {
        (Func<int>, Func<int> ) value = (null, null);
    }
}";

            DiagnosticResult[] expected =
            {
                Diagnostic(DescriptorNotPreceded).WithLocation(7, 19),
                Diagnostic(DescriptorNotFollowed).WithLocation(7, 19),
                Diagnostic(DescriptorNotPreceded).WithLocation(7, 32),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a pattern in a switch statement that ends with a generic type does not produce a diagnostic.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [WorkItem(2735, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/2735")]
        public async Task TestPatternMatchingInSwitchStatementWithGenericTypesAsync()
        {
            var testCode = @"using System.Collections.Generic;

public class TestClass
{
    public void TestMethod(int i, object value)
    {
        switch (i)
        {
            case 1 when value is List<TestClass>:
                break;
        }

        var x = i == 3 ? value as List<TestClass> : null;
    }
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
