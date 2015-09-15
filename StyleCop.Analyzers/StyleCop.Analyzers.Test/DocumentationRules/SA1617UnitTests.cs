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
    /// This class contains unit tests for <see cref="SA1617VoidReturnValueMustNotBeDocumented"/>.
    /// </summary>
    public class SA1617UnitTests : CodeFixVerifier
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
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
                this.CSharpDiagnostic().WithLocation(10, 9),
                this.CSharpDiagnostic().WithLocation(16, 9)
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
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
                this.CSharpDiagnostic().WithLocation(10, 9)
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);

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

            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
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
                this.CSharpDiagnostic().WithLocation(9, 17)
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);

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

            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
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
                this.CSharpDiagnostic().WithLocation(7, 9)
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);

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

            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
        }

        protected override IEnumerable<DiagnosticAnalyzer> GetCSharpDiagnosticAnalyzers()
        {
            yield return new SA1617VoidReturnValueMustNotBeDocumented();
        }

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new SA1617CodeFixProvider();
        }
    }
}
