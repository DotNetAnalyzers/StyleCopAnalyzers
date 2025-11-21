// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.Test.CSharp8.HelperTests
{
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using StyleCop.Analyzers.Test.CSharp7.HelperTests;
    using Xunit;

    /// <summary>
    /// Unit tests for the <see cref="StyleCop.Analyzers.Helpers.SymbolNameHelpers"/> class in the context of C# 8.0.
    /// </summary>
    public partial class SymbolNameHelpersCSharp8UnitTests : SymbolNameHelpersCSharp7UnitTests
    {
        /// <summary>
        /// Verify the workings of <see cref="StyleCop.Analyzers.Helpers.SymbolNameHelpers.ToQualifiedString(ISymbol, NameSyntax)"/>
        /// for nullable reference types.
        /// </summary>
        /// <param name="inputString">A string representation of a type or namespace to process.</param>
        /// <param name="isNamespace"><see langword="true"/> if <paramref name="inputString"/> is a namespace;
        /// <see langword="false"/> if <paramref name="inputString"/> is a type to be used as an alias target.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [InlineData("System.ValueTuple<System.Int32, System.Object?>")]
        [InlineData("System.ValueTuple<int, object?>")]
        [InlineData("System.ValueTuple<int, object?[]>")]
        [InlineData("System.ValueTuple<int, object[]?>")]
        [InlineData("System.ValueTuple<int, object?[]?>")]
        [InlineData("System.ValueTuple<int?[]?, object>")]
        [InlineData("System.ValueTuple<int, (int, object?)>")]
        [InlineData("System.ValueTuple<int, (int, object?)?>")]
        [InlineData("System.ValueTuple<int, (int, object?[])>")]
        [InlineData("System.ValueTuple<int, (int, object[]?)>")]
        [InlineData("System.ValueTuple<int, (int, object?[]?)>")]
        [InlineData("System.Collections.Generic.KeyValuePair<int, object?>")]
        [InlineData("System.Collections.Generic.KeyValuePair<int, object?[]>")]
        [InlineData("System.Collections.Generic.KeyValuePair<int, object[]?>")]
        [InlineData("System.Collections.Generic.KeyValuePair<int, object?[]?>")]
        [InlineData("System.Collections.Generic.KeyValuePair<int, (int, object?)>")]
        [InlineData("System.Collections.Generic.KeyValuePair<int, (int, object?[])>")]
        [InlineData("System.Collections.Generic.KeyValuePair<int, (int, object[]?)>")]
        [InlineData("System.Collections.Generic.KeyValuePair<int, (int, object?[]?)>")]
        [InlineData("System.Nullable<(int, object?)>")]
        [WorkItem(3149, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3149")]
        public Task VerifyToQualifiedStringNullableReferenceTypesAsync(string inputString, bool isNamespace = false)
        {
            return this.PerformTestAsync(inputString, isNamespace);
        }

        /// <summary>
        /// Provides a <see cref="CSharpCompilationOptions"/> instance with enabled nullable reference types
        /// to be used for the test project in the workspace.
        /// </summary>
        /// <returns>An instance of <see cref="CSharpCompilationOptions"/> describing the project compilation options.</returns>
        protected override CSharpCompilationOptions GetCompilationOptions()
        {
            return new CSharpCompilationOptions(
                OutputKind.DynamicallyLinkedLibrary, nullableContextOptions: NullableContextOptions.Enable);
        }
    }
}
