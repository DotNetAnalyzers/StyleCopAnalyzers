﻿// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
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

    public class SA1135UnitTests : CodeFixVerifier
    {
        [Fact]
        public async Task TestUnqualifiedUsingsAsync()
        {
            const string testCode = @"
namespace System.Threading
{
    using IO;
    using Tasks;
}";
            const string fixedCode = @"
namespace System.Threading
{
    using System.IO;
    using System.Threading.Tasks;
}";

            DiagnosticResult[] expected =
            {
                this.CSharpDiagnostic().WithLocation(4, 5).WithArguments("System.IO"),
                this.CSharpDiagnostic().WithLocation(4, 5).WithArguments("System.IO"),
                this.CSharpDiagnostic().WithLocation(5, 5).WithArguments("System.Threading.Tasks"),
                this.CSharpDiagnostic().WithLocation(5, 5).WithArguments("System.Threading.Tasks")
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestUnqualifiedAliasedUsingsAsync()
        {
            const string testCode = @"
namespace System.Threading
{
    using NA = IO;
    using NB = Tasks;
}";
            const string fixedCode = @"
namespace System.Threading
{
    using NA = System.IO;
    using NB = System.Threading.Tasks;
}";

            DiagnosticResult[] expected =
            {
                this.CSharpDiagnostic().WithLocation(4, 5).WithArguments("System.IO"),
                this.CSharpDiagnostic().WithLocation(4, 5).WithArguments("System.IO"),
                this.CSharpDiagnostic().WithLocation(5, 5).WithArguments("System.Threading.Tasks"),
                this.CSharpDiagnostic().WithLocation(5, 5).WithArguments("System.Threading.Tasks")
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestUnqualifiedAliasedUsingTypesAsync()
        {
            const string testCode = @"
namespace System.Threading
{
    using TP = IO.Path;
    using TT = Tasks.Task;
}";
            const string fixedCode = @"
namespace System.Threading
{
    using TP = System.IO.Path;
    using TT = System.Threading.Tasks.Task;
}";

            DiagnosticResult[] expected =
            {
                this.CSharpDiagnostic().WithLocation(4, 5).WithArguments("System.IO.Path"),
                this.CSharpDiagnostic().WithLocation(4, 5).WithArguments("System.IO.Path"),
                this.CSharpDiagnostic().WithLocation(5, 5).WithArguments("System.Threading.Tasks.Task"),
                this.CSharpDiagnostic().WithLocation(5, 5).WithArguments("System.Threading.Tasks.Task")
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestGlobalUsingsAsync()
        {
            const string testCode = @"
namespace System.Threading
{
    using global::System.IO;
    using global::System.Threading.Tasks;
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestStaticUsingsAsync()
        {
            const string testCode = @"
namespace System.Threading
{
    using static Console;
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new SA1135CodeFixProvider();
        }

        /// <inheritdoc/>
        protected override IEnumerable<DiagnosticAnalyzer> GetCSharpDiagnosticAnalyzers()
        {
            yield return new SA1135UsingDirectivesMustBeQualified();
        }
    }
}
