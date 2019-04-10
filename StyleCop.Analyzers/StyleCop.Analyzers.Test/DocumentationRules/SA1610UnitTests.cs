// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.DocumentationRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.DocumentationRules;
    using StyleCop.Analyzers.Test.Verifiers;
    using TestHelper;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.CustomDiagnosticVerifier<StyleCop.Analyzers.DocumentationRules.SA1610PropertyDocumentationMustHaveValueText>;

    /// <summary>
    /// This class contains unit tests for <see cref="SA1610PropertyDocumentationMustHaveValueText"/>.
    /// </summary>
    public class SA1610UnitTests
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
            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            DiagnosticResult expected = Diagnostic().WithLocation(10, 22);
            await VerifyCSharpFixAsync(testCode, expected, testCode, offerEmptyFixer: true, CancellationToken.None).ConfigureAwait(false);
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

            DiagnosticResult expected = Diagnostic().WithLocation(13, 22);
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, offerEmptyFixer: true, CancellationToken.None).ConfigureAwait(false);
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

            DiagnosticResult expected = Diagnostic().WithLocation(13, 22);
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, offerEmptyFixer: false, CancellationToken.None).ConfigureAwait(false);
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

            DiagnosticResult expected = Diagnostic().WithLocation(13, 22);
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, offerEmptyFixer: false, CancellationToken.None).ConfigureAwait(false);
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

            DiagnosticResult expected = Diagnostic().WithLocation(13, 22);
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, offerEmptyFixer: false, CancellationToken.None).ConfigureAwait(false);
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

            DiagnosticResult expected = Diagnostic().WithLocation(13, 22);
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, offerEmptyFixer: false, CancellationToken.None).ConfigureAwait(false);
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

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
            var expected = Diagnostic().WithLocation(5, 16);
            await VerifyCSharpFixAsync(testCode, expected, testCode, offerEmptyFixer: false, CancellationToken.None).ConfigureAwait(false);
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

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        private static Task VerifyCSharpDiagnosticAsync(string source, DiagnosticResult[] expected, CancellationToken cancellationToken)
            => VerifyCSharpFixAsync(source, expected, fixedSource: null, offerEmptyFixer: false, cancellationToken);

        private static Task VerifyCSharpFixAsync(string source, DiagnosticResult expected, string fixedSource, bool offerEmptyFixer, CancellationToken cancellationToken)
            => VerifyCSharpFixAsync(source, new[] { expected }, fixedSource, offerEmptyFixer, cancellationToken);

        private static Task VerifyCSharpFixAsync(string source, DiagnosticResult[] expected, string fixedSource, bool offerEmptyFixer, CancellationToken cancellationToken)
        {
            string contentWithValue = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<ClassName>
  <Property>
    <summary>Foo</summary>
    <value>Bar</value>
  </Property>
</ClassName>
";
            string contentWithoutValue = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<ClassName>
  <Property>
    <summary>Foo</summary>
  </Property>
</ClassName>
";
            string contentWithEmptyValue = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<ClassName>
  <Property>
    <summary>Foo</summary>
    <value>  </value>
  </Property>
</ClassName>
";
            string contentWithInheritdocValue = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<ClassName>
  <Property>
    <inheritdoc/>
  </Property>
</ClassName>
";

            var test = new StyleCopCodeFixVerifier<SA1610PropertyDocumentationMustHaveValueText, SA1609SA1610CodeFixProvider>.CSharpTest
            {
                TestCode = source,
                FixedCode = fixedSource,
                XmlReferences =
                {
                    { "PropertyWithValue.xml", contentWithValue },
                    { "PropertyWithoutValue.xml", contentWithoutValue },
                    { "PropertyWithEmptyValue.xml", contentWithEmptyValue },
                    { "PropertyWithInheritdoc.xml", contentWithInheritdocValue },
                },
            };

            if (source == fixedSource)
            {
                test.FixedState.InheritanceMode = StateInheritanceMode.AutoInheritAll;
                test.FixedState.MarkupHandling = MarkupMode.Allow;
                test.BatchFixedState.InheritanceMode = StateInheritanceMode.AutoInheritAll;
                test.BatchFixedState.MarkupHandling = MarkupMode.Allow;

                if (offerEmptyFixer)
                {
                    test.NumberOfIncrementalIterations = 1;
                    test.NumberOfFixAllIterations = 1;
                }
            }

            test.ExpectedDiagnostics.AddRange(expected);

            return test.RunAsync(cancellationToken);
        }
    }
}
