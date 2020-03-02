// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.Lightup
{
    using System;
    using Microsoft.CodeAnalysis;
    using StyleCop.Analyzers.Lightup;
    using Xunit;

    public class INamedTypeSymbolExtensionsTests
    {
        [Fact]
        public void TestNull()
        {
            INamedTypeSymbol symbol = null;
            Assert.Throws<NullReferenceException>(() => INamedTypeSymbolExtensions.IsSerializable(symbol));
            Assert.Throws<NullReferenceException>(() => INamedTypeSymbolExtensions.TupleElements(symbol));
            Assert.Throws<NullReferenceException>(() => INamedTypeSymbolExtensions.TupleUnderlyingType(symbol));
        }
    }
}
