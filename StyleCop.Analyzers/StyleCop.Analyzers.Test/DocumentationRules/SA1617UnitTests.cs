// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.DocumentationRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using StyleCop.Analyzers.DocumentationRules;
    using StyleCop.Analyzers.Test.Verifiers;
    using TestHelper;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.CustomDiagnosticVerifier<StyleCop.Analyzers.DocumentationRules.SA1617VoidReturnValueMustNotBeDocumented>;

    /// <summary>
    /// This class contains unit tests for <see cref="SA1617VoidReturnValueMustNotBeDocumented"/>.
    /// </summary>
    public class SA1617UnitTests
    {
        [Fact]
        public async Task TestMethodWithReturnValueNoDocumentationAsync()
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
    public ClassName Method() { return null; }

    /// <value>
    /// Foo
    /// </value>
    public delegate ClassName MethodDelegate();
}";
            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestMethodWithReturnValueWithDocumentationAsync()
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
    /// <returns>null</returns>
    public ClassName Method() { return null; }

    /// <value>
    /// Foo
    /// </value>
    /// <returns>Some value</returns>
    public delegate ClassName MethodDelegate();
}";
            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestNoDocumentationAsync()
        {
            var testCode = @"
/// <summary>
/// Foo
/// </summary>
public class ClassName
{
    public ClassName Method() { return null; }

    public delegate ClassName MethodDelegate();
}";
            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
    public ClassName Method() { return null; }

    /// <inheritdoc/>
    public delegate ClassName MethodDelegate();
}";
            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestMethodWithoutReturnValueNoDocumentationAsync()
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
    public void Method() { }

    /// <value>
    /// Foo
    /// </value>
    public delegate void MethodDelegate();
}";
            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestMethodWithoutReturnValueWithDocumentationAsync()
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
    /// <returns>null</returns>
    public void Method() { }

    /// <value>
    /// Foo
    /// </value>
    /// <returns>Some value</returns>
    public delegate void MethodDelegate();
}";

            var expected = new[]
            {
                Diagnostic().WithLocation(10, 9),
                Diagnostic().WithLocation(16, 9),
            };

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestCodeFixWithNoDataAsync()
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
    /// <returns>null</returns>
    public void Method() { }
}";

            var expected = new[]
            {
                Diagnostic().WithLocation(10, 9),
            };

            var fixedCode = @"
/// <summary>
/// Foo
/// </summary>
public class ClassName
{
    /// <value>
    /// Foo
    /// </value>
    public void Method() { }
}";

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestCodeFixShareLineWithValueAsync()
        {
            var testCode = @"
/// <summary>
/// Foo
/// </summary>
public class ClassName
{
    /// <value>
    /// Foo
    /// </value><returns>null</returns>
    public void Method() { }
}";

            var expected = new[]
            {
                Diagnostic().WithLocation(9, 17),
            };

            var fixedCode = @"
/// <summary>
/// Foo
/// </summary>
public class ClassName
{
    /// <value>
    /// Foo
    /// </value>
    public void Method() { }
}";

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestCodeFixBeforeValueAsync()
        {
            var testCode = @"
/// <summary>
/// Foo
/// </summary>
public class ClassName
{
    /// <returns>null</returns> <value>
    /// Foo
    /// </value>
    public void Method() { }
}";

            var expected = new[]
            {
                Diagnostic().WithLocation(7, 9),
            };

            var fixedCode = @"
/// <summary>
/// Foo
/// </summary>
public class ClassName
{
    /// <value>
    /// Foo
    /// </value>
    public void Method() { }
}";

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that included method documentation without a returns tag will be accepted.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestMethodWithValidIncludeAsync()
        {
            var testCode = @"
public class ClassName
{
    /// <include file='MethodWithoutReturns.xml' path='/ClassName/Method/*'/>
    public void Method() { }
}";

            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that included method documentation with a returns tag will be flagged.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestMethodWithReturnsInIncludeAsync()
        {
            var testCode = @"
public class ClassName
{
    /// <include file='MethodWithReturns.xml' path='/ClassName/Method/*'/>
    public void Method() { }
}";

            var expected = Diagnostic().WithLocation(4, 9);
            await VerifyCSharpFixAsync(testCode, expected, testCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that included method documentation containing &gt;inheritdoc/&lt; will be accepted.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestMethodWithInheritdocInIncludeAsync()
        {
            var testCode = @"
public interface ITestInterface
{
  /// <summary>
  /// Foo bar.
  /// </summary>
  void Method();
}

public class ClassName : ITestInterface
{
    /// <include file='MethodWithInheritdoc.xml' path='/ClassName/Method/*'/>
    public void Method() { }
}";

            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        private static Task VerifyCSharpDiagnosticAsync(string source, DiagnosticResult expected, CancellationToken cancellationToken)
            => VerifyCSharpFixAsync(source, new[] { expected }, fixedSource: null, cancellationToken);

        private static Task VerifyCSharpDiagnosticAsync(string source, DiagnosticResult[] expected, CancellationToken cancellationToken)
            => VerifyCSharpFixAsync(source, expected, fixedSource: null, cancellationToken);

        private static Task VerifyCSharpFixAsync(string source, DiagnosticResult expected, string fixedSource, CancellationToken cancellationToken)
            => VerifyCSharpFixAsync(source, new[] { expected }, fixedSource, cancellationToken);

        private static Task VerifyCSharpFixAsync(string source, DiagnosticResult[] expected, string fixedSource, CancellationToken cancellationToken)
        {
            string contentWithoutReturns = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<ClassName>
  <Method>
    <summary>Foo</summary>
  </Method>
</ClassName>
";
            string contentWithReturns = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<ClassName>
  <Method>
    <summary>Foo</summary>
    <returns>Bar</returns>
  </Method>
</ClassName>
";
            string contentWithInheritdocValue = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<ClassName>
  <Method>
    <inheritdoc/>
  </Method>
</ClassName>
";

            var test = new StyleCopCodeFixVerifier<SA1617VoidReturnValueMustNotBeDocumented, SA1617CodeFixProvider>.CSharpTest
            {
                TestCode = source,
                FixedCode = fixedSource,
                XmlReferences =
                {
                    { "MethodWithoutReturns.xml", contentWithoutReturns },
                    { "MethodWithReturns.xml", contentWithReturns },
                    { "MethodWithInheritdoc.xml", contentWithInheritdocValue },
                },
            };

            test.ExpectedDiagnostics.AddRange(expected);
            return test.RunAsync(cancellationToken);
        }
    }
}
