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
/// Class name.
/// </summary>
public class ClassName
{
    /// <summary>
    /// Join together two strings.
    /// </summary>
    ///<param name=""first"">First string.</param>
    ///<param name=""second"">Second string.</param>
    /// <$$>  </$$>
    public string JoinStrings(string first, string second) { return first + second; }
}";
            var expectedDiagnostic = this.CSharpDiagnostic().WithLocation(12, 9).WithArguments(element);
            await this.VerifyCSharpDiagnosticAsync(testCode.Replace("$$", element), expectedDiagnostic, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Checks an element with a multiple blank values give multiple errors.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestMemberWithMultipleBlankElementsAsync()
        {
            var testCode = @"
/// <summary>
/// Class name.
/// </summary>
public class ClassName
{
    /// <summary>
    /// Join together two strings.
    /// </summary>
    ///<param name=""first"">First string.</param>
    ///<param name=""second"">Second string.</param>
    /// <remarks>Single line remark.</remarks>
    /// <example></example>
    /// <exception>
    ///
    /// </exception>
    /// <permission>
    /// Multi line notes.
    /// Multi line notes.
    /// </permission>
    public string JoinStrings(string first, string second) { return first + second; }
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
/// Class name.
/// </summary>
public class ClassName
{
    /// <summary>
    /// Join together two strings.
    /// </summary>
    ///<param name=""first"">First string.</param>
    ///<param name=""second"">Second string.</param>
    /// <$$ />
    public string JoinStrings(string first, string second) { return first + second; }
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
/// Class name.
/// </summary>
public class ClassName
{
    /// <summary>
    /// Join together two strings.
    /// </summary>
    ///<param name=""first"">First string.</param>
    ///<param name=""second"">Second string.</param>
    /// <$$>Not blank element.</$$>
    public string JoinStrings(string first, string second) { return first + second; }
}";
            await this.VerifyCSharpDiagnosticAsync(testCode.Replace("$$", element), EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        protected override IEnumerable<DiagnosticAnalyzer> GetCSharpDiagnosticAnalyzers()
        {
            yield return new SA1627DocumentationTextMustNotBeEmpty();
        }
    }
}
