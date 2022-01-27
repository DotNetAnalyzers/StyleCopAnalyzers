// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.DocumentationRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.DocumentationRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.DocumentationRules.SA1602EnumerationItemsMustBeDocumented,
        StyleCop.Analyzers.DocumentationRules.SA1602CodeFixProvider>;

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
            var expectedCode = @"
enum TypeName
{
    /// <summary>
    /// Bar.
    /// </summary>
    Bar
}";

            DiagnosticResult expected = Diagnostic().WithLocation(4, 5);

            await VerifyCSharpFixAsync(testCode, expected, expectedCode, CancellationToken.None).ConfigureAwait(false);
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

            var fixedCode = @"
/// <summary>
/// Some Documentation
/// </summary>
enum TypeName
{
    /// <summary>
    /// Bar.
    /// </summary>
    Bar
}";

            DiagnosticResult expected = Diagnostic().WithLocation(10, 5);

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
