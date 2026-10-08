// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp11.SpacingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp10.SpacingRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.SpacingRules.SA1014OpeningGenericBracketsMustBeSpacedCorrectly,
        StyleCop.Analyzers.SpacingRules.TokenSpacingCodeFixProvider>;

    public partial class SA1014CSharp11UnitTests : SA1014CSharp10UnitTests
    {
        /// <summary>
        /// Verifies that the spacing of the opening bracket of a generic attribute is checked.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [WorkItem(3995, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3995")]
        public async Task TestGenericAttributeAsync()
        {
            var testCode = @"using System;

class TypeAttribute<T> : Attribute
{
}

[Type{|#0:<|} int>]
class TestClass
{
}

[Type {|#1:<|}int>]
class TestClass2
{
}
";

            var fixedCode = @"using System;

class TypeAttribute<T> : Attribute
{
}

[Type<int>]
class TestClass
{
}

[Type<int>]
class TestClass2
{
}
";

            DiagnosticResult[] expected =
            {
                Diagnostic().WithLocation(0).WithArguments("followed"),
                Diagnostic().WithLocation(1).WithArguments("preceded"),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
