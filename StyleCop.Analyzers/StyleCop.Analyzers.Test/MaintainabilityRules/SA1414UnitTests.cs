// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.MaintainabilityRules
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.MaintainabilityRules;
    using TestHelper;
    using Xunit;

    /// <summary>
    /// This class contains unit tests for <see cref="SA1414InternalFieldMustBeAutoProperty"/>.
    /// </summary>
    public class SA1414UnitTests : DiagnosticVerifier
    {
        [Fact]
        public async Task TestClassWithInternalFieldAsync()
        {
            var testCode = @"public class Foo
{
    internal string bar;
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(3, 21);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestClassWithProtectedInternalFieldAsync()
        {
            var testCode = @"public class Foo
{
    protected internal string bar;
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(3, 31);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestClassWithInternalConstFieldAsync()
        {
            var testCode = @"public class Foo
{
    internal const string bar = ""bar"";
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestClassWithInternalStaticFieldAsync()
        {
            var testCode = @"public class Foo
{
    internal static string bar;
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        protected override IEnumerable<DiagnosticAnalyzer> GetCSharpDiagnosticAnalyzers()
        {
            yield return new SA1414InternalFieldMustBeAutoProperty();
        }
    }
}
