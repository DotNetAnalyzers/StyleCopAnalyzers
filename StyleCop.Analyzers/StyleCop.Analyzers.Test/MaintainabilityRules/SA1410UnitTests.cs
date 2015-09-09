// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.MaintainabilityRules
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.MaintainabilityRules;
    using TestHelper;
    using Xunit;

    public class SA1410UnitTests : CodeFixVerifier
    {
        [Fact]
        public async Task TestMissingParenthesisAsync()
        {
            var testCode = @"public class Foo
{
    public void Bar()
    {
        System.Func<int> getRandomNumber = delegate { return 3; };
    }
}";
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestNonEmptyParameterListAsync()
        {
            var testCode = @"public class Foo
{
    public void Bar()
    {
        System.Func<int, int> getNumber = delegate (int i) { return i; };
    }
}";
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestEmptyParameterListAsync()
        {
            var testCode = @"public class Foo
{
    public void Bar()
    {
        System.Func<int> getRandomNumber = delegate() { return 3; };
    }
}";
            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(5, 52);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestCodeFixAsync()
        {
            var oldSource = @"public class Foo
{
    public void Bar()
    {
        System.Func<int> getRandomNumber = delegate() { return 3; };
    }
}";

            var newSource = @"public class Foo
{
    public void Bar()
    {
        System.Func<int> getRandomNumber = delegate { return 3; };
    }
}";

            await this.VerifyCSharpFixAsync(oldSource, newSource, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestCodeFixDoesNotRemoveExteriorTriviaAsync()
        {
            var oldSource = @"public class Foo
{
    public void Bar()
    {
        System.Func<int> getRandomNumber = delegate/*Foo*/(/*Bar*/)/*Foo*/ { return 3; };
    }
}";

            var newSource = @"public class Foo
{
    public void Bar()
    {
        System.Func<int> getRandomNumber = delegate/*Foo*//*Bar*//*Foo*/ { return 3; };
    }
}";

            await this.VerifyCSharpFixAsync(oldSource, newSource, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        protected override IEnumerable<DiagnosticAnalyzer> GetCSharpDiagnosticAnalyzers()
        {
            yield return new SA1410RemoveDelegateParenthesisWhenPossible();
        }

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new SA1410SA1411CodeFixProvider();
        }
    }
}