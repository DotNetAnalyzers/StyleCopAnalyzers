// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.DocumentationRules
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Analyzers.DocumentationRules;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using TestHelper;
    using Xunit;

    /// <summary>
    /// This class contains unit tests for <see cref="SA1615ElementReturnValueMustBeDocumented"/>.
    /// </summary>
    public class SA1615UnitTests : CodeFixVerifier
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
            await this.VerifyCSharpDiagnosticAsync(testCode.Replace("$$", declaration), EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
            await this.VerifyCSharpDiagnosticAsync(testCode.Replace("$$", declaration), EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
            await this.VerifyCSharpDiagnosticAsync(testCode.Replace("$$", declaration), EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            var expected = this.CSharpDiagnostic().WithLocation(10, 21);

            await this.VerifyCSharpDiagnosticAsync(testCode.Replace("$$", declaration), expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode.Replace("$$", declaration), EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode.Replace("$$", declaration), fixedCode.Replace("$$", declaration), cancellationToken: CancellationToken.None).ConfigureAwait(false);
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

            var expected = this.CSharpDiagnostic().WithLocation(12, 21);

            await this.VerifyCSharpDiagnosticAsync(testCode.Replace("$$", declaration), expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode.Replace("$$", declaration), EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode.Replace("$$", declaration), fixedCode.Replace("$$", declaration), cancellationToken: CancellationToken.None).ConfigureAwait(false);
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

            var expected = this.CSharpDiagnostic().WithLocation(13, 21);

            await this.VerifyCSharpDiagnosticAsync(testCode.Replace("$$", declaration).Replace("##", testAttribute), expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode.Replace("$$", declaration).Replace("##", testAttribute), EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode.Replace("$$", declaration).Replace("##", testAttribute), fixedCode.Replace("$$", declaration).Replace("##", testAttribute), cancellationToken: CancellationToken.None).ConfigureAwait(false);
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
            await this.VerifyCSharpDiagnosticAsync(testCode.Replace("$$", declaration), EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
            await this.VerifyCSharpDiagnosticAsync(testCode.Replace("$$", declaration), EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        protected override IEnumerable<DiagnosticAnalyzer> GetCSharpDiagnosticAnalyzers()
        {
            yield return new SA1615ElementReturnValueMustBeDocumented();
        }

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new SA1615SA1616CodeFixProvider();
        }
    }
}
