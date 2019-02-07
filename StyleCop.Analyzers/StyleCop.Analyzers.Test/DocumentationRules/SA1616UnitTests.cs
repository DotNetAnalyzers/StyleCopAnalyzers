// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

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
    using static StyleCop.Analyzers.Test.Verifiers.CustomDiagnosticVerifier<StyleCop.Analyzers.DocumentationRules.SA1616ElementReturnValueDocumentationMustHaveText>;

    /// <summary>
    /// This class contains unit tests for <see cref="SA1616ElementReturnValueDocumentationMustHaveText"/>.
    /// </summary>
    public class SA1616UnitTests
    {
        public static IEnumerable<object[]> Declarations
        {
            get
            {
                yield return new[] { "    public          ClassName Method(string foo, string bar) { return null; }" };
                yield return new[] { "    public delegate ClassName Method(string foo, string bar);" };
            }
        }

        public static IEnumerable<object[]> AsynchronousDeclarations
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

        public static IEnumerable<object[]> AsynchronousUnitTestDeclarations
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

        [Theory]
        [MemberData(nameof(Declarations))]
        public async Task TestMemberNoDocumentationAsync(string declaration)
        {
            var testCode = @"
/// <summary>
/// Foo
/// </summary>
public class ClassName
{
$$
}";
            await VerifyCSharpDiagnosticAsync(testCode.Replace("$$", declaration), DiagnosticResult.EmptyDiagnosticResults).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(Declarations))]
        public async Task TestMemberWithoutReturnsAsync(string declaration)
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
            await VerifyCSharpDiagnosticAsync(testCode.Replace("$$", declaration), DiagnosticResult.EmptyDiagnosticResults).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(Declarations))]
        public async Task TestMemberWithValidReturnsAsync(string declaration)
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
    ///<returns>Test</returns>
$$
}";
            await VerifyCSharpDiagnosticAsync(testCode.Replace("$$", declaration), DiagnosticResult.EmptyDiagnosticResults).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(Declarations))]
        public async Task TestMemberWithEmptyReturnsAsync(string declaration)
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
    ///<returns>
    ///
    ///                      </returns>
$$
}";

            var expected = Diagnostic().WithLocation(10, 8);

            // The code fix does not alter this case.
            await VerifyCSharpFixAsync(testCode.Replace("$$", declaration), expected, testCode.Replace("$$", declaration), offerEmptyFixer: true).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(Declarations))]
        public async Task TestMemberWithEmptyReturns2Async(string declaration)
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
    ///<returns/>
$$
}";

            var expected = Diagnostic().WithLocation(10, 8);

            // The code fix does not alter this case.
            await VerifyCSharpFixAsync(testCode.Replace("$$", declaration), expected, testCode.Replace("$$", declaration), offerEmptyFixer: true).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(Declarations))]
        public async Task TestMemberWithEmptyReturns3Async(string declaration)
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
    ///<returns></returns>
