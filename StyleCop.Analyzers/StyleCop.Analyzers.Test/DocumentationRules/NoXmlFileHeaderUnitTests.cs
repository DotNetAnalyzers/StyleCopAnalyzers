// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.DocumentationRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.DocumentationRules;
    using StyleCop.Analyzers.Test.Verifiers;
    using TestHelper;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.CustomDiagnosticVerifier<StyleCop.Analyzers.DocumentationRules.FileHeaderAnalyzers>;

    /// <summary>
    /// Unit tests for file header that do not follow the XML syntax.
    /// </summary>
    public class NoXmlFileHeaderUnitTests
    {
        private const string TestSettings = @"
{
  ""settings"": {
    ""documentationRules"": {
      ""companyName"": ""FooCorp"",
      ""copyrightText"": ""Copyright (c) {companyName}. All rights reserved.\nLicensed under the {licenseName} license. See {licenseFile} file in the project root for full license information."",
      ""variables"": {
        ""licenseName"": ""???"",
        ""licenseFile"": ""LICENSE""
      },
      ""xmlHeader"": false
    }
  }
}
";

        private const string TestSettingsWithEmptyLines = @"
{
  ""settings"": {
    ""documentationRules"": {
      ""companyName"": ""FooCorp"",
      ""copyrightText"": ""\nCopyright (c) {companyName}. All rights reserved.\n\nLicensed under the {licenseName} license. See {licenseFile} file in the project root for full license information.\n"",
      ""variables"": {
        ""licenseName"": ""???"",
        ""licenseFile"": ""LICENSE""
      },
      ""xmlHeader"": false
    }
  }
}
";

        /// <summary>
        /// Verifies that the analyzer will report <see cref="FileHeaderAnalyzers.SA1633DescriptorMissing"/> for
        /// projects not using XML headers when the file is completely missing a header.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public virtual async Task TestNoFileHeaderAsync()
        {
            var testCode = @"namespace Foo
{
}
";
            var fixedCode = @"// Copyright (c) FooCorp. All rights reserved.
// Licensed under the ??? license. See LICENSE file in the project root for full license information.

namespace Foo
{
}
";

            var expectedDiagnostic = Diagnostic(FileHeaderAnalyzers.SA1633DescriptorMissing).WithLocation(1, 1);
            await VerifyCSharpFixAsync(testCode, expectedDiagnostic, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the analyzer will report <see cref="FileHeaderAnalyzers.SA1633DescriptorMissing"/> for
        /// projects not using XML headers when the file is completely missing a header.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [WorkItem(2415, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/2415")]
        public virtual async Task TestNoFileHeaderWithUsingDirectiveAsync()
        {
            var testCode = @"using System;

namespace Foo
{
}
";
            var fixedCode = @"// Copyright (c) FooCorp. All rights reserved.
// Licensed under the ??? license. See LICENSE file in the project root for full license information.

using System;

namespace Foo
{
}
";

            var expectedDiagnostic = Diagnostic(FileHeaderAnalyzers.SA1633DescriptorMissing).WithLocation(1, 1);
            await VerifyCSharpFixAsync(testCode, expectedDiagnostic, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the analyzer will report <see cref="FileHeaderAnalyzers.SA1633DescriptorMissing"/> for
        /// projects not using XML headers when the file is completely missing a header.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [WorkItem(2415, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/2415")]
        public virtual async Task TestNoFileHeaderWithBlankLineAndUsingDirectiveAsync()
        {
            var testCode = @"
using System;

namespace Foo
{
}
";
            var fixedCode = @"// Copyright (c) FooCorp. All rights reserved.
// Licensed under the ??? license. See LICENSE file in the project root for full license information.

using System;

namespace Foo
{
}
";

            var expectedDiagnostic = Diagnostic(FileHeaderAnalyzers.SA1633DescriptorMissing).WithLocation(1, 1);
            await VerifyCSharpFixAsync(testCode, expectedDiagnostic, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the analyzer will report <see cref="FileHeaderAnalyzers.SA1633DescriptorMissing"/> for
        /// projects not using XML headers when the file is completely missing a header.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [WorkItem(2415, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/2415")]
        public virtual async Task TestNoFileHeaderWithWhitespaceLineAsync()
        {
            var testCode = "    " + @"
using System;

namespace Foo
{
}
";
            var fixedCode = @"// Copyright (c) FooCorp. All rights reserved.
// Licensed under the ??? license. See LICENSE file in the project root for full license information.

using System;

namespace Foo
{
}
";

            var expectedDiagnostic = Diagnostic(FileHeaderAnalyzers.SA1633DescriptorMissing).WithLocation(1, 1);
            await VerifyCSharpFixAsync(testCode, expectedDiagnostic, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the built-in variable <c>fileName</c> works as expected.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public virtual async Task TestFileNameBuiltInVariableAsync()
        {
            var testSettings = @"
{
  ""settings"": {
    ""documentationRules"": {
      ""companyName"": ""FooCorp"",
      ""copyrightText"": ""{fileName} Copyright (c) {companyName}. All rights reserved.\nLicensed under the {licenseName} license. See {licenseFile} file in the project root for full license information."",
      ""variables"": {
        ""licenseName"": ""???"",
        ""licenseFile"": ""LICENSE""
      },
      ""xmlHeader"": false
    }
  }
}
";

            var testCode = @"namespace Foo
{
}
";
            var fixedCode = @"// Test0.cs Copyright (c) FooCorp. All rights reserved.
// Licensed under the ??? license. See LICENSE file in the project root for full license information.

namespace Foo
{
}
";

            var expectedDiagnostic = Diagnostic(FileHeaderAnalyzers.SA1633DescriptorMissing).WithLocation(1, 1);
            await VerifyCSharpFixAsync(testCode, testSettings, new[] { expectedDiagnostic }, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a used-defined replacement variable <c>fileName</c> works as expected.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public virtual async Task TestFileNameUserVariableAsync()
        {
            var testSettings = @"
{
  ""settings"": {
    ""documentationRules"": {
      ""companyName"": ""FooCorp"",
      ""copyrightText"": ""{fileName} Copyright (c) {companyName}. All rights reserved.\nLicensed under the {licenseName} license. See {licenseFile} file in the project root for full license information."",
      ""variables"": {
        ""licenseName"": ""???"",
        ""licenseFile"": ""LICENSE"",
        ""fileName"": ""Not a file""
      },
      ""xmlHeader"": false
    }
  }
}
";

            var testCode = @"namespace Foo
{
}
";
            var fixedCode = @"// Not a file Copyright (c) FooCorp. All rights reserved.
// Licensed under the ??? license. See LICENSE file in the project root for full license information.

namespace Foo
{
}
";

            var expectedDiagnostic = Diagnostic(FileHeaderAnalyzers.SA1633DescriptorMissing).WithLocation(1, 1);
            await VerifyCSharpFixAsync(testCode, testSettings, new[] { expectedDiagnostic }, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a file header with an auto-generated comment will not produce a diagnostic message.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestAutoGeneratedSourceFileAsync()
        {
            var testCode = @"// <auto-generated/>

namespace Bar
{
}
";

            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a valid file header built using single line comments will not produce a diagnostic message.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestValidFileHeaderWithSingleLineCommentsAsync()
        {
            var testCode = @"// Copyright (c) FooCorp. All rights reserved.
// Licensed under the ??? license. See LICENSE file in the project root for full license information.

namespace Bar
{
}
";

            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a valid file header built using multi-line comments will not produce a diagnostic message.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestValidFileHeaderWithMultiLineComments1Async()
        {
            var testCode = @"/* Copyright (c) FooCorp. All rights reserved.
 * Licensed under the ??? license. See LICENSE file in the project root for full license information.
 */

namespace Bar
{
}
";

            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a valid file header built using multi-line comments will not produce a diagnostic message.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestValidFileHeaderWithMultiLineComments2Async()
        {
            var testCode = @"/* Copyright (c) FooCorp. All rights reserved.
   Licensed under the ??? license. See LICENSE file in the project root for full license information. */

namespace Bar
{
}
";

            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a file header without text / only whitespace will produce the expected diagnostic message.
        /// </summary>
        /// <param name="comment">The comment text.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [InlineData("//")]
        [InlineData("//    ")]
        public async Task TestInvalidFileHeaderWithoutTextAsync(string comment)
        {
            var testCode = $@"{comment}

namespace Bar
{{
}}
";
            var fixedCode = @"// Copyright (c) FooCorp. All rights reserved.
// Licensed under the ??? license. See LICENSE file in the project root for full license information.

namespace Bar
{
}
";

            var expected = Diagnostic(FileHeaderAnalyzers.SA1635Descriptor).WithLocation(1, 1);
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that an invalid file header built using single line comments will produce the expected diagnostic message.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestInvalidFileHeaderWithWrongTextAsync()
        {
            var testCode = @"// Copyright (c) BarCorp. All rights reserved.
// Licensed under the ??? license. See LICENSE file in the project root for full license information.

namespace Bar
{
}
";
            var fixedCode = @"// Copyright (c) FooCorp. All rights reserved.
// Licensed under the ??? license. See LICENSE file in the project root for full license information.

namespace Bar
{
}
";
            var expected = Diagnostic(FileHeaderAnalyzers.SA1636Descriptor).WithLocation(1, 1);
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(2657, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/2657")]
        public async Task TestHeaderMissingRequiredNewLinesAsync()
        {
            var testCode = @"// Copyright (c) FooCorp. All rights reserved.
// Licensed under the ??? license. See LICENSE file in the project root for full license information.

namespace Bar
{
}
";
            var fixedCode = @"//
// Copyright (c) FooCorp. All rights reserved.
//
// Licensed under the ??? license. See LICENSE file in the project root for full license information.
//

namespace Bar
{
}
";

            var expected = Diagnostic(FileHeaderAnalyzers.SA1636Descriptor).WithLocation(1, 1);
            await VerifyCSharpFixAsync(testCode, TestSettingsWithEmptyLines, new[] { expected }, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        private static Task VerifyCSharpDiagnosticAsync(string source, DiagnosticResult expected, CancellationToken cancellationToken)
            => VerifyCSharpFixAsync(source, TestSettings, new[] { expected }, fixedSource: null, cancellationToken);

        private static Task VerifyCSharpDiagnosticAsync(string source, DiagnosticResult[] expected, CancellationToken cancellationToken)
            => VerifyCSharpFixAsync(source, TestSettings, expected, fixedSource: null, cancellationToken);

        private static Task VerifyCSharpFixAsync(string source, DiagnosticResult expected, string fixedSource, CancellationToken cancellationToken)
            => VerifyCSharpFixAsync(source, TestSettings, new[] { expected }, fixedSource, cancellationToken);

        private static Task VerifyCSharpFixAsync(string source, string testSettings, DiagnosticResult[] expected, string fixedSource, CancellationToken cancellationToken)
        {
            var test = new StyleCopCodeFixVerifier<FileHeaderAnalyzers, FileHeaderCodeFixProvider>.CSharpTest
            {
                TestCode = source,
                FixedCode = fixedSource,
                Settings = testSettings,
            };

            test.ExpectedDiagnostics.AddRange(expected);
            return test.RunAsync(cancellationToken);
        }
    }
}
