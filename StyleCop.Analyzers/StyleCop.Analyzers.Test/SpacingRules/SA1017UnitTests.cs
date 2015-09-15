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
    /// Unit tests for <see cref="SA1017ClosingAttributeBracketsMustBeSpacedCorrectly"/>.
    /// </summary>
    public class SA1017UnitTests : CodeFixVerifier
    {
        /// <summary>
        /// Verifies that the analyzer will properly valid bracket placement.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestValidBracketsAsync()
        {
            var testCode = @"
[System.Obsolete]
class ClassName
{
}

[System.Obsolete
]
class ClassName2
{
}

[System.Obsolete
 ]
class ClassName3
{
}

[System.Obsolete /*comment*/]
class ClassNam4
{
}

class ClassName5<[MyAttribute] T>
{
}

class ClassName6<[MyAttribute
] T>
{
    [return: MyAttribute]
    int MethodName([MyAttribute] int x) { return 0; }
}

[System.AttributeUsage(System.AttributeTargets.All)]
sealed class MyAttribute : System.Attribute { }
";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the analyzer will properly report invalid bracket placements.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestInvalidBracketsAsync()
        {
            var testCode = @"
[System.Obsolete ]
class ClassName
{
}

[System.Obsolete  ]
class ClassName2
{
}

[System.Obsolete /*comment*/ ]
class ClassNam3
{
}
";
            var fixedCode = @"
[System.Obsolete]
class ClassName
{
}

[System.Obsolete]
class ClassName2
{
}

[System.Obsolete /*comment*/]
class ClassNam3
{
}
";

            DiagnosticResult[] expected =
            {
                this.CSharpDiagnostic().WithLocation(2, 18),
                this.CSharpDiagnostic().WithLocation(7, 19),
                this.CSharpDiagnostic().WithLocation(12, 30),
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestMissingBracketTokenAsync()
        {
            var testCode = @"
class ClassName
{
    void MethodName()
    {
        int[] x = new int[3;
    }
}
";

            DiagnosticResult[] expected =
            {
                new DiagnosticResult()
                {
                    Id = "CS1003",
                    Message = "Syntax error, ',' expected",
                    Severity = DiagnosticSeverity.Error,
                },
                new DiagnosticResult()
                {
                    Id = "CS0443",
                    Message = "Syntax error; value expected",
                    Severity = DiagnosticSeverity.Error,
                },
                new DiagnosticResult()
                {
                    Id = "CS1003",
                    Message = "Syntax error, ']' expected",
                    Severity = DiagnosticSeverity.Error,
                }
            };

            for (int i = 0; i < expected.Length; i++)
            {
                expected[i] = expected[i].WithLocation(6, 28);
            }

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        protected override IEnumerable<DiagnosticAnalyzer> GetCSharpDiagnosticAnalyzers()
        {
            yield return new SA1017ClosingAttributeBracketsMustBeSpacedCorrectly();
        }

        /// <inheritdoc/>
        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new TokenSpacingCodeFixProvider();
        }
    }
}
