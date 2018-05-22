// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.DocumentationRules
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.DocumentationRules;
    using StyleCop.Analyzers.Test.Helpers;
    using TestHelper;
    using Xunit;

    /// <summary>
    /// This class contains unit tests for <see cref="SA1608ElementDocumentationMustNotHaveDefaultSummary"/>.
    /// </summary>
    public class SA1608UnitTests : DiagnosticVerifier
    {
        [Theory]
        [InlineData("class")]
        [InlineData("struct")]
        [InlineData("interface")]
        public async Task TestTypeNoDocumentationAsync(string typeName)
        {
            var testCode = @"
{0} TypeName
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
/// Foo
/// </summary>
{0} TypeName
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
/// Foo
/// </content>
{0} TypeName
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
{0} TypeName
{{
}}";
            await this.VerifyCSharpDiagnosticAsync(string.Format(testCode, typeName), EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [InlineData("class")]
        [InlineData("struct")]
        [InlineData("interface")]
        public async Task TestTypeWithoutSummaryDocumentationAsync(string typeName)
        {
            var testCode = @"
/// <summary>
/// 
/// </summary>
{0}
TypeName
{{
}}";
            await this.VerifyCSharpDiagnosticAsync(string.Format(testCode, typeName), EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [InlineData("class")]
        [InlineData("struct")]
        [InlineData("interface")]
        public async Task TestTypeWithoutContentDocumentationAsync(string typeName)
        {
            var testCode = @"
/// <content>
/// 
/// </content>
partial {0}
TypeName
{{
}}";
            await this.VerifyCSharpDiagnosticAsync(string.Format(testCode, typeName), EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [InlineData("class")]
        [InlineData("struct")]
        [InlineData("interface")]
        public async Task TestTypeWithDefaultDocumentationAsync(string typeName)
        {
            var testCode = $@"
/// <summary>
/// Summary description for the ClassName class.
/// </summary>
public {typeName} ClassName
{{
}}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(2, 5);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestClassWithDefaultDocumentationMultipleWhitespacesAsync()
        {
            var testCode = @"
/// <summary>
/// Summary           description 
/// for the      ClassName class.
/// </summary>
public class ClassName
{
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(2, 5);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestClassWithIncludedEmptyDocumentationAsync()
        {
            var testCode = @"
/// <include file='ClassWithoutSummary.xml' path='/ClassName/*' />
public class ClassName
{
}";
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestClassWithIncludedSummaryDocumentationAsync()
        {
            var testCode = @"
/// <include file='ClassWithSummary.xml' path='/ClassName/*' />
public class ClassName
{
}";
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestClassWithIncludedDefaultSummaryDocumentationAsync()
        {
            var testCode = @"
/// <include file='ClassWithDefaultSummary.xml' path='/ClassName/*' />
public class ClassName
{
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(3, 14);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        protected override Project ApplyCompilationOptions(Project project)
        {
            var resolver = new TestXmlReferenceResolver();

            string contentWithoutSummary = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<ClassName>
</ClassName>
";
            resolver.XmlReferences.Add("ClassWithoutSummary.xml", contentWithoutSummary);

            string contentWithSummary = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<ClassName>
  <summary>
    Foo
  </summary>
</ClassName>
";
            resolver.XmlReferences.Add("ClassWithSummary.xml", contentWithSummary);

            string contentWithDefaultSummary = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<ClassName>
  <summary>
    Summary description for the ClassName class.
  </summary>
</ClassName>
";
            resolver.XmlReferences.Add("ClassWithDefaultSummary.xml", contentWithDefaultSummary);

            project = base.ApplyCompilationOptions(project);
            project = project.WithCompilationOptions(project.CompilationOptions.WithXmlReferenceResolver(resolver));
            return project;
        }

        /// <inheritdoc/>
        protected override IEnumerable<DiagnosticAnalyzer> GetCSharpDiagnosticAnalyzers()
        {
            yield return new SA1608ElementDocumentationMustNotHaveDefaultSummary();
        }
    }
}
