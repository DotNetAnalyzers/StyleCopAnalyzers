// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.ReadabilityRules
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Analyzers.ReadabilityRules;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using TestHelper;
    using Xunit;

    public class SA1131UnitTests : CodeFixVerifier
    {
        [Fact]
        public async Task TestYodaComparismAsync()
        {
            var testCode = @"
using System;
public class TypeName
{
    public void Test()
    {
        int i = 5;
        const int j = 6;
        if (j == i) { }
    }
}";
            var fixedCode = @"
using System;
public class TypeName
{
    public void Test()
    {
        int i = 5;
        const int j = 6;
        if (i == j) { }
    }
}";

            var expected = new[]
            {
                this.CSharpDiagnostic().WithLocation(9, 13),
            };
            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestYodaComparismAsAnArgumentAsync()
        {
            var testCode = @"
using System;
public class TypeName
{
    public void Test()
    {
        int i = 5;
        const int j = 6;
        Test(j == i);
    }
    public void Test(bool arg) { }
}";
            var fixedCode = @"
using System;
public class TypeName
{
    public void Test()
    {
        int i = 5;
        const int j = 6;
        Test(i == j);
    }
    public void Test(bool arg) { }
}";

            var expected = new[]
            {
                this.CSharpDiagnostic().WithLocation(9, 14),
            };
            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestYodaComparismOutsideIfAsync()
        {
            var testCode = @"
using System;
public class TypeName
{
    public void Test()
    {
        int i = 5;
        const int j = 6;
        bool b = j == i;
    }
}";
            var fixedCode = @"
using System;
public class TypeName
{
    public void Test()
    {
        int i = 5;
        const int j = 6;
        bool b = i == j;
    }
}";
            var expected = new[]
            {
                this.CSharpDiagnostic().WithLocation(9, 18),
            };
            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestCorrectComparismAsync()
        {
            var testCode = @"
using System;
public class TypeName
{
    public void Test()
    {
        const int i = 5;
        int j = 6;
        if (j == i) { }
    }
}";
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestCorrectComparismOutsideIfAsync()
        {
            var testCode = @"
using System;
public class TypeName
{
    public void Test()
    {
        const int i = 5;
        int j = 6;
        bool b = j == i;
    }
}";
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestCorrectComparismNoConstantAsync()
        {
            var testCode = @"
using System;
public class TypeName
{
    public void Test()
    {
        int i = 5;
        int j = 6;
        if (j == i) { }
    }
}";
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestCorrectComparismOutsideIfNoConstantAsync()
        {
            var testCode = @"
using System;
public class TypeName
{
    public void Test()
    {
        int i = 5;
        int j = 6;
        bool b = j == i;
    }
}";
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestCommentsAsync()
        {
            var testCode = @"
using System;
public class TypeName
{
    public void Test()
    {
        int i = 5;
        const int j = 6;
        bool b = /*1*/j/*2*/==/*3*/i/*4*/;
    }
}";
            var fixedCode = @"
using System;
public class TypeName
{
    public void Test()
    {
        int i = 5;
        const int j = 6;
        bool b = /*1*/i/*2*/==/*3*/j/*4*/;
    }
}";
            var expected = new[]
            {
                this.CSharpDiagnostic().WithLocation(9, 23),
            };
            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        protected override IEnumerable<DiagnosticAnalyzer> GetCSharpDiagnosticAnalyzers()
        {
            yield return new SA1131UseReadableConditions();
        }

        /// <inheritdoc/>
        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new SA1131CodeFixProvider();
        }
    }
}
