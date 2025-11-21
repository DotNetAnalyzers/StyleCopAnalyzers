// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.Test.SpacingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.SpacingRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.SpacingRules.SA1017ClosingAttributeBracketsMustBeSpacedCorrectly,
        StyleCop.Analyzers.SpacingRules.TokenSpacingCodeFixProvider>;

    /// <summary>
    /// Unit tests for <see cref="SA1017ClosingAttributeBracketsMustBeSpacedCorrectly"/>.
    /// </summary>
    public class SA1017UnitTests
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

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
                Diagnostic().WithLocation(2, 18),
                Diagnostic().WithLocation(7, 19),
                Diagnostic().WithLocation(12, 30),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
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
                DiagnosticResult.CompilerError("CS0443").WithLocation(6, 28).WithMessage("Syntax error; value expected"),
                DiagnosticResult.CompilerError("CS1003").WithLocation(6, 28).WithMessage("Syntax error, ',' expected"),
                DiagnosticResult.CompilerError("CS1003").WithLocation(6, 28).WithMessage("Syntax error, ']' expected"),
            };

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
