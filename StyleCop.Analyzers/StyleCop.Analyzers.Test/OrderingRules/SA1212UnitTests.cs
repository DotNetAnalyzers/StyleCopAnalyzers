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

    public class SA1212UnitTests : CodeFixVerifier
    {
        [Fact]
        public async Task TestPropertyWithDocumentationAsync()
        {
            var testCode = @"
public class Foo
{
    private int i = 0;

    public int Prop
    {
        /// <summary>
        /// The setter documentation
        /// </summary>
        set
        {
            i = value;
        }

        /// <summary>
        /// The getter documentation
        /// </summary>
        get
        {
            return i;
        }
    }
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(11, 9);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);

            var fixedCode = @"
public class Foo
{
    private int i = 0;

    public int Prop
    {
        /// <summary>
        /// The getter documentation
        /// </summary>
        get
        {
            return i;
        }

        /// <summary>
        /// The setter documentation
        /// </summary>
        set
        {
            i = value;
        }
    }
}";

            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestPropertyWithDocumentationNoBlankLineAsync()
        {
            var testCode = @"
public class Foo
{
    private int i = 0;

    public int Prop
    {
        // The setter documentation
        set
        {
            i = value;
        }
        // The getter documentation
        get
        {
            return i;
        }
    }
}";
            var fixedCode = @"
public class Foo
{
    private int i = 0;

    public int Prop
    {
        // The getter documentation
        get
        {
            return i;
        }
        // The setter documentation
        set
        {
            i = value;
        }
    }
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(9, 9);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestPropertyWithLineCommentAsync()
        {
            var testCode = @"
public class Foo
{
    private int i = 0;

    public int Prop
    {
        // The setter documentation
        set
        {
            i = value;
        }

        // The getter documentation
        get
        {
            return i;
        }
    }
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(9, 9);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);

            var fixedCode = @"
public class Foo
{
    private int i = 0;

    public int Prop
    {
        // The getter documentation
        get
        {
            return i;
        }

        // The setter documentation
        set
        {
            i = value;
        }
    }
}";

            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestPropertyWithBlockCommentAsync()
        {
            var testCode = @"
public class Foo
{
    private int i = 0;

    public int Prop
    {
        /*
         * The setter documentation
         */
        set
        {
            i = value;
        }

        /*
         * The getter documentation
         */
        get
        {
            return i;
        }
    }
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(11, 9);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);

            var fixedCode = @"
public class Foo
{
    private int i = 0;

    public int Prop
    {
        /*
         * The getter documentation
         */
        get
        {
            return i;
        }

        /*
         * The setter documentation
         */
        set
        {
            i = value;
        }
    }
}";

            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestPropertyWithBackingFieldDeclarationSetterBeforeGetterAsync()
        {
            var testCode = @"
public class Foo
{
    private int i = 0;

    public int Prop
    {
        set
        {
            i = value;
        }

        get
        {
            return i;
        }
    }
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(8, 9);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);

            var fixedCode = @"
public class Foo
{
    private int i = 0;

    public int Prop
    {
        get
        {
            return i;
        }

        set
        {
            i = value;
        }
    }
}";

            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestPropertyWithBackingFieldDeclarationGetterBeforeSetterAsync()
        {
            var testCode = @"
public class Foo
{
    private int i = 0;

    public int Prop
    {
        get
        {
            return i;
        }

        set
        {
            i = value;
        }
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestPropertyWithBackingFieldDeclarationOnlyGetterAsync()
        {
            var testCode = @"
public class Foo
{
    private int i = 0;

    public int Prop
    {
        get
        {
            return i;
        }
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestPropertyWithBackingFieldDeclarationOnlySetterAsync()
        {
            var testCode = @"
public class Foo
{
    private int i = 0;

    public int Prop
    {
        set
        {
            i = value;
        }
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestAutoPropertydDeclarationSetterBeforeGetterAsync()
        {
            var testCode = @"
public class Foo
{
    public int Prop
    {
        set;get;
    }
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(6, 9);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);

            var fixedCode = @"
public class Foo
{
    public int Prop
    {
        get;
        set;
    }
}";

            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestIndexerDeclarationSetterBeforeGetterAsync()
        {
            var testCode = @"
public class Foo
{
    private int field;

    public int this[int index]
    {
        set
        {
            field = value;
        }

        get
        {
            return field;
        }
    }
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(8, 9);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);

            var fixedCode = @"
public class Foo
{
    private int field;

    public int this[int index]
    {
        get
        {
            return field;
        }

        set
        {
            field = value;
        }
    }
}";

            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestIndexerDeclarationGetterBeforeSetterAsync()
        {
            var testCode = @"
public class Foo
{
    private int field;

    public int this[int index]
    {
        get
        {
            return field;
        }

        set
        {
            field = value;
        }
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestIndexerDeclarationOnlySetterAsync()
        {
            var testCode = @"
public class Foo
{
    private int field;

    public int this[int index]
    {
        set
        {
            field = value;
        }
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestIndexerDeclarationOnlyGetterAsync()
        {
            var testCode = @"
public class Foo
{
    private int field;

    public int this[int index]
    {
        get
        {
            return field;
        }
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestExpressionPropertyAsync()
        {
            var testCode = @"
public class Foo
{
    private int field;

    public int this[int index] => field;

    public int Property => field;
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        protected override IEnumerable<DiagnosticAnalyzer> GetCSharpDiagnosticAnalyzers()
        {
            yield return new SA1212PropertyAccessorsMustFollowOrder();
        }

        /// <inheritdoc/>
        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new SA1212SA1213CodeFixProvider();
        }
    }
}
