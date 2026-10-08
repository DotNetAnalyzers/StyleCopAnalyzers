// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp14.DocumentationRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp13.DocumentationRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopDiagnosticVerifier<StyleCop.Analyzers.DocumentationRules.SA1600ElementsMustBeDocumented>;

    public partial class SA1600CSharp14UnitTests : SA1600CSharp13UnitTests
    {
        [Fact]
        [WorkItem(4023, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4023")]
        public async Task TestExtensionBlockWithoutDocumentationAsync()
        {
            // The extension block itself doesn't need documentation, but its exposed members do.
            var testCode = @"
/// <summary>
/// Xyz.
/// </summary>
public static class TestClass
{
    extension(string source)
    {
        public int {|#0:Length1|} => source.Length;

        public int {|#1:GetLength|}() => source.Length;

        public static int {|#2:Create|}() => 0;

        internal int {|#3:Length2|} => source.Length;

        private int Length3 => source.Length;
    }
}

/// <summary>
/// Xyz.
/// </summary>
internal static class TestClass2
{
    extension(string source)
    {
        public int {|#4:Length1|} => source.Length;

        private int Length3 => source.Length;
    }
}
";

            DiagnosticResult[] expected =
            {
                Diagnostic().WithLocation(0),
                Diagnostic().WithLocation(1),
                Diagnostic().WithLocation(2),
                Diagnostic().WithLocation(3),
                Diagnostic().WithLocation(4),
            };

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(4029, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4029")]
        public async Task TestPartialConstructorAndEventWithoutDocumentationAsync()
        {
            // Partial members are handled by SA1601, which only reports them when no part of the member is documented.
            var testCode = @"
using System;

/// <summary>
/// A class.
/// </summary>
public partial class TestClass
{
    public partial TestClass(int x);

    public partial event EventHandler TestEvent;
}

/// <content>
/// More.
/// </content>
public partial class TestClass
{
    public partial TestClass(int x)
    {
    }

    public partial event EventHandler TestEvent
    {
        add { }
        remove { }
    }
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
