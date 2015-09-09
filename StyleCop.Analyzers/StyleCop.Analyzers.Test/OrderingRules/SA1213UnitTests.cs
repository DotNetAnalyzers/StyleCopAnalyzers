// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.OrderingRules
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.OrderingRules;
    using TestHelper;
    using Xunit;

    public class SA1213UnitTests : CodeFixVerifier
    {
        [Fact]
        public async Task TestAddAccessorAfterRemoveAccessorAsync()
        {
            var testCode = @"
using System;
public class Foo
{
    private EventHandler nameChanged;

    public event EventHandler NameChanged
    {
        remove
        {
            this.nameChanged -= value;
        }
        add
        {
            this.nameChanged += value;
        }
    }
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(9, 9);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);

            var fixedCode = @"
using System;
public class Foo
{
    private EventHandler nameChanged;

    public event EventHandler NameChanged
    {
        add
        {
            this.nameChanged += value;
        }
        remove
        {
            this.nameChanged -= value;
        }
    }
}";

            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestAddAccessorAfterRemoveAccessorWithLineCommentAsync()
        {
            var testCode = @"
using System;
public class Foo
{
    private EventHandler nameChanged;

    public event EventHandler NameChanged
    {
        // This is the remove accessor.
        remove
        {
            this.nameChanged -= value;
        }

        // This is the add accessor.
        add
        {
            this.nameChanged += value;
        }
    }
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(10, 9);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);

            var fixedCode = @"
using System;
public class Foo
{
    private EventHandler nameChanged;

    public event EventHandler NameChanged
    {
        // This is the add accessor.
        add
        {
            this.nameChanged += value;
        }

        // This is the remove accessor.
        remove
        {
            this.nameChanged -= value;
        }
    }
}";

            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestAddAccessorAfterRemoveAccessorWithBlockCommentAsync()
        {
            var testCode = @"
using System;
public class Foo
{
    private EventHandler nameChanged;

    public event EventHandler NameChanged
    {
        /*
         * This is the remove accessor.
         */
        remove
        {
            this.nameChanged -= value;
        }

        /*
         * This is the add accessor.
         */
        add
        {
            this.nameChanged += value;
        }
    }
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(12, 9);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);

            var fixedCode = @"
using System;
public class Foo
{
    private EventHandler nameChanged;

    public event EventHandler NameChanged
    {
        /*
         * This is the add accessor.
         */
        add
        {
            this.nameChanged += value;
        }

        /*
         * This is the remove accessor.
         */
        remove
        {
            this.nameChanged -= value;
        }
    }
}";

            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestAddAccessorAfterRemoveAccessorSameLineAsync()
        {
            var testCode = @"
using System;
public class Foo
{
    private EventHandler nameChanged;

    public event EventHandler NameChanged
    {
        remove { this.nameChanged -= value; } add { this.nameChanged += value; }
    }
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(9, 9);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);

            var fixedCode = @"
using System;
public class Foo
{
    private EventHandler nameChanged;

    public event EventHandler NameChanged
    {
        add { this.nameChanged += value; }
        remove { this.nameChanged -= value; } 
    }
}";

            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestAddAccessorBeforeRemoveAccessorAsync()
        {
            var testCode = @"
using System;
public class Foo
{
    private EventHandler nameChanged;

    public event EventHandler NameChanged
    {
        add { this.nameChanged += value; }
        remove { this.nameChanged -= value; }
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestAddAccessorBeforeRemoveAccessorSameLineAsync()
        {
            var testCode = @"
using System;
public class Foo
{
    private EventHandler nameChanged;

    public event EventHandler NameChanged
    {
        add { this.nameChanged += value; } remove { this.nameChanged -= value; }
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        protected override IEnumerable<DiagnosticAnalyzer> GetCSharpDiagnosticAnalyzers()
        {
            yield return new SA1213EventAccessorsMustFollowOrder();
        }

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new SA1212SA1213CodeFixProvider();
        }
    }
}
