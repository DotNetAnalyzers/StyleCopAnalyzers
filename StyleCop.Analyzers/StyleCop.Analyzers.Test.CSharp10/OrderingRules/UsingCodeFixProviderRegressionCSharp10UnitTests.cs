// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp10.OrderingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.OrderingRules;
    using StyleCop.Analyzers.Test.CSharp9.OrderingRules;
    using StyleCop.Analyzers.Test.Verifiers;
    using Xunit;
    using static StyleCop.Analyzers.Test.OrderingRules.CombinedUsingDirectivesVerifier;

    public partial class UsingCodeFixProviderRegressionCSharp10UnitTests : UsingCodeFixProviderRegressionCSharp9UnitTests
    {
        [Fact]
        [WorkItem(3964, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3964")]
        public async Task TestGlobalUsingDirectivesDoNotInteractWithLocalOrderingAsync()
        {
            var testCode = @"global using AliasZ = System.IO;

using System;

namespace TestNamespace
{
    {|#0:using AliasA = System.Text.StringBuilder;|}
}";
            var fixedCode = @"global using AliasZ = System.IO;

using System;
using AliasA = System.Text.StringBuilder;

namespace TestNamespace
{
}";

            DiagnosticResult[] expected =
            {
                Diagnostic(SA1200UsingDirectivesMustBePlacedCorrectly.DescriptorOutside).WithLocation(0),
            };
            await this.VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(3964, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3964")]
        public async Task TestGlobalUsingDirectivesAreReorderedAndFixedAsync()
        {
            var testCode = @"{|#0:global using System.IO;|}
global using AliasB = System.Linq;
{|#1:global using static System.Math;|}
{|#2:global using System;|}
{|#3:global using AliasA = System.Text;|}
global using static System.Array;

class TestClass
{
}";

            var fixedCode = @"global using System;
global using System.IO;
global using static System.Array;
global using static System.Math;
global using AliasA = System.Text;
global using AliasB = System.Linq;

class TestClass
{
}";

            DiagnosticResult[] expected =
            {
                StyleCopDiagnosticVerifier<SA1210UsingDirectivesMustBeOrderedAlphabeticallyByNamespace>.Diagnostic().WithLocation(0),
                StyleCopDiagnosticVerifier<SA1216UsingStaticDirectivesMustBePlacedAtTheCorrectLocation>.Diagnostic().WithLocation(1),
                StyleCopDiagnosticVerifier<SA1217UsingStaticDirectivesMustBeOrderedAlphabetically>.Diagnostic().WithLocation(1).WithArguments("System.Math", "System.Array"),
                StyleCopDiagnosticVerifier<SA1208SystemUsingDirectivesMustBePlacedBeforeOtherUsingDirectives>.Diagnostic().WithLocation(2).WithArguments("System", "System.Math"),
                StyleCopDiagnosticVerifier<SA1211UsingAliasDirectivesMustBeOrderedAlphabeticallyByAliasName>.Diagnostic().WithLocation(3).WithArguments("AliasA", "AliasB"),
            };
            await this.VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
