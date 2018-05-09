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
    /// This class contains unit tests for <see cref="SA1627DocumentationTextMustNotBeEmpty"/>.
    /// </summary>
    public class SA1627UnitTests : DiagnosticVerifier
    {
        public static IEnumerable<object[]> Elements
        {
            get
            {
                yield return new[] { "remarks" };
                yield return new[] { "example" };
                yield return new[] { "permission" };
                yield return new[] { "exception" };
            }
        }

        /// <summary>
        /// Checks an element with a blank value gives an error.
        /// </summary>
        /// <param name="element">Element to check</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [MemberData(nameof(Elements))]
        public async Task TestMemberWithBlankElementAsync(string element)
        {
            var testCode = @"
/// <summary>
/// Class name.
/// </summary>
public class ClassName
{
    /// <summary>
    /// Join together two strings.
    /// </summary>
    ///<param name=""first"">First string.</param>
    ///<param name=""second"">Second string.</param>
    /// <$$>  </$$>
    public string JoinStrings(string first, string second) { return first + second; }
}";
            var expectedDiagnostic = this.CSharpDiagnostic().WithLocation(12, 9).WithArguments(element);
            await this.VerifyCSharpDiagnosticAsync(testCode.Replace("$$", element), expectedDiagnostic, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Checks an element with a multiple blank values give multiple errors.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestMemberWithMultipleBlankElementsAsync()
        {
            var testCode = @"
/// <summary>
/// Class name.
/// </summary>
public class ClassName
{
    /// <summary>
    /// Join together two strings.
    /// </summary>
    ///<param name=""first"">First string.</param>
    ///<param name=""second"">Second string.</param>
    /// <remarks>Single line remark.</remarks>
    /// <example></example>
    /// <exception>
    ///
    /// </exception>
    /// <permission>
    /// Multi line notes.
    /// Multi line notes.
    /// </permission>
    public string JoinStrings(string first, string second) { return first + second; }
}";
            var expectedDiagnostics = new[]
            {
                this.CSharpDiagnostic().WithLocation(13, 9).WithArguments("example"),
                this.CSharpDiagnostic().WithLocation(14, 9).WithArguments("exception"),
            };
            await this.VerifyCSharpDiagnosticAsync(testCode, expectedDiagnostics, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Checks an element with an empty element gives an error.
        /// </summary>
        /// <param name="element">Element to check</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [MemberData(nameof(Elements))]
        public async Task TestMemberWithEmptyElementAsync(string element)
        {
            var testCode = @"
/// <summary>
/// Class name.
/// </summary>
public class ClassName
{
    /// <summary>
    /// Join together two strings.
    /// </summary>
    ///<param name=""first"">First string.</param>
    ///<param name=""second"">Second string.</param>
    /// <$$ />
    public string JoinStrings(string first, string second) { return first + second; }
}";
            var expectedDiagnostic = this.CSharpDiagnostic().WithLocation(12, 9).WithArguments(element);
            await this.VerifyCSharpDiagnosticAsync(testCode.Replace("$$", element), expectedDiagnostic, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Checks an element with non blank text does not give an error.
        /// </summary>
        /// <param name="element">Element to check</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [MemberData(nameof(Elements))]
        public async Task TestMemberWithValidElementAsync(string element)
        {
            var testCode = @"
/// <summary>
/// Class name.
/// </summary>
public class ClassName
{
    /// <summary>
    /// Join together two strings.
    /// </summary>
    ///<param name=""first"">First string.</param>
    ///<param name=""second"">Second string.</param>
    /// <$$>Not blank element.</$$>
    public string JoinStrings(string first, string second) { return first + second; }
}";
            await this.VerifyCSharpDiagnosticAsync(testCode.Replace("$$", element), EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Checks an element with a custom (unsupported) empty element does not give an error.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestMemberWithCustomElementAsync()
        {
            var testCode = @"
/// <summary>
/// Class name.
/// </summary>
public class ClassName
{
    /// <summary>
    /// Join together two strings.
    /// </summary>
    /// <param name=""first"">First string.</param>
    /// <param name=""second"">Second string.</param>
    /// <custom1/>
    public string JoinStrings(string first, string second) { return first + second; }
}";
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that an orphaned include documentation statement does not produce diagnostics.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestOrphanedIncludedDocumentationAsync()
        {
            var testCode = @"
/// <include file='AllFilled.xml' path='/TestClass/TestMethod/*'/>
";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that member with valid elements in the included documentation does not produce diagnostics.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestMemberWithValidElementsInIncludedDocumentationAsync()
        {
            var testCode = @"
/// <summary>Test class</summary>
public class TestClass
{
    /// <include file='AllFilled.xml' path='/TestClass/TestMethod/*'/>
    public void TestMethod(int first) { }
}
";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that member with an &lt;inheritdoc&gt; tag in the included documentation does not produce diagnostics.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestMemberWithInheritDocInIncludedDocumentationAsync()
        {
            var testCode = @"
/// <summary>Test class</summary>
public class TestClass
{
    /// <include file='InheritDoc.xml' path='/TestClass/TestMethod/*'/>
    public void TestMethod(int first) { }
}
";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that member with a single invalid elements in the included documentation does produce the expected diagnostics.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestMemberWithOneInvalidElementsInIncludedDocumentationAsync()
        {
            var testCode = @"
/// <summary>Test class</summary>
public class TestClass
{
    /// <include file='AllButOneFilled.xml' path='/TestClass/TestMethod/*'/>
    public void TestMethod(int first) { }
}
";
            var expected = this.CSharpDiagnostic().WithLocation(5, 9).WithArguments("permission");
            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that member with multiple invalid elements in the included documentation does produce the expected diagnostics.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestMemberWithMultipleInvalidElementsInIncludedDocumentationAsync()
        {
            var testCode = @"
/// <summary>Test class</summary>
public class TestClass
{
    /// <include file='NoneFilled.xml' path='/TestClass/TestMethod/*'/>
    public void TestMethod(int first) { }
}
";

            DiagnosticResult[] expected =
            {
                this.CSharpDiagnostic().WithLocation(5, 9).WithArguments("remarks"),
                this.CSharpDiagnostic().WithLocation(5, 9).WithArguments("example"),
                this.CSharpDiagnostic().WithLocation(5, 9).WithArguments("permission"),
                this.CSharpDiagnostic().WithLocation(5, 9).WithArguments("exception"),
            };
            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        protected override Project ApplyCompilationOptions(Project project)
        {
            var resolver = new TestXmlReferenceResolver();

            string contentAllFilled = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<TestClass>
  <TestMethod>
    <summary>This is a test method.</summary>
    <param name=""first"">The first parameter.</param>
    <remarks>Test remarks</remarks>
    <example>Test example</example>
    <permission>Test permission</permission>
    <exception>Test exception</exception>
  </TestMethod>
</TestClass>
";
            resolver.XmlReferences.Add("AllFilled.xml", contentAllFilled);

            string contentInheritDoc = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<TestClass>
  <TestMethod>
    <inheritdoc/>
  </TestMethod>
</TestClass>
";
            resolver.XmlReferences.Add("InheritDoc.xml", contentInheritDoc);

            string contentAllButOneFilled = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<TestClass>
  <TestMethod>
    <summary>This is a test method.</summary>
    <param name=""first"">The first parameter.</param>
    <remarks>Test remarks</remarks>
    <example>Test example</example>
    <permission></permission>
    <exception>Test exception</exception>
  </TestMethod>
</TestClass>
";
            resolver.XmlReferences.Add("AllButOneFilled.xml", contentAllButOneFilled);

            string contentNoneFilled = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<TestClass>
  <TestMethod>
    <summary>This is a test method.</summary>
    <param name=""first"">The first parameter.</param>
    <remarks></remarks>
    <example></example>
    <permission></permission>
    <exception></exception>
  </TestMethod>
</TestClass>
";
            resolver.XmlReferences.Add("NoneFilled.xml", contentNoneFilled);

            project = base.ApplyCompilationOptions(project);
            project = project.WithCompilationOptions(project.CompilationOptions.WithXmlReferenceResolver(resolver));
            return project;
        }

        protected override IEnumerable<DiagnosticAnalyzer> GetCSharpDiagnosticAnalyzers()
        {
            yield return new SA1627DocumentationTextMustNotBeEmpty();
        }
    }
}
