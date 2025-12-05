// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.Test.DocumentationRules
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.DocumentationRules;
    using StyleCop.Analyzers.Test.Verifiers;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.CustomDiagnosticVerifier<StyleCop.Analyzers.DocumentationRules.SA1615ElementReturnValueMustBeDocumented>;

    /// <summary>
    /// This class contains unit tests for <see cref="SA1615ElementReturnValueMustBeDocumented"/>.
    /// </summary>
    public class SA1615UnitTests
    {
        public static IEnumerable<object[]> WithReturnValue
        {
            get
            {
                yield return new[] { "    public          ClassName Method(string foo, string bar) { return null; }" };
                yield return new[] { "    public delegate ClassName Method(string foo, string bar);" };
            }
        }

        public static IEnumerable<object[]> AsynchronousWithReturnValue
        {
            get
            {
                yield return new[] { "    public          Task      MethodAsync(string foo, string bar) { return null; }" };
                yield return new[] { "    public          Task<int> MethodAsync(string foo, string bar) { return null; }" };
                yield return new[] { "    public          TASK      MethodAsync(string foo, string bar) { return null; }" };
                yield return new[] { "    public delegate Task      MethodAsync(string foo, string bar);" };
                yield return new[] { "    public delegate Task<int> MethodAsync(string foo, string bar);" };
                yield return new[] { "    public delegate TASK      MethodAsync(string foo, string bar);" };
            }
        }

        public static IEnumerable<object[]> AsynchronousUnitTestWithReturnValue
        {
            get
            {
                yield return new[] { "    public          Task      MethodAsync(string foo, string bar) { return null; }", "TestMethod" };
                yield return new[] { "    public          Task      MethodAsync(string foo, string bar) { return null; }", "Fact" };
                yield return new[] { "    public          Task      MethodAsync(string foo, string bar) { return null; }", "Theory" };
                yield return new[] { "    public          Task      MethodAsync(string foo, string bar) { return null; }", "Test" };
                yield return new[] { "    public          Task<int> MethodAsync(string foo, string bar) { return null; }", "TestMethod" };
                yield return new[] { "    public          Task<int> MethodAsync(string foo, string bar) { return null; }", "Fact" };
                yield return new[] { "    public          Task<int> MethodAsync(string foo, string bar) { return null; }", "Theory" };
                yield return new[] { "    public          Task<int> MethodAsync(string foo, string bar) { return null; }", "Test" };
                yield return new[] { "    public          TASK      MethodAsync(string foo, string bar) { return null; }", "TestMethod" };
                yield return new[] { "    public          TASK      MethodAsync(string foo, string bar) { return null; }", "Fact" };
                yield return new[] { "    public          TASK      MethodAsync(string foo, string bar) { return null; }", "Theory" };
                yield return new[] { "    public          TASK      MethodAsync(string foo, string bar) { return null; }", "Test" };
            }
        }

        public static IEnumerable<object[]> WithoutReturnValue
        {
            get
            {
                yield return new[] { "    public void Method(string foo, string bar) { }" };
                yield return new[] { "    public delegate void Method(string foo, string bar);" };
            }
        }

        [Theory]
        [MemberData(nameof(WithReturnValue))]
        [MemberData(nameof(WithoutReturnValue))]
        public async Task TestMethodWithoutDocumentationAsync(string declaration)
        {
            var testCode = @"
/// <summary>
/// Foo
/// </summary>
public class ClassName
{
$$
}";
            await VerifyCSharpDiagnosticAsync(testCode.Replace("$$", declaration), DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(WithoutReturnValue))]
        public async Task TestMethodWithoutReturnTypeWithoutReturnTypeDocumentationAsync(string declaration)
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
$$
}";
            await VerifyCSharpDiagnosticAsync(testCode.Replace("$$", declaration), DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(WithoutReturnValue))]
        public async Task TestMethodWithVoidWithDocumentationAsync(string declaration)
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
    /// <returns>Foo</returns>
$$
}";
            await VerifyCSharpDiagnosticAsync(testCode.Replace("$$", declaration), DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(WithReturnValue))]
        public async Task TestMethodWithReturnTypeWithoutReturnTypeDocumentationAsync(string declaration)
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
$$
}";
            var fixedCode = @"
