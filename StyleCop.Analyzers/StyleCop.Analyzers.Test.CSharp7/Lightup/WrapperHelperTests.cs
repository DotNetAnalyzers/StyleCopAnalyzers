// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp7.Lightup
{
    using System.Linq;
    using System.Reflection;
    using StyleCop.Analyzers.Lightup;
    using Xunit;

    public class WrapperHelperTests
    {
        [Fact]
        public void VerifyThatAllWrapperClassesArePresent()
        {
            var wrapperTypes = typeof(ISyntaxWrapper<>).Assembly.GetTypes()
                .Where(t => t.GetTypeInfo().ImplementedInterfaces.Any(i => i.IsGenericType && (i.GetGenericTypeDefinition() == typeof(ISyntaxWrapper<>))));

            foreach (var wrapperType in wrapperTypes)
            {
                var wrappedType = WrapperHelper.GetWrappedType(wrapperType);
                Assert.NotNull(wrapperType);
            }
        }
    }
}
