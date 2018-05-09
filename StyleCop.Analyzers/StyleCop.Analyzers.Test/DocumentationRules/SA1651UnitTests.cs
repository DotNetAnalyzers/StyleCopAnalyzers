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
    /// This class contains unit tests for <see cref="SA1651DoNotUsePlaceholderElements"/>.
    /// </summary>
    public class SA1651UnitTests : CodeFixVerifier
    {
        [Fact]
        public async Task TestEmptyDocumentationAsync()
        {
            var testCode = @"namespace FooNamespace
{
    ///
    ///
    ///
    public class ClassName
    {
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestDocumentationWithoutPlaceholdersAsync()
        {
            var testCode = @"namespace FooNamespace
{
    /// <summary>
    /// Content.
    /// </summary>
    public class ClassName
    {
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestTopLevelPlaceholderAsync()
        {
            var testCode = @"namespace FooNamespace
{
    /// <placeholder><summary>
    /// Content.
    /// </summary></placeholder>
    public class ClassName
    {
    }
}";

            var fixedCode = @"namespace FooNamespace
{
    /// <summary>
    /// Content.
    /// </summary>
    public class ClassName
    {
    }
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(3, 9);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);

            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestTopLevelEmptyPlaceholderAsync()
        {
            var testCode = @"namespace FooNamespace
{
    /// <placeholder/>
    public class ClassName
    {
    }
}";

            // Empty placeholders are not altered by the current code fix.
            var fixedCode = testCode;

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(3, 9);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);

            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestEmbeddedPlaceholderAsync()
        {
            var testCode = @"namespace FooNamespace
{
    /// <summary>
    /// <placeholder>Content.</placeholder>
    /// </summary>
    public class ClassName
    {
    }
}";

            var fixedCode = @"namespace FooNamespace
{
    /// <summary>
    /// Content.
    /// </summary>
    public class ClassName
    {
    }
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(4, 9);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);

            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestEmbeddedEmptyPlaceholderAsync()
        {
            var testCode = @"namespace FooNamespace
{
    /// <summary>
    /// Content.<placeholder/>
    /// </summary>
    public class ClassName
    {
    }
}";

            // Empty placeholders are not altered by the current code fix.
            var fixedCode = testCode;

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(4, 17);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);

            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestDeeplyEmbeddedPlaceholderAsync()
        {
            var testCode = @"namespace FooNamespace
{
    /// <summary>
    /// Content.
    /// </summary>
    /// <remarks>
    /// <list type=""bullet"">
    /// <item><placeholder>Nested content.</placeholder></item>
    /// </list>
    /// </remarks>
    public class ClassName
    {
    }
}";

            var fixedCode = @"namespace FooNamespace
{
    /// <summary>
    /// Content.
    /// </summary>
    /// <remarks>
    /// <list type=""bullet"">
    /// <item>Nested content.</item>
    /// </list>
    /// </remarks>
    public class ClassName
    {
    }
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(8, 15);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);

            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestFormattingPreservedAsync()
        {
            var testCode = @"namespace FooNamespace
{
   ///  <placeholder> <summary>
     /// Content <placeholder
            /// >.</placeholder>
  ///</summary>  </placeholder
///> <remarks/>
    public class ClassName
    {
    }
}";

            var fixedCode = @"namespace FooNamespace
{
   ///   <summary>
     /// Content .
  ///</summary>   <remarks/>
    public class ClassName
    {
    }
}";

            DiagnosticResult[] expected =
            {
                this.CSharpDiagnostic().WithLocation(3, 9),
                this.CSharpDiagnostic().WithLocation(4, 18),
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that included documentation without place holders will not produce diagnostics.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestNoPlaceHolderInIncludedDocumentationAsync()
        {
            var testCode = @"
/// <include file='SummaryWithoutPlaceHolder.xml' path='/TestClass/*'/>
public class TestClass
{
}
";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that included documentation with place holders will not produce the expected diagnostics.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestPlaceHolderInIncludedDocumentationAsync()
        {
            var testCode = @"
/// <include file='SummaryWithPlaceHolder.xml' path='/TestClass/*'/>
public class TestClass
{
}
";

            var expected = this.CSharpDiagnostic().WithLocation(2, 5);
            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            var offeredFixes = await this.GetOfferedCSharpFixesAsync(testCode).ConfigureAwait(false);
            Assert.Empty(offeredFixes);
        }

        protected override Project ApplyCompilationOptions(Project project)
        {
            var resolver = new TestXmlReferenceResolver();

            string contentSummaryWithoutPlaceHolder = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<TestClass>
  <summary>
  This is a test class.
  </summary>
</TestClass>
";
            resolver.XmlReferences.Add("SummaryWithoutPlaceHolder.xml", contentSummaryWithoutPlaceHolder);

            string contentSummaryWithPlaceHolder = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<TestClass>
  <summary>
  <placeholder>This is a test class.</placeholder>
  </summary>
</TestClass>
";
            resolver.XmlReferences.Add("SummaryWithPlaceHolder.xml", contentSummaryWithPlaceHolder);

            project = base.ApplyCompilationOptions(project);
            project = project.WithCompilationOptions(project.CompilationOptions.WithXmlReferenceResolver(resolver));
            return project;
        }

        protected override IEnumerable<DiagnosticAnalyzer> GetCSharpDiagnosticAnalyzers()
        {
            yield return new SA1651DoNotUsePlaceholderElements();
        }

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new SA1651CodeFixProvider();
        }
    }
}
