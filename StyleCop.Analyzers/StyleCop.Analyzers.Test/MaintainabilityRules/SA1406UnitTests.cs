// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.MaintainabilityRules
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.MaintainabilityRules;
    using TestHelper;
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

        [Fact]
        public async Task TestCustomDebugClassAsync()
        {
            this.IncludeSystemDll = false;

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
                this.CSharpDiagnostic().WithLocation(17, 13)
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        protected override IEnumerable<DiagnosticAnalyzer> GetCSharpDiagnosticAnalyzers()
        {
            yield return new SA1406DebugFailMustProvideMessageText();
        }
    }
}
