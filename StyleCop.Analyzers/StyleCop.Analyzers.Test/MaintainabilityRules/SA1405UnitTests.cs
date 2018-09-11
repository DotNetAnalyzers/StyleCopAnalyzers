// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.MaintainabilityRules
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.MaintainabilityRules;
    using Xunit;

    public class SA1405UnitTests : DebugMessagesUnitTestsBase
    {
        protected override string MethodName
        {
            get
            {
                return nameof(Debug.Assert);
            }
        }

        protected override IEnumerable<string> InitialArguments
        {
            get
            {
                yield return "true";
            }
        }

        protected override DiagnosticAnalyzer Analyzer => new SA1405DebugAssertMustProvideMessageText();

        [Fact]
        public async Task TestWrongOverloadAsync()
        {
            var testCode = @"using System.Diagnostics;
public class Foo
{
    public void Bar()
    {
        Debug.Assert(true);
    }
}";

            DiagnosticResult expected = this.Diagnostic().WithLocation(6, 9);
            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

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
            Debug.Assert(true);
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
