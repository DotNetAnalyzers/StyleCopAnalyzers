// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp11.Lightup
{
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Moq;
    using StyleCop.Analyzers.Lightup;
    using StyleCop.Analyzers.Test.CSharp10.Lightup;
    using Xunit;

    public partial class IImportScopeWrapperCSharp11UnitTests : IImportScopeWrapperCSharp10UnitTests
    {
        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        public void TestCompatibleInstance(int numberOfAliasSymbols)
        {
            var obj = CreateImportScope(numberOfAliasSymbols);
            Assert.True(IImportScopeWrapper.IsInstance(obj));
            var wrapper = IImportScopeWrapper.FromObject(obj);
            Assert.Equal(obj.Aliases, wrapper.Aliases);
        }

        private static IImportScope CreateImportScope(int numberOfAliasSymbols)
        {
            var aliasSymbolMocks = new List<IAliasSymbol>();
            for (var i = 0; i < numberOfAliasSymbols; i++)
            {
                aliasSymbolMocks.Add(Mock.Of<IAliasSymbol>());
            }

            var importScopeMock = new Mock<IImportScope>();
            importScopeMock.Setup(x => x.Aliases).Returns(aliasSymbolMocks.ToImmutableArray());
            return importScopeMock.Object;
        }
    }
}
