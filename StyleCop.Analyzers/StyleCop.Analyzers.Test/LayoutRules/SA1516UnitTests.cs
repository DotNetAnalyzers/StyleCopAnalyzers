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
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestCorrectSpacing()
        {
            
            await this.VerifyCSharpDiagnosticAsync(CorrectCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
    class Foo { }
}
";

            var expected = new[]
            {
                this.CSharpDiagnostic().WithLocation(2, 7),
                this.CSharpDiagnostic().WithLocation(5, 11),
                this.CSharpDiagnostic().WithLocation(12, 23),
                this.CSharpDiagnostic().WithLocation(13, 23),
                this.CSharpDiagnostic().WithLocation(18, 23),
                this.CSharpDiagnostic().WithLocation(24, 13),
                this.CSharpDiagnostic().WithLocation(29, 23),
                this.CSharpDiagnostic().WithLocation(30, 21),
                this.CSharpDiagnostic().WithLocation(36, 17),
                this.CSharpDiagnostic().WithLocation(41, 11),
                this.CSharpDiagnostic().WithLocation(44, 11),
                this.CSharpDiagnostic().WithLocation(45, 11)
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);

            await this.VerifyCSharpFixAsync(testCode, CorrectCode).ConfigureAwait(false);
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
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
                this.CSharpDiagnostic().WithLocation(8, 23),
                this.CSharpDiagnostic().WithLocation(12, 16),
                this.CSharpDiagnostic().WithLocation(16, 10)
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);

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

            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
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
                this.CSharpDiagnostic().WithLocation(2, 18),
                this.CSharpDiagnostic().WithLocation(5, 12),
                this.CSharpDiagnostic().WithLocation(7, 15),
                this.CSharpDiagnostic().WithLocation(10, 10),
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);


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

            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
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
                this.CSharpDiagnostic().WithLocation(11, 9),
                this.CSharpDiagnostic().WithLocation(23, 9)
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);


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

            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
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