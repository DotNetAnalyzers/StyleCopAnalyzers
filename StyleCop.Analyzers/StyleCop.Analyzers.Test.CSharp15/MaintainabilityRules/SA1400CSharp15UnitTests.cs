// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp15.MaintainabilityRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp14.MaintainabilityRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.MaintainabilityRules.SA1400AccessModifierMustBeDeclared,
        StyleCop.Analyzers.MaintainabilityRules.SA1400CodeFixProvider>;

    public partial class SA1400CSharp15UnitTests : SA1400CSharp14UnitTests
    {
        /// <summary>
        /// Verifies that union declarations without an access modifier are reported once, and that the code fix adds
        /// the default access modifier (internal for top-level unions, private for nested unions).
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [WorkItem(4182, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4182")]
        public async Task TestUnionDeclarationAsync()
        {
            var testCode = @"
union {|#0:Pet|}(int, string);

readonly union {|#1:ReadOnlyPet|}(int, string);

public class Outer
{
    union {|#2:NestedPet|}(int, string);
}

public union Animal(int, string)
{
    union {|#3:NestedUnion|}(int, long);
}
";

            var fixedCode = @"
internal union Pet(int, string);

internal readonly union ReadOnlyPet(int, string);

public class Outer
{
    private union NestedPet(int, string);
}

public union Animal(int, string)
{
    private union NestedUnion(int, long);
}
";

            DiagnosticResult[] expected =
            {
                Diagnostic().WithArguments("Pet").WithLocation(0),
                Diagnostic().WithArguments("ReadOnlyPet").WithLocation(1),
                Diagnostic().WithArguments("NestedPet").WithLocation(2),
                Diagnostic().WithArguments("NestedUnion").WithLocation(3),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that members of a union without an access modifier are reported exactly once.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [WorkItem(4182, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4182")]
        public async Task TestUnionMembersAsync()
        {
            var testCode = @"
public union Pet(int, string)
{
    static int {|#0:count|};

    int {|#1:Legs|} => 4;

    void {|#2:Feed|}()
    {
    }
}
";

            var fixedCode = @"
public union Pet(int, string)
{
    private static int count;

    private int Legs => 4;

    private void Feed()
    {
    }
}
";

            DiagnosticResult[] expected =
            {
                Diagnostic().WithArguments("count").WithLocation(0),
                Diagnostic().WithArguments("Legs").WithLocation(1),
                Diagnostic().WithArguments("Feed").WithLocation(2),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
