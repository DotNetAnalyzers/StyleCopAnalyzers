// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.Test.DocumentationRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.DocumentationRules;
    using StyleCop.Analyzers.Test.Verifiers;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.CustomDiagnosticVerifier<StyleCop.Analyzers.DocumentationRules.SA1651DoNotUsePlaceholderElements>;

    /// <summary>
    /// This class contains unit tests for <see cref="SA1651DoNotUsePlaceholderElements"/>.
    /// </summary>
    public class SA1651UnitTests
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

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            DiagnosticResult expected = Diagnostic().WithLocation(3, 9);
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
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

            DiagnosticResult expected = Diagnostic().WithLocation(3, 9);
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
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

            DiagnosticResult expected = Diagnostic().WithLocation(4, 9);
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
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

            DiagnosticResult expected = Diagnostic().WithLocation(4, 17);
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
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

            DiagnosticResult expected = Diagnostic().WithLocation(8, 15);
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
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
                Diagnostic().WithLocation(3, 9),
                Diagnostic().WithLocation(4, 18),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
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

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            var expected = Diagnostic().WithLocation(2, 5);
            await VerifyCSharpFixAsync(testCode, expected, testCode, CancellationToken.None).ConfigureAwait(false);
        }

        private static Task VerifyCSharpDiagnosticAsync(string source, DiagnosticResult[] expected, CancellationToken cancellationToken)
        {
            var test = CreateTest(expected);
            test.TestCode = source;

            return test.RunAsync(cancellationToken);
        }

        private static Task VerifyCSharpFixAsync(string source, DiagnosticResult expected, string fixedSource, CancellationToken cancellationToken)
            => VerifyCSharpFixAsync(source, new[] { expected }, fixedSource, cancellationToken);

        private static Task VerifyCSharpFixAsync(string source, DiagnosticResult[] expected, string fixedSource, CancellationToken cancellationToken)
        {
            var test = CreateTest(expected);
            test.TestCode = source;
            test.FixedCode = fixedSource;

            return test.RunAsync(cancellationToken);
        }

        private static StyleCopCodeFixVerifier<SA1651DoNotUsePlaceholderElements, SA1651CodeFixProvider>.CSharpTest CreateTest(DiagnosticResult[] expected)
        {
            string contentSummaryWithoutPlaceHolder = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<TestClass>
  <summary>
  This is a test class.
  </summary>
</TestClass>
";
            string contentSummaryWithPlaceHolder = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<TestClass>
  <summary>
  <placeholder>This is a test class.</placeholder>
  </summary>
</TestClass>
";

            var test = new StyleCopCodeFixVerifier<SA1651DoNotUsePlaceholderElements, SA1651CodeFixProvider>.CSharpTest
            {
                XmlReferences =
                {
                    { "SummaryWithoutPlaceHolder.xml", contentSummaryWithoutPlaceHolder },
                    { "SummaryWithPlaceHolder.xml", contentSummaryWithPlaceHolder },
                },
            };

            test.ExpectedDiagnostics.AddRange(expected);
            return test;
        }
    }
}
