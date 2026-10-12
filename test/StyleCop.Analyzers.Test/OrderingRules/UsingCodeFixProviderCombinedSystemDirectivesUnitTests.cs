// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.OrderingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.OrderingRules;
    using StyleCop.Analyzers.Settings.ObjectModel;
    using StyleCop.Analyzers.Test.Verifiers;
    using Xunit;
    using static CombinedUsingDirectivesVerifier;

    /// <summary>
    /// Unit tests for the <see cref="UsingCodeFixProvider"/> for the special case where
    /// <see cref="OrderingSettings.SystemUsingDirectivesFirst"/> is <see langword="false"/>.
    /// </summary>
    public class UsingCodeFixProviderCombinedSystemDirectivesUnitTests
    {
        private protected UsingDirectivesPlacement UsingDirectivesPlacement { get; set; }

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

            DiagnosticResult[] expected =
            {
                Diagnostic(SA1200UsingDirectivesMustBePlacedCorrectly.DescriptorInside).WithLocation(1, 1),
                Diagnostic(SA1200UsingDirectivesMustBePlacedCorrectly.DescriptorInside).WithLocation(2, 1),
                Diagnostic(SA1200UsingDirectivesMustBePlacedCorrectly.DescriptorInside).WithLocation(3, 1),
                StyleCopDiagnosticVerifier<SA1216UsingStaticDirectivesMustBePlacedAtTheCorrectLocation>.Diagnostic().WithLocation(3, 1),
                Diagnostic(SA1200UsingDirectivesMustBePlacedCorrectly.DescriptorInside).WithLocation(4, 1),
                Diagnostic(SA1200UsingDirectivesMustBePlacedCorrectly.DescriptorInside).WithLocation(6, 1),
                Diagnostic(SA1200UsingDirectivesMustBePlacedCorrectly.DescriptorInside).WithLocation(7, 1),
                StyleCopDiagnosticVerifier<SA1209UsingAliasDirectivesMustBePlacedAfterOtherUsingDirectives>.Diagnostic().WithLocation(7, 1),
                StyleCopDiagnosticVerifier<SA1211UsingAliasDirectivesMustBeOrderedAlphabeticallyByAliasName>.Diagnostic().WithLocation(7, 1).WithArguments("MyFunc", "SystemAction"),
                Diagnostic(SA1200UsingDirectivesMustBePlacedCorrectly.DescriptorInside).WithLocation(9, 1),
                StyleCopDiagnosticVerifier<SA1210UsingDirectivesMustBeOrderedAlphabeticallyByNamespace>.Diagnostic().WithLocation(9, 1),
                Diagnostic(SA1200UsingDirectivesMustBePlacedCorrectly.DescriptorInside).WithLocation(10, 1),
            };
            await this.VerifyCSharpFixAsync(testCode, expected, fixedTestCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            DiagnosticResult[] expected =
            {
                Diagnostic(SA1200UsingDirectivesMustBePlacedCorrectly.DescriptorInside).WithLocation(3, 1),
                StyleCopDiagnosticVerifier<SA1210UsingDirectivesMustBeOrderedAlphabeticallyByNamespace>.Diagnostic().WithLocation(3, 1),
                Diagnostic(SA1200UsingDirectivesMustBePlacedCorrectly.DescriptorInside).WithLocation(4, 1),
            };
            await this.VerifyCSharpFixAsync(testCode, expected, fixedTestCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            this.UsingDirectivesPlacement = UsingDirectivesPlacement.OutsideNamespace;
            DiagnosticResult[] expected =
            {
                StyleCopDiagnosticVerifier<SA1216UsingStaticDirectivesMustBePlacedAtTheCorrectLocation>.Diagnostic().WithLocation(3, 1),
                StyleCopDiagnosticVerifier<SA1209UsingAliasDirectivesMustBePlacedAfterOtherUsingDirectives>.Diagnostic().WithLocation(7, 1),
                StyleCopDiagnosticVerifier<SA1211UsingAliasDirectivesMustBeOrderedAlphabeticallyByAliasName>.Diagnostic().WithLocation(7, 1).WithArguments("MyFunc", "SystemAction"),
                StyleCopDiagnosticVerifier<SA1210UsingDirectivesMustBeOrderedAlphabeticallyByNamespace>.Diagnostic().WithLocation(9, 1),
            };
            await this.VerifyCSharpFixAsync(testCode, expected, fixedTestCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            this.UsingDirectivesPlacement = UsingDirectivesPlacement.OutsideNamespace;
            var expected = StyleCopDiagnosticVerifier<SA1210UsingDirectivesMustBeOrderedAlphabeticallyByNamespace>.Diagnostic().WithLocation(3, 1);
            await this.VerifyCSharpFixAsync(testCode, new[] { expected }, fixedTestCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            this.UsingDirectivesPlacement = UsingDirectivesPlacement.OutsideNamespace;
            var expected = StyleCopDiagnosticVerifier<SA1210UsingDirectivesMustBeOrderedAlphabeticallyByNamespace>.Diagnostic().WithLocation(5, 1);
            await this.VerifyCSharpFixAsync(testCode, new[] { expected }, fixedTestCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            this.UsingDirectivesPlacement = UsingDirectivesPlacement.OutsideNamespace;
            var expected = StyleCopDiagnosticVerifier<SA1210UsingDirectivesMustBeOrderedAlphabeticallyByNamespace>.Diagnostic().WithLocation(5, 1);
            await this.VerifyCSharpFixAsync(testCode, new[] { expected }, fixedTestCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            DiagnosticResult[] expected =
            {
                Diagnostic(SA1200UsingDirectivesMustBePlacedCorrectly.DescriptorInside).WithLocation(1, 1),
                Diagnostic(SA1200UsingDirectivesMustBePlacedCorrectly.DescriptorInside).WithLocation(2, 1),
                Diagnostic(SA1200UsingDirectivesMustBePlacedCorrectly.DescriptorInside).WithLocation(3, 1),
                StyleCopDiagnosticVerifier<SA1216UsingStaticDirectivesMustBePlacedAtTheCorrectLocation>.Diagnostic().WithLocation(3, 1),
                Diagnostic(SA1200UsingDirectivesMustBePlacedCorrectly.DescriptorInside).WithLocation(4, 1),
                Diagnostic(SA1200UsingDirectivesMustBePlacedCorrectly.DescriptorInside).WithLocation(6, 1),
                Diagnostic(SA1200UsingDirectivesMustBePlacedCorrectly.DescriptorInside).WithLocation(7, 1),
                StyleCopDiagnosticVerifier<SA1209UsingAliasDirectivesMustBePlacedAfterOtherUsingDirectives>.Diagnostic().WithLocation(7, 1),
                StyleCopDiagnosticVerifier<SA1211UsingAliasDirectivesMustBeOrderedAlphabeticallyByAliasName>.Diagnostic().WithLocation(7, 1).WithArguments("MyFunc", "SystemAction"),
                Diagnostic(SA1200UsingDirectivesMustBePlacedCorrectly.DescriptorInside).WithLocation(9, 1),
                StyleCopDiagnosticVerifier<SA1210UsingDirectivesMustBeOrderedAlphabeticallyByNamespace>.Diagnostic().WithLocation(9, 1),
                Diagnostic(SA1200UsingDirectivesMustBePlacedCorrectly.DescriptorInside).WithLocation(10, 1),
            };

            // The code fix is not able to correct all violations due to the use of multiple namespaces in a single file
            DiagnosticResult[] remainingDiagnostics =
            {
                Diagnostic(SA1200UsingDirectivesMustBePlacedCorrectly.DescriptorInside).WithLocation(1, 1),
                Diagnostic(SA1200UsingDirectivesMustBePlacedCorrectly.DescriptorInside).WithLocation(2, 1),
                Diagnostic(SA1200UsingDirectivesMustBePlacedCorrectly.DescriptorInside).WithLocation(3, 1),
                Diagnostic(SA1200UsingDirectivesMustBePlacedCorrectly.DescriptorInside).WithLocation(4, 1),
                Diagnostic(SA1200UsingDirectivesMustBePlacedCorrectly.DescriptorInside).WithLocation(5, 1),
                Diagnostic(SA1200UsingDirectivesMustBePlacedCorrectly.DescriptorInside).WithLocation(6, 1),
                Diagnostic(SA1200UsingDirectivesMustBePlacedCorrectly.DescriptorInside).WithLocation(7, 1),
                Diagnostic(SA1200UsingDirectivesMustBePlacedCorrectly.DescriptorInside).WithLocation(8, 1),
            };

            await this.VerifyCSharpFixAsync(testCode, expected, fixedTestCode, remainingDiagnostics, CancellationToken.None).ConfigureAwait(false);
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

            DiagnosticResult[] expected =
            {
                StyleCopDiagnosticVerifier<SA1216UsingStaticDirectivesMustBePlacedAtTheCorrectLocation>.Diagnostic().WithLocation(3, 1),
                StyleCopDiagnosticVerifier<SA1210UsingDirectivesMustBeOrderedAlphabeticallyByNamespace>.Diagnostic().WithLocation(4, 1),
                StyleCopDiagnosticVerifier<SA1209UsingAliasDirectivesMustBePlacedAfterOtherUsingDirectives>.Diagnostic().WithLocation(7, 1),
                StyleCopDiagnosticVerifier<SA1211UsingAliasDirectivesMustBeOrderedAlphabeticallyByAliasName>.Diagnostic().WithLocation(7, 1).WithArguments("MyFunc", "SystemAction"),
                StyleCopDiagnosticVerifier<SA1210UsingDirectivesMustBeOrderedAlphabeticallyByNamespace>.Diagnostic().WithLocation(9, 1),
            };
            await this.VerifyCSharpFixAsync(testCode, expected, fixedTestCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        protected Task VerifyCSharpFixAsync(string source, DiagnosticResult[] expected, string fixedSource, DiagnosticResult[] remainingDiagnostics, CancellationToken cancellationToken)
        {
            string testSettings = $@"
{{
  ""settings"": {{
    ""orderingRules"": {{
      ""systemUsingDirectivesFirst"": false,
      ""usingDirectivesPlacement"": ""{this.UsingDirectivesPlacement}""
    }}
  }}
}}
";

            var test = new CSharpTest
            {
                TestCode = source,
                FixedCode = fixedSource,
                Settings = testSettings,
            };

            test.ExpectedDiagnostics.AddRange(expected);
            test.RemainingDiagnostics.AddRange(remainingDiagnostics);
            return test.RunAsync(cancellationToken);
        }
    }
}
