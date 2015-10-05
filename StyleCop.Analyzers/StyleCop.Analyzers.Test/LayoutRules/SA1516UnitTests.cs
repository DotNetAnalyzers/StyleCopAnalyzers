// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.LayoutRules
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.LayoutRules;
    using TestHelper;
    using Xunit;

    public class SA1516UnitTests : CodeFixVerifier
    {
        private const string CorrectCode = @"extern alias corlib;

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

        public string FooValue, BarValue;

        [Obsolete]
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
    extern alias system;

    using System;

    // Foo
    class Foo { }
}
";

        [Fact]
        public void DiagnosticMessageFormatIsCorrect()
        {
            Assert.Equal("Elements must be separated by blank line", this.GetCSharpDiagnosticAnalyzers().Single().SupportedDiagnostics.Single().MessageFormat.ToString());
        }

        [Fact]
        public async Task TestCorrectSpacingAsync()
        {
            await this.VerifyCSharpDiagnosticAsync(CorrectCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestWrongSpacingAsync()
        {
            var testCode = @"extern alias corlib;
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
        public string FooValue, BarValue;
        [Obsolete]
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
    extern alias system;
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
                this.CSharpDiagnostic().WithLocation(37, 1),
                this.CSharpDiagnostic().WithLocation(42, 1),
                this.CSharpDiagnostic().WithLocation(45, 1),
                this.CSharpDiagnostic().WithLocation(46, 1)
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);

            await this.VerifyCSharpFixAsync(testCode, CorrectCode).ConfigureAwait(false);
        }

        [Fact]
        public async Task HasEmptyLineWorksCorrectlyAsync()
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
        public async Task GetDiagnosticLocationWorksCorrectlyAsync()
        {
            // This test increases code coverage in SA1516ElementsMustBeSeparatedByBlankLine.GetDiagnosticLocation
            var testCode = @"using System;

namespace Foo
{
    public class Bar
    {
        public string TestProperty1 { get; set; }
        public void Foo()
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

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);

            var fixedCode = @"using System;

namespace Foo
{
    public class Bar
    {
        public string TestProperty1 { get; set; }

        public void Foo()
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
        public async Task TestInterfacesAndStructsAsync()
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
        public async Task TestIndexerAndEventsAsync()
        {
            string testCode = @"using System;

public class Foo
{
    public string this[string i]
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

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);

            string fixedCode = @"using System;

public class Foo
{
    public string this[string i]
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

        [Fact]
        public async Task TestThatCodeFixWorksOnFieldsAdjacentToMultiLineFieldsAsync()
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

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);

            string fixedCode = @"using System;

public class Foo
{
    private string experiment =
        string.Empty;

    private string experiment2;
}";

            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestThatDiagnosticIgnoresSingleLinePropertyAccessorsAsync()
        {
            string testCode = @"using System;

public class Foo
{
    public string FooProperty
    {
        get { return ""bar""; }
        set { }
    }
}";
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestThatDiagnosticIgnoresSingleLineEventAccessorsAsync()
        {
            string testCode = @"using System;

public class Foo
{
    public event System.EventHandler FooProperty
    {
        add { }
        remove { }
    }
}";
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestThatDiagnosticIsReportedOnDifferentLinePropertyAccessors1Async()
        {
            string testCode = @"using System;

public class Foo
{
    public string FooProperty
    {
        get { return ""bar""; }
        set
        {
        }
    }
}";
            var expected = new[]
            {
                this.CSharpDiagnostic().WithLocation(8, 1)
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestThatDiagnosticIIsReportedOnDifferentLineEventAccessors1Async()
        {
            string testCode = @"using System;

public class Foo
{
    public event System.EventHandler FooProperty
    {
        add { }
        remove
        {
        }
    }
}";
            var expected = new[]
            {
                this.CSharpDiagnostic().WithLocation(8, 1)
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestThatDiagnosticIsReportedOnDifferentLinePropertyAccessors2Async()
        {
            string testCode = @"using System;

public class Foo
{
    public string FooProperty
    {
        get
        {
            return ""bar"";
        }
        set { }
    }
}";
            var expected = new[]
            {
                this.CSharpDiagnostic().WithLocation(11, 1)
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestThatDiagnosticIIsReportedOnDifferentLineEventAccessors2Async()
        {
            string testCode = @"using System;

public class Foo
{
    public event System.EventHandler FooProperty
    {
        add
        {
        }
        remove { }
    }
}";
            var expected = new[]
            {
                this.CSharpDiagnostic().WithLocation(10, 1)
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that private fields with attributes are handled properly.
        /// This is a regression test for #1595
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task VerifyThatPrivateFieldsAreHandledProperlyAsync()
        {
            string testCode = @"using System;

public class TestClass
{
    [Obsolete]
    private int test1;
    private bool test2;
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        protected override IEnumerable<DiagnosticAnalyzer> GetCSharpDiagnosticAnalyzers()
        {
            yield return new SA1516ElementsMustBeSeparatedByBlankLine();
        }

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new SA1516CodeFixProvider();
        }
    }
}