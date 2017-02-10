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
    using StyleCop.Analyzers.Settings.ObjectModel;
    using TestHelper;
    using Xunit;

    /// <summary>
    /// Unit tests for the <see cref="UsingCodeFixProvider"/> for the special case where
    /// <see cref="OrderingSettings.SystemUsingDirectivesFirst"/> is <see langword="false"/>.
    /// </summary>
    public class UsingCodeFixProviderCombinedSystemDirectivesUnitTests : CodeFixVerifier
    {
        private UsingDirectivesPlacement usingDirectivesPlacement;

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

namespace NamespaceName
{
    public class Bar
    {
    }
}
";

            var fixedTestCode = @"namespace NamespaceName
{
    using Microsoft.CodeAnalysis;
    using System;
    using System.Collections;
    using System.Collections.Generic;
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

using System;
using Microsoft.CodeAnalysis;

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
    using Microsoft.CodeAnalysis;
    using System;

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
using MyFunc = System.Func<int, bool>;

using System.Collections.Generic;
using System.Collections;

namespace NamespaceName
{
    public class Bar
    {
    }
}
";

            var fixedTestCode = @"using Microsoft.CodeAnalysis;
using System;
using System.Collections;
using System.Collections.Generic;
using static System.Math;
using static System.String;
using MyFunc = System.Func<int, bool>;
using SystemAction = System.Action;

namespace NamespaceName
{
    public class Bar
    {
    }
}
";

            this.usingDirectivesPlacement = UsingDirectivesPlacement.OutsideNamespace;
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

using System;
using Microsoft.CodeAnalysis;

namespace Foo
{
    public class Bar
    {
    }
}
";

            var fixedTestCode = @"// This is a file header.

using Microsoft.CodeAnalysis;
using System;

namespace Foo
{
    public class Bar
    {
    }
}
";

            this.usingDirectivesPlacement = UsingDirectivesPlacement.OutsideNamespace;
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

using System;
using Microsoft.CodeAnalysis;

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

using Microsoft.CodeAnalysis;
using System;

namespace Foo
{
    public class Bar
    {
    }
}
";

            this.usingDirectivesPlacement = UsingDirectivesPlacement.OutsideNamespace;
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

using System;
using Microsoft.CodeAnalysis;

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

using Microsoft.CodeAnalysis;
using System;

namespace Foo
{
    public class Bar
    {
    }
}
";

            this.usingDirectivesPlacement = UsingDirectivesPlacement.OutsideNamespace;
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

            var fixedTestCode = @"using Microsoft.CodeAnalysis;
using System;
using System.Collections;
using System.Collections.Generic;
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
                this.CSharpDiagnostic(SA1200UsingDirectivesMustBePlacedCorrectly.DiagnosticId).WithLocation(1, 1),
                this.CSharpDiagnostic(SA1200UsingDirectivesMustBePlacedCorrectly.DiagnosticId).WithLocation(2, 1),
                this.CSharpDiagnostic(SA1200UsingDirectivesMustBePlacedCorrectly.DiagnosticId).WithLocation(3, 1),
                this.CSharpDiagnostic(SA1200UsingDirectivesMustBePlacedCorrectly.DiagnosticId).WithLocation(4, 1),
                this.CSharpDiagnostic(SA1200UsingDirectivesMustBePlacedCorrectly.DiagnosticId).WithLocation(5, 1),
                this.CSharpDiagnostic(SA1200UsingDirectivesMustBePlacedCorrectly.DiagnosticId).WithLocation(6, 1),
                this.CSharpDiagnostic(SA1200UsingDirectivesMustBePlacedCorrectly.DiagnosticId).WithLocation(7, 1),
                this.CSharpDiagnostic(SA1200UsingDirectivesMustBePlacedCorrectly.DiagnosticId).WithLocation(8, 1),
            };

            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode, numberOfFixAllIterations: 2, cancellationToken: CancellationToken.None).ConfigureAwait(false);
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

namespace NamespaceName
{
    public class Bar
    {
    }
}
";

            var fixedTestCode = @"using Microsoft.CodeAnalysis;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using static System.Math;
using static System.String;
using MyFunc = System.Func<int,bool>;
using SystemAction = System.Action;

[assembly: AssemblyVersion(""1.0.0.0"")]

namespace NamespaceName
{
    public class Bar
    {
    }
}
";

            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        protected override string GetSettings()
        {
            string testSettings = $@"
{{
  ""settings"": {{
    ""orderingRules"": {{
      ""systemUsingDirectivesFirst"": false,
      ""usingDirectivesPlacement"": ""{this.usingDirectivesPlacement}""
    }}
  }}
}}
";

            return testSettings;
        }

        /// <inheritdoc/>
        protected override IEnumerable<DiagnosticAnalyzer> GetCSharpDiagnosticAnalyzers()
        {
            yield return new SA1200UsingDirectivesMustBePlacedCorrectly();
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
