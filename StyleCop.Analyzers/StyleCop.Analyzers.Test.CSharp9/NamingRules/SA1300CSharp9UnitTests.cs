﻿// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp9.NamingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp8.NamingRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.NamingRules.SA1300ElementMustBeginWithUpperCaseLetter,
        StyleCop.Analyzers.NamingRules.RenameToUpperCaseCodeFixProvider>;

    public partial class SA1300CSharp9UnitTests : SA1300CSharp8UnitTests
    {
        [Fact]
        public async Task TestPositionalRecord1Async()
        {
            var testCode = @"
public record {|#0:r|}(int A)
{
    public r(int a, int b)
        : this(A: a)
    {
    }
}
";

            var fixedCode = @"
public record R(int A)
{
    public R(int a, int b)
        : this(A: a)
    {
    }
}
";

            await VerifyCSharpFixAsync(testCode, this.GetExpectedResultTestPositionalRecord1(), fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestPositionalRecord2Async()
        {
            var testCode = @"
public record R(int [|a|])
{
    public R(int a, int b)
        : this(a: a)
    {
    }
}
";

            var fixedCode = @"
public record R(int A)
{
    public R(int a, int b)
        : this(A: a)
    {
    }
}
";

            await VerifyCSharpFixAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        protected virtual DiagnosticResult[] GetExpectedResultTestPositionalRecord1()
        {
            // NOTE: Seems like a Roslyn bug made diagnostics be reported twice. Fixed in a later version.
            return new[]
            {
                // /0/Test0.cs(2,15): warning SA1300: Element 'r' should begin with an uppercase letter
                Diagnostic().WithLocation(0).WithArguments("r"),

                // /0/Test0.cs(2,15): warning SA1300: Element 'r' should begin with an uppercase letter
                Diagnostic().WithLocation(0).WithArguments("r"),
            };
        }
    }
}
