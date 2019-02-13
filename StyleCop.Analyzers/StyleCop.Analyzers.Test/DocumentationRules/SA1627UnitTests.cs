// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.DocumentationRules
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.DocumentationRules;
    using StyleCop.Analyzers.Test.Verifiers;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.CustomDiagnosticVerifier<Analyzers.DocumentationRules.SA1627DocumentationTextMustNotBeEmpty>;

    /// <summary>
    /// This class contains unit tests for <see cref="SA1627DocumentationTextMustNotBeEmpty"/>.
    /// </summary>
    public class SA1627UnitTests
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
        /// <param name="element">Element to check.</param>
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
            var expectedDiagnostic = Diagnostic().WithLocation(12, 9).WithArguments(element);
            await VerifyCSharpDiagnosticAsync(testCode.Replace("$$", element), expectedDiagnostic, CancellationToken.None).ConfigureAwait(false);
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
                Diagnostic().WithLocation(13, 9).WithArguments("example"),
                Diagnostic().WithLocation(14, 9).WithArguments("exception"),
            };
            await VerifyCSharpDiagnosticAsync(testCode, expectedDiagnostics, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Checks an element with an empty element gives an error.
        /// </summary>
        /// <param name="element">Element to check.</param>
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
            var expectedDiagnostic = Diagnostic().WithLocation(12, 9).WithArguments(element);
            await VerifyCSharpDiagnosticAsync(testCode.Replace("$$", element), expectedDiagnostic, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Checks an element with non blank text does not give an error.
        /// </summary>
        /// <param name="element">Element to check.</param>
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
            await VerifyCSharpDiagnosticAsync(testCode.Replace("$$", element), DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
            var expected = Diagnostic().WithLocation(5, 9).WithArguments("permission");
            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
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
                Diagnostic().WithLocation(5, 9).WithArguments("remarks"),
                Diagnostic().WithLocation(5, 9).WithArguments("example"),
                Diagnostic().WithLocation(5, 9).WithArguments("permission"),
                Diagnostic().WithLocation(5, 9).WithArguments("exception"),
            };
            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        private static Task VerifyCSharpDiagnosticAsync(string source, DiagnosticResult expected, CancellationToken cancellationToken)
            => VerifyCSharpDiagnosticAsync(source, new[] { expected }, cancellationToken);

        private static Task VerifyCSharpDiagnosticAsync(string source, DiagnosticResult[] expected, CancellationToken cancellationToken)
        {
            var test = CreateTest(expected);
            test.TestCode = source;

            return test.RunAsync(cancellationToken);
        }

        private static StyleCopDiagnosticVerifier<SA1627DocumentationTextMustNotBeEmpty>.CSharpTest CreateTest(DiagnosticResult[] expected)
        {
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
            string contentInheritDoc = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<TestClass>
  <TestMethod>
    <inheritdoc/>
  </TestMethod>
</TestClass>
";
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

            var test = new StyleCopDiagnosticVerifier<SA1627DocumentationTextMustNotBeEmpty>.CSharpTest
            {
                XmlReferences =
                {
                    { "AllFilled.xml", contentAllFilled },
                    { "InheritDoc.xml", contentInheritDoc },
                    { "AllButOneFilled.xml", contentAllButOneFilled },
                    { "NoneFilled.xml", contentNoneFilled },
                },
            };

            test.ExpectedDiagnostics.AddRange(expected);
            return test;
        }
    }
}
