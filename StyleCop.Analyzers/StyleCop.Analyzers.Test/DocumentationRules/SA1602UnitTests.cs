// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.Test.DocumentationRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.DocumentationRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopDiagnosticVerifier<StyleCop.Analyzers.DocumentationRules.SA1602EnumerationItemsMustBeDocumented>;

    /// <summary>
    /// This class contains unit tests for <see cref="SA1602EnumerationItemsMustBeDocumented"/>.
    /// </summary>
    public class SA1602UnitTests
    {
        [Fact]
        public async Task TestEnumWithDocumentationAsync()
        {
            var testCode = @"
/// <summary>
/// Some Documentation
/// </summary>
enum TypeName
{
    /// <summary>
    /// Some Documentation
    /// </summary>
    Bar
}";
            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestEnumWithoutDocumentationAsync()
        {
            var testCode = @"
enum TypeName
{
    Bar
}";

            DiagnosticResult expected = Diagnostic().WithLocation(4, 5);

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestNestedPrivateEnumWithoutDocumentationAsync()
        {
            var testCode = @"
class ClassName
{
    private enum TypeName
    {
        Bar
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestEnumWithEmptyDocumentationAsync()
        {
            var testCode = @"
/// <summary>
/// Some Documentation
/// </summary>
enum TypeName
{
    /// <summary>
    /// 
    /// </summary>
    Bar
}";

            DiagnosticResult expected = Diagnostic().WithLocation(10, 5);

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
