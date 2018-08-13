// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.OrderingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using StyleCop.Analyzers.OrderingRules;
    using TestHelper;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.OrderingRules.SA1211UsingAliasDirectivesMustBeOrderedAlphabeticallyByAliasName,
        StyleCop.Analyzers.OrderingRules.UsingCodeFixProvider>;

    /// <summary>
    /// Unit tests for <see cref="SA1211UsingAliasDirectivesMustBeOrderedAlphabeticallyByAliasName"/>.
    /// </summary>
    public class SA1211UnitTests
    {
        /// <summary>
        /// Verifies that the analyzer will not produce diagnostics for correctly ordered using directives inside a namespace.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestValidUsingDirectivesInNamespaceAsync()
        {
            const string testCode = @"namespace Foo
{
    using System;
    using Character = System.Char;
    using @int = System.Int32;
    using StringBuilder = System.Text.StringBuilder;
    using StringWriter = System.IO.StringWriter;
}
";

            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the analyzer will not produce diagnostics for correctly ordered using directives in multiple namespaces.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestValidUsingDirectivesInMultipleNamespacesAsync()
        {
            const string testCode = @"namespace Foo
{
    using System;
    using character = System.Char;
    using \u0069nt = System.Int32;
}

namespace Bar
{
    using System;
    using MemoryStream = System.IO.MemoryStream;
    using Stream = System.IO.Stream;
    using StringBuilder = System.Text.StringBuilder;
    using StringWriter = System.IO.StringWriter;
}

namespace Spam
{
    using System;
    using Character = System.Char;
    using @int = System.Int32;
}
";

            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the analyzer will not produce diagnostics for correctly ordered using directives in the compilation unit.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestValidUsingDirectivesInCompilationUnitAsync()
        {
            const string testCode = @"using System;
using character = System.Char;
using \u0069nt = System.Int32;
using StringBuilder = System.Text.StringBuilder;
using StringWriter = System.IO.StringWriter;

public class Foo
{
}
";

            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
    using \u0069nt = System.Int32;
    using character = System.Char;
}

namespace Bar
{
    using System;
    using Stream = System.IO.Stream;
    using StringBuilder = System.Text.StringBuilder;
    using StringWriter = System.IO.StringWriter;
    using MemoryStream = System.IO.MemoryStream;
}

namespace Spam
{
    using System;
    using @int = System.Int32;
    using Character = System.Char;
}
";

            var fixedTestCode = @"namespace Foo
{
    using System;
    using character = System.Char;
    using \u0069nt = System.Int32;
}

namespace Bar
{
    using System;
    using MemoryStream = System.IO.MemoryStream;
    using Stream = System.IO.Stream;
    using StringBuilder = System.Text.StringBuilder;
    using StringWriter = System.IO.StringWriter;
}

namespace Spam
{
    using System;
    using Character = System.Char;
    using @int = System.Int32;
}
";

            DiagnosticResult[] expectedDiagnostics =
            {
                Diagnostic().WithLocation(5, 5).WithArguments("character", "int"),
                Diagnostic().WithLocation(14, 5).WithArguments("MemoryStream", "Stream"),
                Diagnostic().WithLocation(21, 5).WithArguments("Character", "int"),
            };

            await VerifyCSharpFixAsync(testCode, expectedDiagnostics, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestPreprocessorDirectivesAsync()
        {
            var testCode = @"
using System;
using Microsoft.Win32;
using MyList = System.Collections.Generic.List<int>;

#if true
using BThing = System.Threading.Tasks;
using AThing = System.Threading;
#else
using BThing = System.Threading.Tasks;
using AThing = System.Threading;
#endif";

            var fixedTestCode = @"
using System;
using Microsoft.Win32;
using MyList = System.Collections.Generic.List<int>;

#if true
using AThing = System.Threading;
using BThing = System.Threading.Tasks;
#else
using BThing = System.Threading.Tasks;
using AThing = System.Threading;
#endif";

            // else block is skipped
            var expected = Diagnostic().WithLocation(8, 1).WithArguments("AThing", "BThing");

            await VerifyCSharpFixAsync(testCode, expected, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the analyzer will not combine using directives that refer to the same type. This is a
        /// regression test for DotNetAnalyzers/StyleCopAnalyzers#2000.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestSameUsingWithDifferentAliasAsync()
        {
            string testCode = @"namespace NamespaceName
{
    using Func2 = System.Func<int>;
    using Func1 = System.Func<int>;

    internal delegate void DelegateName(Func1 arg1, Func2 arg2);
}
";
            string fixedCode = @"namespace NamespaceName
{
    using Func1 = System.Func<int>;
    using Func2 = System.Func<int>;

    internal delegate void DelegateName(Func1 arg1, Func2 arg2);
}
";

            var expected = Diagnostic().WithLocation(4, 5).WithArguments("Func1", "Func2");

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
