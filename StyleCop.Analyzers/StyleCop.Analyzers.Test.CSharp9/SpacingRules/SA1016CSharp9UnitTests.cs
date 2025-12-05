// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp9.SpacingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp8.SpacingRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.SpacingRules.SA1016OpeningAttributeBracketsMustBeSpacedCorrectly,
        StyleCop.Analyzers.SpacingRules.TokenSpacingCodeFixProvider>;

    public partial class SA1016CSharp9UnitTests : SA1016CSharp8UnitTests
    {
        [Fact]
        [WorkItem(3978, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3978")]
        public async Task TestAttributesOnLocalFunctionsAsync()
        {
            var testCode = @"using System;

class TestClass
{
    void Outer()
    {
        {|#0:[|} Obsolete]
        void Local1()
        {
        }

        void Local2<{|#1:[|} MyAttribute] T>()
        {
        }

        void Local3({|#2:[|} MyAttribute] int value)
        {
        }
    }
}

[AttributeUsage(AttributeTargets.All)]
class MyAttributeAttribute : Attribute
{
}
";

            var fixedCode = @"using System;

class TestClass
{
    void Outer()
    {
        [Obsolete]
        void Local1()
        {
        }

        void Local2<[MyAttribute] T>()
        {
        }

        void Local3([MyAttribute] int value)
        {
        }
    }
}

[AttributeUsage(AttributeTargets.All)]
class MyAttributeAttribute : Attribute
{
}
";

            DiagnosticResult[] expected =
            {
                Diagnostic().WithLocation(0),
                Diagnostic().WithLocation(1),
                Diagnostic().WithLocation(2),
            };
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
