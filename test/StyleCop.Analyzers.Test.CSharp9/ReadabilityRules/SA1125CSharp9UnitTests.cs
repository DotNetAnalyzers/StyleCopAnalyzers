// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp9.ReadabilityRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp8.ReadabilityRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopDiagnosticVerifier<StyleCop.Analyzers.ReadabilityRules.SA1125UseShorthandForNullableTypes>;

    public partial class SA1125CSharp9UnitTests : SA1125CSharp8UnitTests
    {
        [Theory]
        [InlineData("System.Nullable<nint>")]
        [InlineData("Nullable<IntPtr>")]
        [InlineData("System.Nullable<nuint>")]
        [InlineData("Nullable<UIntPtr>")]
        [WorkItem(3969, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3969")]
        public async Task TestNativeSizedNullableTypesAsync(string nullableForm)
        {
            var testCode = @"
using System;

class TestClass
{
    {|#0:" + nullableForm + @"|} value;
}";

            await VerifyCSharpDiagnosticAsync(testCode, Diagnostic().WithLocation(0), CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(3969, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3969")]
        public async Task TestNativeSizedNullableAliasesDoNotReportAsync()
        {
            var testCode = @"
class TestClass
{
    nint? nativeInt;
    nuint? nativeUInt;
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
