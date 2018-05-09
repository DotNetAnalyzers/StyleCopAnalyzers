// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.DocumentationRules
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using StyleCop.Analyzers.Test.Helpers;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.DocumentationRules;
    using TestHelper;
    using Xunit;

    /// <summary>
    /// This class contains unit tests for <see cref="SA1610PropertyDocumentationMustHaveValueText"/>.
    /// </summary>
    public class SA1610UnitTests : CodeFixVerifier
    {
        [Fact]
        public async Task TestPropertyWithDocumentationAsync()
        {
            var testCode = @"
/// <summary>
/// Foo
/// </summary>
public class ClassName
{
    /// <value>
    /// Foo
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
/// Foo
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
/// Foo
/// </summary>
public class ClassName
{
    public ClassName Property { get; set; }
}";
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestPropertySummaryOnlyAsync()
        {
            var testCode = @"
/// <summary>
/// Foo
/// </summary>
public class ClassName
{
    /// <summary>
    /// Gets or sets something.
    /// </summary>
    public ClassName Property { get; set; }
}";

            // Reported by SA1609 instead.
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestPropertyWithEmptyDocumentationNoSummaryAsync()
        {
            var testCode = @"
/// <summary>
/// Foo
/// </summary>
public class ClassName
{
    /// <value>
    /// 
    /// </value>
    public ClassName Property { get; set; }
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(10, 22);
            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, testCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
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
    /// <value>
    /// 
    /// </value>
    public ClassName Property { get; set; }
}";

            // No changes are made.
            var fixedCode = testCode;

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(13, 22);
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
    /// <value>
    /// 
    /// </value>
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

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(13, 22);
            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestGetterOnlyPropertyWithStandardSummaryDocumentationAsync()
        {
            var testCode = @"
/// <summary>
/// 
/// </summary>
public class ClassName
{
    /// <summary>
    /// Gets a property.
    /// </summary>
    /// <value>
    /// 
    /// </value>
    public ClassName Property { get; }
}";

            var fixedCode = @"
/// <summary>
/// 
/// </summary>
public class ClassName
{
    /// <summary>
    /// Gets a property.
    /// </summary>
    /// <value>
    /// <placeholder>A property.</placeholder>
    /// </value>
    public ClassName Property { get; }
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(13, 22);
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
    /// <value>
    /// 
    /// </value>
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

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(13, 22);
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
    /// <value>
    /// 
    /// </value>
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

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(13, 22);
            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that included property documentation will be accepted.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestPropertyWithValidIncludeAsync()
        {
            var testCode = @"
public class ClassName
{
    /// <include file='PropertyWithValue.xml' path='/ClassName/Property/*'/>
    public int Property
    {
        get;
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that included property documentation without a value tag will be accepted (this is handled by SA1609).
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestPropertyWithoutValueInIncludeAsync()
        {
            var testCode = @"
public class ClassName
{
    /// <include file='PropertyWithoutValue.xml' path='/ClassName/Property/*'/>
    public int Property
    {
        get;
    }
}";
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that included property documentation with an empty value tag will be flagged.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestPropertyWithEmptyValueInIncludeAsync()
        {
            var testCode = @"
public class ClassName
{
    /// <include file='PropertyWithEmptyValue.xml' path='/ClassName/Property/*'/>
    public int Property
    {
        get;
    }
}";
            var expected = this.CSharpDiagnostic().WithLocation(5, 16);
            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            var offeredFixes = await this.GetOfferedCSharpFixesAsync(testCode).ConfigureAwait(false);
            Assert.Empty(offeredFixes);
        }

        /// <summary>
        /// Verifies that included property documentation containing &gt;inheritdoc/&lt; will be accepted.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestPropertyWithInheritdocInIncludeAsync()
        {
            var testCode = @"
public interface ITestInterface
{
  /// <summary>
  /// Gets the test property value.
  /// </summary>
  /// <value>Test number.</value>
  int Property { get; }
}

public class ClassName : ITestInterface
{
    /// <include file='PropertyWithInheritdoc.xml' path='/ClassName/Property/*'/>
    public int Property
    {
        get;
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        protected override Project ApplyCompilationOptions(Project project)
        {
            var resolver = new TestXmlReferenceResolver();

            string contentWithValue = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<ClassName>
  <Property>
    <summary>Foo</summary>
    <value>Bar</value>
  </Property>
</ClassName>
";
            resolver.XmlReferences.Add("PropertyWithValue.xml", contentWithValue);

            string contentWithoutValue = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<ClassName>
  <Property>
    <summary>Foo</summary>
  </Property>
</ClassName>
";
            resolver.XmlReferences.Add("PropertyWithoutValue.xml", contentWithoutValue);

            string contentWithEmptyValue = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<ClassName>
  <Property>
    <summary>Foo</summary>
    <value>  </value>
  </Property>
</ClassName>
";
            resolver.XmlReferences.Add("PropertyWithEmptyValue.xml", contentWithEmptyValue);

            string contentWithInheritdocValue = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<ClassName>
  <Property>
    <inheritdoc/>
  </Property>
</ClassName>
";
            resolver.XmlReferences.Add("PropertyWithInheritdoc.xml", contentWithInheritdocValue);

            project = base.ApplyCompilationOptions(project);
            project = project.WithCompilationOptions(project.CompilationOptions.WithXmlReferenceResolver(resolver));
            return project;
        }

        protected override IEnumerable<DiagnosticAnalyzer> GetCSharpDiagnosticAnalyzers()
        {
            yield return new SA1610PropertyDocumentationMustHaveValueText();
        }

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new SA1609SA1610CodeFixProvider();
        }
    }
}
