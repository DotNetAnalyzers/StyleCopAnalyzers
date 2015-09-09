﻿namespace StyleCop.Analyzers.Test.OrderingRules
{
    using System.Collections.Generic;
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

    using MyFunc = System.Func<int,bool>;
    using SystemAction = System.Action;

    using static System.Math;
    using static System.String;

    public class Bar
    {
    }
}
";

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

using MyFunc = System.Func<int,bool>;
using SystemAction = System.Action;

using static System.Math;
using static System.String;

namespace Foo
{
    public class Bar
    {
    }
}
";

            this.suppressSA1200 = true;
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

using MyFunc = System.Func<int,bool>;
using SystemAction = System.Action;

using static System.Math;
using static System.String;

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

using MyFunc = System.Func<int,bool>;
using SystemAction = System.Action;

using static System.Math;
using static System.String;

[assembly: AssemblyVersion(""1.0.0.0"")]

namespace Foo
{
    public class Bar
    {
    }
}
";

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
            yield return new SA1216UsingStaticDirectivesMustBePlacedAfterOtherUsingDirectives();
            yield return new SA1217UsingStaticDirectivesMustBeOrderedAlphabetically();
        }

        /// <inheritdoc/>
        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new UsingCodeFixProvider();
        }
    }
}
