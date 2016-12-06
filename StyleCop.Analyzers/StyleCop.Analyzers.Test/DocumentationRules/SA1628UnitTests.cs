// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.DocumentationRules
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.DocumentationRules;
    using StyleCop.Analyzers.Test.Helpers;
    using TestHelper;
    using Xunit;

    /// <summary>
    /// This class contains unit tests for <see cref="SA1628DocumentationTextMustBeginWithACapitalLetter"/>.
    /// </summary>
    public class SA1628UnitTests : CodeFixVerifier
    {
        [Fact]
        public void TestDisabledByDefault()
        {
            var analyzer = this.GetCSharpDiagnosticAnalyzers().Single();
            Assert.Equal(1, analyzer.SupportedDiagnostics.Length);
            Assert.False(analyzer.SupportedDiagnostics[0].IsEnabledByDefault);
        }

        [Fact]
        public async Task TestClassWithXmlCommentAsync()
        {
            var testCode = @"/// <summary>
/// xML Documentation
/// </summary>
public class TypeName
{
}
";
            var fixedCode = @"/// <summary>
/// XML Documentation
/// </summary>
public class TypeName
{
}
";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(2, 5).WithArguments("summary");

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestMethodWithTwoXmlCommentsAsync()
        {
            var testCode = @"public class TypeName
{
    /// <summary>
    /// xML Documentation
    /// </summary>
    /// <param name=""bar"">bar description</param>
    public void Foo(string bar)
    {
    }
}
";
            var fixedCode = @"public class TypeName
{
    /// <summary>
    /// XML Documentation
    /// </summary>
    /// <param name=""bar"">Bar description</param>
    public void Foo(string bar)
    {
    }
}
";

            var expected = new DiagnosticResult[]
            {
                this.CSharpDiagnostic().WithLocation(4, 9).WithArguments("summary"),
                this.CSharpDiagnostic().WithLocation(6, 27).WithArguments("param")
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestMethodWithSingeLineDocumentationAsync()
        {
            var testCode = @"public class TypeName
{
    /// <summary>summary text</summary>
    public void Bar()
    {
    }
}
";
            var fixedCode = @"public class TypeName
{
    /// <summary>Summary text</summary>
    public void Bar()
    {
    }
}
";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(3, 18).WithArguments("summary");

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestMethodWithSummaryAndDocumentationThatStartsWithSeeAsync()
        {
            var testCode = @"public class TypeName
{
    /// <summary>
    /// <see cref=""Foo""/> text
    /// </summary>
    public void Bar()
    {
    }
}
";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestMethodWithEmptySummaryAndCDATADocumentationAsync()
        {
            var testCode = @"public class TypeName
{
    /// <summary/>
    /// <param name=""bar"">
    /// <![CDATA[bar description]]>
    /// </param>
    public void Foo(string bar)
    {
    }
}
";
            var fixedCode = @"public class TypeName
{
    /// <summary/>
    /// <param name=""bar"">
    /// <![CDATA[Bar description]]>
    /// </param>
    public void Foo(string bar)
    {
    }
}
";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(5, 18).WithArguments("param");

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestMethodWithEmptySummaryAndReturnsDocumentationAsync()
        {
            var testCode = @"public class TypeName
{
    /// <summary>
    /// </summary>
    /// <returns>the bar</returns>
    public string Foo()
    {
        return ""bar"";
    }
}
";
            var fixedCode = @"public class TypeName
{
    /// <summary>
    /// </summary>
    /// <returns>The bar</returns>
    public string Foo()
    {
        return ""bar"";
    }
}
";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(5, 18).WithArguments("returns");

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestIncludedDocumentationAsync()
        {
            var testCode = @"
class Class1
{
    /// <include file='ClassWithSummary.xml' path='/Class1/MethodName/*'/>
    public void MethodName()
    {
    }
}
";
            var fixedCode = @"
class Class1
{
    /// <include file='ClassWithSummary.xml' path='/Class1/MethodName/*'/>
    public void MethodName()
    {
    }
}
";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(5, 17).WithArguments("summary");

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestIncludedCorrectDocumentationAsync()
        {
            var testCode = @"
class Class1
{
    /// <include file='ClassWithCorrectSummary.xml' path='/Class1/MethodName/*'/>
    public void MethodName()
    {
    }
}
";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestIncludedEmptyDocumentationAsync()
        {
            var testCode = @"
class Class1
{
    /// <include file='ClassWithEmptySummary.xml' path='/Class1/MethodName/*'/>
    public void MethodName()
    {
    }
}
";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestIncludedDocumentationThatStartsWithSeeAsync()
        {
            var testCode = @"
class Class1
{
    /// <include file='ClassWithSummaryThatStartsWithSee.xml' path='/Class1/MethodName/*'/>
    public void MethodName()
    {
    }
}
";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        protected override Project ApplyCompilationOptions(Project project)
        {
            var resolver = new TestXmlReferenceResolver();
            string contentWithSummary = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<Class1>
  <MethodName>
    <summary>
      <![CDATA[sample method.]]>
    </summary>
    <returns>
      a <see cref=""Task""/> representing the asynchronous operation.
    </returns>
  </MethodName>
</Class1>
";
            resolver.XmlReferences.Add("ClassWithSummary.xml", contentWithSummary);

            string contentWithCorrectSummary = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<Class1>
  <MethodName>
    <summary>Correct</summary>
  </MethodName>
</Class1>
";
            resolver.XmlReferences.Add("ClassWithCorrectSummary.xml", contentWithCorrectSummary);

            string contentWithEmptySummary = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<Class1>
  <MethodName>
    <summary></summary>
  </MethodName>
</Class1>
";
            resolver.XmlReferences.Add("ClassWithEmptySummary.xml", contentWithEmptySummary);

            string contentWithSummaryThatStartsWithSee = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<Class1>
  <MethodName>
    <summary><see cref=""Foo""/> text</summary>
  </MethodName>
</Class1>
";
            resolver.XmlReferences.Add("ClassWithSummaryThatStartsWithSee.xml", contentWithSummaryThatStartsWithSee);

            project = base.ApplyCompilationOptions(project);
            project = project.WithCompilationOptions(project.CompilationOptions.WithXmlReferenceResolver(resolver));
            return project;
        }

        /// <inheritdoc/>
        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new SA1628CodeFixProvider();
        }

        /// <inheritdoc/>
        protected override IEnumerable<DiagnosticAnalyzer> GetCSharpDiagnosticAnalyzers()
        {
            yield return new SA1628DocumentationTextMustBeginWithACapitalLetter();
        }
    }
}
