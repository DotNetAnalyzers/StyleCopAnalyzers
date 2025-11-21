// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.Test.CSharp10.OrderingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp9.OrderingRules;
    using Xunit;

    public partial class SA1200PreserveCSharp10UnitTests : SA1200PreserveCSharp9UnitTests
    {
        [Fact]
        public async Task TestValidUsingStatementsInFileScopedNamespaceAsync()
        {
            var testCode = @"namespace TestNamespace;

using System;
using System.Threading;
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that having using statements in the compilation unit will not diagnostics, even if they could be
        /// moved inside a file-scoped namespace.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestIgnoredUsingStatementsInCompilationUnitWithFileScopedNamespaceAsync()
        {
            var testCode = @"using System;
using System.Threading;

namespace TestNamespace;
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
