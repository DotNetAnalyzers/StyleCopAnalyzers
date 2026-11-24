// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp8.OrderingRules;

using System.Threading.Tasks;
using StyleCop.Analyzers.Test.CSharp7.OrderingRules;
using Xunit;
using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
    StyleCop.Analyzers.OrderingRules.SA1201ElementsMustAppearInTheCorrectOrder,
    StyleCop.Analyzers.OrderingRules.ElementOrderCodeFixProvider>;

public partial class SA1201CSharp8UnitTests : SA1201CSharp7UnitTests
{
    [Fact]
    public async Task TestMethodNameOrderFromEditorConfigAsync()
    {
        var test = new CSharpTest
        {
            TestCode = """
                public class C
                {
                    public void Zulu() { }
                    public void {|#0:Alpha|}() { }
                }
                """,
            FixedCode = """
                public class C
                {
                    public void Alpha() { }
                    public void Zulu() { }
                }
                """,
        };
        test.TestState.AnalyzerConfigFiles.Add(("/.editorconfig", """
            root = true

            [*]
            stylecop.ordering.elementOrder = kind, accessibility, static, name
            """));
        test.ExpectedDiagnostics.Add(Diagnostic().WithLocation(0).WithMessage("Member 'Alpha' should appear before member 'Zulu'"));
        await test.RunAsync().ConfigureAwait(false);
    }
}
