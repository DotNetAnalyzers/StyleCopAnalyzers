using Microsoft.CodeAnalysis.CodeFixes;

namespace StyleCop.Analyzers.Test.OrderingRules
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.OrderingRules;
    using TestHelper;
    using Xunit;

    public class SA1212UnitTests : CodeFixVerifier
    {
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

            var fixTestCode = @"
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

            await this.VerifyCSharpDiagnosticAsync(fixTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixTestCode).ConfigureAwait(false);
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

            var fixTestCode = @"
public class Foo
{
    public int Prop
    {
        get;
        set;
    }
}";

            await this.VerifyCSharpDiagnosticAsync(fixTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixTestCode).ConfigureAwait(false);
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

            var fixTestCode = @"
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

            await this.VerifyCSharpDiagnosticAsync(fixTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixTestCode).ConfigureAwait(false);
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

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new SA1212SA1213CodeFixProvider();
        }
    }
}
