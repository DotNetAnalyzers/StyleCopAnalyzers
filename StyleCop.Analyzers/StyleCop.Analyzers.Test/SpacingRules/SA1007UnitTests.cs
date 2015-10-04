// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.SpacingRules
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.SpacingRules;
    using TestHelper;
    using Xunit;

    /// <summary>
    /// This class contains unit tests for <see cref="SA1007OperatorKeywordMustBeFollowedBySpace"/> and
    /// <see cref="TokenSpacingCodeFixProvider"/>.
    /// </summary>
    public class SA1007UnitTests : CodeFixVerifier
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
                this.CSharpDiagnostic().WithLocation(5, 24),
                this.CSharpDiagnostic().WithLocation(6, 24),
                this.CSharpDiagnostic().WithLocation(7, 28),
                this.CSharpDiagnostic().WithLocation(10, 28),
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
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
                new DiagnosticResult
                {
                    Id = "CS1003",
                    Severity = DiagnosticSeverity.Error,
                    Message = "Syntax error, 'operator' expected",
                    Locations = new[] { new DiagnosticResultLocation("Test0.cs", 4, 28) }
                }
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        protected override IEnumerable<DiagnosticAnalyzer> GetCSharpDiagnosticAnalyzers()
        {
            yield return new SA1007OperatorKeywordMustBeFollowedBySpace();
        }

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new TokenSpacingCodeFixProvider();
        }
    }
}
