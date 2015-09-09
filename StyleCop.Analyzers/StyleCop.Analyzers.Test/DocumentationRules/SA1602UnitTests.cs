// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.DocumentationRules
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.DocumentationRules;
    using TestHelper;
    using Xunit;

    /// <summary>
    /// This class contains unit tests for <see cref="SA1602EnumerationItemsMustBeDocumented"/>.
    /// </summary>
    public class SA1602UnitTests : DiagnosticVerifier
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
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestEnumWithoutDocumentationAsync()
        {
            var testCode = @"
enum TypeName
{
    Bar
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(4, 5);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
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

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(10, 5);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        protected override IEnumerable<DiagnosticAnalyzer> GetCSharpDiagnosticAnalyzers()
        {
            yield return new SA1602EnumerationItemsMustBeDocumented();
        }
    }
}
