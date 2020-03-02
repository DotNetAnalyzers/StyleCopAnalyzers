// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.LightJson.Serialization
{
    using global::LightJson.Serialization;
    using Xunit;

    public class JsonParseExceptionTests
    {
        [Fact]
        public void TestDefaultConstructor()
        {
            var ex = new JsonParseException();
            Assert.Equal(JsonParseException.ErrorType.Unknown, ex.Type);
            Assert.False(string.IsNullOrEmpty(ex.Message));
        }
    }
}
