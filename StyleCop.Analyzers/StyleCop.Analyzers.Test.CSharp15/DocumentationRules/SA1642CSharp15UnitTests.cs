// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp15.DocumentationRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp14.DocumentationRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.DocumentationRules.SA1642ConstructorSummaryDocumentationMustBeginWithStandardText,
        StyleCop.Analyzers.DocumentationRules.SA1642SA1643CodeFixProvider>;

    public partial class SA1642CSharp15UnitTests : SA1642CSharp14UnitTests
    {
        /// <summary>
        /// Verifies that the standard text for a union constructor refers to a union, that the diagnostic is reported
        /// exactly once, and that the code fix adds the union text.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [WorkItem(4182, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4182")]
        public async Task TestUnionConstructorAsync()
        {
            var testCode = @"
/// <summary>
/// A union.
/// </summary>
public union Pet(int, string)
{
    /// [|<summary>
    /// Creates a pet.
    /// </summary>|]
    /// <param name=""legs"">The number of legs.</param>
    /// <param name=""name"">The name.</param>
    public Pet(int legs, string name)
        : this(legs)
    {
    }
}
";

            var fixedCode = @"
/// <summary>
/// A union.
/// </summary>
public union Pet(int, string)
{
    /// <summary>
    /// Initializes a new instance of the <see cref=""Pet""/> union.
    /// Creates a pet.
    /// </summary>
    /// <param name=""legs"">The number of legs.</param>
    /// <param name=""name"">The name.</param>
    public Pet(int legs, string name)
        : this(legs)
    {
    }
}
";

            await VerifyCSharpFixAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the code fix for a constructor of a generic union includes the type parameters in the
        /// <c>cref</c>.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [WorkItem(4182, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4182")]
        public async Task TestGenericUnionConstructorAsync()
        {
            var testCode = @"
/// <summary>
/// A union.
/// </summary>
/// <typeparam name=""T"">The value type.</typeparam>
public union Result<T>(T, string)
{
    /// [|<summary>
    /// Creates a result.
    /// </summary>|]
    /// <param name=""value"">The value.</param>
    /// <param name=""message"">The message.</param>
    public Result(T value, string message)
        : this(value)
    {
    }
}
";

            var fixedCode = @"
/// <summary>
/// A union.
/// </summary>
/// <typeparam name=""T"">The value type.</typeparam>
public union Result<T>(T, string)
{
    /// <summary>
    /// Initializes a new instance of the <see cref=""Result{T}""/> union.
    /// Creates a result.
    /// </summary>
    /// <param name=""value"">The value.</param>
    /// <param name=""message"">The message.</param>
    public Result(T value, string message)
        : this(value)
    {
    }
}
";

            await VerifyCSharpFixAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a union constructor documented with the union standard text is not reported.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [WorkItem(4182, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4182")]
        public async Task TestUnionConstructorWithUnionTextAsync()
        {
            var testCode = @"
/// <summary>
/// A union.
/// </summary>
public union Pet(int, string)
{
    /// <summary>
    /// Initializes a new instance of the <see cref=""Pet""/> union.
    /// </summary>
    /// <param name=""legs"">The number of legs.</param>
    /// <param name=""name"">The name.</param>
    public Pet(int legs, string name)
        : this(legs)
    {
    }
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the standard text for a static union constructor refers to a union, that the diagnostic is
        /// reported exactly once, and that the code fix adds the union text.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [WorkItem(4182, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4182")]
        public async Task TestUnionStaticConstructorAsync()
        {
            var testCode = @"
/// <summary>
/// A union.
/// </summary>
public union Pet(int, string)
{
    /// [|<summary>
    /// Sets up shared state.
    /// </summary>|]
    static Pet()
    {
    }
}
";

            var fixedCode = @"
/// <summary>
/// A union.
/// </summary>
public union Pet(int, string)
{
    /// <summary>
    /// Initializes static members of the <see cref=""Pet""/> union.
    /// Sets up shared state.
    /// </summary>
    static Pet()
    {
    }
}
";

            await VerifyCSharpFixAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a union constructor documented with the struct standard text is reported exactly once, and that
        /// the code fix adds the union text.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [WorkItem(4182, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4182")]
        public async Task TestUnionConstructorWithStructTextAsync()
        {
            var testCode = @"
/// <summary>
/// A union.
/// </summary>
public union Pet(int, string)
{
    /// [|<summary>
    /// Initializes a new instance of the <see cref=""Pet""/> struct.
    /// </summary>|]
    /// <param name=""legs"">The number of legs.</param>
    /// <param name=""name"">The name.</param>
    public Pet(int legs, string name)
        : this(legs)
    {
    }
}
";

            var fixedCode = @"
/// <summary>
/// A union.
/// </summary>
public union Pet(int, string)
{
    /// <summary>
    /// Initializes a new instance of the <see cref=""Pet""/> union.
    /// Initializes a new instance of the <see cref=""Pet""/> struct.
    /// </summary>
    /// <param name=""legs"">The number of legs.</param>
    /// <param name=""name"">The name.</param>
    public Pet(int legs, string name)
        : this(legs)
    {
    }
}
";

            await VerifyCSharpFixAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
