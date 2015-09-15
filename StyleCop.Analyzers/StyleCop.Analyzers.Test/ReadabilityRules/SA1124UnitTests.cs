// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.ReadabilityRules
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Analyzers.ReadabilityRules;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using TestHelper;
    using Xunit;

    /// <summary>
    /// This class contains unit tests for <see cref="SA1124DoNotUseRegions"/> and
    /// <see cref="RemoveRegionCodeFixProvider"/>.
    /// </summary>
    public class SA1124UnitTests : CodeFixVerifier
    {
        public string DiagnosticId { get; } = SA1124DoNotUseRegions.DiagnosticId;

        [Fact]
        public async Task TestRegionInMethodAsync()
        {
            var testCode = @"public class Foo
{
    public void Bar()
    {
#region Foo
        string test = """";
#endregion
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestRegionPartialyInMethodAsync()
        {
            var testCode = @"public class Foo
{
    public void Bar()
    {
#region Foo
        string test = """";
    }
#endregion
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(5, 1);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestRegionPartialyInMethod2Async()
        {
            var testCode = @"public class Foo
{
    public void Bar()
#region Foo
    {
        string test = """";
    }
#endregion
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(4, 1);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);

            string fixedCode = @"public class Foo
{
    public void Bar()
    {
        string test = """";
    }
}";

            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
            await this.VerifyCSharpFixAllFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestRegionPartialyMultipleMethodsAsync()
        {
            var testCode = @"public class Foo
{
    public void Bar()
    {
#region Foo
        string test = """";
    }
    public void FooBar()
    {
        string test = """";
#endregion
    }
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(5, 1);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);

            string fixedCode = @"public class Foo
{
    public void Bar()
    {
        string test = """";
    }
    public void FooBar()
    {
        string test = """";
    }
}";

            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
            await this.VerifyCSharpFixAllFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestEndRegionInMethodAsync()
        {
            var testCode = @"public class Foo
{
#region Foo
    public void Bar()
    {
        string test = """";
#endregion
    }
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(3, 1);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);

            string fixedCode = @"public class Foo
{
    public void Bar()
    {
        string test = """";
    }
}";

            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
            await this.VerifyCSharpFixAllFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestRegionOutsideMethodAsync()
        {
            var testCode = @"public class Foo
{
#region Foo
#endregion
    public void Bar()
    {
        string test = """";
    }
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(3, 1);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);

            string fixedCode = @"public class Foo
{
    public void Bar()
    {
        string test = """";
    }
}";

            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
            await this.VerifyCSharpFixAllFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestRegionOutsideMethod2Async()
        {
            var testCode = @"public class Foo
{
#region Foo
    public void Bar()
    {
        string test = """";
    }
#endregion
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(3, 1);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);

            string fixedCode = @"public class Foo
{
    public void Bar()
    {
        string test = """";
    }
}";

            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
            await this.VerifyCSharpFixAllFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        protected override IEnumerable<DiagnosticAnalyzer> GetCSharpDiagnosticAnalyzers()
        {
            yield return new SA1124DoNotUseRegions();
        }

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new RemoveRegionCodeFixProvider();
        }
    }
}
