// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp10.LayoutRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp9.LayoutRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.LayoutRules.SA1516ElementsMustBeSeparatedByBlankLine,
        StyleCop.Analyzers.LayoutRules.SA1516CodeFixProvider>;

    public class SA1516CSharp10UnitTests : SA1516CSharp9UnitTests
    {
        private const string CorrectCode = @"extern alias corlib;

using System;
using System.Linq;
using a = System.Collections.Generic;

namespace Foo;

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
";

        /// <summary>
        /// Verifies that SA1516 is not reported for code with correct blank lines.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [WorkItem(3512, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3512")]
        public async Task TestFileScopedNamespaceCorrectSpacingAsync()
        {
            await VerifyCSharpDiagnosticAsync(CorrectCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that SA1516 is reported for code with missing correct blank lines.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [WorkItem(3512, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3512")]
        public async Task TestFileScopedNamespaceWrongSpacingAsync()
        {
            var testCode = @"extern alias corlib;
{|#0:using|} System;
using System.Linq;
using a = System.Collections.Generic;
{|#1:namespace|} Foo;
{|#2:public|} class Bar
{
}
{|#3:public|} enum Foobar
{
}
";

            var fixedCode = @"extern alias corlib;

using System;
using System.Linq;
using a = System.Collections.Generic;

namespace Foo;

public class Bar
{
}

public enum Foobar
{
}
";

            var test = new CSharpTest(LanguageVersion.CSharp10)
            {
                ReferenceAssemblies = ReferenceAssemblies.Net.Net50,
                TestState =
                {
                    OutputKind = OutputKind.DynamicallyLinkedLibrary,
                    Sources = { testCode },
                },
                FixedCode = fixedCode,
            };
            var expectedDiagnostic = new[] {
                Diagnostic().WithLocation(0),
                Diagnostic().WithLocation(1),
                Diagnostic().WithLocation(2),
                Diagnostic().WithLocation(3),
            };
            test.TestState.ExpectedDiagnostics.AddRange(expectedDiagnostic);
            await test.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }
    }
}
