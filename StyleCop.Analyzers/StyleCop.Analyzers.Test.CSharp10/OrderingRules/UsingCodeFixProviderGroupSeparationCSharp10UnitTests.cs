// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp10.OrderingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.OrderingRules;
    using StyleCop.Analyzers.Settings.ObjectModel;
    using StyleCop.Analyzers.Test.CSharp9.OrderingRules;
    using StyleCop.Analyzers.Test.Verifiers;
    using Xunit;
    using static StyleCop.Analyzers.Test.OrderingRules.CombinedUsingDirectivesVerifier;

    public partial class UsingCodeFixProviderGroupSeparationCSharp10UnitTests : UsingCodeFixProviderGroupSeparationCSharp9UnitTests
    {
        [Fact]
        [WorkItem(3964, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3964")]
        public async Task TestGlobalUsingDirectivesAreProcessedIndependentlyAsync()
        {
            var testCode = @"global using System.Linq;

{|#0:using System;|}

namespace TestNamespace
{
}";
            var fixedCode = @"global using System.Linq;

namespace TestNamespace
{
    using System;

}";

            DiagnosticResult[] expected =
            {
                Diagnostic(SA1200UsingDirectivesMustBePlacedCorrectly.DescriptorInside).WithLocation(0),
            };

            await this.VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(3964, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3964")]
        public async Task TestGlobalUsingDirectivesAreReorderedAndFixedAsync()
        {
            var testCode = @"{|#0:global using System.Text;|}
{|#1:global using static System.Math;|}
global using AliasZ = System.IO;
{|#2:global using static System.Array;|}
{|#3:global using System;|}

class TestClass
{
}";

            var fixedCode = @"global using System;
global using System.Text;

global using static System.Array;
global using static System.Math;

global using AliasZ = System.IO;

class TestClass
{
}";

            DiagnosticResult[] expected =
            {
                StyleCopDiagnosticVerifier<SA1210UsingDirectivesMustBeOrderedAlphabeticallyByNamespace>.Diagnostic().WithLocation(0),
                StyleCopDiagnosticVerifier<SA1217UsingStaticDirectivesMustBeOrderedAlphabetically>.Diagnostic().WithLocation(1).WithArguments("System.Math", "System.Array"),
                StyleCopDiagnosticVerifier<SA1216UsingStaticDirectivesMustBePlacedAtTheCorrectLocation>.Diagnostic().WithLocation(2),
                StyleCopDiagnosticVerifier<SA1208SystemUsingDirectivesMustBePlacedBeforeOtherUsingDirectives>.Diagnostic().WithLocation(3).WithArguments("System", "System.Array"),
            };

            await this.VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
