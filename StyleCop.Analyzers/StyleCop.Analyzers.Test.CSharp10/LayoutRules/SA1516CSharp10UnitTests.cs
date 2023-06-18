// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp10.LayoutRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp9.LayoutRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.LayoutRules.SA1516ElementsMustBeSeparatedByBlankLine,
        StyleCop.Analyzers.LayoutRules.SA1516CodeFixProvider>;

    public class SA1516CSharp10UnitTests : SA1516CSharp9UnitTests
    {
        /// <summary>
        /// Verifies that SA1516 is reported for usings and extern alias outside a file scoped namespace.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [WorkItem(3512, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3512")]
        public async Task TestThatDiagnosticIIsReportedOnUsingsAndExternAliasOutsideFileScopedNamespaceAsync()
        {
            var testCode = @"extern alias corlib;
[|using|] System;
using System.Linq;
using a = System.Collections;
[|namespace|] Foo;
";

            var fixedCode = @"extern alias corlib;

using System;
using System.Linq;
using a = System.Collections;

namespace Foo;
";

            await VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that SA1516 is reported for usings inside a file scoped namespace.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [WorkItem(3512, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3512")]
        public async Task TestThatDiagnosticIIsReportedOnSpacingWithUsingsInsideFileScopedNamespaceAsync()
        {
            var testCode = @"namespace Foo;
[|using|] System;
using System.Linq;
using a = System.Collections;
";

            var fixedCode = @"namespace Foo;

using System;
using System.Linq;
using a = System.Collections;
";

            await VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that SA1516 is reported for member declarations inside a file scoped namespace.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [WorkItem(3512, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3512")]
        public async Task TestThatDiagnosticIIsReportedOnMemberDeclarationsInsideFileScopedNamespaceAsync()
        {
            var testCode = @"namespace Foo;
[|public|] class Bar
{
}
[|public|] enum Foobar
{
}
";

            var fixedCode = @"namespace Foo;

public class Bar
{
}

public enum Foobar
{
}
";

            await VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that SA1516 is reported for usings and member declarations inside a file scoped namespace.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [WorkItem(3512, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3512")]
        public async Task TestThatDiagnosticIIsReportedOnUsingsAndMemberDeclarationsInsideFileScopedNamespaceAsync()
        {
            var testCode = @"namespace Foo;
[|using|] System;
using System.Linq;
using a = System.Collections;
[|public|] class Bar
{
}
[|public|] enum Foobar
{
}
";

            var fixedCode = @"namespace Foo;

using System;
using System.Linq;
using a = System.Collections;

public class Bar
{
}

public enum Foobar
{
}
";

            await VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that SA1516 is reported extern alias inside a file scoped namespace.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [WorkItem(3512, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3512")]
        public async Task TestThatDiagnosticIIsReportedOnExternAliasInsideFileScopedNamespaceAsync()
        {
            var testCode = @"namespace Foo;
[|extern|] alias corlib;
";

            var fixedCode = @"namespace Foo;

extern alias corlib;
";

            await VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that SA1516 is reported extern alias and usings inside a file scoped namespace.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [WorkItem(3512, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3512")]
        public async Task TestThatDiagnosticIIsReportedOnExternAliasAndUsingsInsideFileScopedNamespaceAsync()
        {
            var testCode = @"namespace Foo;
[|extern|] alias corlib;
[|using|] System;
using System.Linq;
using a = System.Collections;
";

            var fixedCode = @"namespace Foo;

extern alias corlib;

using System;
using System.Linq;
using a = System.Collections;
";

            await VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that SA1516 is reported extern alias, usings and member declarations
        /// inside a file scoped namespace.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [WorkItem(3512, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3512")]
        public async Task TestThatDiagnosticIIsReportedOnExternAliasUsingsAndMemberDeclarationsInsideFileScopedNamespaceAsync()
        {
            var testCode = @"namespace Foo;
[|extern|] alias corlib;
[|using|] System;
using System.Linq;
using a = System.Collections;
[|public|] class Bar
{
}
[|public|] enum Foobar
{
}
";

            var fixedCode = @"namespace Foo;

extern alias corlib;

using System;
using System.Linq;
using a = System.Collections;

public class Bar
{
}

public enum Foobar
{
}
";

            await VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
        }

        private static Task VerifyCSharpFixAsync(string testCode, string fixedCode)
        {
            var test = new CSharpTest()
            {
                ReferenceAssemblies = ReferenceAssemblies.Net.Net60,
                TestState =
                {
                    Sources = { testCode },
                },
                FixedCode = fixedCode,
            };

            return test.RunAsync(CancellationToken.None);
        }
    }
}
