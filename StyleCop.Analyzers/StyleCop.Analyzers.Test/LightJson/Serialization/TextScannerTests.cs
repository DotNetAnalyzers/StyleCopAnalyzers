// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.LightJson.Serialization
{
    using global::LightJson;
    using global::LightJson.Serialization;
    using Xunit;

    public class TextScannerTests
    {
        [Fact]
        public void TestUnexpectedLookahead()
        {
            JsonParseException ex;

            ex = Assert.ThrowsAny<JsonParseException>(() => JsonValue.Parse("trUe"));
            Assert.Equal(JsonParseException.ErrorType.InvalidOrUnexpectedCharacter, ex.Type);
            Assert.Equal(0, ex.Position.Line);
            Assert.Equal(2, ex.Position.Column);

            ex = Assert.ThrowsAny<JsonParseException>(() => JsonValue.Parse("tr"));
            Assert.Equal(JsonParseException.ErrorType.IncompleteMessage, ex.Type);
            Assert.Equal(0, ex.Position.Line);
            Assert.Equal(2, ex.Position.Column);
        }

        [Fact]
        public void TestIncompleteComment()
        {
            var ex = Assert.ThrowsAny<JsonParseException>(() => JsonValue.Parse("{ /1 }"));
            Assert.Equal(JsonParseException.ErrorType.InvalidOrUnexpectedCharacter, ex.Type);
            Assert.Contains("'1'", ex.Message);
            Assert.Equal(0, ex.Position.Line);
            Assert.Equal(3, ex.Position.Column);

            ex = Assert.ThrowsAny<JsonParseException>(() => JsonValue.Parse("{ // ignored text }"));
            Assert.Equal(JsonParseException.ErrorType.IncompleteMessage, ex.Type);

            ex = Assert.ThrowsAny<JsonParseException>(() => JsonValue.Parse("{ /* ignored text }"));
            Assert.Equal(JsonParseException.ErrorType.IncompleteMessage, ex.Type);
        }

        [Fact]
        public void TestBlockCommentTermination()
        {
            var obj = JsonValue.Parse("{ /* * / */ }");
            Assert.Equal(JsonValueType.Object, obj.Type);
            Assert.Equal(0, obj.AsJsonObject.Count);
        }
    }
}
