// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp15.DocumentationRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using StyleCop.Analyzers.Test.CSharp14.DocumentationRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopDiagnosticVerifier<
        StyleCop.Analyzers.DocumentationRules.SA1618GenericTypeParametersMustBeDocumented>;

    public partial class SA1618CSharp15UnitTests : SA1618CSharp14UnitTests
    {
        /// <summary>
        /// Verifies that an undocumented type parameter of a generic union is reported exactly once.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [WorkItem(4182, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4182")]
        public async Task TestGenericUnionAsync()
        {
            var testCode = @"
/// <summary>
/// A union.
/// </summary>
public union Result<{|#0:T|}>(T, string);

/// <summary>
/// A documented union.
/// </summary>
/// <typeparam name=""T"">The value type.</typeparam>
public union Option<T>(T, string);
";

            var expected = Diagnostic().WithLocation(0).WithArguments("T");

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
