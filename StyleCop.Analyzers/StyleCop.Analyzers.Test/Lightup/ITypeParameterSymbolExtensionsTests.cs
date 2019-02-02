// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.Lightup
{
    using System;
    using Microsoft.CodeAnalysis;
    using StyleCop.Analyzers.Lightup;
    using Xunit;

    public class ITypeParameterSymbolExtensionsTests
    {
        [Fact]
        public void TestNull()
        {
            ITypeParameterSymbol symbol = null;
            Assert.Throws<NullReferenceException>(() => ITypeParameterSymbolExtensions.HasUnmanagedTypeConstraint(symbol));
        }
    }
}
