// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.Test.CSharp7.HelperTests
{
    using System.Threading.Tasks;
    using StyleCop.Analyzers.Test.HelperTests;
    using Xunit;

    /// <summary>
    /// Unit tests for the <see cref="StyleCop.Analyzers.Helpers.SymbolNameHelpers"/> class in the context of C# 7.x.
    /// </summary>
    public partial class SymbolNameHelpersCSharp7UnitTests : SymbolNameHelpersUnitTests
    {
        /// <summary>
        /// Verify the workings of <see cref="StyleCop.Analyzers.Helpers.SymbolNameHelpers.ToQualifiedString(Microsoft.CodeAnalysis.ISymbol, Microsoft.CodeAnalysis.CSharp.Syntax.NameSyntax)"/>
        /// for standard use cases.
        /// </summary>
        /// <param name="inputString">A string representation of a type or namespace to process.</param>
        /// <param name="isNamespace"><see langword="true"/> if <paramref name="inputString"/> is a namespace;
        /// <see langword="false"/> if <paramref name="inputString"/> is a type to be used as an alias target.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [InlineData("System.ValueTuple<int, object[]>")]
        [InlineData("System.ValueTuple<int, (int, object)>")]
        [InlineData("System.ValueTuple<int, (int, object)?>")]
        [InlineData("System.ValueTuple<int, (int, object[])>")]
        [InlineData("System.Collections.Generic.KeyValuePair<int, (int, object)>")]
        [InlineData("System.Collections.Generic.KeyValuePair<int, (int, object[])>")]
        [InlineData("System.Nullable<(int, object)>")]
        [WorkItem(3149, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3149")]
        public Task VerifyToQualifiedStringTuplesAsync(string inputString, bool isNamespace = false)
        {
            return this.PerformTestAsync(inputString, isNamespace);
        }
    }
}
