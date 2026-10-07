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

    public partial class UsingCodeFixProviderCSharp10UnitTests : UsingCodeFixProviderCSharp9UnitTests
    {
        [Fact]
        [WorkItem(3964, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3964")]
        public async Task TestGlobalUsingDirectivesAreProcessedIndependentlyAsync()
        {
            var testCode = @"global using AliasZ = System.IO;
{|#0:global using static System.Math;|}

{|#1:using System;|}

namespace TestNamespace
{
    using AliasA = System.Text.StringBuilder;
    {|#2:using static System.Array;|}
}";
            var fixedCode = @"global using static System.Math;
global using AliasZ = System.IO;

namespace TestNamespace
{
    using System;
    using static System.Array;
    using AliasA = System.Text.StringBuilder;
}";

            DiagnosticResult[] expected =
            {
                StyleCopDiagnosticVerifier<SA1216UsingStaticDirectivesMustBePlacedAtTheCorrectLocation>.Diagnostic().WithLocation(0),
                Diagnostic(SA1200UsingDirectivesMustBePlacedCorrectly.DescriptorInside).WithLocation(1),
                StyleCopDiagnosticVerifier<SA1216UsingStaticDirectivesMustBePlacedAtTheCorrectLocation>.Diagnostic().WithLocation(2),
            };

            DiagnosticResult[] fixedExpected =
            {
            };

            await this.VerifyCSharpFixAsync(testCode, expected, fixedCode, fixedExpected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(3964, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3964")]
        public async Task TestGlobalUsingDirectivesAreReorderedAndFixedAsync()
        {
            var testCode = @"{|#0:global using AliasZ = System.IO;|}
{|#1:global using System;|}
{|#2:global using static System.Math;|}

namespace TestNamespace
{
    {|#3:using AliasA = System.Text.StringBuilder;|}
    {|#4:using System;|}
    {|#5:using static System.Array;|}
}";

            var fixedCode = @"global using System;
global using static System.Math;
global using AliasZ = System.IO;

namespace TestNamespace
{
    using System;
    using static System.Array;
    using AliasA = System.Text.StringBuilder;
}";

            DiagnosticResult[] expected =
            {
                StyleCopDiagnosticVerifier<SA1209UsingAliasDirectivesMustBePlacedAfterOtherUsingDirectives>.Diagnostic().WithLocation(0),
                StyleCopDiagnosticVerifier<SA1208SystemUsingDirectivesMustBePlacedBeforeOtherUsingDirectives>.Diagnostic().WithLocation(1).WithArguments("System", "System.IO"),
                StyleCopDiagnosticVerifier<SA1216UsingStaticDirectivesMustBePlacedAtTheCorrectLocation>.Diagnostic().WithLocation(2),
                StyleCopDiagnosticVerifier<SA1209UsingAliasDirectivesMustBePlacedAfterOtherUsingDirectives>.Diagnostic().WithLocation(3),
                StyleCopDiagnosticVerifier<SA1208SystemUsingDirectivesMustBePlacedBeforeOtherUsingDirectives>.Diagnostic().WithLocation(4).WithArguments("System", "System.Text.StringBuilder"),
                StyleCopDiagnosticVerifier<SA1216UsingStaticDirectivesMustBePlacedAtTheCorrectLocation>.Diagnostic().WithLocation(5),
            };

            DiagnosticResult[] fixedExpected =
            {
            };

            await this.VerifyCSharpFixAsync(testCode, expected, fixedCode, fixedExpected, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
