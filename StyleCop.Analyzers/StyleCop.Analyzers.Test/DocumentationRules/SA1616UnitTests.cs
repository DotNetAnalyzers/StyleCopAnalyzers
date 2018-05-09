// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.DocumentationRules
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using StyleCop.Analyzers.DocumentationRules;
    using StyleCop.Analyzers.Test.Helpers;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using TestHelper;
    using Xunit;

    /// <summary>
    /// This class contains unit tests for <see cref="SA1616ElementReturnValueDocumentationMustHaveText"/>.
    /// </summary>
    public class SA1616UnitTests : CodeFixVerifier
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
            await this.VerifyCSharpDiagnosticAsync(testCode.Replace("$$", declaration), EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
            await this.VerifyCSharpDiagnosticAsync(testCode.Replace("$$", declaration), EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
            await this.VerifyCSharpDiagnosticAsync(testCode.Replace("$$", declaration), EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            var expected = this.CSharpDiagnostic().WithLocation(10, 8);

            await this.VerifyCSharpDiagnosticAsync(testCode.Replace("$$", declaration), expected, CancellationToken.None).ConfigureAwait(false);

            // The code fix does not alter this case.
            await this.VerifyCSharpFixAsync(testCode.Replace("$$", declaration), testCode.Replace("$$", declaration), cancellationToken: CancellationToken.None).ConfigureAwait(false);
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

            var expected = this.CSharpDiagnostic().WithLocation(10, 8);

            await this.VerifyCSharpDiagnosticAsync(testCode.Replace("$$", declaration), expected, CancellationToken.None).ConfigureAwait(false);

            // The code fix does not alter this case.
            await this.VerifyCSharpFixAsync(testCode.Replace("$$", declaration), testCode.Replace("$$", declaration), cancellationToken: CancellationToken.None).ConfigureAwait(false);
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

            var expected = this.CSharpDiagnostic().WithLocation(10, 8);

            await this.VerifyCSharpDiagnosticAsync(testCode.Replace("$$", declaration), expected, CancellationToken.None).ConfigureAwait(false);

            // The code fix does not alter this case.
            await this.VerifyCSharpFixAsync(testCode.Replace("$$", declaration), testCode.Replace("$$", declaration), cancellationToken: CancellationToken.None).ConfigureAwait(false);
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

            var expected = this.CSharpDiagnostic().WithLocation(12, 9);

            await this.VerifyCSharpDiagnosticAsync(testCode.Replace("$$", declaration), expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode.Replace("$$", declaration), EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode.Replace("$$", declaration), fixedCode.Replace("$$", declaration), cancellationToken: CancellationToken.None).ConfigureAwait(false);
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

            var expected = this.CSharpDiagnostic().WithLocation(12, 9);

            await this.VerifyCSharpDiagnosticAsync(testCode.Replace("$$", declaration), expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode.Replace("$$", declaration), EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode.Replace("$$", declaration), fixedCode.Replace("$$", declaration), cancellationToken: CancellationToken.None).ConfigureAwait(false);
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

            var expected = this.CSharpDiagnostic().WithLocation(12, 9);

            await this.VerifyCSharpDiagnosticAsync(testCode.Replace("$$", declaration), expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode.Replace("$$", declaration), EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode.Replace("$$", declaration), fixedCode.Replace("$$", declaration), cancellationToken: CancellationToken.None).ConfigureAwait(false);
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

            var expected = this.CSharpDiagnostic().WithLocation(12, 9);

            await this.VerifyCSharpDiagnosticAsync(testCode.Replace("$$", declaration).Replace("##", testAttribute), expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode.Replace("$$", declaration).Replace("##", testAttribute), EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode.Replace("$$", declaration).Replace("##", testAttribute), fixedCode.Replace("$$", declaration).Replace("##", testAttribute), cancellationToken: CancellationToken.None).ConfigureAwait(false);
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

            var expected = this.CSharpDiagnostic().WithLocation(12, 9);

            await this.VerifyCSharpDiagnosticAsync(testCode.Replace("$$", declaration).Replace("##", testAttribute), expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode.Replace("$$", declaration).Replace("##", testAttribute), EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode.Replace("$$", declaration).Replace("##", testAttribute), fixedCode.Replace("$$", declaration).Replace("##", testAttribute), cancellationToken: CancellationToken.None).ConfigureAwait(false);
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

            var expected = this.CSharpDiagnostic().WithLocation(12, 9);

            await this.VerifyCSharpDiagnosticAsync(testCode.Replace("$$", declaration).Replace("##", testAttribute), expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode.Replace("$$", declaration).Replace("##", testAttribute), EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode.Replace("$$", declaration).Replace("##", testAttribute), fixedCode.Replace("$$", declaration).Replace("##", testAttribute), cancellationToken: CancellationToken.None).ConfigureAwait(false);
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
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            var expected = this.CSharpDiagnostic().WithLocation(8, 22);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);

            // The code fix does not alter this case.
            await this.VerifyCSharpFixAsync(testCode, testCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
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

            var expected = this.CSharpDiagnostic().WithLocation(8, 22);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);

            // The code fix does not alter this case.
            await this.VerifyCSharpFixAsync(testCode, testCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
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

            var expected = this.CSharpDiagnostic().WithLocation(8, 22);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);

            // The code fix does not alter this case.
            await this.VerifyCSharpFixAsync(testCode, testCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        protected override Project ApplyCompilationOptions(Project project)
        {
            var resolver = new TestXmlReferenceResolver();

            string contentWithoutDocumentation = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<ClassName>
    <Method>
    </Method>
</ClassName>
";
            resolver.XmlReferences.Add("NoDocumentation.xml", contentWithoutDocumentation);

            string contentWithoutReturns = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<ClassName>
    <Method>
        <summary>
            Foo
        </summary>
    </Method>
</ClassName>
";
            resolver.XmlReferences.Add("WithoutReturns.xml", contentWithoutReturns);

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
            resolver.XmlReferences.Add("WithReturns.xml", contentWithReturns);

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
            resolver.XmlReferences.Add("WithEmptyReturns.xml", contentWithEmptyReturns);

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
            resolver.XmlReferences.Add("WithEmptyReturns2.xml", contentWithEmptyReturns2);

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
            resolver.XmlReferences.Add("WithEmptyReturns3.xml", contentWithEmptyReturns3);

            project = base.ApplyCompilationOptions(project);
            project = project.WithCompilationOptions(project.CompilationOptions.WithXmlReferenceResolver(resolver));
            return project;
        }

        protected override IEnumerable<DiagnosticAnalyzer> GetCSharpDiagnosticAnalyzers()
        {
            yield return new SA1616ElementReturnValueDocumentationMustHaveText();
        }

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new SA1615SA1616CodeFixProvider();
        }
    }
}
