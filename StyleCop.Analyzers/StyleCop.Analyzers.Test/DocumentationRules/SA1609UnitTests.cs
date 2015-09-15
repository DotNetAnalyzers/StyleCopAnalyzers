// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.DocumentationRules
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Analyzers.DocumentationRules;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using TestHelper;
    using Xunit;

    /// <summary>
    /// This class contains unit tests for <see cref="SA1609PropertyDocumentationMustHaveValue"/>.
    /// </summary>
    public class SA1609UnitTests : CodeFixVerifier
    {
        [Fact]
        public async Task TestPropertyWithDocumentationAsync()
        {
            var testCode = @"
/// <summary>
/// 
/// </summary>
public class ClassName
{
    /// <summary>
    ///
    /// </summary>
    /// <value>
    ///
    /// </value>
    public ClassName Property { get; set; }
}";
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestPropertyWithInheritedDocumentationAsync()
        {
            var testCode = @"
/// <summary>
/// 
/// </summary>
public class ClassName
{
    /// <inheritdoc/>
    public ClassName Property { get; set; }
}";
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestPropertyNoDocumentationAsync()
        {
            var testCode = @"
/// <summary>
/// 
/// </summary>
public class ClassName
{
    public ClassName Property { get; set; }
}";
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestPropertyWithEmptySummaryDocumentationAsync()
        {
            var testCode = @"
/// <summary>
/// 
/// </summary>
public class ClassName
{
    /// <summary>
    /// 
    /// </summary>
    public ClassName Property { get; set; }
}";

            var fixedCode = @"
/// <summary>
/// 
/// </summary>
public class ClassName
{
    /// <summary>
    /// 
    /// </summary>
    /// <value>
    /// 
    /// </value>
    public ClassName Property { get; set; }
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(10, 22);
            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestPropertyWithStandardSummaryDocumentationAsync()
        {
            var testCode = @"
/// <summary>
/// 
/// </summary>
public class ClassName
{
    /// <summary>
    /// Gets or sets a property.
    /// </summary>
    public ClassName Property { get; set; }
}";

            var fixedCode = @"
/// <summary>
/// 
/// </summary>
public class ClassName
{
    /// <summary>
    /// Gets or sets a property.
    /// </summary>
    /// <value>
    /// <placeholder>A property.</placeholder>
    /// </value>
    public ClassName Property { get; set; }
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(10, 22);
            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [InlineData("Gets a property.", "A property.")]
        [InlineData("Gets a", "A")]
        [InlineData("property.", "property.")]
        public async Task TestGetterOnlyPropertyWithStandardSummaryDocumentationAsync(string summaryText, string valueText)
        {
            var testCode = $@"
/// <summary>
/// 
/// </summary>
public class ClassName
{{
    /// <summary>
    /// {summaryText}
    /// </summary>
    public ClassName Property {{ get; }}
}}";

            var fixedCode = $@"
/// <summary>
/// 
/// </summary>
public class ClassName
{{
    /// <summary>
    /// {summaryText}
    /// </summary>
    /// <value>
    /// <placeholder>{valueText}</placeholder>
    /// </value>
    public ClassName Property {{ get; }}
}}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(10, 22);
            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestSetterOnlyPropertyWithStandardSummaryDocumentationAsync()
        {
            var testCode = @"
/// <summary>
/// 
/// </summary>
public class ClassName
{
    /// <summary>
    /// Sets a property.
    /// </summary>
    public ClassName Property { set { } }
}";

            var fixedCode = @"
/// <summary>
/// 
/// </summary>
public class ClassName
{
    /// <summary>
    /// Sets a property.
    /// </summary>
    /// <value>
    /// <placeholder>A property.</placeholder>
    /// </value>
    public ClassName Property { set { } }
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(10, 22);
            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestPropertyWithNonStandardSummaryDocumentationAsync()
        {
            var testCode = @"
/// <summary>
/// 
/// </summary>
public class ClassName
{
    /// <summary>
    /// A property.
    /// </summary>
    public ClassName Property { get; set; }
}";

            var fixedCode = @"
/// <summary>
/// 
/// </summary>
public class ClassName
{
    /// <summary>
    /// A property.
    /// </summary>
    /// <value>
    /// <placeholder>A property.</placeholder>
    /// </value>
    public ClassName Property { get; set; }
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(10, 22);
            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        protected override IEnumerable<DiagnosticAnalyzer> GetCSharpDiagnosticAnalyzers()
        {
            yield return new SA1609PropertyDocumentationMustHaveValue();
        }

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new SA1609SA1610CodeFixProvider();
        }
    }
}
