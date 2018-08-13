// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.OrderingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using StyleCop.Analyzers.OrderingRules;
    using StyleCop.Analyzers.Settings.ObjectModel;
    using StyleCop.Analyzers.Test.Verifiers;
    using TestHelper;
    using Xunit;
    using static CombinedUsingDirectivesVerifier;

    /// <summary>
    /// Unit tests for the <see cref="UsingCodeFixProvider"/>.
    /// </summary>
    public class UsingCodeFixProviderUnitTests
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

            DiagnosticResult[] expected =
            {
                Diagnostic(SA1200UsingDirectivesMustBePlacedCorrectly.DescriptorInside).WithLocation(1, 1),
                Diagnostic(SA1200UsingDirectivesMustBePlacedCorrectly.DescriptorInside).WithLocation(2, 1),
                Diagnostic(SA1200UsingDirectivesMustBePlacedCorrectly.DescriptorInside).WithLocation(3, 1),
                DiagnosticVerifier<SA1216UsingStaticDirectivesMustBePlacedAtTheCorrectLocation>.Diagnostic().WithLocation(3, 1),
                Diagnostic(SA1200UsingDirectivesMustBePlacedCorrectly.DescriptorInside).WithLocation(4, 1),
                DiagnosticVerifier<SA1208SystemUsingDirectivesMustBePlacedBeforeOtherUsingDirectives>.Diagnostic().WithLocation(4, 1).WithArguments("System", "System.Math"),
                Diagnostic(SA1200UsingDirectivesMustBePlacedCorrectly.DescriptorInside).WithLocation(6, 1),
                Diagnostic(SA1200UsingDirectivesMustBePlacedCorrectly.DescriptorInside).WithLocation(7, 1),
                DiagnosticVerifier<SA1209UsingAliasDirectivesMustBePlacedAfterOtherUsingDirectives>.Diagnostic().WithLocation(7, 1),
                DiagnosticVerifier<SA1211UsingAliasDirectivesMustBeOrderedAlphabeticallyByAliasName>.Diagnostic().WithLocation(7, 1).WithArguments("MyFunc", "SystemAction"),
                Diagnostic(SA1200UsingDirectivesMustBePlacedCorrectly.DescriptorInside).WithLocation(9, 1),
                DiagnosticVerifier<SA1208SystemUsingDirectivesMustBePlacedBeforeOtherUsingDirectives>.Diagnostic().WithLocation(9, 1).WithArguments("System.Collections.Generic", "System.Math"),
                DiagnosticVerifier<SA1210UsingDirectivesMustBeOrderedAlphabeticallyByNamespace>.Diagnostic().WithLocation(9, 1),
                Diagnostic(SA1200UsingDirectivesMustBePlacedCorrectly.DescriptorInside).WithLocation(10, 1),
                DiagnosticVerifier<SA1208SystemUsingDirectivesMustBePlacedBeforeOtherUsingDirectives>.Diagnostic().WithLocation(10, 1).WithArguments("System.Collections", "System.Math"),
            };
            await this.VerifyCSharpFixAsync(testCode, expected, fixedTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            DiagnosticResult[] expected =
            {
                Diagnostic(SA1200UsingDirectivesMustBePlacedCorrectly.DescriptorInside).WithLocation(3, 1),
                Diagnostic(SA1200UsingDirectivesMustBePlacedCorrectly.DescriptorInside).WithLocation(4, 1),
                DiagnosticVerifier<SA1208SystemUsingDirectivesMustBePlacedBeforeOtherUsingDirectives>.Diagnostic().WithLocation(4, 1).WithArguments("System", "Microsoft.CodeAnalysis"),
            };
            await this.VerifyCSharpFixAsync(testCode, expected, fixedTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
using MyFunc = System.Func<int, bool>;
using SystemAction = System.Action;

namespace Foo
{
    public class Bar
    {
    }
}
";

            this.usingDirectivesPlacement = UsingDirectivesPlacement.OutsideNamespace;
            DiagnosticResult[] expected =
            {
                DiagnosticVerifier<SA1216UsingStaticDirectivesMustBePlacedAtTheCorrectLocation>.Diagnostic().WithLocation(3, 1),
                DiagnosticVerifier<SA1208SystemUsingDirectivesMustBePlacedBeforeOtherUsingDirectives>.Diagnostic().WithLocation(4, 1).WithArguments("System", "System.Math"),
                DiagnosticVerifier<SA1209UsingAliasDirectivesMustBePlacedAfterOtherUsingDirectives>.Diagnostic().WithLocation(7, 1),
                DiagnosticVerifier<SA1211UsingAliasDirectivesMustBeOrderedAlphabeticallyByAliasName>.Diagnostic().WithLocation(7, 1).WithArguments("MyFunc", "SystemAction"),
                DiagnosticVerifier<SA1208SystemUsingDirectivesMustBePlacedBeforeOtherUsingDirectives>.Diagnostic().WithLocation(9, 1).WithArguments("System.Collections.Generic", "System.Math"),
                DiagnosticVerifier<SA1210UsingDirectivesMustBeOrderedAlphabeticallyByNamespace>.Diagnostic().WithLocation(9, 1),
                DiagnosticVerifier<SA1208SystemUsingDirectivesMustBePlacedBeforeOtherUsingDirectives>.Diagnostic().WithLocation(10, 1).WithArguments("System.Collections", "System.Math"),
            };
            await this.VerifyCSharpFixAsync(testCode, expected, fixedTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            this.usingDirectivesPlacement = UsingDirectivesPlacement.OutsideNamespace;
            var expected = DiagnosticVerifier<SA1208SystemUsingDirectivesMustBePlacedBeforeOtherUsingDirectives>.Diagnostic().WithLocation(4, 1).WithArguments("System", "Microsoft.CodeAnalysis");
            await this.VerifyCSharpFixAsync(testCode, new[] { expected }, fixedTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            this.usingDirectivesPlacement = UsingDirectivesPlacement.OutsideNamespace;
            var expected = DiagnosticVerifier<SA1208SystemUsingDirectivesMustBePlacedBeforeOtherUsingDirectives>.Diagnostic().WithLocation(6, 1).WithArguments("System", "Microsoft.CodeAnalysis");
            await this.VerifyCSharpFixAsync(testCode, new[] { expected }, fixedTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            this.usingDirectivesPlacement = UsingDirectivesPlacement.OutsideNamespace;
            var expected = DiagnosticVerifier<SA1208SystemUsingDirectivesMustBePlacedBeforeOtherUsingDirectives>.Diagnostic().WithLocation(6, 1).WithArguments("System", "Microsoft.CodeAnalysis");
            await this.VerifyCSharpFixAsync(testCode, new[] { expected }, fixedTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            DiagnosticResult[] expected =
            {
                Diagnostic(SA1200UsingDirectivesMustBePlacedCorrectly.DescriptorInside).WithLocation(1, 1),
                Diagnostic(SA1200UsingDirectivesMustBePlacedCorrectly.DescriptorInside).WithLocation(2, 1),
                Diagnostic(SA1200UsingDirectivesMustBePlacedCorrectly.DescriptorInside).WithLocation(3, 1),
                DiagnosticVerifier<SA1216UsingStaticDirectivesMustBePlacedAtTheCorrectLocation>.Diagnostic().WithLocation(3, 1),
                Diagnostic(SA1200UsingDirectivesMustBePlacedCorrectly.DescriptorInside).WithLocation(4, 1),
                DiagnosticVerifier<SA1208SystemUsingDirectivesMustBePlacedBeforeOtherUsingDirectives>.Diagnostic().WithLocation(4, 1).WithArguments("System", "System.Math"),
                Diagnostic(SA1200UsingDirectivesMustBePlacedCorrectly.DescriptorInside).WithLocation(6, 1),
                Diagnostic(SA1200UsingDirectivesMustBePlacedCorrectly.DescriptorInside).WithLocation(7, 1),
                DiagnosticVerifier<SA1209UsingAliasDirectivesMustBePlacedAfterOtherUsingDirectives>.Diagnostic().WithLocation(7, 1),
                DiagnosticVerifier<SA1211UsingAliasDirectivesMustBeOrderedAlphabeticallyByAliasName>.Diagnostic().WithLocation(7, 1).WithArguments("MyFunc", "SystemAction"),
                Diagnostic(SA1200UsingDirectivesMustBePlacedCorrectly.DescriptorInside).WithLocation(9, 1),
                DiagnosticVerifier<SA1208SystemUsingDirectivesMustBePlacedBeforeOtherUsingDirectives>.Diagnostic().WithLocation(9, 1).WithArguments("System.Collections.Generic", "System.Math"),
                DiagnosticVerifier<SA1210UsingDirectivesMustBeOrderedAlphabeticallyByNamespace>.Diagnostic().WithLocation(9, 1),
                Diagnostic(SA1200UsingDirectivesMustBePlacedCorrectly.DescriptorInside).WithLocation(10, 1),
                DiagnosticVerifier<SA1208SystemUsingDirectivesMustBePlacedBeforeOtherUsingDirectives>.Diagnostic().WithLocation(10, 1).WithArguments("System.Collections", "System.Math"),
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

            await this.VerifyCSharpFixAsync(testCode, expected, fixedTestCode, remainingDiagnostics, cancellationToken: CancellationToken.None).ConfigureAwait(false);
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

            DiagnosticResult[] expected =
            {
                DiagnosticVerifier<SA1216UsingStaticDirectivesMustBePlacedAtTheCorrectLocation>.Diagnostic().WithLocation(3, 1),
                DiagnosticVerifier<SA1208SystemUsingDirectivesMustBePlacedBeforeOtherUsingDirectives>.Diagnostic().WithLocation(4, 1).WithArguments("System.Reflection", "System.Math"),
                DiagnosticVerifier<SA1210UsingDirectivesMustBeOrderedAlphabeticallyByNamespace>.Diagnostic().WithLocation(4, 1),
                DiagnosticVerifier<SA1209UsingAliasDirectivesMustBePlacedAfterOtherUsingDirectives>.Diagnostic().WithLocation(7, 1),
                DiagnosticVerifier<SA1211UsingAliasDirectivesMustBeOrderedAlphabeticallyByAliasName>.Diagnostic().WithLocation(7, 1).WithArguments("MyFunc", "SystemAction"),
                DiagnosticVerifier<SA1208SystemUsingDirectivesMustBePlacedBeforeOtherUsingDirectives>.Diagnostic().WithLocation(9, 1).WithArguments("System.Collections.Generic", "System.Math"),
                DiagnosticVerifier<SA1210UsingDirectivesMustBeOrderedAlphabeticallyByNamespace>.Diagnostic().WithLocation(9, 1),
                DiagnosticVerifier<SA1208SystemUsingDirectivesMustBePlacedBeforeOtherUsingDirectives>.Diagnostic().WithLocation(10, 1).WithArguments("System.Collections", "System.Math"),
            };
            await this.VerifyCSharpFixAsync(testCode, expected, fixedTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
            var fixedTestCode = @"namespace TestNamespace1
{
    using System;

    public class TestClass1
    {
    }
}
";

            var expected = Diagnostic(SA1200UsingDirectivesMustBePlacedCorrectly.DescriptorInside).WithLocation(1, 1);
            await this.VerifyCSharpFixAsync(testCode, new[] { expected }, fixedTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            var expected = Diagnostic(SA1200UsingDirectivesMustBePlacedCorrectly.DescriptorInside).WithLocation(1, 1);
            await this.VerifyCSharpFixAsync(testCode, new[] { expected }, testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the code fix will handle using statements in the else part of a #if directive trivia.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [WorkItem(1528, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/1528")]
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
            var expected = DiagnosticVerifier<SA1210UsingDirectivesMustBeOrderedAlphabeticallyByNamespace>.Diagnostic().WithLocation(6, 5);
            await this.VerifyCSharpFixAsync(testCode, new[] { expected }, fixedTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the code fix will handle using statements with directive trivia outside of namespaces.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [WorkItem(1733, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/1733")]
        public async Task VerifyCodefixForDirectiveTriviaOutsideOfNamespacesAsync()
        {
            var testCode = @"// <copyright file=""Program.cs"" company=""PlaceholderCompany"" >
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

#if DEBUG
using Fish;
#else
using Fish.Face;
#endif
using System.Text;
using System;

namespace StyleCopBugRepro
{
    class Program
    {
        static void Main(string[] args)
        {
            Int32 q;
            Haddock h;
            StringBuilder sb;
        }
    }
}

namespace Fish
{
    public class Haddock { }

    namespace Face
    {
        public class Haddock { }
    }
}
";

            var fixedTestCode = @"// <copyright file=""Program.cs"" company=""PlaceholderCompany"" >
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

#if DEBUG
using Fish;
#else
using Fish.Face;
#endif
using System;
using System.Text;

namespace StyleCopBugRepro
{
    class Program
    {
        static void Main(string[] args)
        {
            Int32 q;
            Haddock h;
            StringBuilder sb;
        }
    }
}

namespace Fish
{
    public class Haddock { }

    namespace Face
    {
        public class Haddock { }
    }
}
";

            DiagnosticResult[] expected =
            {
                Diagnostic(SA1200UsingDirectivesMustBePlacedCorrectly.DescriptorInside).WithLocation(8, 1),
                Diagnostic(SA1200UsingDirectivesMustBePlacedCorrectly.DescriptorInside).WithLocation(10, 1),
                DiagnosticVerifier<SA1210UsingDirectivesMustBeOrderedAlphabeticallyByNamespace>.Diagnostic().WithLocation(10, 1),
                Diagnostic(SA1200UsingDirectivesMustBePlacedCorrectly.DescriptorInside).WithLocation(11, 1),
            };

            DiagnosticResult[] fixedExpected =
            {
                Diagnostic(SA1200UsingDirectivesMustBePlacedCorrectly.DescriptorInside).WithLocation(8, 1),
                Diagnostic(SA1200UsingDirectivesMustBePlacedCorrectly.DescriptorInside).WithLocation(10, 1),
                Diagnostic(SA1200UsingDirectivesMustBePlacedCorrectly.DescriptorInside).WithLocation(11, 1),
            };

            await this.VerifyCSharpFixAsync(testCode, expected, fixedTestCode, fixedExpected, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        private Task VerifyCSharpFixAsync(string source, DiagnosticResult[] expected, string fixedSource, DiagnosticResult[] remainingDiagnostics, CancellationToken cancellationToken)
        {
            string testSettings = $@"
{{
  ""settings"": {{
    ""orderingRules"": {{
      ""usingDirectivesPlacement"": ""{this.usingDirectivesPlacement}""
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
