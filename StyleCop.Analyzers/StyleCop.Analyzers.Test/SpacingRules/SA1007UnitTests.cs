// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.SpacingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.SpacingRules;
    using TestHelper;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.SpacingRules.SA1007OperatorKeywordMustBeFollowedBySpace,
        StyleCop.Analyzers.SpacingRules.TokenSpacingCodeFixProvider>;

    /// <summary>
    /// This class contains unit tests for <see cref="SA1007OperatorKeywordMustBeFollowedBySpace"/> and
    /// <see cref="TokenSpacingCodeFixProvider"/>.
    /// </summary>
    public class SA1007UnitTests
    {
        [Fact]
        public async Task TestOperatorKeywordCasesAsync()
        {
            string testCode = @"
using System;
class ClassName
{
    public static bool operator==(ClassName x, ClassName y) { return false; }
    public static bool operator!=(ClassName x, ClassName y) { return false; }
    public static explicit operator@Boolean(ClassName x) { return false; }
    public static explicit operator
        int(ClassName x) { return 0; }
    public static explicit operator/*comment*/long(ClassName x) { return 0; }
}
";

            string fixedCode = @"
using System;
class ClassName
{
    public static bool operator ==(ClassName x, ClassName y) { return false; }
    public static bool operator !=(ClassName x, ClassName y) { return false; }
    public static explicit operator @Boolean(ClassName x) { return false; }
    public static explicit operator
        int(ClassName x) { return 0; }
    public static explicit operator /*comment*/long(ClassName x) { return 0; }
}
";

            DiagnosticResult[] expected =
            {
                Diagnostic().WithLocation(5, 24),
                Diagnostic().WithLocation(6, 24),
                Diagnostic().WithLocation(7, 28),
                Diagnostic().WithLocation(10, 28),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestMissingOperatorKeywordAsync()
        {
            string testCode = @"
class ClassName
{
    public static explicit bool(ClassName x)
    {
        throw new System.Exception();
    }
}
";

            DiagnosticResult[] expected =
            {
                DiagnosticResult.CompilerError("CS1003").WithMessage("Syntax error, 'operator' expected").WithLocation(4, 28),
            };

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
