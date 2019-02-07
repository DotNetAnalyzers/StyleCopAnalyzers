// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp7.SpacingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.SpacingRules;
    using TestHelper;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.SpacingRules.SA1024ColonsMustBeSpacedCorrectly,
        StyleCop.Analyzers.SpacingRules.TokenSpacingCodeFixProvider>;

    public class SA1024CSharp7UnitTests : SA1024UnitTests
    {
        /// <summary>
        /// Verifies spacing around a <c>:</c> character in tuple expressions.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestSpacingAroundColonInTupleExpressionsAsync()
        {
            const string testCode = @"using System;

public class Foo
{
    public void TestMethod()
    {
        var values = (x :3, y :4);
    }
}";
            const string fixedCode = @"using System;

public class Foo
{
    public void TestMethod()
    {
        var values = (x: 3, y: 4);
    }
}";

            DiagnosticResult[] expected =
            {
                Diagnostic().WithLocation(7, 25).WithArguments(" not", "preceded", string.Empty),
                Diagnostic().WithLocation(7, 25).WithArguments(string.Empty, "followed", string.Empty),
                Diagnostic().WithLocation(7, 31).WithArguments(" not", "preceded", string.Empty),
                Diagnostic().WithLocation(7, 31).WithArguments(string.Empty, "followed", string.Empty),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedCode).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestSpacingAroundColonInCasePatternSwitchLabelAsync()
        {
            const string testCode = @"
public class Foo
{
    public void TestMethod()
    {
        switch (new object())
        {
        case int a when (a > 0):
        case short b when (b > 0) :
        case int x:
        case short y :
        default:
            break;
        }
    }
}";
            const string fixedCode = @"
public class Foo
{
    public void TestMethod()
    {
        switch (new object())
        {
        case int a when (a > 0):
        case short b when (b > 0):
        case int x:
        case short y:
        default:
            break;
        }
    }
}";

            DiagnosticResult[] expected =
            {
                Diagnostic().WithLocation(9, 35).WithArguments(" not", "preceded", string.Empty),
                Diagnostic().WithLocation(11, 22).WithArguments(" not", "preceded", string.Empty),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedCode).ConfigureAwait(false);
        }
    }
}
