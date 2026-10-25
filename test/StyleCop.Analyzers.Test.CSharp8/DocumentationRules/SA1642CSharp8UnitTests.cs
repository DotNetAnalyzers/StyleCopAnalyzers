// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp8.DocumentationRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.DocumentationRules.SA1642ConstructorSummaryDocumentationMustBeginWithStandardText,
        StyleCop.Analyzers.DocumentationRules.SA1642SA1643CodeFixProvider>;

    public partial class SA1642CSharp8UnitTests
    {
        /// <summary>
        /// Verifies that the standard text for a static constructor of an interface refers to an interface.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestInterfaceStaticConstructorWithInterfaceTextAsync()
        {
            var testCode = @"
/// <summary>
/// An interface.
/// </summary>
public interface ITest
{
    /// <summary>
    /// Initializes static members of the <see cref=""ITest""/> interface.
    /// </summary>
    static ITest()
    {
    }
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the "class" text is reported for a static constructor of an interface, and that the code fix
        /// adds the interface text.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestInterfaceStaticConstructorWithClassTextAsync()
        {
            var testCode = @"
/// <summary>
/// An interface.
/// </summary>
/// <typeparam name=""T"">The type.</typeparam>
public interface ITest<T>
{
    /// [|<summary>
    /// Initializes static members of the <see cref=""ITest{T}""/> class.
    /// </summary>|]
    static ITest()
    {
    }
}
";

            var fixedCode = @"
/// <summary>
/// An interface.
/// </summary>
/// <typeparam name=""T"">The type.</typeparam>
public interface ITest<T>
{
    /// <summary>
    /// Initializes static members of the <see cref=""ITest{T}""/> interface.
    /// Initializes static members of the <see cref=""ITest{T}""/> class.
    /// </summary>
    static ITest()
    {
    }
}
";

            await VerifyCSharpFixAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
