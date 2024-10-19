﻿// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

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
