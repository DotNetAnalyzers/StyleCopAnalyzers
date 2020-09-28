// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.OrderingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.OrderingRules;
    using StyleCop.Analyzers.Settings.ObjectModel;
    using StyleCop.Analyzers.Test.Verifiers;
    using TestHelper;
    using Xunit;

    /// <summary>
    /// Unit tests for the <see cref="SA1200UsingDirectivesMustBePlacedCorrectly"/> when configured to use
    /// <see cref="UsingDirectivesPlacement.OutsideNamespace"/>.
    /// </summary>
    public class SA1200OutsideNamespaceUnitTests
    {
        private const string TestSettings = @"
{
  ""settings"": {
    ""orderingRules"": {
      ""usingDirectivesPlacement"": ""outsideNamespace""
    },
    ""documentationRules"": {
      ""xmlHeader"": false
    }
  }
}
";

        private const string ClassDefinition = @"public class TestClass
{
}";

        private const string StructDefinition = @"public struct TestStruct
{
}";

        private const string InterfaceDefinition = @"public interface TestInterface
{
}";

        private const string EnumDefinition = @"public enum TestEnum
{
    TestValue
}";

        private const string DelegateDefinition = @"public delegate void TestDelegate();";

        /// <summary>
        /// Verifies that using statements in a namespace produces the expected diagnostics.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestInvalidUsingStatementsInNamespaceAsync()
        {
            var testCode = @"namespace TestNamespace
{
    using System;
    using System.Threading;
}
";
            var fixedTestCode = @"using System;
using System.Threading;

namespace TestNamespace
{
}
";

            DiagnosticResult[] expectedResults =
            {
                Diagnostic(SA1200UsingDirectivesMustBePlacedCorrectly.DescriptorOutside).WithLocation(3, 5),
                Diagnostic(SA1200UsingDirectivesMustBePlacedCorrectly.DescriptorOutside).WithLocation(4, 5),
            };

            await VerifyCSharpFixAsync(testCode, expectedResults, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that simplified using statements in a namespace are expanded during the code fix operation.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestInvalidSimplifiedUsingStatementsInNamespaceAsync()
        {
            var testCode = @"namespace System
{
    using System;
    using System.Threading;
    using Reflection;
}
";
            var fixedTestCode = @"using System;
using System.Reflection;
using System.Threading;

namespace System
{
}
";

            DiagnosticResult[] expectedResults =
            {
                Diagnostic(SA1200UsingDirectivesMustBePlacedCorrectly.DescriptorOutside).WithLocation(3, 5),
                Diagnostic(SA1200UsingDirectivesMustBePlacedCorrectly.DescriptorOutside).WithLocation(4, 5),
                Diagnostic(SA1200UsingDirectivesMustBePlacedCorrectly.DescriptorOutside).WithLocation(5, 5),
            };

            await VerifyCSharpFixAsync(testCode, expectedResults, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that simplified using statements in a namespace are expanded during the code fix operation.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestInvalidSimplifiedUsingStatementsInExtensionNamespaceAsync()
        {
            var testCode = @"namespace System.MyExtension
{
    using System.Threading;
    using Reflection;
    using Assembly = Reflection.Assembly;
    using List = Collections.Generic.IList<int>;
}
";
            var fixedTestCode = @"using System.Reflection;
using System.Threading;
using Assembly = System.Reflection.Assembly;
using List = System.Collections.Generic.IList<int>;

namespace System.MyExtension
{
}
";

            DiagnosticResult[] expectedResults =
            {
                Diagnostic(SA1200UsingDirectivesMustBePlacedCorrectly.DescriptorOutside).WithLocation(3, 5),
                Diagnostic(SA1200UsingDirectivesMustBePlacedCorrectly.DescriptorOutside).WithLocation(4, 5),
                Diagnostic(SA1200UsingDirectivesMustBePlacedCorrectly.DescriptorOutside).WithLocation(5, 5),
                Diagnostic(SA1200UsingDirectivesMustBePlacedCorrectly.DescriptorOutside).WithLocation(6, 5),
            };

            await VerifyCSharpFixAsync(testCode, expectedResults, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that having using statements in the compilation unit will not produce any diagnostics when there are type definition present.
        /// </summary>
        /// <param name="typeDefinition">The type definition to test.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [InlineData(ClassDefinition)]
        [InlineData(StructDefinition)]
        [InlineData(InterfaceDefinition)]
        [InlineData(EnumDefinition)]
        [InlineData(DelegateDefinition)]
        public async Task TestValidUsingStatementsInCompilationUnitWithTypeDefinitionAsync(string typeDefinition)
        {
            var testCode = $@"using System;

{typeDefinition}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that having using statements in the compilation unit will not produce any diagnostics when there are attributes present.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestValidUsingStatementsInCompilationUnitWithAttributesAsync()
        {
            var testCode = @"using System.Reflection;

[assembly: AssemblyVersion(""1.0.0.0"")]

namespace TestNamespace
{
    using System;
    using System.Threading;
}
";
            var fixedCode = @"using System;
using System.Reflection;
using System.Threading;

[assembly: AssemblyVersion(""1.0.0.0"")]

namespace TestNamespace
{
}
";

            DiagnosticResult[] expectedResults =
            {
                Diagnostic(SA1200UsingDirectivesMustBePlacedCorrectly.DescriptorOutside).WithLocation(7, 5),
                Diagnostic(SA1200UsingDirectivesMustBePlacedCorrectly.DescriptorOutside).WithLocation(8, 5),
            };

            await VerifyCSharpFixAsync(testCode, expectedResults, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that having using statements in the compilation unit will not produce diagnostics.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestValidUsingStatementsInCompilationUnitAsync()
        {
            var testCode = @"using System;
using System.Threading;

namespace TestNamespace
{
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the file header of a file is properly preserved when moving using statements out of a namespace.
        /// This is a regression test for #1941.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestFileHeaderIsProperlyPreservedWhenMovingUsingStatementsAsync()
        {
            var testCode = @"// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace TestNamespace
{
    using System;
}
";
            var fixedTestCode = @"// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;

namespace TestNamespace
{
}
";

            DiagnosticResult[] expectedResults =
            {
                Diagnostic(SA1200UsingDirectivesMustBePlacedCorrectly.DescriptorOutside).WithLocation(6, 5),
            };

            await VerifyCSharpFixAsync(testCode, expectedResults, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestFileHeaderIsProperlyPreservedWhenMovingUsingStatementsWithCommentsAsync()
        {
            var testCode = @"// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace TestNamespace
{
    // Separated Comment

    using System.Collections;
    // Comment
    using System;
}
";
            var fixedTestCode = @"// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

// Separated Comment

// Comment
using System;
using System.Collections;

namespace TestNamespace
{
}
";

            DiagnosticResult[] expectedResults =
            {
                Diagnostic(SA1200UsingDirectivesMustBePlacedCorrectly.DescriptorOutside).WithLocation(8, 5),
                Diagnostic(SA1200UsingDirectivesMustBePlacedCorrectly.DescriptorOutside).WithLocation(10, 5),
            };

            await VerifyCSharpFixAsync(testCode, expectedResults, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(2928, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/2928")]
        public async Task TestMovingValueTupleAliasOutsideAsync()
        {
            var testCode = @"using System;

namespace MyNamespace
{
    using QuotesRange = ValueTuple<QuoteType, DateTime, DateTime>;

    public class QuoteType
    {
    }

    public class UseClass
    {
        private void Test()
        {
            QuotesRange t;
        }
    }
}
";
            var fixedTestCode = @"using System;
using QuotesRange = System.ValueTuple<MyNamespace.QuoteType, System.DateTime, System.DateTime>;

namespace MyNamespace
{
    public class QuoteType
    {
    }

    public class UseClass
    {
        private void Test()
        {
            QuotesRange t;
        }
    }
}
";

            DiagnosticResult[] expectedResults =
            {
                Diagnostic(SA1200UsingDirectivesMustBePlacedCorrectly.DescriptorOutside).WithLocation(5, 5),
            };

            await VerifyCSharpFixAsync(testCode, expectedResults, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }

        private static DiagnosticResult Diagnostic(DiagnosticDescriptor descriptor)
            => StyleCopCodeFixVerifier<SA1200UsingDirectivesMustBePlacedCorrectly, UsingCodeFixProvider>.Diagnostic(descriptor);

        private static Task VerifyCSharpDiagnosticAsync(string source, DiagnosticResult[] expected, CancellationToken cancellationToken)
        {
            var test = new StyleCopCodeFixVerifier<SA1200UsingDirectivesMustBePlacedCorrectly, UsingCodeFixProvider>.CSharpTest
            {
                TestCode = source,
                Settings = TestSettings,
            };

            test.ExpectedDiagnostics.AddRange(expected);
            return test.RunAsync(cancellationToken);
        }

        private static Task VerifyCSharpFixAsync(string source, DiagnosticResult[] expected, string fixedSource, CancellationToken cancellationToken)
        {
            var test = new StyleCopCodeFixVerifier<SA1200UsingDirectivesMustBePlacedCorrectly, UsingCodeFixProvider>.CSharpTest
            {
                TestCode = source,
                FixedCode = fixedSource,
                Settings = TestSettings,
            };

            test.ExpectedDiagnostics.AddRange(expected);
            return test.RunAsync(cancellationToken);
        }
    }
}
