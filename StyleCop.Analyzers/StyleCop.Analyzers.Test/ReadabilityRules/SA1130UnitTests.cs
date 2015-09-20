// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.ReadabilityRules
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Analyzers.ReadabilityRules;
    using Microsoft.CodeAnalysis.Diagnostics;
    using TestHelper;
    using Xunit;

    public class SA1130UnitTests : DiagnosticVerifier
    {
        [Fact]
        public async Task TestSimpleDelegateUseAsync()
        {
            var testCode = @"
using System;
public class TypeName
{
    public void Test()
    {
        Action action1 = delegate { };
        Action action2 = delegate() { };
        Action<int> action3 = delegate(int i) { };
    }
}";
            var expected = new[]
            {
                this.CSharpDiagnostic().WithLocation(7, 26),
                this.CSharpDiagnostic().WithLocation(8, 26),
                this.CSharpDiagnostic().WithLocation(9, 31)
            };
            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestDelegateUseAsMethodArgumentsAsync()
        {
            var testCode = @"
using System;
public class TypeName
{
    public void Test(Action argument)
    {

    }

    public void Test()
    {
        Test(delegate { });
    }
}";
            var expected = this.CSharpDiagnostic().WithLocation(12, 14);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestDelegateUseAsMethodArgumentsWithConflictingExpressionOverloadAsync()
        {
            var testCode = @"
using System;
using System.Linq.Expressions;
public class TypeName
{
    public void Test(Action argument)
    {

    }

    public void Test(Expression<Action> argument)
    {

    }

    public void Test()
    {
        Test(delegate { });
    }
}";
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestDelegateUseAsMethodArgumentsWithNonConflictingExpressionOverloadAsync()
        {
            var testCode = @"
using System;
using System.Linq.Expressions;
public class TypeName
{
    public void Test(Action argument)
    {

    }

    public void Test(Expression<Func<int>> argument)
    {

    }

    public void Test()
    {
        Test(delegate { });
    }
}";
            var expected = this.CSharpDiagnostic().WithLocation(18, 14);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        protected override IEnumerable<DiagnosticAnalyzer> GetCSharpDiagnosticAnalyzers()
        {
            yield return new SA1130UseLambdaSyntax();
        }
    }
}
