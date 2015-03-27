namespace StyleCop.Analyzers.Test.LayoutRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.LayoutRules;
    using TestHelper;
    using Xunit;

    public class SA1516UnitTests : CodeFixVerifier
    {
        private const string DiagnosticId = SA1516ElementsMustBeSeparatedByBlankLine.DiagnosticId;

        private const string CorrectCode = @"extern alias Foo1;

using System;
using System.Linq;
using a = System.Collections.Generic;

namespace Foo
{
    public class Bar
    {
        public string Test1;
        public string Test2;
        public string Test3;

        public string TestProperty1 { get; set; }

        public string TestProperty2 { get; set; }
        /// <summary>
        /// A summary.
        /// </summary>
        public string TestProperty3 { get; set; }

        public string TestProperty4
        {
            get
            {
                return Test1;
            }

            set
            {
                Test1 = value;
            }
        }

        public string Foo, Bar;

        public enum TestEnum
        {
            Value1,
            Value2
        }
    }

    public enum Foobar
    {

    }
}

namespace Foot
{
    extern alias Foo2;

    using System;

    // Foo
    class Foo { }
}
";

        [Fact]
        public void DiagnosticMessageFormatIsCorrect()
        {
            Assert.Equal("Elements must be separated by blank line", this.GetCSharpDiagnosticAnalyzer().SupportedDiagnostics[0].MessageFormat.ToString());
        }

        [Fact]
        public async Task TestEmptySource()
        {
            var testCode = string.Empty;
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestCorrectSpacing()
        {
            
            await this.VerifyCSharpDiagnosticAsync(CorrectCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestWrongSpacing()
        {
            var testCode = @"extern alias Foo1;
using System;
using System.Linq;
using a = System.Collections.Generic;
namespace Foo
{
    public class Bar
    {
        public string Test1;
        public string Test2;
        public string Test3;
        public string TestProperty1 { get; set; }
        public string TestProperty2 { get; set; }
        /// <summary>
        /// A summary.
        /// </summary>
        public string TestProperty3 { get; set; }
        public string TestProperty4
        {
            get
            {
                return Test1;
            }
            set
            {
                Test1 = value;
            }
        }
        public string Foo, Bar;
        public enum TestEnum
        {
            Value1,
            Value2
        }
    }
    public enum Foobar
    {

    }
}
namespace Foot
{
    extern alias Foo2;
    using System;
    // Foo
    class Foo { }
}
";

            var expected = new[]
            {
                this.CSharpDiagnostic().WithLocation(2, 1),
                this.CSharpDiagnostic().WithLocation(5, 1),
                this.CSharpDiagnostic().WithLocation(12, 1),
                this.CSharpDiagnostic().WithLocation(13, 1),
                this.CSharpDiagnostic().WithLocation(18, 1),
                this.CSharpDiagnostic().WithLocation(24, 1),
                this.CSharpDiagnostic().WithLocation(29, 1),
                this.CSharpDiagnostic().WithLocation(30, 1),
                this.CSharpDiagnostic().WithLocation(36, 1),
                this.CSharpDiagnostic().WithLocation(41, 1),
                this.CSharpDiagnostic().WithLocation(44, 1),
                this.CSharpDiagnostic().WithLocation(45, 1)
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);

            await this.VerifyCSharpFixAsync(testCode, CorrectCode);
        }

        [Fact]
        public async Task HasEmptyLineWorksCorrectly()
        {
            // This test increases code coverage in SA1516ElementsMustBeSeparatedByBlankLine.HasEmptyLine

            var testCode = @"using System;

namespace Foo
{
    public class Bar
    {

        public string TestProperty1 { get; set; } // A comment
        // A comment
        // A comment
        // A comment

        // More comments
        public string TestProperty2 { get; set; }
    }
}
";
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task GetDiagnosticLocationWorksCorrectly()
        {
            // This test increases code coverage in SA1516ElementsMustBeSeparatedByBlankLine.GetDiagnosticLocation

            var testCode = @"using System;

namespace Foo
{
    public class Bar
    {
        public string TestProperty1 { get; set; }
        public string Foo()
        {

        }
        public Bar()
        {

        }
        ~Bar()
        {

        }
    }
}
";
            var expected = new[]
            {
                this.CSharpDiagnostic().WithLocation(8, 1),
                this.CSharpDiagnostic().WithLocation(12, 1),
                this.CSharpDiagnostic().WithLocation(16, 1)
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);

            var fixedCode = @"using System;

namespace Foo
{
    public class Bar
    {
        public string TestProperty1 { get; set; }

        public string Foo()
        {

        }

        public Bar()
        {

        }

        ~Bar()
        {

        }
    }
}
";

            await this.VerifyCSharpFixAsync(testCode, fixedCode);
        }

        [Fact]
        public async Task TestInterfacesAndStructs()
        {
            string testCode = @"using System;
public interface IInterface
{
    string Foo();
    string Bar();
}
public struct Struct
{
    void Foo() { }
    void Bar() { }
}";
            var expected = new[]
            {
                this.CSharpDiagnostic().WithLocation(2, 1),
                this.CSharpDiagnostic().WithLocation(5, 1),
                this.CSharpDiagnostic().WithLocation(7, 1),
                this.CSharpDiagnostic().WithLocation(10, 1),
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);

            string fixedCode = @"using System;

public interface IInterface
{
    string Foo();

    string Bar();
}

public struct Struct
{
    void Foo() { }

    void Bar() { }
}";

            await this.VerifyCSharpFixAsync(testCode, fixedCode);
        }

        [Fact]
        public async Task TestIndexerAndEvents()
        {
            string testCode = @"using System;

public class Foo
{
    public string this[int i]
    {
        get
        {
            return i;
        }
        set
        {

        }
    }

    public event EventHandler MyEvent
    {
        add
        {

        }
        remove
        {

        }
    }
}";
            var expected = new[]
            {
                this.CSharpDiagnostic().WithLocation(11, 1),
                this.CSharpDiagnostic().WithLocation(23, 1)
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);

            string fixedCode = @"using System;

public class Foo
{
    public string this[int i]
    {
        get
        {
            return i;
        }

        set
        {

        }
    }

    public event EventHandler MyEvent
    {
        add
        {

        }

        remove
        {

        }
    }
}";

            await this.VerifyCSharpFixAsync(testCode, fixedCode);
        }

        [Fact]
        public async Task TestThatCodeFixWorksOnFieldsAdjacentToMultiLineFields()
        {
            string testCode = @"using System;

public class Foo
{
    private string experiment =
        string.Empty;
    private string experiment2;
}";
            var expected = new[]
            {
                this.CSharpDiagnostic().WithLocation(7, 1)
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);


            string fixedCode = @"using System;

public class Foo
{
    private string experiment =
        string.Empty;

    private string experiment2;
}";

            await this.VerifyCSharpFixAsync(testCode, fixedCode);
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new SA1516ElementsMustBeSeparatedByBlankLine();
        }

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new SA1516CodeFixProvider();
        }
    }
}