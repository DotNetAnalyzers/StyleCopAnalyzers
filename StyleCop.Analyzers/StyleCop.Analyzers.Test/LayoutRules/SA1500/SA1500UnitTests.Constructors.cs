﻿namespace StyleCop.Analyzers.Test.LayoutRules
{
    using System.Threading;
    using System.Threading.Tasks;

    using StyleCop.Analyzers.LayoutRules;
    using TestHelper;
    using Xunit;

    /// <summary>
    /// Unit tests for <see cref="SA1500CurlyBracketsForMultiLineStatementsMustNotShareLine"/>.
    /// </summary>
    public partial class SA1500UnitTests : DiagnosticVerifier
    {
        /// <summary>
        /// Verifies that no diagnostics are reported for the valid constructors defined in this test.
        /// </summary>
        /// <remarks>
        /// These are valid for SA1500 only, some will report other diagnostics from the layout (SA15xx) series.
        /// </remarks>
        [Fact]
        public async Task TestConstructorValid()
        {
            var testCode = @"using System.Diagnostics;

public class Foo
{
    // Valid constructor #1
    public Foo()
    {
    }

    // Valid constructor #2
    public Foo(bool a)
    {
        Debug.Indent();
    }

    // Valid constructor #3 (Valid only for SA1500)
    public Foo(byte a) { }

    // Valid constructor #4 (Valid only for SA1500)
    public Foo(short a) { Debug.Indent(); }

    // Valid constructor #5 (Valid only for SA1500)
    public Foo(short a) 
    { Debug.Indent(); }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that diagnostics will be reported for all invalid constructor definitions.
        /// </summary>
        [Fact(Skip = "Disabled until the SA1500 implementation is available")]
        public async Task TestConstructorInvalid()
        {
            var testCode = @"using System.Diagnostics;

public class Foo
{
    // Invalid constructor #1
    public Foo() {
    }

    // Invalid constructor #2
    public Foo(bool a) {
        Debug.Indent(); 
    }

    // Invalid constructor #3
    public Foo(byte a) {
        Debug.Indent(); }

    // Invalid constructor #4
    public Foo(short a) { Debug.Indent(); 
    }

    // Invalid constructor #5
    public Foo(ushort a) 
    { 
        Debug.Indent(); }

    // Invalid constructor #6
    public Foo(int a) 
    { Debug.Indent(); 
    }
}";

            var expectedDiagnostics = new[]
            {
                // Invalid constructor #1
                this.CSharpDiagnostic().WithLocation(6, 18),
                // Invalid constructor #2
                this.CSharpDiagnostic().WithLocation(10, 24),
                // Invalid constructor #3
                this.CSharpDiagnostic().WithLocation(15, 24),
                this.CSharpDiagnostic().WithLocation(16, 25),
                // Invalid constructor #4
                this.CSharpDiagnostic().WithLocation(19, 25),
                // Invalid constructor #5
                this.CSharpDiagnostic().WithLocation(25, 25),
                // Invalid constructor #6
                this.CSharpDiagnostic().WithLocation(29, 5)
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expectedDiagnostics, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
