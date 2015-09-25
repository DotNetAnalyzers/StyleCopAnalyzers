// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.OrderingRules
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.OrderingRules;
    using TestHelper;
    using Xunit;

    /// <summary>
    /// Unit tests for the <see cref="UsingCodeFixProvider"/>.
    /// </summary>
    public class UsingCodeFixProviderUnitTests : CodeFixVerifier
    {
        private bool suppressSA1200;

        /// <summary>
        /// Verifies that the code fix will properly reorder using statements.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task VerifyUsingReorderingAsync()
        {
            var testCode = @"using Microsoft.CodeAnalysis;
using SystemAction = System.Action;
using static System.Math;
using System;

using static System.String;
using MyFunc = System.Func<int,bool>;

using System.Collections.Generic;
using System.Collections;

namespace Foo
{
    public class Bar
    {
    }
}
";

            var fixedTestCode = @"namespace Foo
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using Microsoft.CodeAnalysis;
    using static System.Math;
    using static System.String;
    using MyFunc = System.Func<int,bool>;
    using SystemAction = System.Action;

    public class Bar
    {
    }
}
";

            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the code fix will properly reorder using statements, but will not move a file header comment.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task VerifyUsingReorderingWithFileHeaderAsync()
        {
            var testCode = @"// This is a file header.

using Microsoft.CodeAnalysis;
using System;

namespace Foo
{
    public class Bar
    {
    }
}
";

            var fixedTestCode = @"// This is a file header.

namespace Foo
{
    using System;
    using Microsoft.CodeAnalysis;

    public class Bar
    {
    }
}
";

            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the code fix will properly reorder using statements, without moving them inside a namespace when SA1200 is suppressed.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task VerifyUsingReorderingWithoutMovingAsync()
        {
            var testCode = @"using Microsoft.CodeAnalysis;
using SystemAction = System.Action;
using static System.Math;
using System;

using static System.String;
using MyFunc = System.Func<int,bool>;

using System.Collections.Generic;
using System.Collections;

namespace Foo
{
    public class Bar
    {
    }
}
";

            var fixedTestCode = @"using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using static System.Math;
using static System.String;
using MyFunc = System.Func<int,bool>;
using SystemAction = System.Action;

namespace Foo
{
    public class Bar
    {
    }
}
";

            this.suppressSA1200 = true;
            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the code fix will properly reorder using statements, without moving them inside a namespace
        /// when SA1200 is suppressed. The file header is not moved by the code fix.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task VerifyUsingReorderingWithoutMovingWithFileHeaderAsync()
        {
            var testCode = @"// This is a file header.

using Microsoft.CodeAnalysis;
using System;

namespace Foo
{
    public class Bar
    {
    }
}
";

            var fixedTestCode = @"// This is a file header.

using System;
using Microsoft.CodeAnalysis;

namespace Foo
{
    public class Bar
    {
    }
}
";

            this.suppressSA1200 = true;
            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the code fix will properly reorder using statements, without moving them inside a namespace
        /// when SA1200 is suppressed. The file header is not moved by the code fix.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task VerifyUsingReorderingWithoutMovingWithMultiLineFileHeaderAsync()
        {
            var testCode = @"// <copyright file=""Test0.cs"" company=""FooCorp"">
//   Copyright (c) FooCorp. All rights reserved.
// </copyright>

using Microsoft.CodeAnalysis;
using System;

namespace Foo
{
    public class Bar
    {
    }
}
";

            var fixedTestCode = @"// <copyright file=""Test0.cs"" company=""FooCorp"">
//   Copyright (c) FooCorp. All rights reserved.
// </copyright>

using System;
using Microsoft.CodeAnalysis;

namespace Foo
{
    public class Bar
    {
    }
}
";

            this.suppressSA1200 = true;
            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the code fix will properly reorder using statements, without moving them inside a namespace
        /// when SA1200 is suppressed. The file header is not moved by the code fix.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task VerifyUsingReorderingWithoutMovingWithMultiLineCommentAsync()
        {
            var testCode = @"/*
 * Copyright by FooCorp Inc.
 */

using Microsoft.CodeAnalysis;
using System;

namespace Foo
{
    public class Bar
    {
    }
}
";

            var fixedTestCode = @"/*
 * Copyright by FooCorp Inc.
 */

using System;
using Microsoft.CodeAnalysis;

namespace Foo
{
    public class Bar
    {
    }
}
";

            this.suppressSA1200 = true;
            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the code fix will properly reorder using statements, without moving them inside a namespace when there are multiple namespaces.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task VerifyUsingReorderingWithMultipleNamespacesAsync()
        {
            var testCode = @"using Microsoft.CodeAnalysis;
using SystemAction = System.Action;
using static System.Math;
using System;

using static System.String;
using MyFunc = System.Func<int,bool>;

using System.Collections.Generic;
using System.Collections;

namespace TestNamespace1
{
    public class TestClass1
    {
    }
}

namespace TestNamespace2
{
}
";

            var fixedTestCode = @"using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using static System.Math;
using static System.String;
using MyFunc = System.Func<int,bool>;
using SystemAction = System.Action;

namespace TestNamespace1
{
    public class TestClass1
    {
    }
}

namespace TestNamespace2
{
}
";

            // The code fix is not able to correct all violations due to the use of multiple namespaces in a single file
            DiagnosticResult[] expected =
            {
                this.CSharpDiagnostic(SA1200UsingDirectivesMustBePlacedWithinNamespace.DiagnosticId).WithLocation(1, 1),
                this.CSharpDiagnostic(SA1200UsingDirectivesMustBePlacedWithinNamespace.DiagnosticId).WithLocation(2, 1),
                this.CSharpDiagnostic(SA1200UsingDirectivesMustBePlacedWithinNamespace.DiagnosticId).WithLocation(3, 1),
                this.CSharpDiagnostic(SA1200UsingDirectivesMustBePlacedWithinNamespace.DiagnosticId).WithLocation(4, 1),
                this.CSharpDiagnostic(SA1200UsingDirectivesMustBePlacedWithinNamespace.DiagnosticId).WithLocation(5, 1),
                this.CSharpDiagnostic(SA1200UsingDirectivesMustBePlacedWithinNamespace.DiagnosticId).WithLocation(6, 1),
                this.CSharpDiagnostic(SA1200UsingDirectivesMustBePlacedWithinNamespace.DiagnosticId).WithLocation(7, 1),
                this.CSharpDiagnostic(SA1200UsingDirectivesMustBePlacedWithinNamespace.DiagnosticId).WithLocation(8, 1),
            };

            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the code fix will leave using statements needed for global attributes at the appropriate location.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task VerifyUsingReorderingWithGlobalAttributesAsync()
        {
            var testCode = @"using Microsoft.CodeAnalysis;
using SystemAction = System.Action;
using static System.Math;
using System.Reflection;

using static System.String;
using MyFunc = System.Func<int,bool>;

using System.Collections.Generic;
using System.Collections;

[assembly: AssemblyVersion(""1.0.0.0"")]

namespace Foo
{
    public class Bar
    {
    }
}
";

            var fixedTestCode = @"using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.CodeAnalysis;
using static System.Math;
using static System.String;
using MyFunc = System.Func<int,bool>;
using SystemAction = System.Action;

[assembly: AssemblyVersion(""1.0.0.0"")]

namespace Foo
{
    public class Bar
    {
    }
}
";

            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a code fix will  be offered for SA1200 diagnostics when a single namespace is present.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task VerifyCodefixForSA1200WithSingleNamespaceAsync()
        {
            var testCode = @"using System;

namespace TestNamespace1
{
    public class TestClass1
    {
    }
}
";

            var offeredFixes = await this.GetOfferedCSharpFixesAsync(testCode).ConfigureAwait(false);
            Assert.NotEmpty(offeredFixes);
        }

        /// <summary>
        /// Verifies that a code fix will not be offered for SA1200 diagnostics when multiple namespaces are present.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task VerifyNoCodefixForSA1200WithMultipleNamespacesAsync()
        {
            var testCode = @"using System;

namespace TestNamespace1
{
    public class TestClass1
    {
    }
}

namespace TestNamespace2
{
}
";

            var offeredFixes = await this.GetOfferedCSharpFixesAsync(testCode).ConfigureAwait(false);
            Assert.Empty(offeredFixes);
        }

        /// <summary>
        /// Verifies that the code fix will handle using statements in the else part of a #if directive trivia.
        /// This is a regression test for #1528
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task VerifyCodefixForElsePartOfDirectiveTriviaAsync()
        {
            var testCode = @"namespace NamespaceName
{
#if false
    using System.Runtime.CompilerServices;
#else
    using System.Collections.Generic;
    using System.Collections.Concurrent;
#endif
}
";

            var fixedTestCode = @"namespace NamespaceName
{
#if false
    using System.Runtime.CompilerServices;
#else
    using System.Collections.Concurrent;
    using System.Collections.Generic;
#endif
}
";
            var expected = this.CSharpDiagnostic(SA1210UsingDirectivesMustBeOrderedAlphabeticallyByNamespace.DiagnosticId).WithLocation(6, 5);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        protected override IEnumerable<string> GetDisabledDiagnostics()
        {
            if (this.suppressSA1200)
            {
                yield return SA1200UsingDirectivesMustBePlacedWithinNamespace.DiagnosticId;
            }
        }

        /// <inheritdoc/>
        protected override IEnumerable<DiagnosticAnalyzer> GetCSharpDiagnosticAnalyzers()
        {
            yield return new SA1200UsingDirectivesMustBePlacedWithinNamespace();
            yield return new SA1208SystemUsingDirectivesMustBePlacedBeforeOtherUsingDirectives();
            yield return new SA1209UsingAliasDirectivesMustBePlacedAfterOtherUsingDirectives();
            yield return new SA1210UsingDirectivesMustBeOrderedAlphabeticallyByNamespace();
            yield return new SA1211UsingAliasDirectivesMustBeOrderedAlphabeticallyByAliasName();
            yield return new SA1216UsingStaticDirectivesMustBePlacedAtTheCorrectLocation();
            yield return new SA1217UsingStaticDirectivesMustBeOrderedAlphabetically();
        }

        /// <inheritdoc/>
        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new UsingCodeFixProvider();
        }
    }
}
