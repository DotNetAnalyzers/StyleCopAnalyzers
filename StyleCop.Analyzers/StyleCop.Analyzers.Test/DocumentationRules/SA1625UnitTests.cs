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
    using TestHelper;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.CustomDiagnosticVerifier<StyleCop.Analyzers.DocumentationRules.SA1625ElementDocumentationMustNotBeCopiedAndPasted>;

    /// <summary>
    /// This class contains the unit tests for SA1625.
    /// </summary>
    public class SA1625UnitTests
    {
        public static IEnumerable<object[]> Members
        {
            get
            {
                yield return new[] { "public void Test() { }" };
                yield return new[] { "public string Test { get; set; }" };
                yield return new[] { "public string Test;" };
                yield return new[] { "public class Test { }" };
                yield return new[] { "public struct Test { }" };
                yield return new[] { "public enum Test { }" };
                yield return new[] { "public delegate void Test();" };
            }
        }

        [Theory]
        [MemberData(nameof(Members))]
        public async Task VerifyThatCorrectDocumentationDoesNotReportADiagnosticAsync(string member)
        {
            var testCode = $@"
public class TestClass
{{
    /// <summary>
    /// Some documentation.
    /// </summary>
    /// <remark>Some remark.</remark>
    {member}
}}
";
            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(Members))]
        public async Task VerifyThatCorrectDocumentationWithEmptyElementsDoesNotReportADiagnosticAsync(string member)
        {
            var testCode = $@"
public class TestClass
{{
    /// <summary>
    /// Some documentation <see cref=""TestClass""/>.
    /// </summary>
    /// <summary>
    /// Some documentation <see cref=""TestClass2""/>.
    /// </summary>
    /// <remark>Some remark.</remark>
    {member}
}}
public class TestClass2 {{ }}
";
            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(Members))]
        public async Task VerifyThatTheAnalyzerDoesNotCrashOnInheritDocAsync(string member)
        {
            var testCode = $@"
public class TestClass
{{
    /// <inheritdoc/>
    {member}
}}
";
            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(Members))]
        public async Task VerifyThatWhitespacesAreNormalizedForEmptyXmlElementsAsync(string member)
        {
            var testCode = $@"
public class TestClass
{{
    /// <summary>
    /// Some documentation <see cref=""TestClass""/>.
    /// </summary>
    /// <summary>
    /// Some documentation <see       cref  =   ""TestClass""     />.
    /// </summary>
    /// <remark>Some remark.</remark>
    {member}
}}
";
            var expected = Diagnostic().WithLocation(7, 9);
            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(Members))]
        public async Task VerifyThatDublicatedDocumentationDoesReportADiagnosticAsync(string member)
        {
            var testCode = $@"
public class TestClass
{{
    /// <summary>Some documentation.</summary>
    /// <remark>Some documentation.</remark>
    {member}
}}
";
            var expected = Diagnostic().WithLocation(5, 9);

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(Members))]
        public async Task VerifyThatAnalyzerIgnoresLeadingAndTrailingWhitespaceAsync(string member)
        {
            var testCode = $@"
public class TestClass
{{
    /// <summary>
    ///                                             Some documentation.
    ///
    ///
    /// </summary>
    /// <remark>    Some documentation.      </remark>
    {member}
}}
";
            var expected = Diagnostic().WithLocation(9, 9);
            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(Members))]
        public async Task VerifyThatAnalysisIgnoresUnusedParametersAsync(string member)
        {
            var testCode = $@"
public class TestClass
{{
    /// <summary>The parameter is not used.</summary>
    /// <remark>Documentation</remark>
    /// <remark>The parameter is not used.</remark>
    /// <remark>Documentation</remark>
    {member}
}}
";
            var expected = Diagnostic().WithLocation(7, 9);
            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(Members))]
        public async Task VerifyThatAnalysisIgnoresEmptyElementsAsync(string member)
        {
            var testCode = $@"
public class TestClass
{{
    /// <summary></summary>
    /// <remark>Documentation</remark>
    /// <remark></remark>
    /// <remark>Documentation</remark>
    {member}
}}
";
            var expected = Diagnostic().WithLocation(7, 9);
            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(Members))]
        public async Task VerifyThatCorrectDocumentationDoesNotReportADiagnosticMultiLineAsync(string member)
        {
            var testCode = $@"
public class TestClass
{{
    /** <summary>
    * Some documentation.
    * </summary>
    * <remark>Some remark.</remark>
    **/
    {member}
}}
";
            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(Members))]
        public async Task VerifyThatDublicatedDocumentationDoesReportADiagnosticMultiLineAsync(string member)
        {
            var testCode = $@"
public class TestClass
{{
    /** <summary>Some documentation.</summary>
    * <remark>Some documentation.</remark>
    **/
    {member}
}}
";
            var expected = Diagnostic().WithLocation(5, 7);

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(Members))]
        public async Task VerifyThatAnalyzerIgnoresLeadingAndTrailingWhitespaceMultiLineAsync(string member)
        {
            var testCode = $@"
public class TestClass
{{
    /** <summary>
    *                                             Some documentation.
    *
    *
    * </summary>
    * <remark>    Some documentation.      </remark>
    **/
    {member}
}}
";
            var expected = Diagnostic().WithLocation(9, 7);
            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(Members))]
        public async Task VerifyThatAnalysisIgnoresUnusedParametersMultiLineAsync(string member)
        {
            var testCode = $@"
public class TestClass
{{
    /** <summary>The parameter is not used.</summary>
    * <remark>Documentation</remark>
    * <remark>The parameter is not used.</remark>
    * <remark>Documentation</remark>
    **/
    {member}
}}
";
            var expected = Diagnostic().WithLocation(7, 7);
            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(Members))]
        public async Task VerifyThatAnalysisIgnoresEmptyElementsMultiLineAsync(string member)
        {
            var testCode = $@"
public class TestClass
{{
    /** <summary></summary>
    * <remark>Documentation</remark>
    * <remark></remark>
    * <remark>Documentation</remark>
    **/
    {member}
}}
";
            var expected = Diagnostic().WithLocation(7, 7);
            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task VerifyThatCorrectIncludedDocumentationDoesNotReportADiagnosticAsync()
        {
            var testCode = $@"
public class TestClass
{{
    /// <include file='Correct.xml' path='/TestClass/Test/*' />
    public void Test() {{ }}
}}
";
            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task VerifyThatCorrectIncludedDocumentationWithEmptyElementsDoesNotReportADiagnosticAsync()
        {
            var testCode = $@"
public class TestClass
{{
    /// <include file='CorrectWithEmptyElements.xml' path='/TestClass/Test/*' />
    public void Test() {{ }}
}}
public class TestClass2 {{ }}
";
            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task VerifyThatTheAnalyzerDoesNotCrashOnIncludedInheritDocAsync()
        {
            var testCode = $@"
public class TestClass
{{
    /// <include file='Inherited.xml' path='/TestClass/Test/*' />
    public void Test() {{ }}
}}
";
            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task VerifyThatIncludedWhitespacesAreNormalizedForEmptyXmlElementsAsync()
        {
            var testCode = $@"
public class TestClass
{{
    /// <include file='BadWithNormalization.xml' path='/TestClass/Test/*' />
    public void Test() {{ }}
}}
";
            var expected = Diagnostic().WithLocation(5, 17);
            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task VerifyThatIncludedDuplicatedDocumentationDoesReportADiagnosticAsync()
        {
            var testCode = $@"
public class TestClass
{{
    /// <include file='BadWithDuplicates.xml' path='/TestClass/Test/*' />
    public void Test() {{ }}
}}
";
            var expected = Diagnostic().WithLocation(5, 17);

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task VerifyThatAnalyzerIgnoresLeadingAndTrailingWhitespaceInIncludedDocumentationAsync()
        {
            var testCode = $@"
public class TestClass
{{
    /// <include file='BadWithDuplicates2.xml' path='/TestClass/Test/*' />
    public void Test() {{ }}
}}
";
            var expected = Diagnostic().WithLocation(5, 17);
            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task VerifyThatAnalysisIgnoresUnusedParametersInIncludedDocumentationAsync()
        {
            var testCode = $@"
public class TestClass
{{
    /// <include file='WithIgnoredParameters.xml' path='/TestClass/Test/*' />
    public void Test() {{ }}
}}
";
            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task VerifyThatAnalysisIgnoresEmptyElementsInIncludedDocumentationAsync()
        {
            var testCode = $@"
public class TestClass
{{
    /// <include file='CorrectEmpty.xml' path='/TestClass/Test/*' />
    public void Test() {{ }}
}}
";
            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        private static Task VerifyCSharpDiagnosticAsync(string source, DiagnosticResult expected, CancellationToken cancellationToken)
            => VerifyCSharpDiagnosticAsync(source, new[] { expected }, cancellationToken);

        private static Task VerifyCSharpDiagnosticAsync(string source, DiagnosticResult[] expected, CancellationToken cancellationToken)
        {
            var test = CreateTest(expected);
            test.TestCode = source;

            return test.RunAsync(cancellationToken);
        }

        private static StyleCopDiagnosticVerifier<SA1625ElementDocumentationMustNotBeCopiedAndPasted>.CSharpTest CreateTest(DiagnosticResult[] expected)
        {
            string correctDocumentation = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<TestClass>
    <Test>
        <summary>
            Some documentation.
        </summary>
        <remark>Some remark.</remark>
    </Test>
</TestClass>
";
            string correctWithEmptyReferenceElements = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<TestClass>
    <Test>
        <summary>
            Some documentation <see cref=""TestClass""/>.
        </summary>
        <summary>
            Some documentation <see cref=""TestClass2""/>.
        </summary>
        <remark>Some remark.</remark>
    </Test>
</TestClass>
";
            string correctWithEmptyElements = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<TestClass>
    <Test>
        <summary></summary>
        <remark></remark>
    </Test>
</TestClass>
";
            string inherited = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<TestClass>
    <Test>
        <inheritdoc />
    </Test>
</TestClass>
";
            string badWithNormalization = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<TestClass>
    <Test>
        <summary>
            Some documentation <see cref=""TestClass""/>.
        </summary>
        <summary>
            Some documentation <see       cref  =   ""TestClass""     />.
        </summary>
        <remark>Some remark.</remark>
    </Test>
</TestClass>
";
            string badWithDuplicates = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<TestClass>
    <Test>
        <summary>Some documentation.</summary>
        <remark>Some documentation.</remark>
    </Test>
</TestClass>
";
            string badWithDuplicates2 = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<TestClass>
    <Test>
        <summary>
                                                     Some documentation.
        
        
        </summary>
        <remark>    Some documentation.      </remark>
    </Test>
</TestClass>
";
            string withIgnoredParameters = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<TestClass>
    <Test>
        <summary>The parameter is not used.</summary>
        <remark>The parameter is not used.</remark>
    </Test>
</TestClass>
";

            var test = new StyleCopDiagnosticVerifier<SA1625ElementDocumentationMustNotBeCopiedAndPasted>.CSharpTest
            {
                XmlReferences =
                {
                    { "Correct.xml", correctDocumentation },
                    { "CorrectWithEmptyElements.xml", correctWithEmptyReferenceElements },
                    { "CorrectEmpty.xml", correctWithEmptyElements },
                    { "Inherited.xml", inherited },
                    { "BadWithNormalization.xml", badWithNormalization },
                    { "BadWithDuplicates.xml", badWithDuplicates },
                    { "BadWithDuplicates2.xml", badWithDuplicates2 },
                    { "WithIgnoredParameters.xml", withIgnoredParameters },
                },
            };

            test.ExpectedDiagnostics.AddRange(expected);
            return test;
        }
    }
}
