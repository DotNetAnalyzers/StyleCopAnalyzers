// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.Helpers
{
    using System;
    using global::LightJson;

    internal class JsonTestHelper
    {
        public static JsonObject MergeJsonObjects(JsonObject priority, JsonObject fallback)
        {
            foreach (var pair in priority)
            {
                if (pair.Value.IsJsonObject)
                {
                    switch (fallback[pair.Key].Type)
                    {
                    case JsonValueType.Null:
                        fallback[pair.Key] = pair.Value;
                        break;

                    case JsonValueType.Object:
                        fallback[pair.Key] = MergeJsonObjects(pair.Value.AsJsonObject, fallback[pair.Key].AsJsonObject);
                        break;

                    default:
                        throw new InvalidOperationException($"Cannot merge objects of type '{pair.Value.Type}' and '{fallback[pair.Key].Type}'.");
                    }
                }
                else
                {
                    fallback[pair.Key] = pair.Value;
                }
            }

            return fallback;
        }
    }
}
