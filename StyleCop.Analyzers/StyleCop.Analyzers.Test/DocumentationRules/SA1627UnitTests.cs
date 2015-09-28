// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.DocumentationRules
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Analyzers.DocumentationRules;
    using Microsoft.CodeAnalysis.Diagnostics;
    using TestHelper;
    using Xunit;

    /// <summary>
    /// This class contains unit tests for <see cref="SA1627DocumentationTextMustNotBeEmpty"/>.
    /// </summary>
    public class SA1627UnitTests : DiagnosticVerifier
    {
        public static IEnumerable<object[]> Elements
        {
            get
            {
                yield return new[] { "remarks" };
                yield return new[] { "example" };
                yield return new[] { "permission" };
                yield return new[] { "exception" };
            }
        }

        /// <summary>
        /// Checks an element with a blank value gives an error.
        /// </summary>
        /// <param name="element">Element to check</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [MemberData(nameof(Elements))]
        public async Task TestMemberWithBlankElementAsync(string element)
        {
            var testCode = @"
/// <summary>
/// Foo
/// </summary>
public class ClassName
{
    /// <summary>
    /// Foo
    /// </summary>
    ///<param name=""foo"">Test</param>
    ///<param name=""bar"">Test</param>
    /// <$$>  </$$>
    public ClassName Method(string foo, string bar) { return null; }
}";
            var expectedDiagnostic = this.CSharpDiagnostic().WithLocation(12, 9).WithArguments(element);
            await this.VerifyCSharpDiagnosticAsync(testCode.Replace("$$", element), expectedDiagnostic, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Checks an element with a blank value gives an error.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestMemberWithMultipleBlankElementsAsync()
        {
            var testCode = @"
/// <summary>
/// Foo
/// </summary>
public class ClassName
{
    /// <summary>
    /// Foo
    /// </summary>
    ///<param name=""foo"">Test</param>
    ///<param name=""bar"">Test</param>
    /// <remarks>Foo bar</remarks>
    /// <example></example>
    /// <exception>
    ///
    /// </exception>
    /// <permission>
    /// Multi line notes
    /// Multi line notes
    /// </permission>
    public ClassName Method(string foo, string bar) { return null; }
}";
            var expectedDiagnostics = new[]
            {
                this.CSharpDiagnostic().WithLocation(13, 9).WithArguments("example"),
                this.CSharpDiagnostic().WithLocation(14, 9).WithArguments("exception")
            };
            await this.VerifyCSharpDiagnosticAsync(testCode, expectedDiagnostics, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Checks an element with an empty element gives an error.
        /// </summary>
        /// <param name="element">Element to check</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [MemberData(nameof(Elements))]
        public async Task TestMemberWithEmptyElementAsync(string element)
        {
            var testCode = @"
/// <summary>
/// Foo
/// </summary>
public class ClassName
{
    /// <summary>
    /// Foo
    /// </summary>
    ///<param name=""foo"">Test</param>
    ///<param name=""bar"">Test</param>
    /// <$$ />
    public ClassName Method(string foo, string bar) { return null; }
}";
            var expectedDiagnostic = this.CSharpDiagnostic().WithLocation(12, 9).WithArguments(element);
            await this.VerifyCSharpDiagnosticAsync(testCode.Replace("$$", element), expectedDiagnostic, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Checks an element with non blank text does not give an error.
        /// </summary>
        /// <param name="element">Element to check</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [MemberData(nameof(Elements))]
        public async Task TestMemberWithValidElementAsync(string element)
        {
            var testCode = @"
/// <summary>
/// Foo
/// </summary>
public class ClassName
{
    /// <summary>
    /// Foo
    /// </summary>
    ///<param name=""foo"">Test</param>
    ///<param name=""bar"">Test</param>
    /// <$$>FooBar</$$>
    public ClassName Method(string foo, string bar) { return null; }
}";
            await this.VerifyCSharpDiagnosticAsync(testCode.Replace("$$", element), EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        protected override IEnumerable<DiagnosticAnalyzer> GetCSharpDiagnosticAnalyzers()
        {
            yield return new SA1627DocumentationTextMustNotBeEmpty();
        }
    }
}
