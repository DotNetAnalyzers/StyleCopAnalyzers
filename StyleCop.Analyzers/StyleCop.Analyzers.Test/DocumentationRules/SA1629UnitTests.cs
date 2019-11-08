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
    using static StyleCop.Analyzers.Test.Verifiers.CustomDiagnosticVerifier<StyleCop.Analyzers.DocumentationRules.SA1629DocumentationTextMustEndWithAPeriod>;

    /// <summary>
    /// The class contains unit tests for <see cref="SA1629DocumentationTextMustEndWithAPeriod"/>.
    /// </summary>
    public class SA1629UnitTests
    {
        [Fact]
        public async Task TestDocumentationAsync()
        {
            var testCode = @"
/// <summary>
/// Test class
/// </summary>
public class TestClass
{
    /// <summary>
    /// Gets or sets test property #1
    /// </summary>
    /// <value>
    /// Dummy integer
    /// </value>
    public int TestProperty1 { get; set; }

    /// <summary>Gets or sets test property #2</summary>
    /// <value>Dummy integer</value>
    public int TestProperty2 { get; set; }

    /// <summary>
    /// Test method #1
    /// </summary>
    /// <typeparam name=""T"">
    /// Template type
    /// </typeparam>
    /// <param name=""arg1"">
    /// First argument
    /// </param>
    /// <returns>
    /// Some value
    /// </returns>
    /// <remarks>
    /// Random remark
    /// </remarks>
    /// <example>
    /// Random example
    /// </example>
    /// <exception cref=""System.Exception"">
    /// Exception description
    /// </exception>
    /// <permission cref=""System.Security.PermissionSet"">
    /// Everyone can access this method
    /// </permission>
    public int TestMethod1<T>(T arg1)
    {
        return 0;
    }

    /// <summary>Test method #2</summary>
    /// <typeparam name=""T"">Template type</typeparam>
    /// <param name=""arg1"">First argument</param>
    /// <returns>Some value</returns>
    /// <remarks>Random remark</remarks>
    /// <example>Random example</example>
    /// <exception cref=""System.Exception"">Exception description</exception>
    /// <permission cref=""System.Security.PermissionSet"">Everyone can access this method</permission>
    public int TestMethod2<T>(T arg1)
    {
        return 0;
    }
}
";

            var fixedTestCode = @"
/// <summary>
/// Test class.
/// </summary>
public class TestClass
{
    /// <summary>
    /// Gets or sets test property #1.
    /// </summary>
    /// <value>
    /// Dummy integer.
    /// </value>
    public int TestProperty1 { get; set; }

    /// <summary>Gets or sets test property #2.</summary>
    /// <value>Dummy integer.</value>
    public int TestProperty2 { get; set; }

    /// <summary>
    /// Test method #1.
    /// </summary>
    /// <typeparam name=""T"">
    /// Template type.
    /// </typeparam>
    /// <param name=""arg1"">
    /// First argument.
    /// </param>
    /// <returns>
    /// Some value.
    /// </returns>
    /// <remarks>
    /// Random remark.
    /// </remarks>
    /// <example>
    /// Random example.
    /// </example>
    /// <exception cref=""System.Exception"">
    /// Exception description.
    /// </exception>
    /// <permission cref=""System.Security.PermissionSet"">
    /// Everyone can access this method.
    /// </permission>
    public int TestMethod1<T>(T arg1)
    {
        return 0;
    }

    /// <summary>Test method #2.</summary>
    /// <typeparam name=""T"">Template type.</typeparam>
    /// <param name=""arg1"">First argument.</param>
    /// <returns>Some value.</returns>
    /// <remarks>Random remark.</remarks>
    /// <example>Random example.</example>
    /// <exception cref=""System.Exception"">Exception description.</exception>
    /// <permission cref=""System.Security.PermissionSet"">Everyone can access this method.</permission>
    public int TestMethod2<T>(T arg1)
    {
        return 0;
    }
}
";

            DiagnosticResult[] expectedDiagnostics =
            {
                Diagnostic().WithLocation(3, 15),
                Diagnostic().WithLocation(8, 38),
                Diagnostic().WithLocation(11, 22),
                Diagnostic().WithLocation(15, 47),
                Diagnostic().WithLocation(16, 29),
                Diagnostic().WithLocation(20, 23),
                Diagnostic().WithLocation(23, 22),
                Diagnostic().WithLocation(26, 23),
                Diagnostic().WithLocation(29, 19),
                Diagnostic().WithLocation(32, 22),
                Diagnostic().WithLocation(35, 23),
                Diagnostic().WithLocation(38, 30),
                Diagnostic().WithLocation(41, 40),
                Diagnostic().WithLocation(48, 32),
                Diagnostic().WithLocation(49, 42),
                Diagnostic().WithLocation(50, 42),
                Diagnostic().WithLocation(51, 28),
                Diagnostic().WithLocation(52, 31),
                Diagnostic().WithLocation(53, 32),
                Diagnostic().WithLocation(54, 65),
                Diagnostic().WithLocation(55, 89),
            };

            await VerifyCSharpFixAsync(testCode, expectedDiagnostics, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestAugmentedInheritedDocumentationAsync()
        {
            var testCode = @"
/// <summary>
/// Test interface.
/// </summary>
public interface ITest
{
    /// <summary>Test method.</summary>
    /// <typeparam name=""T"">Template type.</typeparam>
    /// <param name=""arg1"">First argument.</param>
    /// <returns>Some value.</returns>
    int TestMethod<T>(T arg1);
}

/// <summary>
/// Test class.
/// </summary>
public class TestClass : ITest
{
    /// <inheritdoc/>
    /// <remarks>Random remark</remarks>
    /// <example>Random example</example>
    /// <exception cref=""System.Exception"">Exception description</exception>
    /// <permission cref=""System.Security.PermissionSet"">Everyone can access this method</permission>
    public int TestMethod<T>(T arg1)
    {
        return 0;
    }
}
";

            var fixedTestCode = @"
/// <summary>
/// Test interface.
/// </summary>
public interface ITest
{
    /// <summary>Test method.</summary>
    /// <typeparam name=""T"">Template type.</typeparam>
    /// <param name=""arg1"">First argument.</param>
    /// <returns>Some value.</returns>
    int TestMethod<T>(T arg1);
}

/// <summary>
/// Test class.
/// </summary>
public class TestClass : ITest
{
    /// <inheritdoc/>
    /// <remarks>Random remark.</remarks>
    /// <example>Random example.</example>
    /// <exception cref=""System.Exception"">Exception description.</exception>
    /// <permission cref=""System.Security.PermissionSet"">Everyone can access this method.</permission>
    public int TestMethod<T>(T arg1)
    {
        return 0;
    }
}
";

            DiagnosticResult[] expectedDiagnostics =
            {
                Diagnostic().WithLocation(20, 31),
                Diagnostic().WithLocation(21, 32),
                Diagnostic().WithLocation(22, 65),
                Diagnostic().WithLocation(23, 89),
            };

            await VerifyCSharpFixAsync(testCode, expectedDiagnostics, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestIncludedDocumentationAsync()
        {
            var testCode = @"
/// <include file='ClassInheritDoc.xml' path='/TestClass/*'/>
public class TestClass
{
    /// <include file='PropertyInheritDoc.xml' path='/TestClass/TestProperty/*'/>
    public int TestProperty { get; set; }

    /// <include file='MethodInheritDoc.xml' path='/TestClass/TestMethod/*'/>
    public int TestMethod<T>(T arg1)
    {
        return 0;
    }
}
";

            DiagnosticResult[] expectedDiagnostics =
            {
                Diagnostic().WithLocation(3, 14),
                Diagnostic().WithLocation(6, 16),
                Diagnostic().WithLocation(9, 16),
            };

            await VerifyCSharpFixAsync(testCode, expectedDiagnostics, testCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestInvalidIncludedDocumentationAsync()
        {
            var testCode = @"
/// <include file='InvalidClassInheritDoc.xml' path='/TestClass/*'/>
public class TestClass
{
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(2680, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/2680")]
        public async Task TestReportingAfterEmptyElementAsync()
        {
            var testCode = @"
/// <summary>
/// Test interface <see cref=""ITest""/>
/// </summary>
public interface ITest
{
    /// <summary>
    /// Test method <see cref=""Method""/>
    /// </summary>
    void Method();
}
";

            var fixedTestCode = @"
/// <summary>
/// Test interface <see cref=""ITest""/>.
/// </summary>
public interface ITest
{
    /// <summary>
    /// Test method <see cref=""Method""/>.
    /// </summary>
    void Method();
}
";

            DiagnosticResult[] expected =
            {
                Diagnostic().WithLocation(3, 39),
                Diagnostic().WithLocation(8, 41),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(2680, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/2680")]
        public async Task TestReportingAfterTwoEmptyElementsAsync()
        {
            var testCode = @"
/// <summary>
/// Test interface <see cref=""ITest""/> <see cref=""ITest""/>
/// </summary>
public interface ITest
{
    /// <summary>
    /// Test method <see cref=""Method""/><see cref=""Method""/>
    /// </summary>
    void Method();
}
";

            var fixedTestCode = @"
/// <summary>
/// Test interface <see cref=""ITest""/> <see cref=""ITest""/>.
/// </summary>
public interface ITest
{
    /// <summary>
    /// Test method <see cref=""Method""/><see cref=""Method""/>.
    /// </summary>
    void Method();
}
";

            DiagnosticResult[] expected =
            {
                Diagnostic().WithLocation(3, 59),
                Diagnostic().WithLocation(8, 61),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(2680, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/2680")]
        public async Task TestReportingAfterEmptyElementTwoSentencesAsync()
        {
            var testCode = @"
/// <summary>
/// Test interface. <see cref=""ITest""/>
/// </summary>
public interface ITest
{
    /// <summary>
    /// Test method. <see cref=""Method""/><see cref=""Method""/>
    /// </summary>
    void Method();
}
";

            var fixedTestCode = @"
/// <summary>
/// Test interface. <see cref=""ITest""/>.
/// </summary>
public interface ITest
{
    /// <summary>
    /// Test method. <see cref=""Method""/><see cref=""Method""/>.
    /// </summary>
    void Method();
}
";

            DiagnosticResult[] expected =
            {
                Diagnostic().WithLocation(3, 40),
                Diagnostic().WithLocation(8, 62),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(2679, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/2679")]
        public async Task TestElementsThatDoNotRequirePeriodsAsync()
        {
            var testCode = @"
/// <summary>
/// Test interface <see cref=""ITest"">a see element</see>
/// </summary>
/// <seealso href=""https://docs.microsoft.com/en-us/dotnet/framework/wpf/index"">Windows Presentation Foundation</seealso>
public interface ITest
{
}
";

            var fixedTestCode = @"
/// <summary>
/// Test interface <see cref=""ITest"">a see element</see>.
/// </summary>
/// <seealso href=""https://docs.microsoft.com/en-us/dotnet/framework/wpf/index"">Windows Presentation Foundation</seealso>
public interface ITest
{
}
";

            DiagnosticResult expected = Diagnostic().WithLocation(3, 57);

            await VerifyCSharpFixAsync(testCode, expected, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [WorkItem(2744, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/2744")]
        [InlineData("Summary. (For example.)")]
        [InlineData("Summary (for example).")]
        public async Task TestSentenceEndingWithParenthesesAsync(string allowedSummary)
        {
            var testCode = $@"
/// <summary>
/// {allowedSummary}
/// </summary>
public interface ITest
{{
}}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(2744, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/2744")]
        public async Task TestSentenceEndingWithParenthesesWithoutPeriodAsync()
        {
            var testCode = @"
/// <summary>
/// Summary (for example)
/// </summary>
public interface ITest
{
}
";
            var fixedTestCode = $@"
/// <summary>
/// Summary (for example).
/// </summary>
public interface ITest
{{
}}
";

            DiagnosticResult expected = Diagnostic().WithLocation(3, 26);
            await VerifyCSharpFixAsync(testCode, expected, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestMultipleParagraphBlocksAsync()
        {
            var testCode = @"
/// <summary>
/// <para>Paragraph 1</para>
/// <para>Paragraph 2</para>
/// <para>Paragraph 3</para>
/// </summary>
public interface ITest
{
}
";

            var fixedTestCode = @"
/// <summary>
/// <para>Paragraph 1.</para>
/// <para>Paragraph 2.</para>
/// <para>Paragraph 3.</para>
/// </summary>
public interface ITest
{
}
";

            DiagnosticResult[] expected =
            {
                Diagnostic().WithLocation(3, 22),
                Diagnostic().WithLocation(4, 22),
                Diagnostic().WithLocation(5, 22),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestMultipleParagraphBlocksWithColonsAsync()
        {
            var testCode = @"
/// <summary>
/// <para>Paragraph 1:</para>
/// <para>Paragraph 2:</para>
/// <para>Paragraph 3:</para>
/// </summary>
public interface ITest
{
}
";

            var fixedTestCode = @"
/// <summary>
/// <para>Paragraph 1:</para>
/// <para>Paragraph 2:</para>
/// <para>Paragraph 3:.</para>
/// </summary>
public interface ITest
{
}
";

            DiagnosticResult expected = Diagnostic().WithLocation(5, 23);

            await VerifyCSharpFixAsync(testCode, expected, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestMultipleParagraphInlinesAsync()
        {
            var testCode = @"
/// <summary>
/// Paragraph 1
/// <para/>
/// Paragraph 2
/// <para/>
/// Paragraph 3
/// </summary>
public interface ITest
{
}
";

            var fixedTestCode = @"
/// <summary>
/// Paragraph 1.
/// <para/>
/// Paragraph 2.
/// <para/>
/// Paragraph 3.
/// </summary>
public interface ITest
{
}
";

            DiagnosticResult[] expected =
            {
                Diagnostic().WithLocation(3, 16),
                Diagnostic().WithLocation(5, 16),
                Diagnostic().WithLocation(7, 16),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestMultipleParagraphBlocksAfterFirstAsync()
        {
            var testCode = @"
/// <summary>
/// Paragraph 1
/// <para>Paragraph 2</para>
/// <para>Paragraph 3</para>
/// </summary>
public interface ITest
{
}
";

            var fixedTestCode = @"
/// <summary>
/// Paragraph 1.
/// <para>Paragraph 2.</para>
/// <para>Paragraph 3.</para>
/// </summary>
public interface ITest
{
}
";

            DiagnosticResult[] expected =
            {
                Diagnostic().WithLocation(3, 16),
                Diagnostic().WithLocation(4, 22),
                Diagnostic().WithLocation(5, 22),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestMultipleParagraphBlocksAfterFirstInNoteAsync()
        {
            var testCode = @"
/// <summary>
/// Paragraph 0
/// <note>
/// Paragraph 1
/// <para>Paragraph 2</para>
/// <para>Paragraph 3</para>
/// </note>
/// </summary>
public interface ITest
{
}
";

            var fixedTestCode = @"
/// <summary>
/// Paragraph 0.
/// <note>
/// Paragraph 1.
/// <para>Paragraph 2.</para>
/// <para>Paragraph 3.</para>
/// </note>
/// </summary>
public interface ITest
{
}
";

            DiagnosticResult[] expected =
            {
                Diagnostic().WithLocation(3, 16),
                Diagnostic().WithLocation(5, 16),
                Diagnostic().WithLocation(6, 22),
                Diagnostic().WithLocation(7, 22),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestCodeBetweenParagraphBlocksAsync()
        {
            var testCode = @"
/// <summary>
/// Paragraph 1
/// <code>Code block</code>
/// <para>Paragraph 2</para>
/// </summary>
public interface ITest
{
}
";

            var fixedTestCode = @"
/// <summary>
/// Paragraph 1.
/// <code>Code block</code>
/// <para>Paragraph 2.</para>
/// </summary>
public interface ITest
{
}
";

            DiagnosticResult[] expected =
            {
                Diagnostic().WithLocation(3, 16),
                Diagnostic().WithLocation(5, 22),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(2712, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/2712")]
        public async Task TestExceptionElementsWithStandardFormAsync()
        {
            var testCode = @"
using System;
public interface ITest
{
    /// <exception cref=""ArgumentNullException"">
    /// <para>If <paramref name=""name""/> is <see langword=""null""/></para>
    /// <para>-or-</para>
    /// <para>If <paramref name=""value""/> is <see langword=""null""/></para>
    /// </exception>
    void Method(string name, string value);
}
";

            var fixedTestCode = @"
using System;
public interface ITest
{
    /// <exception cref=""ArgumentNullException"">
    /// <para>If <paramref name=""name""/> is <see langword=""null""/>.</para>
    /// <para>-or-</para>
    /// <para>If <paramref name=""value""/> is <see langword=""null""/>.</para>
    /// </exception>
    void Method(string name, string value);
}
";

            DiagnosticResult[] expected =
            {
                Diagnostic().WithLocation(6, 67),
                Diagnostic().WithLocation(8, 68),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(2712, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/2712")]
        public async Task TestExceptionElementsWithAlternateFormAsync()
        {
            var testCode = @"
using System;
public interface ITest
{
    /// <exception cref=""ArgumentNullException"">
    /// <para>If <paramref name=""name""/> is <see langword=""null""/></para>
    /// -or-
    /// <para>If <paramref name=""value""/> is <see langword=""null""/></para>
    /// </exception>
    void Method(string name, string value);
}
";

            var fixedTestCode = @"
using System;
public interface ITest
{
    /// <exception cref=""ArgumentNullException"">
    /// <para>If <paramref name=""name""/> is <see langword=""null""/>.</para>
    /// -or-
    /// <para>If <paramref name=""value""/> is <see langword=""null""/>.</para>
    /// </exception>
    void Method(string name, string value);
}
";

            DiagnosticResult[] expected =
            {
                Diagnostic().WithLocation(6, 67),
                Diagnostic().WithLocation(8, 68),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestConfigurationSettingsAsync()
        {
            var testSettings = @"
{
  ""settings"": {
    ""documentationRules"": {
      ""excludeFromPunctuationCheck"": [ ""typeparam"",""example"",""exception"",""permission"",""author"" ]
    }
  }
}
";

            var testCode = @"
public interface ITest
{
    /// <summary>
    /// Test method #1.
    /// </summary>
    /// <typeparam name=""T"">
    /// Template type
    /// </typeparam>
    /// <param name=""arg1"">
    /// First argument.
    /// </param>
    /// <returns>
    /// Some value.
    /// </returns>
    /// <remarks>
    /// Random remark.
    /// </remarks>
    /// <example>
    /// Random example
    /// </example>
    /// <exception cref=""System.Exception"">
    /// Exception description
    /// </exception>
    /// <permission cref=""System.Security.PermissionSet"">
    /// Everyone can access this method
    /// </permission>
    /// <author>
    /// john.doe@example.com
    /// </author>
    int TestMethod1<T>(T arg1);
}
";

            await VerifyCSharpDiagnosticAsync(testCode, testSettings, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestConfigurationSettingsEmptyAsync()
        {
            var testSettings = @"
{
  ""settings"": {
    ""documentationRules"": {
      ""excludeFromPunctuationCheck"": [ ]
    }
  }
}
";

            var testCode = @"
public interface ITest
{
    /// <summary>Test method #1.</summary>
    /// <seealso>Missing period</seealso>
    void TestMethod1();
}
";

            DiagnosticResult[] expectedResult =
            {
                Diagnostic().WithLocation(5, 32),
            };

            await VerifyCSharpDiagnosticAsync(testCode, testSettings, expectedResult, CancellationToken.None).ConfigureAwait(false);
        }

        private static Task VerifyCSharpDiagnosticAsync(string source, DiagnosticResult[] expected, CancellationToken cancellationToken)
            => VerifyCSharpDiagnosticAsync(source, testSettings: null, expected, cancellationToken);

        private static Task VerifyCSharpDiagnosticAsync(string source, string testSettings, DiagnosticResult[] expected, CancellationToken cancellationToken)
        {
            var test = CreateTest(testSettings, expected);
            test.TestCode = source;

            return test.RunAsync(cancellationToken);
        }

        private static Task VerifyCSharpFixAsync(string source, DiagnosticResult expected, string fixedSource, CancellationToken cancellationToken)
            => VerifyCSharpFixAsync(source, new[] { expected }, fixedSource, cancellationToken);

        private static Task VerifyCSharpFixAsync(string source, DiagnosticResult[] expected, string fixedSource, CancellationToken cancellationToken)
        {
            var test = CreateTest(testSettings: null, expected);
            test.TestCode = source;
            test.FixedCode = fixedSource;

            return test.RunAsync(cancellationToken);
        }

        private static StyleCopCodeFixVerifier<SA1629DocumentationTextMustEndWithAPeriod, SA1629CodeFixProvider>.CSharpTest CreateTest(string testSettings, DiagnosticResult[] expected)
        {
            string contentClassInheritDoc = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<TestClass>
  <summary>Test class</summary>
</TestClass>
";
            string contentInvalidClassInheritDoc = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<TestClass>
  <summary>Test class<summary>
</TestClass>
";
            string contentPropertyInheritDoc = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<TestClass>
  <TestProperty>
    <summary>Gets or sets test property</summary>
    <value>Dummy integer</value>
  </TestProperty>
</TestClass>
";
            string contentMethodInheritDoc = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<TestClass>
  <TestMethod>
    <summary>Test method</summary>
    <typeparam name=""T"">Template type</typeparam>
    <param name=""arg1"">First argument</param>
    <returns>Some value</returns>
    <remarks>Random remark</remarks>
    <example>Random example</example>
    <exception cref=""System.Exception"">Exception description</exception>
    <permission cref=""System.Security.PermissionSet"">Everyone can access this method</permission>
  </TestMethod>
</TestClass>
";

            var test = new StyleCopCodeFixVerifier<SA1629DocumentationTextMustEndWithAPeriod, SA1629CodeFixProvider>.CSharpTest
            {
                XmlReferences =
                {
                    { "ClassInheritDoc.xml", contentClassInheritDoc },
                    { "InvalidClassInheritDoc.xml", contentInvalidClassInheritDoc },
                    { "PropertyInheritDoc.xml", contentPropertyInheritDoc },
                    { "MethodInheritDoc.xml", contentMethodInheritDoc },
                },
            };

            test.ExpectedDiagnostics.AddRange(expected);
            if (testSettings != null)
            {
                test.Settings = testSettings;
            }

            return test;
        }
    }
}
