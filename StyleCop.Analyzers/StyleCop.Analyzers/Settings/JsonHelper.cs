// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers
{
    using System;
    using System.Collections.Generic;
    using LightJson;

    /// <summary>
    /// Class containing helper methods to work with LightJson more easily.
    /// </summary>
    internal static class JsonHelper
    {
        /// <summary>
        /// Converts a JSON value to a boolean.
        /// </summary>
        /// <param name="jsonValue">The key value pair identifying the JSON value.</param>
        /// <returns>The boolean value contained within the JSON value.</returns>
        internal static bool ToBooleanValue(this KeyValuePair<string, JsonValue> jsonValue)
        {
            if (!jsonValue.Value.IsBoolean)
            {
                throw new InvalidSettingsException($"{jsonValue.Key} must contain a boolean value");
            }

            return jsonValue.Value.AsBoolean;
        }

        /// <summary>
        /// Converts a JSON value to an integer.
        /// </summary>
        /// <param name="jsonValue">The key value pair identifying the JSON value.</param>
        /// <returns>The integer value contained within the JSON value.</returns>
        internal static int ToInt32Value(this KeyValuePair<string, JsonValue> jsonValue)
        {
            if (!jsonValue.Value.IsInteger)
            {
                throw new InvalidSettingsException($"{jsonValue.Key} must contain an integer value");
            }

            return jsonValue.Value.AsInteger;
        }

        /// <summary>
        /// Converts a JSON value to a string.
        /// </summary>
        /// <param name="jsonValue">The key value pair identifying the JSON value.</param>
        /// <returns>The string value contained within the JSON value.</returns>
        internal static string ToStringValue(this KeyValuePair<string, JsonValue> jsonValue)
        {
            if (!jsonValue.Value.IsString)
            {
                throw new InvalidSettingsException($"{jsonValue.Key} must contain a string value");
            }

            return jsonValue.Value.AsString;
        }

        /// <summary>
        /// Converts a JSON value to a string.
        /// </summary>
        /// <param name="jsonValue">The key value pair identifying the JSON value.</param>
        /// <param name="elementName">The element name to report in exceptions.</param>
        /// <returns>The string value contained within the JSON value.</returns>
        internal static string ToStringValue(this JsonValue jsonValue, string elementName)
        {
            if (!jsonValue.IsString)
            {
                throw new InvalidSettingsException($"{elementName} must contain a string value");
            }

            return jsonValue.AsString;
        }

        /// <summary>
        /// Converts a JSON value to an enum value.
        /// </summary>
        /// <typeparam name="TEnum">The type of enum to convert to.</typeparam>
        /// <param name="jsonValue">The key value pair identifying the JSON value.</param>
        /// <returns>The enum value contained within the JSON value.</returns>
        internal static TEnum ToEnumValue<TEnum>(this KeyValuePair<string, JsonValue> jsonValue)
            where TEnum : struct
        {
            if (!jsonValue.Value.IsString)
            {
                throw new InvalidSettingsException($"{jsonValue.Key} must contain an enum (string) value");
            }

            TEnum result;
            if (!Enum.TryParse(jsonValue.Value.AsString, true, out result))
            {
                throw new InvalidSettingsException($"{jsonValue.Key} cannot contain enum value '{jsonValue.Value.AsString}'");
            }

            return result;
        }

        /// <summary>
        /// Converts a JSON value to an enum value.
        /// </summary>
        /// <typeparam name="TEnum">The type of enum to convert to.</typeparam>
        /// <param name="jsonValue">The key value pair identifying the JSON value.</param>
        /// <param name="elementName">The element name to report in exceptions.</param>
        /// <returns>The enum value contained within the JSON value.</returns>
        internal static TEnum ToEnumValue<TEnum>(this JsonValue jsonValue, string elementName)
            where TEnum : struct
        {
            if (!jsonValue.IsString)
            {
                throw new InvalidSettingsException($"{elementName} must contain an enum (string) value");
            }

            TEnum result;
            if (!Enum.TryParse(jsonValue.AsString, true, out result))
            {
                throw new InvalidSettingsException($"{elementName} cannot contain enum value '{jsonValue.AsString}'");
            }

            return result;
        }

        /// <summary>
        /// Checks if the given JSON value is an array. Will throw an exception if it is not.
        /// </summary>
        /// <param name="jsonValue">The key value pair identifying the JSON value.</param>
        internal static void AssertIsArray(this KeyValuePair<string, JsonValue> jsonValue)
        {
            if (!jsonValue.Value.IsJsonArray)
            {
                throw new InvalidSettingsException($"{jsonValue.Key} must contain an array");
            }
        }

        /// <summary>
        /// Checks if the given JSON value is an object. Will throw an exception if it is not.
        /// </summary>
        /// <param name="jsonValue">The key value pair identifying the JSON value.</param>
        internal static void AssertIsObject(this KeyValuePair<string, JsonValue> jsonValue)
        {
            if (!jsonValue.Value.IsJsonObject)
            {
                throw new InvalidSettingsException($"{jsonValue.Key} must contain an object");
            }
        }
    }
}
