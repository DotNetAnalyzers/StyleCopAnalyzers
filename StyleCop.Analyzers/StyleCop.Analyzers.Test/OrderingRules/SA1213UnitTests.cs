// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.OrderingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        Analyzers.OrderingRules.SA1213EventAccessorsMustFollowOrder,
        Analyzers.OrderingRules.SA1212SA1213CodeFixProvider>;

    public class SA1213UnitTests
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

            DiagnosticResult expected = Diagnostic().WithLocation(9, 9);

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

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
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

            DiagnosticResult expected = Diagnostic().WithLocation(10, 9);

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

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
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

            DiagnosticResult expected = Diagnostic().WithLocation(12, 9);

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

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
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

            DiagnosticResult expected = Diagnostic().WithLocation(9, 9);

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

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
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

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
