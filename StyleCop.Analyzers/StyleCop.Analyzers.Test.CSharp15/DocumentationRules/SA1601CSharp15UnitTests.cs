// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp15.DocumentationRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp14.DocumentationRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopDiagnosticVerifier<
        StyleCop.Analyzers.DocumentationRules.SA1601PartialElementsMustBeDocumented>;

    public partial class SA1601CSharp15UnitTests : SA1601CSharp14UnitTests
    {
        /// <summary>
        /// Verifies that each undocumented part of a partial union is reported exactly once. Like other partial types,
        /// every part of a partial union needs documentation.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [WorkItem(4182, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4182")]
        public async Task TestPartialUnionWithoutDocumentationAsync()
        {
            var testCode = @"
public partial union {|#0:Pet|}(int, string);

/// <summary>
/// A documented class.
/// </summary>
public class Outer
{
    public partial union {|#1:NestedPet|}(int, string);
}

/// <summary>
/// The documented part.
/// </summary>
public partial union Animal(int, string);

public partial union {|#2:Animal|}
{
}
";

            DiagnosticResult[] expected =
            {
                Diagnostic().WithLocation(0),
                Diagnostic().WithLocation(1),
                Diagnostic().WithLocation(2),
            };

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a partial union is not reported when every part is documented with either a
        /// <c>&lt;summary&gt;</c> or a <c>&lt;content&gt;</c> tag.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [WorkItem(4182, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4182")]
        public async Task TestPartialUnionWithDocumentationAsync()
        {
            var testCode = @"
/// <summary>
/// A union.
/// </summary>
public partial union Pet(int, string);

/// <content>
/// More members of the union.
/// </content>
public partial union Pet
{
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that partial methods in a union follow the partial member policy: one documented part is enough,
        /// and when neither part is documented, each part is reported exactly once.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [WorkItem(4182, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4182")]
        public async Task TestPartialMethodsInUnionAsync()
        {
            var testCode = @"
/// <summary>
/// A union.
/// </summary>
public partial union Pet(int, string)
{
    /// <summary>
    /// Feeds the pet.
    /// </summary>
    public partial void Feed();

    public partial void {|#0:Walk|}();
}

/// <content>
/// The implementations.
/// </content>
public partial union Pet
{
    public partial void Feed()
    {
    }

    public partial void {|#1:Walk|}()
    {
    }
}
";

            DiagnosticResult[] expected =
            {
                Diagnostic().WithLocation(0),
                Diagnostic().WithLocation(1),
            };

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
