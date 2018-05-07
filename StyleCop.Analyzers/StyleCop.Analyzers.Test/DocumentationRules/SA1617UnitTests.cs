// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.DocumentationRules
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Analyzers.DocumentationRules;
    using Helpers;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using TestHelper;
    using Xunit;

    /// <summary>
    /// This class contains unit tests for <see cref="SA1617VoidReturnValueMustNotBeDocumented"/>.
    /// </summary>
    public class SA1617UnitTests : CodeFixVerifier
    {
        [Fact]
        public async Task TestMethodWithReturnValueNoDocumentationAsync()
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
    public ClassName Method() { return null; }

    /// <value>
    /// Foo
    /// </value>
    public delegate ClassName MethodDelegate();
}";
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestMethodWithReturnValueWithDocumentationAsync()
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
    /// <returns>null</returns>
    public ClassName Method() { return null; }

    /// <value>
    /// Foo
    /// </value>
    /// <returns>Some value</returns>
    public delegate ClassName MethodDelegate();
}";
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestNoDocumentationAsync()
        {
            var testCode = @"
/// <summary>
/// Foo
/// </summary>
public class ClassName
{
    public ClassName Method() { return null; }

    public delegate ClassName MethodDelegate();
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
    public ClassName Method() { return null; }

    /// <inheritdoc/>
    public delegate ClassName MethodDelegate();
}";
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestMethodWithoutReturnValueNoDocumentationAsync()
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
    public void Method() { }

    /// <value>
    /// Foo
    /// </value>
    public delegate void MethodDelegate();
}";
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestMethodWithoutReturnValueWithDocumentationAsync()
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
    /// <returns>null</returns>
    public void Method() { }

    /// <value>
    /// Foo
    /// </value>
    /// <returns>Some value</returns>
    public delegate void MethodDelegate();
}";

            var expected = new[]
            {
                this.CSharpDiagnostic().WithLocation(10, 9),
                this.CSharpDiagnostic().WithLocation(16, 9),
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestCodeFixWithNoDataAsync()
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
    /// <returns>null</returns>
    public void Method() { }
}";

            var expected = new[]
            {
                this.CSharpDiagnostic().WithLocation(10, 9),
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);

            var fixedCode = @"
/// <summary>
/// Foo
/// </summary>
public class ClassName
{
    /// <value>
    /// Foo
    /// </value>
    public void Method() { }
}";

            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestCodeFixShareLineWithValueAsync()
        {
            var testCode = @"
/// <summary>
/// Foo
/// </summary>
public class ClassName
{
    /// <value>
    /// Foo
    /// </value><returns>null</returns>
    public void Method() { }
}";

            var expected = new[]
            {
                this.CSharpDiagnostic().WithLocation(9, 17),
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);

            var fixedCode = @"
/// <summary>
/// Foo
/// </summary>
public class ClassName
{
    /// <value>
    /// Foo
    /// </value>
    public void Method() { }
}";

            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestCodeFixBeforeValueAsync()
        {
            var testCode = @"
/// <summary>
/// Foo
/// </summary>
public class ClassName
{
    /// <returns>null</returns> <value>
    /// Foo
    /// </value>
    public void Method() { }
}";

            var expected = new[]
            {
                this.CSharpDiagnostic().WithLocation(7, 9),
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);

            var fixedCode = @"
/// <summary>
/// Foo
/// </summary>
public class ClassName
{
    /// <value>
    /// Foo
    /// </value>
    public void Method() { }
}";

            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that included method documentation without a returns tag will be accepted.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestMethodWithValidIncludeAsync()
        {
            var testCode = @"
public class ClassName
{
    /// <include file='MethodWithoutReturns.xml' path='/ClassName/Method/*'/>
    public void Method() { }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that included method documentation with a returns tag will be flagged.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestMethodWithReturnsInIncludeAsync()
        {
            var testCode = @"
public class ClassName
{
    /// <include file='MethodWithReturns.xml' path='/ClassName/Method/*'/>
    public void Method() { }
}";

            var expected = this.CSharpDiagnostic().WithLocation(4, 9);
            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            var offeredFixes = await this.GetOfferedCSharpFixesAsync(testCode).ConfigureAwait(false);
            Assert.Empty(offeredFixes);
        }

        /// <summary>
        /// Verifies that included method documentation containing &gt;inheritdoc/&lt; will be accepted.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestMethodWithInheritdocInIncludeAsync()
        {
            var testCode = @"
public interface ITestInterface
{
  /// <summary>
  /// Foo bar.
  /// </summary>
  void Method();
}

public class ClassName : ITestInterface
{
    /// <include file='MethodWithInheritdoc.xml' path='/ClassName/Method/*'/>
    public void Method() { }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        protected override Project ApplyCompilationOptions(Project project)
        {
            var resolver = new TestXmlReferenceResolver();

            string contentWithoutReturns = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<ClassName>
  <Method>
    <summary>Foo</summary>
  </Method>
</ClassName>
";
            resolver.XmlReferences.Add("MethodWithoutReturns.xml", contentWithoutReturns);

            string contentWithReturns = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<ClassName>
  <Method>
    <summary>Foo</summary>
    <returns>Bar</returns>
  </Method>
</ClassName>
";
            resolver.XmlReferences.Add("MethodWithReturns.xml", contentWithReturns);

            string contentWithInheritdocValue = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<ClassName>
  <Method>
    <inheritdoc/>
  </Method>
</ClassName>
";
            resolver.XmlReferences.Add("MethodWithInheritdoc.xml", contentWithInheritdocValue);

            project = base.ApplyCompilationOptions(project);
            project = project.WithCompilationOptions(project.CompilationOptions.WithXmlReferenceResolver(resolver));
            return project;
        }

        protected override IEnumerable<DiagnosticAnalyzer> GetCSharpDiagnosticAnalyzers()
        {
            yield return new SA1617VoidReturnValueMustNotBeDocumented();
        }

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new SA1617CodeFixProvider();
        }
    }
}
