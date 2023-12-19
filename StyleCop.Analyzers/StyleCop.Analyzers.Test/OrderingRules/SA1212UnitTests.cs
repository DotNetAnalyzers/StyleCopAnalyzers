// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.Test.OrderingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.OrderingRules.SA1212PropertyAccessorsMustFollowOrder,
        StyleCop.Analyzers.OrderingRules.SA1212SA1213CodeFixProvider>;

    public class SA1212UnitTests
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

            DiagnosticResult expected = Diagnostic().WithLocation(11, 9);

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

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
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

            DiagnosticResult expected = Diagnostic().WithLocation(9, 9);
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
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

            DiagnosticResult expected = Diagnostic().WithLocation(9, 9);

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

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
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

            DiagnosticResult expected = Diagnostic().WithLocation(11, 9);

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

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
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

            DiagnosticResult expected = Diagnostic().WithLocation(8, 9);

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

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
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

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestAutoPropertyDeclarationSetterBeforeGetterAsync()
        {
            var testCode = @"
public class Foo
{
    public int Prop
    {
        set;get;
    }
}";

            DiagnosticResult expected = Diagnostic().WithLocation(6, 9);

            var fixedCode = @"
public class Foo
{
    public int Prop
    {
        get;
        set;
    }
}";

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
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

            DiagnosticResult expected = Diagnostic().WithLocation(8, 9);

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

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
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

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
