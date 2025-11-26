// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.Lightup
{
    using System;
    using StyleCop.Analyzers.Lightup;
    using Xunit;

    public class IImportScopeWrapperUnitTests
    {
        [Fact]
        public void TestNull()
        {
            object? obj = null;
            Assert.False(IImportScopeWrapper.IsInstance(obj!));
            var wrapper = IImportScopeWrapper.FromObject(obj!);
            Assert.Throws<NullReferenceException>(() => wrapper.Aliases);
        }

        [Fact]
        public void TestIncompatibleInstance()
        {
            var obj = new object();
            Assert.False(IImportScopeWrapper.IsInstance(obj));
            Assert.Throws<InvalidCastException>(() => IImportScopeWrapper.FromObject(obj));
        }
    }
}
