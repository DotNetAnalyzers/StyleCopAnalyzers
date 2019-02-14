// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.ReadabilityRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.ReadabilityRules.SA1134AttributesMustNotShareLine,
        StyleCop.Analyzers.ReadabilityRules.SA1134CodeFixProvider>;

    /// <summary>
    /// This class contains unit tests for SA1134.
    /// </summary>
    public class SA1134UnitTests
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
            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that multiple attributes on the same line for type declarations will produce the expected diagnostic.
        /// </summary>
        /// <param name="typeDeclaration">The type declaration to check.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [InlineData("class")]
        [InlineData("struct")]
        [InlineData("interface")]
        [InlineData("enum")]
        public async Task VerifyMultipleAttributesOnSameLineForTypeDeclarationsAsync(string typeDeclaration)
        {
            var testCode = $@"using System.ComponentModel;

namespace TestNamespace
{{
    /// <summary>
    /// Test class.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)][DesignOnly(true)]
    public {typeDeclaration} Test
    {{
    }}

    [DesignOnly(true)] public {typeDeclaration} Test2
    {{
    }}
}}
";

            var fixedTestCode = $@"using System.ComponentModel;

namespace TestNamespace
{{
    /// <summary>
    /// Test class.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [DesignOnly(true)]
    public {typeDeclaration} Test
    {{
    }}

    [DesignOnly(true)]
    public {typeDeclaration} Test2
    {{
    }}
}}
";

            DiagnosticResult[] expected =
            {
                Diagnostic().WithLocation(8, 50),
                Diagnostic().WithLocation(13, 5),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that multiple attributes on the same line for assembly level will produce the expected diagnostic.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task VerifyMultipleAttributesOnSameLineForAssemblyAsync()
        {
            var testCode = @"using System;
using System.Runtime.InteropServices;

#if COM_VISIBLE
[assembly:CLSCompliant(false)][assembly:ComVisible(true)]
#else
[assembly:CLSCompliant(false)][assembly:ComVisible(false)]
#endif
";

            var fixedTestCode = @"using System;
using System.Runtime.InteropServices;

#if COM_VISIBLE
[assembly:CLSCompliant(false)][assembly:ComVisible(true)]
#else
[assembly:CLSCompliant(false)]
[assembly:ComVisible(false)]
#endif
";

            DiagnosticResult[] expected =
            {
                Diagnostic().WithLocation(7, 31),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that multiple attributes on the same line, directly in front of a namespace declaration will produce the expected diagnostic.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task VerifyMultipleAttributesBeforeNamespaceAsync()
        {
            var testCode = @"using System;
using System.Runtime.InteropServices;

[assembly:CLSCompliant(false)][assembly:ComVisible(true)]
namespace TestNamespace
{
}
";

            var fixedTestCode = @"using System;
using System.Runtime.InteropServices;

[assembly:CLSCompliant(false)]
[assembly:ComVisible(true)]
namespace TestNamespace
{
}
";

            DiagnosticResult[] expected =
            {
                Diagnostic().WithLocation(4, 31),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that multiple attributes on the same line for module level will produce the expected diagnostic.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task VerifyMultipleAttributesOnSameLineForModuleAsync()
        {
            var testCode = @"using System.Diagnostics.CodeAnalysis;

[module:SuppressMessage(""Category"", ""TEST01"")][module:SuppressMessage(""Category"", ""TEST02"")][module:SuppressMessage(""Category"", ""TEST03"")]

public class TestClass
{
}
";

            var fixedTestCode = @"using System.Diagnostics.CodeAnalysis;

[module:SuppressMessage(""Category"", ""TEST01"")]
[module:SuppressMessage(""Category"", ""TEST02"")]
[module:SuppressMessage(""Category"", ""TEST03"")]

public class TestClass
{
}
";
            DiagnosticResult[] expected =
            {
                Diagnostic().WithLocation(3, 47),
                Diagnostic().WithLocation(3, 93),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that multiple attributes on the same line for delegate declarations will produce the expected diagnostic.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task VerifyMultipleAttributesOnSameLineForDelegateAsync()
        {
            var testCode = @"using System.ComponentModel;

namespace TestNamespace
{
    [EditorBrowsable(EditorBrowsableState.Never)][DesignOnly(true)]
    public delegate void TestDelegate(int value);

    [EditorBrowsable(EditorBrowsableState.Never)][DesignOnly(true)] public delegate void TestDelegate2(int value);
}
";

            var fixedTestCode = @"using System.ComponentModel;

namespace TestNamespace
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    [DesignOnly(true)]
    public delegate void TestDelegate(int value);

    [EditorBrowsable(EditorBrowsableState.Never)]
    [DesignOnly(true)]
    public delegate void TestDelegate2(int value);
}
";

            DiagnosticResult[] expected =
            {
                Diagnostic().WithLocation(5, 50),
                Diagnostic().WithLocation(8, 50),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that multiple attributes on the same line for member declarations will produce the expected diagnostic.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task VerifyMultipleAttributesOnSameLineForMembersAsync()
        {
            var testCode = @"using System;
using System.ComponentModel;

namespace TestNamespace
{
    public class TestClass
    {
        [EditorBrowsable(EditorBrowsableState.Never)][DesignOnly(true)]
        public TestClass() { }

        [EditorBrowsable(EditorBrowsableState.Never)][DesignOnly(true)]
        public int TestField;

        [EditorBrowsable(EditorBrowsableState.Never)][DesignOnly(true)]
        public void TestMethod() { }

        [EditorBrowsable(EditorBrowsableState.Never)][DesignOnly(true)]
        public int TestProperty { get; set; }

        [EditorBrowsable(EditorBrowsableState.Never)][DesignOnly(true)]
        public int this[int index] { get { return index; } }

        [EditorBrowsable(EditorBrowsableState.Never)][DesignOnly(true)]
        public event EventHandler TestEvent;
    }
}
";

            var fixedTestCode = @"using System;
using System.ComponentModel;

namespace TestNamespace
{
    public class TestClass
    {
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignOnly(true)]
        public TestClass() { }

        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignOnly(true)]
        public int TestField;

        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignOnly(true)]
        public void TestMethod() { }

        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignOnly(true)]
        public int TestProperty { get; set; }

        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignOnly(true)]
        public int this[int index] { get { return index; } }

        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignOnly(true)]
        public event EventHandler TestEvent;
    }
}
";

            DiagnosticResult[] expected =
            {
                Diagnostic().WithLocation(8, 54),
                Diagnostic().WithLocation(11, 54),
                Diagnostic().WithLocation(14, 54),
                Diagnostic().WithLocation(17, 54),
                Diagnostic().WithLocation(20, 54),
                Diagnostic().WithLocation(23, 54),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that multiple attributes on the same line for parameters / return value will produce the expected diagnostic.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task VerifyMultipleAttributesOnSameLineForParametersAsync()
        {
            var testCode = @"using System.ComponentModel;
using System.Runtime.InteropServices;

namespace TestNamespace
{
    public class TestClass
    {
        [EditorBrowsable(EditorBrowsableState.Never)] /* comment1 */ [DesignOnly(true)] /* comment2 */ [return: MarshalAs(UnmanagedType.Bool)] // comment3
        public bool TestMethod([In][MarshalAs(UnmanagedType.I4)] int value)
        {
            return false;
        }

        [return: MarshalAs(UnmanagedType.Bool)] public bool TestMethod2([In][MarshalAs(UnmanagedType.I4)] int value)
        {
            return false;
        }
    }
}
";

            var fixedTestCode = @"using System.ComponentModel;
using System.Runtime.InteropServices;

namespace TestNamespace
{
    public class TestClass
    {
        [EditorBrowsable(EditorBrowsableState.Never)] /* comment1 */
        [DesignOnly(true)] /* comment2 */
        [return: MarshalAs(UnmanagedType.Bool)] // comment3
        public bool TestMethod([In][MarshalAs(UnmanagedType.I4)] int value)
        {
            return false;
        }

        [return: MarshalAs(UnmanagedType.Bool)]
        public bool TestMethod2([In][MarshalAs(UnmanagedType.I4)] int value)
        {
            return false;
        }
    }
}
";

            DiagnosticResult[] expected =
            {
                Diagnostic().WithLocation(8, 70),
                Diagnostic().WithLocation(8, 104),
                Diagnostic().WithLocation(14, 9),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that multiple attributes on the same line for a generic parameter will produce the expected diagnostic.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task VerifyMultipleAttributesOnSameLineForGenericParameterAsync()
        {
            var testCode = @"using System;

namespace TestNamespace
{
    [AttributeUsage(System.AttributeTargets.All, AllowMultiple = true)]
    public sealed class TestAttribute : Attribute
    {
        public string Param { get; private set; }
        public TestAttribute(string param) { Param = param; }
    }

    public class TestClass<[Test(""Test1"")][Test(""Test2"")]T>
    {
    }
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that passing an invalid member syntax into the codefix will not change the code.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [WorkItem(2894, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/2894")]
        public virtual async Task VerifyInvalidMemberSyntaxInCodeFixAsync()
        {
            string testCode = @"class Program
{
    static void Main(string[] args)
    {
        {
        }[;]
    }
}
";

            DiagnosticResult[] expected =
            {
                DiagnosticResult.CompilerError("CS1513").WithLocation(6, 10),
                Diagnostic().WithLocation(6, 10),
                DiagnosticResult.CompilerError("CS1001").WithLocation(6, 11),
                DiagnosticResult.CompilerError("CS1001").WithLocation(6, 11),
                DiagnosticResult.CompilerError("CS1022").WithLocation(8, 1),
            };

            await VerifyCSharpFixAsync(testCode, expected, testCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
