namespace StyleCop.Analyzers.Test.LayoutRules
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.LayoutRules;
    using TestHelper;
    using Xunit;

    public class SA1504UnitTests : DiagnosticVerifier
    {
        [Fact]
        public async Task TestEmptySourceAsync()
        {
            var testCode = string.Empty;
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestPropertyGetInOneLineSetInMultipleLinesAsync()
        {
            var testCode = @"
public class Foo
{
    public int Prop
    {
        get { return 1; }
        set
        {
        }
    }
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(6, 9);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestPropertyGetAndSetOnOneLineAsync()
        {
            var testCode = @"
public class Foo
{
    public int Prop
    {
        get { return 1; }
        set { }
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestPropertyGetAndSetOnOneLineWithCommentsAsync()
        {
            var testCode = @"
public class Foo
{
    public int Prop
    {
        // here's get
        get { return 1; }
        set { }
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestPropertyGetAndSetOnMultipleLinesAsync()
        {
            var testCode = @"
public class Foo
{
    public int Prop
    {
        get 
        { 
            return 1; 
        }
        set
        {
        }
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestPropertyOnlyGetterAsync()
        {
            var testCode = @"
public class Foo
{
    public int Prop
    {
        get 
        { 
            return 1; 
        }
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestAutoPropertyAsync()
        {
            var testCode = @"
public class Foo
{
    public int Prop
    {
        get;set;
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestPropertyExpressionBodyAsync()
        {
            var testCode = @"
public class Foo
{
    public int Prop => 1;
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestIndexerSetInOneLineGetInMultipleLinesAsync()
        {
            var testCode = @"
public class Foo
{
    public int this[int index]
    {
        get 
        { 
            return 1; 
        }
        set {}
    }
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(10, 9);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestIndexerOnlyGetterAsync()
        {
            var testCode = @"
public class Foo
{
    public int this[int index]
    {
        get { return 1; }
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestIndexerGetAndSetOnOneLineAsync()
        {
            var testCode = @"
public class Foo
{
    public int this[int index]
    {
        get { return 1; }
        set { }
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestIndexerGetAndSetOnOneLineWithCommentsAsync()
        {
            var testCode = @"
public class Foo
{
    public int this[int index]
    {
        // here's get
        get { return 1; }
        set { }
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestIndexerGetAndSetOnMultipleLinesAsync()
        {
            var testCode = @"
public class Foo
{
    public int this[int index]
    {
        get 
        { 
            return 1; 
        }
        set
        {
        }
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestEventAddIsOnMultipleLinesAndRemoveIsOnOneLineAsync()
        {
            var testCode = @"
public class Foo
{
    private System.EventHandler nameChanged;

    public event System.EventHandler NameChanged
    {
        add
        {
            this.nameChanged += value;
        }
        remove { this.nameChanged -= value; }
    }
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(12, 9);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestEventAddAndRemoveAreOnOneLineAsync()
        {
            var testCode = @"
public class Foo
{
    private System.EventHandler nameChanged;

    public event System.EventHandler NameChanged
    {
        add { this.nameChanged += value; }
        remove { this.nameChanged -= value; }
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestEventAddAndRemoveAreOnMultipleLinesAsync()
        {
            var testCode = @"
public class Foo
{
    private System.EventHandler nameChanged;

    public event System.EventHandler NameChanged
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

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestEventAddAndRemoveAreOnOneLineWithCommentAsync()
        {
            var testCode = @"
public class Foo
{
    private System.EventHandler nameChanged;

    public event System.EventHandler NameChanged
    {
        // this is add
        add { this.nameChanged += value; }
        remove { this.nameChanged -= value; }
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        protected override IEnumerable<DiagnosticAnalyzer> GetCSharpDiagnosticAnalyzers()
        {
            yield return new SA1504AllAccessorsMustBeSingleLineOrMultiLine();
        }
    }
}