/// <summary>
/// Foo
/// </summary>
public class ClassName
{
    /// <summary>
    /// Foo
    /// </summary>
    /// <returns></returns>
$$
}";

            var expected = Diagnostic().WithLocation(10, 21);
            await VerifyCSharpFixAsync(testCode.Replace("$$", declaration), expected, fixedCode.Replace("$$", declaration), CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(AsynchronousWithReturnValue))]
        public async Task TestAsynchronousMethodWithReturnTypeWithoutReturnTypeDocumentationAsync(string declaration)
        {
            var testCode = @"
using System.Threading.Tasks;
using TASK = System.Threading.Tasks.Task<int>;
/// <summary>
/// Foo
/// </summary>
public class ClassName
{
    /// <summary>
    /// Foo
    /// </summary>
$$
}";
            var fixedCode = @"
using System.Threading.Tasks;
using TASK = System.Threading.Tasks.Task<int>;
/// <summary>
/// Foo
/// </summary>
public class ClassName
{
    /// <summary>
    /// Foo
    /// </summary>
    /// <returns><placeholder>A <see cref=""Task""/> representing the asynchronous operation.</placeholder></returns>
$$
}";

            var expected = Diagnostic().WithLocation(12, 21);
            await VerifyCSharpFixAsync(testCode.Replace("$$", declaration), expected, fixedCode.Replace("$$", declaration), CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(AsynchronousUnitTestWithReturnValue))]
        public async Task TestAsynchronousUnitTestMethodWithReturnTypeWithoutReturnTypeDocumentationAsync(string declaration, string testAttribute)
        {
            var testCode = @"
using System.Threading.Tasks;
using TASK = System.Threading.Tasks.Task<int>;
/// <summary>
/// Foo
/// </summary>
public class ClassName
{
    /// <summary>
    /// Foo
    /// </summary>
    [##]
$$
}
internal sealed class ##Attribute : System.Attribute { }
";
            var fixedCode = @"
using System.Threading.Tasks;
using TASK = System.Threading.Tasks.Task<int>;
/// <summary>
/// Foo
/// </summary>
public class ClassName
{
    /// <summary>
    /// Foo
    /// </summary>
    /// <returns><placeholder>A <see cref=""Task""/> representing the asynchronous unit test.</placeholder></returns>
    [##]
$$
}
internal sealed class ##Attribute : System.Attribute { }
";

            var expected = Diagnostic().WithLocation(13, 21);
            await VerifyCSharpFixAsync(testCode.Replace("$$", declaration).Replace("##", testAttribute), expected, fixedCode.Replace("$$", declaration).Replace("##", testAttribute), CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(WithReturnValue))]
        public async Task TestMethodWithReturnTypeWithDocumentationAsync(string declaration)
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
    /// <returns>Foo</returns>
$$
}";
            await VerifyCSharpDiagnosticAsync(testCode.Replace("$$", declaration), DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(WithReturnValue))]
        [MemberData(nameof(WithoutReturnValue))]
        public async Task TestMethodWithInheritedDocumentationAsync(string declaration)
        {
            var testCode = @"
/// <summary>
/// Foo
/// </summary>
public class ClassName
{
    /// <inheritdoc/>
    public ClassName Test() { return null; }
}";
            await VerifyCSharpDiagnosticAsync(testCode.Replace("$$", declaration), DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestIncludedDocumentationAsync()
        {
            var testCode = @"
class Class1
{
    /// <include file='MethodWithoutReturns.xml' path='/Class1/MethodName/*'/>
    public int MethodName()
    {
        return 0;
    }
}
";

            DiagnosticResult expected = Diagnostic().WithLocation(5, 12);

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestIncludedInheritedDocumentationAsync()
        {
            var testCode = @"
class Class1
{
    /// <include file='MethodWithInheritedReturns.xml' path='/Class1/MethodName/*'/>
    public int MethodName()
    {
        return 0;
    }
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestIncludedIncompleteDocumentationAsync()
        {
            var testCode = @"
class Class1
{
    /// <include file='MethodWithReturns.xml' path='/Class1/MethodName/*'/>
    public int MethodName()
    {
        return 0;
    }
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(2445, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/2445")]
        public async Task TestPrivateMethodMissingReturnsAsync()
        {
            var testCode = @"
internal class ClassName
{
    ///
    private int Test1(int arg) { throw new System.NotImplementedException(); }

    /**
     *
     */
    private int Test2(int arg) { throw new System.NotImplementedException(); }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        protected static Task VerifyCSharpDiagnosticAsync(string source, DiagnosticResult expected, CancellationToken cancellationToken)
            => VerifyCSharpFixAsync(source, new[] { expected }, fixedSource: null, cancellationToken);

        protected static Task VerifyCSharpDiagnosticAsync(string source, DiagnosticResult[] expected, CancellationToken cancellationToken)
            => VerifyCSharpFixAsync(source, expected, fixedSource: null, cancellationToken);

        private static Task VerifyCSharpFixAsync(string source, DiagnosticResult expected, string fixedSource, CancellationToken cancellationToken)
            => VerifyCSharpFixAsync(source, new[] { expected }, fixedSource, cancellationToken);

        private static Task VerifyCSharpFixAsync(string source, DiagnosticResult[] expected, string fixedSource, CancellationToken cancellationToken)
        {
            string contentWithReturns = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<Class1>
  <MethodName>
    <summary>
      Sample method.
    </summary>
    <returns>
      A <see cref=""Task""/> representing the asynchronous operation.
    </returns>
  </MethodName>
</Class1>
";
            string contentWithInheritedReturns = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<Class1>
  <MethodName>
    <inheritdoc/>
  </MethodName>
</Class1>
";
            string contentWithoutReturns = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<Class1>
  <MethodName>
    <summary>
      Sample method.
    </summary>
  </MethodName>
</Class1>
";

            var test = new StyleCopCodeFixVerifier<SA1615ElementReturnValueMustBeDocumented, SA1615SA1616CodeFixProvider>.CSharpTest
            {
                TestCode = source,
                FixedCode = fixedSource,
                XmlReferences =
                {
                    { "MethodWithReturns.xml", contentWithReturns },
                    { "MethodWithInheritedReturns.xml", contentWithInheritedReturns },
                    { "MethodWithoutReturns.xml", contentWithoutReturns },
                },
            };

            test.ExpectedDiagnostics.AddRange(expected);
            return test.RunAsync(cancellationToken);
        }
    }
}
