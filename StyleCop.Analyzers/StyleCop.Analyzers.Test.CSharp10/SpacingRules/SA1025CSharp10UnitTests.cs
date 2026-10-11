// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp10.SpacingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp9.SpacingRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.SpacingRules.SA1025CodeMustNotContainMultipleWhitespaceInARow,
        StyleCop.Analyzers.SpacingRules.SA1025CodeFixProvider>;

    public partial class SA1025CSharp10UnitTests : SA1025CSharp9UnitTests
    {
        [Fact]
        [WorkItem(3992, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3992")]
        public async Task TestMappedLineDirectiveDoesNotAffectIndentationAsync()
        {
            var testCode = @"#line (5,5)-(5,10) ""File.cs""
class TestClass
{
    public void Test()
    {
        int value = 0;
    }
}
#line default";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
