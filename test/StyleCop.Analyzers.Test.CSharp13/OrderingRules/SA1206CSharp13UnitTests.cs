// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp13.OrderingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp12.OrderingRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopDiagnosticVerifier<
        StyleCop.Analyzers.OrderingRules.SA1206DeclarationKeywordsMustFollowOrder>;

    public partial class SA1206CSharp13UnitTests : SA1206CSharp12UnitTests
    {
        [Fact]
        [WorkItem(4021, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4021")]
        public async Task TestPartialPropertyCorrectKeywordOrderAsync()
        {
            var testCode = @"
public partial class TypeName
{
    public partial int Test { get; set; }
}

public partial class TypeName
{
    public partial int Test
    {
        get => 0;
        set { }
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(4021, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4021")]
        public async Task TestPartialIndexerCorrectKeywordOrderAsync()
        {
            var testCode = @"
public partial class TypeName
{
    public partial int this[int index] { get; set; }
}

public partial class TypeName
{
    public partial int this[int index]
    {
        get => 0;
        set { }
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(4021, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4021")]
        public async Task TestPartialPropertyWrongKeywordOrderAsync()
        {
            // 'partial' must appear immediately before the return type (CS0267), so this exercises a modifier-order
            // violation among the *other* modifiers ('static' before 'public') on a partial property, rather than
            // one involving 'partial' itself.
            var testCode = @"
public partial class TypeName
{
    static {|#0:public|} partial int Test { get; set; }
}

public partial class TypeName
{
    public static partial int Test
    {
        get => 0;
        set { }
    }
}";

            var expected = Diagnostic().WithLocation(0).WithArguments("public", "static");

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
