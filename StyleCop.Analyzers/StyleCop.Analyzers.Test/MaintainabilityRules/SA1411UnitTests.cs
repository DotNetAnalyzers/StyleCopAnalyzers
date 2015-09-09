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

    public class SA1411UnitTests : CodeFixVerifier
    {
        [Fact]
        public async Task TestMissingParenthesisAsync()
        {
            var testCode = @"public class Foo
{
    [System.Obsolete]
    public void Bar()
    {
    }
}";
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestNonEmptyParameterListAsync()
        {
            var testCode = @"public class Foo
{
    [System.Obsolete(""bar"")]
    public void Bar()
    {
    }
}";
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestNonEmptyParameterListNamedArgumentAsync()
        {
            var testCode = @"
using System.Runtime.CompilerServices;
public class Foo
{
    [MethodImpl(MethodCodeType = MethodCodeType.IL)]
    public void Bar()
    {
    }
}";
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestEmptyParameterListAsync()
        {
            var testCode = @"public class Foo
{
    [System.Obsolete()]
    public void Bar()
    {
    }
}";
            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(3, 21);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestEmptyParameterListMultipleAttributesAsync()
        {
            var testCode = @"public class Foo
{
    [System.Obsolete(), System.Runtime.CompilerServices.MethodImpl()]
    public void Bar()
    {
    }
}";
            DiagnosticResult[] expected =
                {
                    this.CSharpDiagnostic().WithLocation(3, 21),
                    this.CSharpDiagnostic().WithLocation(3, 67)
                };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestCodeFixAsync()
        {
            var oldSource = @"public class Foo
{
    [System.Obsolete()]
    public void Bar()
    {
    }
}";

            var newSource = @"public class Foo
{
    [System.Obsolete]
    public void Bar()
    {
    }
}";

            await this.VerifyCSharpFixAsync(oldSource, newSource, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestCodeFixDoesNotRemoveExteriorTriviaAsync()
        {
            var oldSource = @"public class Foo
{
    [System.Obsolete/*Foo*/(/*Bar*/)/*Foo*/]
    public void Bar()
    {
    }
}";

            var newSource = @"public class Foo
{
    [System.Obsolete/*Foo*//*Bar*//*Foo*/]
    public void Bar()
    {
    }
}";

            await this.VerifyCSharpFixAsync(oldSource, newSource, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestCodeFixMultipleAttributesAsync()
        {
            var oldSource = @"public class Foo
{
    [System.Obsolete(), System.Runtime.CompilerServices.MethodImpl()]
    public void Bar()
    {
    }
}";

            var newSource = @"public class Foo
{
    [System.Obsolete, System.Runtime.CompilerServices.MethodImpl]
    public void Bar()
    {
    }
}";

            await this.VerifyCSharpFixAsync(oldSource, newSource, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        protected override IEnumerable<DiagnosticAnalyzer> GetCSharpDiagnosticAnalyzers()
        {
            yield return new SA1411AttributeConstructorMustNotUseUnnecessaryParenthesis();
        }

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new SA1410SA1411CodeFixProvider();
        }
    }
}