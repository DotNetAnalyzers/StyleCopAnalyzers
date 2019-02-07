// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.ReadabilityRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.ReadabilityRules;
    using TestHelper;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.ReadabilityRules.SA1133DoNotCombineAttributes,
        StyleCop.Analyzers.ReadabilityRules.SA1133CodeFixProvider>;

    /// <summary>
    /// This class contains unit tests for the <see cref="SA1133DoNotCombineAttributes"/> class.
    /// </summary>
    public class SA1133UnitTests
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
            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults).ConfigureAwait(false);
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
            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults).ConfigureAwait(false);
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
                Diagnostic().WithLocation(3, 47),
                Diagnostic().WithLocation(9, 51),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedTestCode).ConfigureAwait(false);
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
                Diagnostic().WithLocation(3, 47),
                Diagnostic().WithLocation(7, 20),
                Diagnostic().WithLocation(14, 51),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedTestCode).ConfigureAwait(false);
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
                Diagnostic().WithLocation(line, column),
            };

            await VerifyCSharpFixAsync(codeBefore, expected, codeAfter).ConfigureAwait(false);
        }

        /// <summary>
        /// Fixing exception "Unable to cast object of type 'Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax' to type 'Microsoft.CodeAnalysis.CSharp.Syntax.AttributeSyntax'".
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [WorkItem(1878, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/1878")]
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
                Diagnostic().WithLocation(7, 15),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedTestCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that attribute list with multiple attributes for (generic) parameters will not produce diagnostics.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [WorkItem(1882, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/1882")]
        public async Task VerifyAttributeListForParametersAsync()
        {
            var testCode = @"using System;

internal class TestClass
{
    internal T TestMethod<[Foo, Bar]T>([Bar, Foo] int value)
    {
        return default(T);
    }
}

internal class FooAttribute : Attribute
{
}

internal class BarAttribute : Attribute
{
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults).ConfigureAwait(false);
        }

        /// <summary>
        /// Regression test for "SA1133CodeFixProvider does only half the work".
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [WorkItem(1879, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/1879")]
        public async Task TestFixAllAsync()
        {
            var testCode = @"
namespace Stylecop_rc1_bug_repro
{
    class Foo
    {
        [CanBeNull, UsedImplicitly(ImplicitUseKindFlags.Assign)]
        public string Foo1{ get; set; }

        [CanBeNull, UsedImplicitly(ImplicitUseKindFlags.Assign)]
        public string Foo2{ get; set; }

        [CanBeNull, UsedImplicitly(ImplicitUseKindFlags.Assign)]
        public string Foo3 { get; set; }

        [CanBeNull, UsedImplicitly(ImplicitUseKindFlags.Assign)]
        public string Foo4{ get; set; }

        [CanBeNull, UsedImplicitly(ImplicitUseKindFlags.Assign)]
        public string Foo5{ get; set; }

        [CanBeNull, UsedImplicitly(ImplicitUseKindFlags.Assign)]
        public string Foo6{ get; set; }

        [CanBeNull, UsedImplicitly(ImplicitUseKindFlags.Assign)]
        public string Foo7{ get; set; }

        [CanBeNull, UsedImplicitly(ImplicitUseKindFlags.Assign)]
        public string Foo8{ get; set; }

    }
}

public class CanBeNullAttribute : System.Attribute { }
public class UsedImplicitly : System.Attribute
{
    public UsedImplicitly (ImplicitUseKindFlags flags) { }
}

public enum ImplicitUseKindFlags { Assign }
";

            var fixedTestCode = @"
namespace Stylecop_rc1_bug_repro
{
    class Foo
    {
        [CanBeNull]
        [UsedImplicitly(ImplicitUseKindFlags.Assign)]
        public string Foo1{ get; set; }

        [CanBeNull]
        [UsedImplicitly(ImplicitUseKindFlags.Assign)]
        public string Foo2{ get; set; }

        [CanBeNull]
        [UsedImplicitly(ImplicitUseKindFlags.Assign)]
        public string Foo3 { get; set; }

        [CanBeNull]
        [UsedImplicitly(ImplicitUseKindFlags.Assign)]
        public string Foo4{ get; set; }

        [CanBeNull]
        [UsedImplicitly(ImplicitUseKindFlags.Assign)]
        public string Foo5{ get; set; }

        [CanBeNull]
        [UsedImplicitly(ImplicitUseKindFlags.Assign)]
        public string Foo6{ get; set; }

        [CanBeNull]
        [UsedImplicitly(ImplicitUseKindFlags.Assign)]
        public string Foo7{ get; set; }

        [CanBeNull]
        [UsedImplicitly(ImplicitUseKindFlags.Assign)]
        public string Foo8{ get; set; }

    }
}

public class CanBeNullAttribute : System.Attribute { }
public class UsedImplicitly : System.Attribute
{
    public UsedImplicitly (ImplicitUseKindFlags flags) { }
}

public enum ImplicitUseKindFlags { Assign }
";

            DiagnosticResult[] expected =
            {
                Diagnostic().WithLocation(6, 21),
                Diagnostic().WithLocation(9, 21),
                Diagnostic().WithLocation(12, 21),
                Diagnostic().WithLocation(15, 21),
                Diagnostic().WithLocation(18, 21),
                Diagnostic().WithLocation(21, 21),
                Diagnostic().WithLocation(24, 21),
                Diagnostic().WithLocation(27, 21),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedTestCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Regression test for "whitespace is preserved incorrectly".
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [WorkItem(1883, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/1883")]
        public async Task TestWhitespaceIsHandledCorrectlyAsync()
        {
            var testCode = @"
namespace SA1133CodeFix
{
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;

    [DefaultValue(true),
    SuppressMessage(null, null)]
    internal class Foo
    {
    }
}
";

            var fixedTestCode = @"
namespace SA1133CodeFix
{
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;

    [DefaultValue(true)]
    [SuppressMessage(null, null)]
    internal class Foo
    {
    }
}
";

            DiagnosticResult[] expected =
            {
                Diagnostic().WithLocation(8, 5),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedTestCode).ConfigureAwait(false);
        }
    }
}
