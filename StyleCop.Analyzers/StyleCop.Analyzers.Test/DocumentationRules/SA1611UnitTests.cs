// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.DocumentationRules
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using StyleCop.Analyzers.DocumentationRules;
    using StyleCop.Analyzers.Test.Verifiers;
    using TestHelper;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.CustomDiagnosticVerifier<StyleCop.Analyzers.DocumentationRules.SA1611ElementParametersMustBeDocumented>;

    /// <summary>
    /// This class contains unit tests for <see cref="SA1611ElementParametersMustBeDocumented"/>.
    /// </summary>
    public class SA1611UnitTests
    {
        public static IEnumerable<object[]> Data
        {
            get
            {
                // These method names are chosen so that the position of the parameters are always the same. This makes testing easier
                yield return new object[] { "         ClassName(string param1, string param2, string param3) { }" };
                yield return new object[] { "void Foooooooooooo(string param1, string param2, string param3) { }" };
                yield return new object[] { "delegate void Fooo(string param1, string param2, string param3);" };
                yield return new object[] { "System.String this[string param1, string param2, string param3] { get { return param1; } }" };
                yield return new object[] { "void Foooooooooooo(string param1, string param2, string @param3) { }" };
                yield return new object[] { "delegate void Fooo(string param1, string param2, string @param3);" };
                yield return new object[] { "System.String this[string param1, string param2, string @param3] { get { return param1; } }" };
                yield return new object[] { "void Foooooooooooo(string param1, string param2, string p\\u0061ram3) { }" };
                yield return new object[] { "delegate void Fooo(string param1, string param2, string p\\u0061ram3);" };
                yield return new object[] { "System.String this[string param1, string param2, string p\\u0061ram3] { get { return param1; } }" };
            }
        }

        [Theory]
        [MemberData(nameof(Data))]
        public async Task TestWithAllDocumentationAsync(string p)
        {
            var testCode = @"
/// <summary>
/// Foo
/// </summary>
public class ClassName
{
    /// <summary>
    /// Foo
    /// </summary>
    /// <param name=""param1"">Param 1</param>
    /// <param name=""param2""></param>
    /// <param name=""param3"">Param 3</param>
    public ##
}";
            await VerifyCSharpDiagnosticAsync(testCode.Replace("##", p), EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(Data))]
        public async Task TestWithAllDocumentationAlternativeSyntaxAsync(string p)
        {
            var testCode = @"
/// <summary>
/// Foo
/// </summary>
public class ClassName
{
    /// <summary>
    /// Foo
    /// </summary>
    /// <param name=""param1"">Param 1</param>
    /// <param name=""p&#97;ram2""></param>
    /// <param name=""p&#x61;ram3"">Param 3</param>
    public ##
}";
            await VerifyCSharpDiagnosticAsync(testCode.Replace("##", p), EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(Data))]
        public async Task TestWithAllDocumentationWrongOrderAsync(string p)
        {
            var testCode = @"
/// <summary>
/// Foo
/// </summary>
public class ClassName
{
    /// <summary>
    /// Foo
    /// </summary>
    /// <param name=""param1"">Param 1</param>
    /// <param name=""param3"">Param 3</param>
    /// <param name=""param2""></param>
    public ##
}";
            await VerifyCSharpDiagnosticAsync(testCode.Replace("##", p), EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(Data))]
        public async Task TestWithNoDocumentationAsync(string p)
        {
            var testCode = @"
/// <summary>
/// Foo
/// </summary>
public class ClassName
{
    public ##
}";
            await VerifyCSharpDiagnosticAsync(testCode.Replace("##", p), EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(Data))]
        public async Task TestInheritDocAsync(string p)
        {
            var testCode = @"
/// <summary>
/// Foo
/// </summary>
public class ClassName
{
    /// <inheritdoc/>
    public ##
}";
            await VerifyCSharpDiagnosticAsync(testCode.Replace("##", p), EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(Data))]
        public async Task TestMissingParametersAsync(string p)
        {
            var testCode = @"
/// <summary>
/// Foo
/// </summary>
public class ClassName
{
    /// <summary>
    /// Foo
    /// </summary>
    public ##
}";
            DiagnosticResult[] expected =
            {
                Diagnostic().WithLocation(10, 38).WithArguments("param1"),
                Diagnostic().WithLocation(10, 53).WithArguments("param2"),
                Diagnostic().WithLocation(10, 68).WithArguments("param3"),
            };

            await VerifyCSharpDiagnosticAsync(testCode.Replace("##", p), expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(2444, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/2444")]
        public async Task TestPrivateMethodMissingParametersAsync()
        {
            var testCode = @"
internal class ClassName
{
    ///
    private void Test1(int arg) { }

    /**
     *
     */
    private void Test2(int arg) { }
}";

            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(2444, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/2444")]
        public async Task TestPrivateMethodMissingParametersInIncludedDocumentationAsync()
        {
            var testCode = @"
internal class ClassName
{
    /// <include file='MissingElementDocumentation.xml' path='/TestClass/TestMethod/*' />
    private void TestMethod(string param1, string param2, string param3) { }
}";

            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that valid operator declarations will not produce diagnostics.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task VerifyValidOperatorDeclarationsAsync()
        {
            var testCode = @"
/// <summary>
/// Test class
/// </summary>
public class TestClass
{
    /// <summary>
    /// Foo
    /// </summary>
    /// <param name=""value"">The value to use.</param>
    public static TestClass operator +(TestClass value)
    {   
        return value;
    }

    /// <summary>
    /// Foo
    /// </summary>
    /// <param name=""value"">The value to use.</param>
    public static explicit operator TestClass(int value)
    {   
        return new TestClass();
    }
}
";

            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that invalid operator declarations will produce the expected diagnostics.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task VerifyInvalidOperatorDeclarationsAsync()
        {
            var testCode = @"
/// <summary>
/// Test class
/// </summary>
public class TestClass
{
    /// <summary>
    /// Foo
    /// </summary>
    public static TestClass operator +(TestClass value)
    {   
        return value;
    }

    /// <summary>
    /// Foo
    /// </summary>
    public static explicit operator TestClass(int value)
    {   
        return new TestClass();
    }
}
";

            DiagnosticResult[] expected =
            {
                Diagnostic().WithLocation(10, 50).WithArguments("value"),
                Diagnostic().WithLocation(18, 51).WithArguments("value"),
            };

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that included documentation with valid documentation does not produce diagnostics.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task VerifyIncludedDocumentationAsync()
        {
            var testCode = @"
/// <summary>
/// Foo
/// </summary>
public class ClassName
{
    /// <include file='WithElementDocumentation.xml' path='/TestClass/TestMethod/*' />
    public void TestMethod(string param1, string param2, string param3)
    {
    }
}";
            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that included documentation with missing elements produces the expected diagnostics.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task VerifyIncludedDocumentationMissingElementsAsync()
        {
            var testCode = @"
/// <summary>
/// Foo
/// </summary>
public class ClassName
{
    /// <include file='MissingElementDocumentation.xml' path='/TestClass/TestMethod/*' />
    public void TestMethod(string param1, string param2, string param3)
    {
    }
}";
            DiagnosticResult[] expected =
            {
                Diagnostic().WithLocation(8, 35).WithArguments("param1"),
                Diagnostic().WithLocation(8, 50).WithArguments("param2"),
                Diagnostic().WithLocation(8, 65).WithArguments("param3"),
            };

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that included documentation with an <c>&lt;inheritdoc&gt;</c> tag is ignored.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task VerifyIncludedInheritedDocumentationAsync()
        {
            var testCode = @"
/// <summary>
/// Foo
/// </summary>
public class ClassName
{
    /// <include file='InheritedDocumentation.xml' path='/TestClass/TestMethod/*' />
    public void TestMethod(string param1, string param2, string param3)
    {
    }
}";
            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        private static Task VerifyCSharpDiagnosticAsync(string source, DiagnosticResult expected, CancellationToken cancellationToken)
            => VerifyCSharpDiagnosticAsync(source, new[] { expected }, cancellationToken);

        private static Task VerifyCSharpDiagnosticAsync(string source, DiagnosticResult[] expected, CancellationToken cancellationToken)
        {
            string contentWithoutElementDocumentation = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<TestClass>
    <TestMethod>
        <summary>
            Foo
        </summary>
    </TestMethod>
</TestClass>
";
            string contentWithElementDocumentation = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<TestClass>
    <TestMethod>
        <summary>
            Foo
        </summary>
        <param name=""param1"">Param 1</param>
        <param name=""param2"">Param 2</param>
        <param name=""param3"">Param 3</param>
    </TestMethod>
</TestClass>
";
            string contentWithInheritedDocumentation = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
 <TestClass>
    <TestMethod>
        <inheritdoc />
    </TestMethod>
 </TestClass>
 ";

            var test = new StyleCopDiagnosticVerifier<SA1611ElementParametersMustBeDocumented>.CSharpTest
            {
                TestCode = source,
                XmlReferences =
                {
                    { "MissingElementDocumentation.xml", contentWithoutElementDocumentation },
                    { "WithElementDocumentation.xml", contentWithElementDocumentation },
                    { "InheritedDocumentation.xml", contentWithInheritedDocumentation },
                },
            };

            test.ExpectedDiagnostics.AddRange(expected);
            return test.RunAsync(cancellationToken);
        }
    }
}
