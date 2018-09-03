// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.DocumentationRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.DocumentationRules;
    using StyleCop.Analyzers.Test.Helpers;
    using StyleCop.Analyzers.Test.Verifiers;
    using TestHelper;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.CustomDiagnosticVerifier<StyleCop.Analyzers.DocumentationRules.SA1643DestructorSummaryDocumentationMustBeginWithStandardText>;

    /// <summary>
    /// This class contains unit tests for <see cref="SA1643DestructorSummaryDocumentationMustBeginWithStandardText"/>.
    /// </summary>
    [UseCulture("en-US")]
    public class SA1643UnitTests
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
            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestDestructorCorrectDocumentationSimpleAsync()
        {
            await TestDestructorCorrectDocumentationSimpleImplAsync(DocumentationResources.DestructorStandardTextFirstPart, DocumentationResources.DestructorStandardTextSecondPart, false).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestDestructorCorrectDocumentationCustomizedAsync()
        {
            await TestDestructorCorrectDocumentationCustomizedImplAsync(DocumentationResources.DestructorStandardTextFirstPart, DocumentationResources.DestructorStandardTextSecondPart, false).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestNonPrivateConstructorCorrectDocumentationGenericSimpleAsync()
        {
            await TestDestructorCorrectDocumentationSimpleImplAsync(DocumentationResources.DestructorStandardTextFirstPart, DocumentationResources.DestructorStandardTextSecondPart, true).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestDestructorCorrectDocumentationGenericCustomizedAsync()
        {
            await TestDestructorCorrectDocumentationCustomizedImplAsync(DocumentationResources.DestructorStandardTextFirstPart, DocumentationResources.DestructorStandardTextSecondPart, true).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestDestructorMissingDocumentationAsync()
        {
            await TestDestructorMissingDocumentationImplAsync(DocumentationResources.DestructorStandardTextFirstPart, DocumentationResources.DestructorStandardTextSecondPart, false).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestDestructorMissingDocumentationGenericAsync()
        {
            await TestDestructorMissingDocumentationImplAsync(DocumentationResources.DestructorStandardTextFirstPart, DocumentationResources.DestructorStandardTextSecondPart, true).ConfigureAwait(false);
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

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            var expected = Diagnostic().WithLocation(4, 9);
            await VerifyCSharpFixAsync(testCode, expected, testCode, CancellationToken.None).ConfigureAwait(false);
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

            var expected = Diagnostic().WithLocation(4, 9);
            await VerifyCSharpFixAsync(testCode, expected, testCode, CancellationToken.None).ConfigureAwait(false);
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

            var expected = Diagnostic().WithLocation(4, 9);
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
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

            var expected = Diagnostic().WithLocation(4, 9);
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestEmptyDestructorAsync()
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
            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        private static async Task TestDestructorCorrectDocumentationAsync(string part1, string part2, string part3, bool generic)
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

            await VerifyCSharpDiagnosticAsync(string.Format(testCode, generic ? "<T1, T2>" : string.Empty, generic ? "{T1, T2}" : string.Empty, part1, part2, part3), DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);

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

            await VerifyCSharpDiagnosticAsync(string.Format(testCode, generic ? "<T1, T2>" : string.Empty, generic ? "{T1, T2}" : string.Empty, part1, part2, part3), DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);

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

            await VerifyCSharpDiagnosticAsync(string.Format(testCode, generic ? "<T1, T2>" : string.Empty, generic ? "{T1, T2}" : string.Empty, part1, part2, part3), DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        private static async Task TestDestructorCorrectDocumentationSimpleImplAsync(string part1, string part2, bool generic)
        {
            await TestDestructorCorrectDocumentationAsync(part1, part2, ".", generic).ConfigureAwait(false);
        }

        private static async Task TestDestructorCorrectDocumentationCustomizedImplAsync(string part1, string part2, bool generic)
        {
            await TestDestructorCorrectDocumentationAsync(part1, part2, " with A and B.", generic).ConfigureAwait(false);
        }

        private static async Task TestDestructorMissingDocumentationImplAsync(string part1, string part2, bool generic)
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

            DiagnosticResult expected = Diagnostic().WithLocation(5, 13);

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
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
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

        private static StyleCopCodeFixVerifier<SA1643DestructorSummaryDocumentationMustBeginWithStandardText, SA1642SA1643CodeFixProvider>.CSharpTest CreateTest(DiagnosticResult[] expected)
        {
            string contentValidSummary = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<TestClass>
  <Destructor>
    <summary>Finalizes an instance of the <see cref=""TestClass""/> class.</summary>
  </Destructor>
</TestClass>
";
            string contentMissingSummary = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<TestClass>
  <Destructor>
  </Destructor>
</TestClass>
";
            string contentEmptySummary = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<TestClass>
  <Destructor>
    <summary></summary>
  </Destructor>
</TestClass>
";
            string contentInvalidSummary = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<TestClass>
  <Destructor>
    <summary>Creates the <see cref=""TestClass""/> class.</summary>
  </Destructor>
</TestClass>
";

            var test = new StyleCopCodeFixVerifier<SA1643DestructorSummaryDocumentationMustBeginWithStandardText, SA1642SA1643CodeFixProvider>.CSharpTest
            {
                XmlReferences =
                {
                    { "ValidSummary.xml", contentValidSummary },
                    { "MissingSummary.xml", contentMissingSummary },
                    { "EmptySummary.xml", contentEmptySummary },
                    { "InvalidSummary.xml", contentInvalidSummary },
                },
            };

            test.ExpectedDiagnostics.AddRange(expected);
            return test;
        }
    }
}
