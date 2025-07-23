// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.Test.LayoutRules
{
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.LayoutRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.LayoutRules.SA1516ElementsMustBeSeparatedByBlankLine,
        StyleCop.Analyzers.LayoutRules.SA1516CodeFixProvider>;

    public class SA1516UnitTests
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
            Assert.Equal("Elements should be separated by blank line", new SA1516ElementsMustBeSeparatedByBlankLine().SupportedDiagnostics.Single().MessageFormat.ToString());
        }

        [Fact]
        public async Task TestCorrectSpacingAsync()
        {
            await VerifyCSharpDiagnosticAsync(CorrectCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
                Diagnostic().WithLocation(2, 1),
                Diagnostic().WithLocation(5, 1),
                Diagnostic().WithLocation(12, 1),
                Diagnostic().WithLocation(18, 1),
                Diagnostic().WithLocation(24, 1),
                Diagnostic().WithLocation(29, 1),
                Diagnostic().WithLocation(30, 1),
                Diagnostic().WithLocation(37, 1),
                Diagnostic().WithLocation(42, 1),
                Diagnostic().WithLocation(45, 1),
                Diagnostic().WithLocation(46, 1),
            };

            await VerifyCSharpFixAsync(testCode, expected, CorrectCode, CancellationToken.None).ConfigureAwait(false);
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
            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
                Diagnostic().WithLocation(8, 1),
                Diagnostic().WithLocation(12, 1),
                Diagnostic().WithLocation(16, 1),
            };

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

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
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
                Diagnostic().WithLocation(2, 1),
                Diagnostic().WithLocation(5, 1),
                Diagnostic().WithLocation(7, 1),
                Diagnostic().WithLocation(10, 1),
            };

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

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
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
                Diagnostic().WithLocation(11, 1),
                Diagnostic().WithLocation(23, 1),
            };

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

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
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
                Diagnostic().WithLocation(7, 1),
            };

            string fixedCode = @"using System;

public class Foo
{
    private string experiment =
        string.Empty;

    private string experiment2;
}";

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
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
            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
                Diagnostic().WithLocation(8, 1),
            };

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
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
                Diagnostic().WithLocation(8, 1),
            };

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
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
                Diagnostic().WithLocation(11, 1),
            };

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
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
                Diagnostic().WithLocation(10, 1),
            };

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that private fields with attributes are handled properly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [WorkItem(1595, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/1595")]
        public async Task VerifyThatPrivateFieldsAreHandledProperlyAsync()
        {
            string testCode = @"using System;

public class TestClass
{
    [Obsolete]
    private int test1;
    private bool test2;
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that setters with an accessibility restriction will report a warning.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [WorkItem(2269, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/2269")]
        public async Task TestSetterWithAccessibilityRestrictionAsync()
        {
            var testCode = @"
public class TestClass
{
    public int TestProtected
    {
        get
        {
            return 1;
        }
        protected set
        {
        }
    }

    public int TestInternal
    {
        get
        {
            return 1;
        }
        internal set
        {
        }
    }

    public int TestPrivate
    {
        get
        {
            return 1;
        }
        private set
        {
        }
    }

    public int this[int i]
    {
        get
        {
            return 1;
        }
        protected set
        {
        }
    }
}

public class TestClass2
{
    public int this[int i]
    {
        get
        {
            return 1;
        }
        internal set
        {
        }
    }
}

public class TestClass3
{
    public int this[int i]
    {
        get
        {
            return 1;
        }
        private set
        {
        }
    }
}
";

            var fixedTestCode = @"
public class TestClass
{
    public int TestProtected
    {
        get
        {
            return 1;
        }

        protected set
        {
        }
    }

    public int TestInternal
    {
        get
        {
            return 1;
        }

        internal set
        {
        }
    }

    public int TestPrivate
    {
        get
        {
            return 1;
        }

        private set
        {
        }
    }

    public int this[int i]
    {
        get
        {
            return 1;
        }

        protected set
        {
        }
    }
}

public class TestClass2
{
    public int this[int i]
    {
        get
        {
            return 1;
        }

        internal set
        {
        }
    }
}

public class TestClass3
{
    public int this[int i]
    {
        get
        {
            return 1;
        }

        private set
        {
        }
    }
}
";

            var expected = new[]
            {
                Diagnostic().WithLocation(10, 1),
                Diagnostic().WithLocation(21, 1),
                Diagnostic().WithLocation(32, 1),
                Diagnostic().WithLocation(43, 1),
                Diagnostic().WithLocation(57, 1),
                Diagnostic().WithLocation(71, 1),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [InlineData("namespace")]
        [InlineData("public class")]
        [WorkItem(1923, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/1923")]
        public async Task TestBlankLinesAroundAssemblyAttributesAsync(string followingElementKind)
        {
            string testCode = $@"using System.Runtime.CompilerServices;
[assembly: InternalsVisibleTo(""AnotherAssembly"")]
{followingElementKind} Foo
{{
}}";
            string fixedTestCode = $@"using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo(""AnotherAssembly"")]

{followingElementKind} Foo
{{
}}";

            var expected = new[]
            {
                Diagnostic().WithLocation(2, 1),
                Diagnostic().WithLocation(3, 1),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
