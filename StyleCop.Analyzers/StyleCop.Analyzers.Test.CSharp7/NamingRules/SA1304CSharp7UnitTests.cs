// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp7.NamingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.CSharp;
    using StyleCop.Analyzers.NamingRules;
    using StyleCop.Analyzers.Test.NamingRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.NamingRules.SA1304NonPrivateReadonlyFieldsMustBeginWithUpperCaseLetter,
        StyleCop.Analyzers.NamingRules.RenameToUpperCaseCodeFixProvider>;

    public partial class SA1304CSharp7UnitTests : SA1304UnitTests
    {
        /// <summary>
        /// Verifies that private protected readonly fields are checked independently of SA1307.
        /// </summary>
        /// <param name="disableSA1307">Whether SA1307 is disabled.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        [WorkItem(3557, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3557")]
        public async Task TestPrivateProtectedReadonlyFieldsAsync(bool disableSA1307)
        {
            var test = new CombinedCSharpTest(LanguageVersion.CSharp7_2)
            {
                TestCode = @"public class Foo
{
    private protected readonly int {|#0:bar|}, Upper, _underscore;
    private protected int car;
}",
                FixedCode = @"public class Foo
{
    private protected readonly int Bar, Upper, _underscore;
    private protected int car;
}",
            };
            if (disableSA1307)
            {
                test.DisabledDiagnostics.Add(SA1307AccessibleFieldsMustBeginWithUpperCaseLetter.DiagnosticId);
            }

            test.ExpectedDiagnostics.Add(Diagnostic().WithLocation(0));
            await test.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }
    }
}
