// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.MaintainabilityRules
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using StyleCop.Analyzers.MaintainabilityRules;
    using TestHelper;
    using Xunit;

#pragma warning disable xUnit1000 // Test classes must be public
    internal class SA1406UnitTests : DebugMessagesUnitTestsBase<SA1406DebugFailMustProvideMessageText>
#pragma warning restore xUnit1000 // Test classes must be public
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
                Diagnostic().WithLocation(17, 13),
            };

            await VerifyCSharpDiagnosticAsync(testCode, expected, includeSystemDll: false, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
