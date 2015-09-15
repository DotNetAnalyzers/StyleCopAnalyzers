// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.LayoutRules
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.LayoutRules;
    using TestHelper;
    using Xunit;

    public class SA1515UnitTests : CodeFixVerifier
    {
        /// <summary>
        /// Verifies that all known types valid single line comment lines will not produce a diagnostic.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestValidSingleLineCommentsAsync()
        {
            var testCode = @"// A single line comment at the start of the file is valid
namespace Foo
{
    public class Bar
    {
        // A single line comment at the start of the scope is valid
        private int field1;

        // This is valid as well ofcourse
        private int field2;
        private int field3; // This should not trigger ofcourse

#if (SPECIALTEST)
        private int field4;
#else
        // this is also allowed
        private double field4;
#endif

        // This is valid ofcourse
        private int field5;

        // Multiple single line comments
        // directly after each other are valid as well
        public int Baz()
        {
            var x = field1;
            ////return 0;
            return x;
        }

        public void Qux()
        {
            switch (this.Baz())
            {
                case 1:
                    // Single line comment after case statement is valid
                    break;
                default:
                    // Single line comment after default statement is valid
                    return;
            }
        }
    }
}
";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that invalid single line comment lines will produce a diagnostic.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestInvalidSingleLineCommentsAsync()
        {
            var testCode = @"namespace Foo
{
    public class Bar
    {
        private int field1;
        // This is invalid
        private int field2;

#if (SPECIALTEST)
        private int field3;
#endif
        // This is invalid #2
        private int field4;
    }
}
";

            var fixedTestCode = @"namespace Foo
{
    public class Bar
    {
        private int field1;

        // This is invalid
        private int field2;

#if (SPECIALTEST)
        private int field3;
#endif

        // This is invalid #2
        private int field4;
    }
}
";

            DiagnosticResult[] expectedDiagnostic =
            {
                this.CSharpDiagnostic().WithLocation(6, 9),
                this.CSharpDiagnostic().WithLocation(12, 9)
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expectedDiagnostic, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that an invalid single line comment line within a disabled conditional directive will not produce a diagnostic.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestSingleLineCommentWithinConditionalDirectiveAsync()
        {
            var testCode = @"// A single line comment at the start of the file is valid
namespace Foo
{
    public class Bar
    {
#if (SPECIALTEST)
        private int field1;
        // This is invalid 
        private int field2;
#endif

        public int Baz()
        {
            return 0;
        }
    }
}
";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// This is a regression test for DotNetAnalyzers/StyleCopAnalyzers#1083 "SA1515 fires after some directives
        /// when it shouldn't".
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestPreprocessorInteractionAsync()
        {
            string testCode = @"#region Test

// Inside region
using System;

#endregion

// After region
#pragma warning restore 1234

// After pragma
using System.Reflection;

#if DEBUG

// After if
using System.Resources;

#endif

// After endif
using System.Runtime.InteropServices;
";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies the analyzer will properly handle documentation followed by a comment.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestDocumentationFollowedByCommentAsync()
        {
            var testCode = @"
/// <summary>some documentation</summary>
// some comment
public class TestClass
{
    /// <summary>more documentation.</summary>
    // another comment
    public void TestMethod() { }
}
";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        protected override IEnumerable<DiagnosticAnalyzer> GetCSharpDiagnosticAnalyzers()
        {
            yield return new SA1515SingleLineCommentMustBePrecededByBlankLine();
        }

        /// <inheritdoc/>
        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new SA1515CodeFixProvider();
        }
    }
}
