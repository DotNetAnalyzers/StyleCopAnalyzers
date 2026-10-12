// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.Test.MaintainabilityRules
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.MaintainabilityRules;
    using Xunit;

    public class SA1406UnitTests : DebugMessagesUnitTestsBase
    {
        protected override string MethodName
        {
            get
            {
                return nameof(Debug.Fail);
            }
        }

        protected override IEnumerable<string> InitialArguments
        {
            get
            {
                return Enumerable.Empty<string>();
            }
        }

        protected override DiagnosticAnalyzer Analyzer => new SA1406DebugFailMustProvideMessageText();

        [Fact]
        public async Task TestCustomDebugClassAsync()
        {
            var testCode = @"namespace System.Diagnostics
{
    internal static class Debug
    {
        public static void Assert(bool condition, string message = null)
        {
        }
        public static void Fail(string message = null)
        {
        }
    }

    public class Foo
    {
        public void Bar()
        {
            Debug.Fail();
        }
    }
}";

            DiagnosticResult[] expected =
            {
                this.Diagnostic().WithLocation(17, 13),
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, includeSystemDll: false, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