$$
}";

            var expected = Diagnostic().WithLocation(10, 8);

            // The code fix does not alter this case.
            await VerifyCSharpFixAsync(testCode.Replace("$$", declaration), expected, testCode.Replace("$$", declaration), offerEmptyFixer: true).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(AsynchronousDeclarations))]
        public async Task TestAsynchronousMethodWithEmptyContentReturnsElementAsync(string declaration)
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
    /// <returns></returns>
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

            var expected = Diagnostic().WithLocation(12, 9);
            await VerifyCSharpFixAsync(testCode.Replace("$$", declaration), expected, fixedCode.Replace("$$", declaration), offerEmptyFixer: false).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(AsynchronousDeclarations))]
        public async Task TestAsynchronousMethodWithEmptyReturnsElementAsync(string declaration)
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
    /// <returns/>
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

            var expected = Diagnostic().WithLocation(12, 9);
            await VerifyCSharpFixAsync(testCode.Replace("$$", declaration), expected, fixedCode.Replace("$$", declaration), offerEmptyFixer: false).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(AsynchronousDeclarations))]
        public async Task TestAsynchronousMethodWithWhitespaceContentReturnsElementAsync(string declaration)
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
    /// <returns>
    ///
    ///          </returns>
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

            var expected = Diagnostic().WithLocation(12, 9);
            await VerifyCSharpFixAsync(testCode.Replace("$$", declaration), expected, fixedCode.Replace("$$", declaration), offerEmptyFixer: false).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(AsynchronousUnitTestDeclarations))]
        public async Task TestAsynchronousUnitTestMethodWithEmptyContentReturnsElementAsync(string declaration, string testAttribute)
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
    /// <returns></returns>
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

            var expected = Diagnostic().WithLocation(12, 9);

            await VerifyCSharpFixAsync(testCode.Replace("$$", declaration).Replace("##", testAttribute), expected, fixedCode.Replace("$$", declaration).Replace("##", testAttribute), offerEmptyFixer: false).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(AsynchronousUnitTestDeclarations))]
        public async Task TestAsynchronousUnitTestMethodWithEmptyReturnsElementAsync(string declaration, string testAttribute)
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
    /// <returns/>
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

            var expected = Diagnostic().WithLocation(12, 9);

            await VerifyCSharpFixAsync(testCode.Replace("$$", declaration).Replace("##", testAttribute), expected, fixedCode.Replace("$$", declaration).Replace("##", testAttribute), offerEmptyFixer: false).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(AsynchronousUnitTestDeclarations))]
        public async Task TestAsynchronousUnitTestMethodWithWhitespaceContentReturnsElementAsync(string declaration, string testAttribute)
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
    /// <returns>
    ///
    ///         </returns>
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

            var expected = Diagnostic().WithLocation(12, 9);

            await VerifyCSharpFixAsync(testCode.Replace("$$", declaration).Replace("##", testAttribute), expected, fixedCode.Replace("$$", declaration).Replace("##", testAttribute), offerEmptyFixer: false).ConfigureAwait(false);
        }

        [Fact]
        public async Task VerifyMemberIncludedDocumentationNoDocumentationAsync()
        {
            var testCode = @"
/// <summary>
/// Foo
/// </summary>
public class ClassName
{
    /// <include file='NoDocumentation.xml' path='/ClassName/Method/*' />
    public ClassName Method(string foo, string bar) { return null; }
}";
            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults).ConfigureAwait(false);
        }

        [Fact]
        public async Task VerifyMemberIncludedDocumentationWithoutReturnsAsync()
        {
            var testCode = @"
/// <summary>
/// Foo
/// </summary>
public class ClassName
{
    /// <include file='WithoutReturns.xml' path='/ClassName/Method/*' />
    public ClassName Method(string foo, string bar) { return null; }
}";
            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults).ConfigureAwait(false);
        }

        [Fact]
        public async Task VerifyMemberIncludedDocumentationWithValidReturnsAsync()
        {
            var testCode = @"
/// <summary>
/// Foo
/// </summary>
public class ClassName
{
    /// <include file='WithReturns.xml' path='/ClassName/Method/*' />
    public ClassName Method(string foo, string bar) { return null; }
}";
            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults).ConfigureAwait(false);
        }

        [Fact]
        public async Task VerifyMemberIncludedDocumentationWithEmptyReturnsAsync()
        {
            var testCode = @"
/// <summary>
/// Foo
/// </summary>
public class ClassName
{
    /// <include file='WithEmptyReturns.xml' path='/ClassName/Method/*' />
    public ClassName Method(string foo, string bar) { return null; }
}";

            var expected = Diagnostic().WithLocation(8, 22);

            // The code fix does not alter this case.
            await VerifyCSharpFixAsync(testCode, expected, testCode, offerEmptyFixer: true).ConfigureAwait(false);
        }

        [Fact]
        public async Task VerifyMemberIncludedDocumentationWithEmptyReturns2Async()
        {
            var testCode = @"
/// <summary>
/// Foo
/// </summary>
public class ClassName
{
    /// <include file='WithEmptyReturns2.xml' path='/ClassName/Method/*' />
    public ClassName Method(string foo, string bar) { return null; }
}";

            var expected = Diagnostic().WithLocation(8, 22);

            // The code fix does not alter this case.
            await VerifyCSharpFixAsync(testCode, expected, testCode, offerEmptyFixer: true).ConfigureAwait(false);
        }

        [Fact]
        public async Task VerifyMemberIncludedDocumentationWithEmptyReturns3Async()
        {
            var testCode = @"
/// <summary>
/// Foo
/// </summary>
public class ClassName
{
    /// <include file='WithEmptyReturns3.xml' path='/ClassName/Method/*' />
    public ClassName Method(string foo, string bar) { return null; }
}";

            var expected = Diagnostic().WithLocation(8, 22);

            // The code fix does not alter this case.
            await VerifyCSharpFixAsync(testCode, expected, testCode, offerEmptyFixer: true).ConfigureAwait(false);
        }

        private static Task VerifyCSharpDiagnosticAsync(string source, DiagnosticResult expected, CancellationToken cancellationToken = default)
            => VerifyCSharpFixAsync(source, new[] { expected }, fixedSource: null, offerEmptyFixer: false, cancellationToken);

        private static Task VerifyCSharpDiagnosticAsync(string source, DiagnosticResult[] expected, CancellationToken cancellationToken = default)
            => VerifyCSharpFixAsync(source, expected, fixedSource: null, offerEmptyFixer: false, cancellationToken);

        private static Task VerifyCSharpFixAsync(string source, DiagnosticResult expected, string fixedSource, bool offerEmptyFixer, CancellationToken cancellationToken = default)
            => VerifyCSharpFixAsync(source, new[] { expected }, fixedSource, offerEmptyFixer, cancellationToken);

        private static Task VerifyCSharpFixAsync(string source, DiagnosticResult[] expected, string fixedSource, bool offerEmptyFixer, CancellationToken cancellationToken = default)
        {
            string contentWithoutDocumentation = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<ClassName>
    <Method>
    </Method>
</ClassName>
";
            string contentWithoutReturns = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<ClassName>
    <Method>
        <summary>
            Foo
        </summary>
    </Method>
</ClassName>
";
            string contentWithReturns = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<ClassName>
    <Method>
        <summary>
            Foo
        </summary>
        <returns>Test</returns>
    </Method>
</ClassName>
";
            string contentWithEmptyReturns = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<ClassName>
    <Method>
        <summary>
            Foo
        </summary>
        <returns>
        
                              </returns>
    </Method>
</ClassName>
";
            string contentWithEmptyReturns2 = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<ClassName>
    <Method>
        <summary>
            Foo
        </summary>
        <returns />
    </Method>
</ClassName>
";
            string contentWithEmptyReturns3 = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<ClassName>
    <Method>
        <summary>
            Foo
        </summary>
        <returns></returns>
    </Method>
</ClassName>
";

            var test = new StyleCopCodeFixVerifier<SA1616ElementReturnValueDocumentationMustHaveText, SA1615SA1616CodeFixProvider>.CSharpTest
            {
                TestCode = source,
                FixedCode = fixedSource,
                XmlReferences =
                {
                    { "NoDocumentation.xml", contentWithoutDocumentation },
                    { "WithoutReturns.xml", contentWithoutReturns },
                    { "WithReturns.xml", contentWithReturns },
                    { "WithEmptyReturns.xml", contentWithEmptyReturns },
                    { "WithEmptyReturns2.xml", contentWithEmptyReturns2 },
                    { "WithEmptyReturns3.xml", contentWithEmptyReturns3 },
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
