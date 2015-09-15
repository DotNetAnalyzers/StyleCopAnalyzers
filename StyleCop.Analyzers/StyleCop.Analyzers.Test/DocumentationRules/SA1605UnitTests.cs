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
    /// This class contains unit tests for <see cref="SA1605PartialElementDocumentationMustHaveSummary"/>.
    /// </summary>
    public class SA1605UnitTests : DiagnosticVerifier
    {
        [Theory]
        [InlineData("class")]
        [InlineData("struct")]
        [InlineData("interface")]
        public async Task TestTypeNoDocumentationAsync(string typeName)
        {
            var testCode = @"
partial {0} TypeName
{{
}}";
            await this.VerifyCSharpDiagnosticAsync(string.Format(testCode, typeName), EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [InlineData("class")]
        [InlineData("struct")]
        [InlineData("interface")]
        public async Task TestTypeWithSummaryDocumentationAsync(string typeName)
        {
            var testCode = @"
/// <summary>
/// 
/// </summary>
partial {0} TypeName
{{
}}";
            await this.VerifyCSharpDiagnosticAsync(string.Format(testCode, typeName), EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [InlineData("class")]
        [InlineData("struct")]
        [InlineData("interface")]
        public async Task TestTypeWithContentDocumentationAsync(string typeName)
        {
            var testCode = @"
/// <content>
/// 
/// </content>
partial {0} TypeName
{{
}}";
            await this.VerifyCSharpDiagnosticAsync(string.Format(testCode, typeName), EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [InlineData("class")]
        [InlineData("struct")]
        [InlineData("interface")]
        public async Task TestTypeWithInheritedDocumentationAsync(string typeName)
        {
            var testCode = @"
/// <inheritdoc/>
partial {0} TypeName
{{
}}";
            await this.VerifyCSharpDiagnosticAsync(string.Format(testCode, typeName), EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [InlineData("class")]
        [InlineData("struct")]
        [InlineData("interface")]
        public async Task TestTypeWithoutDocumentationAsync(string typeName)
        {
            var testCode = @"
///
partial {0}
TypeName
{{
}}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(4, 1);

            await this.VerifyCSharpDiagnosticAsync(string.Format(testCode, typeName), expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [InlineData("enum")]
        [InlineData("class")]
        [InlineData("struct")]
        [InlineData("interface")]
        public async Task TestNonPartialTypeWithoutDocumentationAsync(string typeName)
        {
            var testCode = @"
///
{0}
TypeName
{{
}}";

            await this.VerifyCSharpDiagnosticAsync(string.Format(testCode, typeName), EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestMethodNoDocumentationAsync()
        {
            var testCode = @"
/// <summary>
/// 
/// </summary>
public partial class ClassName
{
    partial void Test();
}";
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestMethodWithSummaryDocumentationAsync()
        {
            var testCode = @"
/// <summary>
/// 
/// </summary>
public partial class ClassName
{
    /// <summary>
    ///
    /// </summary>
    partial void Test();
}";
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestMethodWithContentDocumentationAsync()
        {
            var testCode = @"
/// <summary>
/// 
/// </summary>
public partial class ClassName
{
    /// <content>
    ///
    /// </content>
    partial void Test();
}";
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestMethodWithInheritedDocumentationAsync()
        {
            var testCode = @"
/// <summary>
/// 
/// </summary>
public partial class ClassName
{
    /// <inheritdoc/>
    partial void Test();
}";
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestMethodWithoutDocumentationAsync()
        {
            var testCode = @"
/// <summary>
/// 
/// </summary>
public partial class ClassName
{
    ///
    partial void Test();
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(8, 18);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestNonPartialMethodWithoutDocumentationAsync()
        {
            var testCode = @"
/// <summary>
/// 
/// </summary>
public partial class ClassName
{
    ///
    public void Test() { }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        protected override IEnumerable<DiagnosticAnalyzer> GetCSharpDiagnosticAnalyzers()
        {
            yield return new SA1605PartialElementDocumentationMustHaveSummary();
        }
    }
}
