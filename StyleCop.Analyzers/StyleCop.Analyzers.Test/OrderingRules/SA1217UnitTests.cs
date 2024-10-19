// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.Test.OrderingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.OrderingRules;
    using StyleCop.Analyzers.Test.Verifiers;
    using Xunit;

    /// <summary>
    /// Unit tests for <see cref="SA1217UsingStaticDirectivesMustBeOrderedAlphabetically"/>.
    /// </summary>
    public class SA1217UnitTests
    {
        private const string TestSettings = @"
{
  ""settings"": {
    ""orderingRules"": {
      ""systemUsingDirectivesFirst"": true
    }
  }
}
";

        private const string TestSettingsNoSystemDirectivesFirst = @"
{
  ""settings"": {
    ""orderingRules"": {
      ""systemUsingDirectivesFirst"": false
    }
  }
}
";

        private bool useSystemUsingDirectivesFirst;

        /// <summary>
        /// Verifies that the analyzer will not produce diagnostics for correctly ordered using directives inside a namespace.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestValidUsingDirectivesInNamespaceAsync()
        {
            var testCode = @"namespace Foo
{
    using System;
    using Execute = System.Action;
    using static System.Array;
    using static System.Math;
}
";

            await this.VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the analyzer will not produce diagnostics for correctly ordered using directives inside a namespace.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestValidUsingDirectivesInMultipleNamespacesAsync()
        {
            var testCode = @"namespace Foo
{
    using System;
    using Execute = System.Action;
    using static System.Array;
    using static System.Math;
}

namespace Bar
{
    using System;
    using static System.Array;
    using Execute = System.Action;
    using static System.Math;
}
";

            await this.VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the analyzer will not produce diagnostics for correctly ordered using directives in the compilation unit.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestValidUsingDirectivesInCompilationUnitAsync()
        {
            var testCode = @"using System;
using static System.Array;
using Execute = System.Action;
using static System.Math;

public class Foo
{
}
";

            await this.VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the analyzer will produce the proper diagnostics when the using directives are ordered wrong.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestInvalidUsingDirectivesOrderingAsync()
        {
            var testCode = @"namespace Foo
{
    using System;
    using Execute = System.Action;
    using static System.Math;
    using static System.Array;
}

namespace Bar
{
    using static System.Math;
    using Execute = System.Action;
    using static System.Array;
    using System;
}
";

            var fixedTestCode = @"namespace Foo
{
    using System;
    using static System.Array;
    using static System.Math;
    using Execute = System.Action;
}

namespace Bar
{
    using System;
    using static System.Array;
    using static System.Math;
    using Execute = System.Action;
}
";

            DiagnosticResult[] expectedDiagnostics =
            {
                Diagnostic().WithLocation(5, 5).WithArguments("System.Math", "System.Array"),
                Diagnostic().WithLocation(11, 5).WithArguments("System.Math", "System.Array"),
            };

            await this.VerifyCSharpFixAsync(testCode, expectedDiagnostics, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the analyzer will not produce a diagnostic when the using directives have inline comments.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestValidUsingDirectivesWithInlineCommentsAsync()
        {
            var testCode = @"namespace Foo
{
    using System;
    using Execute = System.Action;
    using static /* B */ System.Array;
    using static /* A */ System.Math;
}
";

            await this.VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the analyzer will produce the proper diagnostics when one of the using directives has a global prefixes.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestInvalidUsingDirectivesWithGlobalPrefixAsync()
        {
            var testCode = @"namespace Foo
{
    using System;
    using Execute = System.Action;
    using static System.Math;
    using static global::System.Array;
}
";

            var fixedTestCode = @"namespace Foo
{
    using System;
    using static global::System.Array;
    using static System.Math;
    using Execute = System.Action;
}
";

            DiagnosticResult[] expectedDiagnostic =
            {
                Diagnostic().WithLocation(5, 5).WithArguments("System.Math", "global::System.Array"),
            };

            await this.VerifyCSharpFixAsync(testCode, expectedDiagnostic, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the analyzer will properly handle preprocessor directives.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestPreprocessorDirectivesAsync()
        {
            var testCode = @"
using Microsoft.Win32;
using System;
using MyList = System.Collections.Generic.List<int>;
using static System.Tuple;

#if true
using static System.String;
using static System.Math;
#else
using static System.String;
using static System.Math;
#endif";

            var fixedTestCode = @"
using Microsoft.Win32;
using System;
using static System.Tuple;
using MyList = System.Collections.Generic.List<int>;

#if true
using static System.Math;
using static System.String;
#else
using static System.String;
using static System.Math;
#endif";

            // else block is skipped
            DiagnosticResult[] expectedDiagnostic =
            {
                Diagnostic().WithLocation(8, 1).WithArguments("System.String", "System.Math"),
            };

            await this.VerifyCSharpFixAsync(testCode, expectedDiagnostic, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verify that the systemUsingDirectivesFirst setting is honored correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [WorkItem(2163, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/2163")]
        public async Task VerifySystemUsingDirectivesFirstAsync()
        {
            this.useSystemUsingDirectivesFirst = true;

            var testCode = @"
using static MyNamespace.TestClass;
using static System.Math;

namespace MyNamespace
{
    public static class TestClass
    {
        public static void TestMethod()
        {
        }
    }
}
";

            var fixedTestCode = @"
using static System.Math;
using static MyNamespace.TestClass;

namespace MyNamespace
{
    public static class TestClass
    {
        public static void TestMethod()
        {
        }
    }
}
";

            DiagnosticResult[] expectedDiagnostic =
            {
                Diagnostic().WithLocation(2, 1).WithArguments("MyNamespace.TestClass", "System.Math"),
            };

            await this.VerifyCSharpFixAsync(testCode, expectedDiagnostic, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verify that the systemUsingDirectivesFirst setting is honored correctly when using multiple static system usings.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [WorkItem(2163, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/2163")]
        public async Task VerifyMultipleStaticSystemUsingDirectivesAsync()
        {
            this.useSystemUsingDirectivesFirst = true;

            var testCode = @"
using static System.Math;
using static System.Activator;

namespace MyNamespace
{
    public static class TestClass
    {
        public static void TestMethod()
        {
        }
    }
}
";

            var fixedTestCode = @"
using static System.Activator;
using static System.Math;

namespace MyNamespace
{
    public static class TestClass
    {
        public static void TestMethod()
        {
        }
    }
}
";

            DiagnosticResult[] expectedDiagnostic =
            {
                Diagnostic().WithLocation(2, 1).WithArguments("System.Math", "System.Activator"),
            };

            await this.VerifyCSharpFixAsync(testCode, expectedDiagnostic, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }

        private static DiagnosticResult Diagnostic()
            => StyleCopCodeFixVerifier<SA1217UsingStaticDirectivesMustBeOrderedAlphabetically, UsingCodeFixProvider>.Diagnostic();

        private Task VerifyCSharpDiagnosticAsync(string source, DiagnosticResult[] expected, CancellationToken cancellationToken)
        {
            var test = new StyleCopCodeFixVerifier<SA1217UsingStaticDirectivesMustBeOrderedAlphabetically, UsingCodeFixProvider>.CSharpTest
            {
                TestCode = source,
                Settings = this.useSystemUsingDirectivesFirst ? TestSettings : TestSettingsNoSystemDirectivesFirst,
            };

            test.ExpectedDiagnostics.AddRange(expected);
            return test.RunAsync(cancellationToken);
        }

        private Task VerifyCSharpFixAsync(string source, DiagnosticResult[] expected, string fixedSource, CancellationToken cancellationToken)
        {
            var test = new StyleCopCodeFixVerifier<SA1217UsingStaticDirectivesMustBeOrderedAlphabetically, UsingCodeFixProvider>.CSharpTest
            {
                TestCode = source,
                FixedCode = fixedSource,
                Settings = this.useSystemUsingDirectivesFirst ? TestSettings : TestSettingsNoSystemDirectivesFirst,
            };

            test.ExpectedDiagnostics.AddRange(expected);
            return test.RunAsync(cancellationToken);
        }
    }
}
