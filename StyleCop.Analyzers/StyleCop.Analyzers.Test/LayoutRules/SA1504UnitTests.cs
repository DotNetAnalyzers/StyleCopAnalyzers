using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Diagnostics;
using StyleCop.Analyzers.LayoutRules;
using TestHelper;
using Xunit;

namespace StyleCop.Analyzers.Test.LayoutRules
{
    public class SA1504UnitTests : CodeFixVerifier
    {
        [Fact]
        public async Task TestEmptySource()
        {
            var testCode = string.Empty;
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestPropertyGetInOneLineSetInMultipleLines()
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

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(7, 9);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [Fact]
        public async Task TestPropertyGetAndSetOnOneLine()
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

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestPropertyGetAndSetOnOneLineWithComments()
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

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestPropertyGetAndSetOnMultipleLines()
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

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestPropertyOnlyGetter()
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

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestAutoProperty()
        {
            var testCode = @"
public class Foo
{
    public int Prop
    {
        get;set;
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestPropertyExpressionBody()
        {
            var testCode = @"
public class Foo
{
    public int Prop => 1;
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestIndexerSetInOneLineGetInMultipleLines()
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

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [Fact]
        public async Task TestIndexerOnlyGetter()
        {
            var testCode = @"
public class Foo
{
    public int this[int index]
    {
        get { return 1; }
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestIndexerGetAndSetOnOneLine()
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

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestIndexerGetAndSetOnOneLineWithComments()
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

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestIndexerGetAndSetOnMultipleLines()
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

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestEventAddIsOnMultipleLinesAndRemoveIsOnOneLine()
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

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [Fact]
        public async Task TestEventAddAndRemoveAreOnOneLine()
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

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestEventAddAndRemoveAreOnMultipleLines()
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

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestEventAddAndRemoveAreOnOneLineWithComment()
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

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new SA1504AllAccessorsMustBeSingleLineOrMultiLine();
        }
    }
}