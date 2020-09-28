// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.LightJson.Serialization
{
    using System;
    using global::LightJson;
    using global::LightJson.Serialization;
    using Xunit;

    public class JsonWriterTests
    {
        [Fact]
        public void TestConstructor()
        {
            var writer = new JsonWriter();
            var writerNotPretty = new JsonWriter(pretty: false);
            var writerPretty = new JsonWriter(pretty: true);

            Assert.True(string.IsNullOrEmpty(writer.IndentString));
            Assert.True(string.IsNullOrEmpty(writer.SpacingString));
            Assert.True(string.IsNullOrEmpty(writer.NewLineString));
            Assert.False(writer.SortObjects);

            Assert.Equal(writer.IndentString, writerNotPretty.IndentString);
            Assert.Equal(writer.SpacingString, writerNotPretty.SpacingString);
            Assert.Equal(writer.NewLineString, writerNotPretty.NewLineString);
            Assert.Equal(writer.SortObjects, writerNotPretty.SortObjects);

            Assert.Equal("\t", writerPretty.IndentString);
            Assert.Equal(" ", writerPretty.SpacingString);
            Assert.Equal("\n", writerPretty.NewLineString);
            Assert.False(writerPretty.SortObjects);
        }

        [Fact]
        public void TestSimpleSerialization()
        {
            var writer = new JsonWriter();
            Assert.Equal("null", writer.Serialize(JsonValue.Null));
            Assert.Equal("true", writer.Serialize(true));
            Assert.Equal("1.5", writer.Serialize(1.5));
            Assert.Equal("1", writer.Serialize(1));
            Assert.Equal("\"text\"", writer.Serialize("text"));
            Assert.Equal("{}", writer.Serialize(new JsonObject()));
            Assert.Equal("[]", writer.Serialize(new JsonArray()));
        }

        [Fact]
        public void TestCircularReference()
        {
            var obj = new JsonObject();
            obj["x"] = obj;

            var writer = new JsonWriter();
            var ex = Assert.ThrowsAny<JsonSerializationException>(() => writer.Serialize(obj));
            Assert.Equal(JsonSerializationException.ErrorType.CircularReference, ex.Type);
        }

        [Fact]
        public void TestEscapeSequences()
        {
            var writer = new JsonWriter();
            Assert.Equal("\"\\\"\"", writer.Serialize("\""));
            Assert.Equal("\"\\\\\"", writer.Serialize("\\"));
            Assert.Equal("\"\\/\"", writer.Serialize("/"));
            Assert.Equal("\"\\b\"", writer.Serialize("\b"));
            Assert.Equal("\"\\f\"", writer.Serialize("\f"));
            Assert.Equal("\"\\n\"", writer.Serialize("\n"));
            Assert.Equal("\"\\r\"", writer.Serialize("\r"));
            Assert.Equal("\"\\t\"", writer.Serialize("\t"));

            // The result is not converted to an escape sequence
            Assert.Equal("\"\u0123\u4567\u89AB\uCDEF\"", writer.Serialize("\u0123\u4567\u89AB\uCDEF"));
        }

        [Fact]
        public void TestWriteObject()
        {
            var obj = new JsonObject
            {
                ["a"] = "valueA",
                ["b"] = new JsonObject
                {
                    ["x"] = 0,
                    ["y"] = JsonValue.Null,
                    ["z"] = new JsonObject(),
                },
                ["c"] = 3,
            };

            var writer = new JsonWriter { SortObjects = true };
            Assert.Equal("{\"a\":\"valueA\",\"b\":{\"x\":0,\"y\":null,\"z\":{}},\"c\":3}", writer.Serialize(obj));
        }

        [Theory]
        [InlineData("\t", " ", "\n")]
        [InlineData("  ", "", "\r\n")]
        public void TestWriteObjectPretty(string indent, string space, string newline)
        {
            var obj = new JsonObject
            {
                ["a"] = "valueA",
                ["b"] = new JsonObject
                {
                    ["x"] = 0,
                    ["y"] = JsonValue.Null,
                    ["z"] = new JsonObject(),
                },
                ["c"] = 3,
            };

            var writer = new JsonWriter
            {
                IndentString = indent,
                SpacingString = space,
                NewLineString = newline,
                SortObjects = true,
            };

            var expected =
                $"{{{newline}"
                + $"{indent}\"a\":{space}\"valueA\",{newline}"
                + $"{indent}\"b\":{space}{{{newline}"
                + $"{indent}{indent}\"x\":{space}0,{newline}"
                + $"{indent}{indent}\"y\":{space}null,{newline}"
                + $"{indent}{indent}\"z\":{space}{{{newline}"
                + $"{indent}{indent}}}{newline}"
                + $"{indent}}},{newline}"
                + $"{indent}\"c\":{space}3{newline}"
                + $"}}";
            Assert.Equal(expected, writer.Serialize(obj));
        }

        [Fact]
        public void TestWriteArray()
        {
            var obj = new JsonArray
            {
                "valueA",
                new JsonArray
                {
                    0,
                    JsonValue.Null,
                    new JsonArray(),
                },
                3,
            };

            var writer = new JsonWriter { SortObjects = true };
            Assert.Equal("[\"valueA\",[0,null,[]],3]", writer.Serialize(obj));
        }

        [Theory]
        [InlineData("\t", "\n")]
        [InlineData("  ", "\r\n")]
        public void TestWriteArrayPretty(string indent, string newline)
        {
            var obj = new JsonArray
            {
                "valueA",
                new JsonArray
                {
                    0,
                    JsonValue.Null,
                    new JsonArray(),
                },
                3,
            };

            var writer = new JsonWriter
            {
                IndentString = indent,
                NewLineString = newline,
                SortObjects = true,
            };

            var expected =
                $"[{newline}"
                + $"{indent}\"valueA\",{newline}"
                + $"{indent}[{newline}"
                + $"{indent}{indent}0,{newline}"
                + $"{indent}{indent}null,{newline}"
                + $"{indent}{indent}[{newline}"
                + $"{indent}{indent}]{newline}"
                + $"{indent}],{newline}"
                + $"{indent}3{newline}"
                + $"]";
            Assert.Equal(expected, writer.Serialize(obj));
        }

        [Fact]
        public void TestDispose()
        {
            var writer = new JsonWriter();
            writer.Dispose();
            writer.Dispose(); // Disposing multiple times is allowed

            writer = new JsonWriter();
            writer.Serialize(1);
            writer.Dispose();
            writer.Dispose();
        }

        [Fact]
        public void TestUnsortedEnumerator()
        {
            // Use an object with only one possible ordering for this test
            var obj = new JsonObject { ["x"] = 3 };
            var writer = new JsonWriter { SortObjects = false };
            Assert.Equal("{\"x\":3}", writer.Serialize(obj));
        }
    }
}
