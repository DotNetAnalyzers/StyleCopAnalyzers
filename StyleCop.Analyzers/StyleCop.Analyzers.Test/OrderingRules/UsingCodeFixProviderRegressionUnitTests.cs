// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.OrderingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.OrderingRules;
    using StyleCop.Analyzers.Test.Verifiers;
    using TestHelper;
    using Xunit;
    using static CombinedUsingDirectivesVerifier;

    /// <summary>
    /// Regression unit tests for issue #2026.
    /// </summary>
    public class UsingCodeFixProviderRegressionUnitTests
    {
        private bool disableSA1200;

        /// <summary>
        /// Verifies the regression for issue #2026.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task VerifyRegressionFor2026Async()
        {
            var testCode = @"// Copyright (c) 2015 ACME, Inc. All rights reserved worldwide.

#if !VS2012

using System;
using System.Runtime.InteropServices;
using System.ComponentModel;

#if VS2008
using System.Collections.ObjectModel;
#elif VS2010
using System.Collections.Concurrent;
#endif

using Math = System.Math;
using Queue = System.Collections.Queue;

namespace Microsoft.VisualStudio.Shell
{
#pragma warning disable SA1200 // Using directives should be placed correctly
    // This is required to work around accessibility issues in documentation comments.
    using NativeMethods = System;
#pragma warning restore SA1200 // Using directives should be placed correctly
}

#endif
";

            var fixedTestCode = @"// Copyright (c) 2015 ACME, Inc. All rights reserved worldwide.

#if !VS2012

using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

#if VS2008
using System.Collections.ObjectModel;
#elif VS2010
using System.Collections.Concurrent;
#endif

using Math = System.Math;
using Queue = System.Collections.Queue;

namespace Microsoft.VisualStudio.Shell
{
#pragma warning disable SA1200 // Using directives should be placed correctly
    // This is required to work around accessibility issues in documentation comments.
    using NativeMethods = System;
#pragma warning restore SA1200 // Using directives should be placed correctly
}

#endif
";

            var expected = StyleCopDiagnosticVerifier<SA1210UsingDirectivesMustBeOrderedAlphabeticallyByNamespace>.Diagnostic().WithLocation(6, 1);
            await this.VerifyCSharpFixAsync(testCode, new[] { expected }, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies the regression for issue #2027.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task VerifyRegressionFor2027Async()
        {
            this.disableSA1200 = true;

            var testCode = @"using System;
using System.Reflection;
using System.Net;

namespace MyNamespace
{
    using System.Threading.Tasks;
}
";

            var fixedTestCode = @"using System;
using System.Net;
using System.Reflection;

namespace MyNamespace
{
    using System.Threading.Tasks;
}
";

            var expected = StyleCopDiagnosticVerifier<SA1210UsingDirectivesMustBeOrderedAlphabeticallyByNamespace>.Diagnostic().WithLocation(2, 1);
            await this.VerifyCSharpFixAsync(testCode, new[] { expected }, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }

        private Task VerifyCSharpFixAsync(string source, DiagnosticResult[] expected, string fixedSource, CancellationToken cancellationToken)
        {
            var testSettings = @"
{
  ""$schema"": ""https://raw.githubusercontent.com/DotNetAnalyzers/StyleCopAnalyzers/master/StyleCop.Analyzers/StyleCop.Analyzers/Settings/stylecop.schema.json"",
  ""settings"": {
    ""documentationRules"": {
      ""fileNamingConvention"": ""metadata"",
      ""companyName"": ""ACME, Inc"",
      ""copyrightText"": ""Copyright (c) {year} {companyName}. All rights reserved worldwide."",
      ""xmlHeader"": false,
      ""variables"": {
        ""year"": ""2015""
      }
    },
  ""orderingRules"": {
    ""usingDirectivesPlacement"": ""outsideNamespace""
    }
  }
}
";

            var test = new CSharpTest
            {
                TestCode = source,
                FixedCode = fixedSource,
                Settings = testSettings,
            };

            test.ExpectedDiagnostics.AddRange(expected);

            if (this.disableSA1200)
            {
                test.DisabledDiagnostics.Add(SA1200UsingDirectivesMustBePlacedCorrectly.DiagnosticId);
            }

            return test.RunAsync(cancellationToken);
        }
    }
}
