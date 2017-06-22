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
    /// Regression unit tests for issue #2026.
    /// </summary>
    public class UsingCodeFixProviderRegressionUnitTests : CodeFixVerifier
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

            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
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

            var expected = this.CSharpDiagnostic(SA1210UsingDirectivesMustBeOrderedAlphabeticallyByNamespace.DiagnosticId).WithLocation(2, 1);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        protected override IEnumerable<string> GetDisabledDiagnostics()
        {
            if (this.disableSA1200)
            {
                yield return SA1200UsingDirectivesMustBePlacedCorrectly.DiagnosticId;
            }
        }

        /// <inheritdoc/>
        protected override string GetSettings()
        {
            return @"
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
