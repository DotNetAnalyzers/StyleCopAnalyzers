// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.DocumentationRules
{
    using System.Collections.Generic;
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
    /// This class contains unit tests for <see cref="SA1643DestructorSummaryDocumentationMustBeginWithStandardText"/>-
    /// </summary>
    [UseCulture("en-US")]
    public class SA1643UnitTests : CodeFixVerifier
    {
        [Fact]
        public async Task TestNoDocumentationAsync()
        {
            var testCode = @"namespace FooNamespace
{
    public class Foo<TFoo, TBar>
    {                                                                                                 
        ~Foo()
        {

        }
    }
}";
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestDestructorCorrectDocumentationSimpleAsync()
        {
            await this.TestDestructorCorrectDocumentationSimpleImplAsync(DocumentationResources.DestructorStandardTextFirstPart, DocumentationResources.DestructorStandardTextSecondPart, false).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestDestructorCorrectDocumentationCustomizedAsync()
        {
            await this.TestDestructorCorrectDocumentationCustomizedImplAsync(DocumentationResources.DestructorStandardTextFirstPart, DocumentationResources.DestructorStandardTextSecondPart, false).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestNonPrivateConstructorCorrectDocumentationGenericSimpleAsync()
        {
            await this.TestDestructorCorrectDocumentationSimpleImplAsync(DocumentationResources.DestructorStandardTextFirstPart, DocumentationResources.DestructorStandardTextSecondPart, true).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestDestructorCorrectDocumentationGenericCustomizedAsync()
        {
            await this.TestDestructorCorrectDocumentationCustomizedImplAsync(DocumentationResources.DestructorStandardTextFirstPart, DocumentationResources.DestructorStandardTextSecondPart, true).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestDestructorMissingDocumentationAsync()
        {
            await this.TestDestructorMissingDocumentationImplAsync(DocumentationResources.DestructorStandardTextFirstPart, DocumentationResources.DestructorStandardTextSecondPart, false).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestDestructorMissingDocumentationGenericAsync()
        {
            await this.TestDestructorMissingDocumentationImplAsync(DocumentationResources.DestructorStandardTextFirstPart, DocumentationResources.DestructorStandardTextSecondPart, true).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a destructor with the correct summary text from included documentation will not produce any diagnostics.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestDestructorWithValidSummaryInIncludedDocsAsync()
        {
            var testCode = @"
public class TestClass
{
    /// <include file='ValidSummary.xml' path='/TestClass/Destructor/*'/>
    ~TestClass() { }
}
";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a destructor with the missing summary tag from included documentation will not produce any diagnostics.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestDestructorWithMissingSummaryInIncludedDocsAsync()
        {
            var testCode = @"
public class TestClass
{
    /// <include file='MissingSummary.xml' path='/TestClass/Destructor/*'/>
    ~TestClass() { }
}
";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a destructor with an empty summary tag from included documentation will produce a diagnostic and offer no codefix.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestDestructorWithEmptySummaryInIncludedDocsAsync()
        {
            var testCode = @"
public class TestClass
{
    /// <include file='EmptySummary.xml' path='/TestClass/Destructor/*'/>
    ~TestClass() { }
}
";

            var expected = this.CSharpDiagnostic().WithLocation(4, 9);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            var offeredFixes = await this.GetOfferedCSharpFixesAsync(testCode).ConfigureAwait(false);
            Assert.Empty(offeredFixes);
        }

        /// <summary>
        /// Verifies that a destructor with an invalid summary tag from included documentation will produce a diagnostic and offer no codefix.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestDestructorWithInvalidSummaryInIncludedDocsAsync()
        {
            var testCode = @"
public class TestClass
{
    /// <include file='InvalidSummary.xml' path='/TestClass/Destructor/*'/>
    ~TestClass() { }
}
";

            var expected = this.CSharpDiagnostic().WithLocation(4, 9);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            var offeredFixes = await this.GetOfferedCSharpFixesAsync(testCode).ConfigureAwait(false);
            Assert.Empty(offeredFixes);
        }

        /// <summary>
        /// Verify that the codefix will work properly with Visual Studio generated documentation headers.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestWithDefaultVisualStudioGenerationDocumentationAsync()
        {
            var testCode = @"
public class TestClass
{
    /// <summary>
    /// 
    /// </summary>
    ~TestClass() { }
}
";

            var fixedCode = @"
public class TestClass
{
    /// <summary>
    /// Finalizes an instance of the <see cref=""TestClass""/> class.
    /// </summary>
    ~TestClass() { }
}
";

            var expected = this.CSharpDiagnostic().WithLocation(4, 9);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verify that the codefix will work properly when there are multiple empty lines.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestWithMultipleEmptyLinesAsync()
        {
            var testCode = @"
public class TestClass
{
    /// <summary>
    /// 
    /// </summary>
    ~TestClass() { }
}
";

            var fixedCode = @"
public class TestClass
{
    /// <summary>
    /// Finalizes an instance of the <see cref=""TestClass""/> class.
    /// </summary>
    ~TestClass() { }
}
";

            var expected = this.CSharpDiagnostic().WithLocation(4, 9);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        protected override Project ApplyCompilationOptions(Project project)
        {
            var resolver = new TestXmlReferenceResolver();

            string contentValidSummary = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<TestClass>
  <Destructor>
    <summary>Finalizes an instance of the <see cref=""TestClass""/> class.</summary>
  </Destructor>
</TestClass>
";
            resolver.XmlReferences.Add("ValidSummary.xml", contentValidSummary);

            string contentMissingSummary = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<TestClass>
  <Destructor>
  </Destructor>
</TestClass>
";
            resolver.XmlReferences.Add("MissingSummary.xml", contentMissingSummary);

            string contentEmptySummary = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<TestClass>
  <Destructor>
    <summary></summary>
  </Destructor>
</TestClass>
";
            resolver.XmlReferences.Add("EmptySummary.xml", contentEmptySummary);

            string contentInvalidSummary = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<TestClass>
  <Destructor>
    <summary>Creates the <see cref=""TestClass""/> class.</summary>
  </Destructor>
</TestClass>
";
            resolver.XmlReferences.Add("InvalidSummary.xml", contentInvalidSummary);

            project = base.ApplyCompilationOptions(project);
            project = project.WithCompilationOptions(project.CompilationOptions.WithXmlReferenceResolver(resolver));
            return project;
        }

        protected override IEnumerable<DiagnosticAnalyzer> GetCSharpDiagnosticAnalyzers()
        {
            yield return new SA1643DestructorSummaryDocumentationMustBeginWithStandardText();
        }

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new SA1642SA1643CodeFixProvider();
        }

        [Fact]
        private async Task TestEmptyDestructorAsync()
        {
            var testCode = @"namespace FooNamespace
{
    public class Foo<TFoo, TBar>
    {
        /// 
        /// 
        /// 
        ~Foo()
        {

        }
    }
}";
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        private async Task TestDestructorCorrectDocumentationAsync(string part1, string part2, string part3, bool generic)
        {
            // First test it all on one line
            var testCode = @"namespace FooNamespace
{{
    public class Foo{0}
    {{
        /// <summary>
        /// {2}<see cref=""Foo{1}""/>{3}{4}
        /// </summary>
        ~Foo()
        {{

        }}
    }}
}}";

            await this.VerifyCSharpDiagnosticAsync(string.Format(testCode, generic ? "<T1, T2>" : string.Empty, generic ? "{T1, T2}" : string.Empty, part1, part2, part3), EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);

            // Then test splitting after the <see> element
            testCode = @"namespace FooNamespace
{{
    public class Foo{0}
    {{
        /// <summary>
        /// {2}<see cref=""Foo{1}""/>
        /// {3}{4}
        /// </summary>
        ~Foo()
        {{

        }}
    }}
}}";

            await this.VerifyCSharpDiagnosticAsync(string.Format(testCode, generic ? "<T1, T2>" : string.Empty, generic ? "{T1, T2}" : string.Empty, part1, part2, part3), EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);

            // Then test splitting before the <see> element
            testCode = @"namespace FooNamespace
{{
    public class Foo{0}
    {{
        /// <summary>
        /// {2}
        /// <see cref=""Foo{1}""/>{3}{4}
        /// </summary>
        Foo()
        {{

        }}
    }}
}}";

            await this.VerifyCSharpDiagnosticAsync(string.Format(testCode, generic ? "<T1, T2>" : string.Empty, generic ? "{T1, T2}" : string.Empty, part1, part2, part3), EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        private async Task TestDestructorCorrectDocumentationSimpleImplAsync(string part1, string part2, bool generic)
        {
            await this.TestDestructorCorrectDocumentationAsync(part1, part2, ".", generic).ConfigureAwait(false);
        }

        private async Task TestDestructorCorrectDocumentationCustomizedImplAsync(string part1, string part2, bool generic)
        {
            await this.TestDestructorCorrectDocumentationAsync(part1, part2, " with A and B.", generic).ConfigureAwait(false);
        }

        private async Task TestDestructorMissingDocumentationImplAsync(string part1, string part2, bool generic)
        {
            var testCode = @"namespace FooNamespace
{{
    public class Foo{0}
    {{
        /// <summary>
        /// </summary>
        ~Foo()
        {{

        }}
    }}
}}";
            testCode = string.Format(testCode, generic ? "<T1, T2>" : string.Empty);

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(5, 13);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);

            var fixedCode = @"namespace FooNamespace
{{
    public class Foo{0}
    {{
        /// <summary>
        /// {2}<see cref=""Foo{1}""/>{3}{4}
        /// </summary>
        ~Foo()
        {{

        }}
    }}
}}";

            string part3 = part2.EndsWith(".") ? string.Empty : ".";
            fixedCode = string.Format(fixedCode, generic ? "<T1, T2>" : string.Empty, generic ? "{T1, T2}" : string.Empty, part1, part2, part3);
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
        }
    }
}
