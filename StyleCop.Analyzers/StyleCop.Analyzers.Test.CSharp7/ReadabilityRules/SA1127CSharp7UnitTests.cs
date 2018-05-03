// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp7.ReadabilityRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using StyleCop.Analyzers.Test.ReadabilityRules;
    using TestHelper;
    using Xunit;

    public class SA1127CSharp7UnitTests : SA1127UnitTests
    {
        [Fact]
        public async Task TestViolationWithLocalFunctionDeclarationAsync()
        {
            var testCode = $@"
class Foo
{{
    private void Method()
    {{
        void LocalFunction<T>() where T : class {{ }}
    }}
}}";
            var fixedCode = $@"
class Foo
{{
    private void Method()
    {{
        void LocalFunction<T>()
            where T : class
        {{ }}
    }}
}}";
            var expected = this.CSharpDiagnostic().WithLocation(6, 33);
            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// This is a regression test for DotNetAnalyzers/StyleCopAnalyzers#1476:
        /// https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/1476
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestViolationWithLocalFunctionDeclarationMultiLineParametersAsync()
        {
            var testCode = @"
class Foo
{
    private void Method()
    {
        void LocalFunction<T>(
            int a,
            int b) where T : class { }
    }
}";
            var fixedCode = @"
class Foo
{
    private void Method()
    {
        void LocalFunction<T>(
            int a,
            int b)
            where T : class
        { }
    }
}";
            var expected = this.CSharpDiagnostic().WithLocation(8, 20);
            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// This is a regression test for DotNetAnalyzers/StyleCopAnalyzers#1652:
        /// https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/1652
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestViolationWithLocalFunctionDeclarationRegionDirectiveAsync()
        {
            var testCode = $@"
class Foo
{{
    private void Method()
    {{
        #region Test
        void LocalFunction<T>() where T : class {{ }}
        #endregion
    }}
}}";
            var fixedCode = $@"
class Foo
{{
    private void Method()
    {{
        #region Test
        void LocalFunction<T>()
            where T : class
        {{ }}
        #endregion
    }}
}}";
            var expected = this.CSharpDiagnostic().WithLocation(7, 33);
            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestViolationWithExpressionBodiedLocalFunctionDeclarationAsync()
        {
            var testCode = $@"
class Foo
{{
    private void Method()
    {{
        string LocalFunction<T>() where T : class => typeof(T).Name;
    }}
}}";
            var fixedCode = $@"
class Foo
{{
    private void Method()
    {{
        string LocalFunction<T>()
            where T : class
            => typeof(T).Name;
    }}
}}";
            var expected = this.CSharpDiagnostic().WithLocation(6, 35);
            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestViolationWithLocalFunctionAndThreeTypeConstraintsOnSingleLineAsync()
        {
            var testCode = $@"
class Foo
{{
    private void Method()
    {{
        void LocalFunction<T1, T2, T3>() where T1 : class where T2 : class where T3 : class {{ }}
    }}
}}";
            var fixedCode = $@"
class Foo
{{
    private void Method()
    {{
        void LocalFunction<T1, T2, T3>()
            where T1 : class
            where T2 : class
            where T3 : class
        {{ }}
    }}
}}";
            var expected = this.CSharpDiagnostic().WithLocation(6, 42);
            var expected2 = this.CSharpDiagnostic().WithLocation(6, 59);
            var expected3 = this.CSharpDiagnostic().WithLocation(6, 76);
            await this.VerifyCSharpDiagnosticAsync(testCode, new[] { expected, expected2, expected3 }, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestViolationWithLocalFunctionAndCommentTriviaAtEndOfLineAsync()
        {
            var testCode = $@"
using System;
class Foo
{{
    private void Method()
    {{
        T GenericLocalFunction<T>() where T : class // constrain this to just classes
        {{
            throw new NotImplementedException();
        }}
    }}
}}";
            var fixedCode = $@"
using System;
class Foo
{{
    private void Method()
    {{
        T GenericLocalFunction<T>()
            where T : class // constrain this to just classes
        {{
            throw new NotImplementedException();
        }}
    }}
}}";
            var expected = this.CSharpDiagnostic().WithLocation(7, 37);
            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }
    }
}
