// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.ReadabilityRules
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.ReadabilityRules;
    using TestHelper;
    using Xunit;

    /// <summary>
    /// This class contains unit tests for the <see cref="SA1133DoNotCombineAttributes"/> class.
    /// </summary>
    public class SA1133UnitTests : CodeFixVerifier
    {
        /// <summary>
        /// Verifies that a single attribute will not produce a diagnostic.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task VerifyThatSingleAttributesDoNotProduceDiagnosticAsync()
        {
            var testCode = @"using System.ComponentModel;

[EditorBrowsable(EditorBrowsableState.Never)]
public class TestClass
{
}
";
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a multiple attributes on the same line will not produce a diagnostic.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task VerifyThatMultipleAttributesOnSameLineDoNotProduceDiagnosticAsync()
        {
            var testCode = @"using System.ComponentModel;

[EditorBrowsable(EditorBrowsableState.Never)][DesignOnly(true)]
public class TestClass
{
}
";
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that an attribute list will produce the required diagnostics.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task VerifyThatAttributeListProducesDiagnosticAsync()
        {
            var testCode = @"using System.ComponentModel;

[EditorBrowsable(EditorBrowsableState.Never), DesignOnly(true)]
public class TestClass
{
    /// <summary>
    /// Test method.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never), DesignOnly(true), DisplayName(""Test"")] // test comment
    public void TestMethod()
    {
    }
}
";

            var fixedTestCode = @"using System.ComponentModel;

[EditorBrowsable(EditorBrowsableState.Never)]
[DesignOnly(true)]
public class TestClass
{
    /// <summary>
    /// Test method.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [DesignOnly(true)]
    [DisplayName(""Test"")] // test comment
    public void TestMethod()
    {
    }
}
";

            DiagnosticResult[] expected =
            {
                this.CSharpDiagnostic().WithLocation(3, 47),
                this.CSharpDiagnostic().WithLocation(9, 51)
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that multiple attribute list are handled correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task VerifyThatMultipleAttributeListsAreHandledCorrectlyAsync()
        {
            var testCode = @"using System.ComponentModel;

[EditorBrowsable(EditorBrowsableState.Never), DesignOnly(true)]
#if false
[DataObject(true), Browsable(true)]
#else
[DataObject(true), Browsable(false)]
#endif
public class TestClass
{
    /// <summary>
    /// Test method.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never), DesignOnly(true), DisplayName(""Test"")] // test comment
    public void TestMethod()
    {
    }
}
";

            var fixedTestCode = @"using System.ComponentModel;

[EditorBrowsable(EditorBrowsableState.Never)]
[DesignOnly(true)]
#if false
[DataObject(true), Browsable(true)]
#else
[DataObject(true)]
[Browsable(false)]
#endif
public class TestClass
{
    /// <summary>
    /// Test method.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [DesignOnly(true)]
    [DisplayName(""Test"")] // test comment
    public void TestMethod()
    {
    }
}
";

            DiagnosticResult[] expected =
            {
                this.CSharpDiagnostic().WithLocation(3, 47),
                this.CSharpDiagnostic().WithLocation(7, 20),
                this.CSharpDiagnostic().WithLocation(14, 51)
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a combination of attributes without parameters will produce the required diagnostics.
        /// </summary>
        /// <param name="before">The code part before the code fix.</param>
        /// <param name="after">The code part after the code fix.</param>
        /// <param name="line">The line on which the diagnostic is expected.</param>
        /// <param name="column">The column on which the diagnostic is expected.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [InlineData("[Foo]\r\n[Bar, Car]", "[Foo]\r\n[Bar]\r\n[Car]", 3, 7)]
        [InlineData("[Foo, Bar]\r\n[Car]", "[Foo]\r\n[Bar]\r\n[Car]", 2, 7)]
        [InlineData("[Foo]\r\n[Bar, Car]\r\n[Ear]", "[Foo]\r\n[Bar]\r\n[Car]\r\n[Ear]", 3, 7)]
        public async Task VerifyAttributeCombinationsWithoutParametersAreHandledCorrectlyAsync(string before, string after, int line, int column)
        {
            var testCode = @"using System;
{0}
public class TestClass
{{
}}

public class Foo : Attribute
{{
}}

public class Bar : Attribute
{{
}}

public class Car : Attribute
{{
}}

public class Ear : Attribute
{{
}}";
            var codeBefore = string.Format(testCode, before);
            var codeAfter = string.Format(testCode, after);

            DiagnosticResult[] expected =
            {
                this.CSharpDiagnostic().WithLocation(line, column)
            };

            await this.VerifyCSharpDiagnosticAsync(codeBefore, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(codeAfter, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(codeBefore, codeAfter).ConfigureAwait(false);
        }

        /// <summary>
        /// Regression test for issue 1878 (SA1133CodeFixProvider crash), https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/1878
        /// Fixing exception "Unable to cast object of type 'Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax' to type 'Microsoft.CodeAnalysis.CSharp.Syntax.AttributeSyntax'."
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestRegressionIssue1878Async()
        {
            var testCode = @"namespace Stylecop_rc1_bug_repro
{
    using System;

    internal class Program
    {
        [Foo, Bar]
        private static void Main(string[] args)
        {
        }
    }

    internal class FooAttribute : Attribute
    {
    }

    internal class BarAttribute : Attribute
    {
    }
}
";

            var fixedTestCode = @"namespace Stylecop_rc1_bug_repro
{
    using System;

    internal class Program
    {
        [Foo]
        [Bar]
        private static void Main(string[] args)
        {
        }
    }

    internal class FooAttribute : Attribute
    {
    }

    internal class BarAttribute : Attribute
    {
    }
}
";

            DiagnosticResult[] expected =
            {
                this.CSharpDiagnostic().WithLocation(7, 15)
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new SA1133CodeFixProvider();
        }

        /// <inheritdoc/>
        protected override IEnumerable<DiagnosticAnalyzer> GetCSharpDiagnosticAnalyzers()
        {
            yield return new SA1133DoNotCombineAttributes();
        }
    }
}
