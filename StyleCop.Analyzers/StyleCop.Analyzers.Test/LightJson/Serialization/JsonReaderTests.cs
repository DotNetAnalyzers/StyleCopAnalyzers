// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.LightJson.Serialization
{
    using global::LightJson;
    using global::LightJson.Serialization;
    using Xunit;

    public class JsonReaderTests
    {
        [Fact]
        public void TestKeyMatchesPreviousValue()
        {
            var jsonObject = JsonValue.Parse("{ \"x\": \"value\", \"value\": \"value\" }");
            Assert.NotNull(jsonObject);
            Assert.Equal("value", jsonObject["x"].AsString);
            Assert.Equal("value", jsonObject["value"].AsString);
            Assert.Equal(jsonObject["x"], jsonObject["value"]);
        }

        [Fact]
        public void TestDuplicateKeys()
        {
            Assert.ThrowsAny<JsonParseException>(() => JsonValue.Parse("{ \"x\": \"value\", \"x\": \"value\" }"));
        }
    }
}
