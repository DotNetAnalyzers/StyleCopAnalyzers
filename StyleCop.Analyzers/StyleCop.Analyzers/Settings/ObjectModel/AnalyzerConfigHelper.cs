// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.Settings.ObjectModel
{
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;
    using StyleCop.Analyzers.Lightup;

    internal static class AnalyzerConfigHelper
    {
        internal static bool? TryGetBooleanValue(AnalyzerConfigOptionsWrapper analyzerConfigOptions, string key)
        {
            if (analyzerConfigOptions.TryGetValue(key, out var value)
                && value != "unset"
                && bool.TryParse(value, out var boolValue))
            {
                return boolValue;
            }

            return null;
        }

        internal static int? TryGetInt32Value(AnalyzerConfigOptionsWrapper analyzerConfigOptions, string key)
        {
            if (analyzerConfigOptions.TryGetValue(key, out var value)
                && value != "unset"
                && int.TryParse(value, out var intValue))
            {
                return intValue;
            }

            return null;
        }

        internal static string TryGetStringValue(AnalyzerConfigOptionsWrapper analyzerConfigOptions, string key, bool allowExplicitUnset = true)
        {
            if (analyzerConfigOptions.TryGetValue(key, out var value))
            {
                if (allowExplicitUnset && value == "unset")
                {
                    return null;
                }

                return value;
            }

            return null;
        }

        internal static string TryGetMultiLineStringValue(AnalyzerConfigOptionsWrapper analyzerConfigOptions, string key, bool allowExplicitUnset = true)
        {
            var orgValue = TryGetStringValue(analyzerConfigOptions, key, allowExplicitUnset);
            return orgValue?.Replace("\\r", "\r").Replace("\\n", "\n");
        }

        internal static KeyValuePair<string, string>? TryGetStringValueAndNotification(AnalyzerConfigOptionsWrapper analyzerConfigOptions, string key, bool allowExplicitUnset = true)
        {
            if (analyzerConfigOptions.TryGetValue(key, out var value))
            {
                if (allowExplicitUnset && value == "unset")
                {
                    return null;
                }

                var colonIndex = value.IndexOf(':');
                if (colonIndex >= 0)
                {
                    return new KeyValuePair<string, string>(value.Substring(0, colonIndex), value.Substring(colonIndex + 1));
                }

                return new KeyValuePair<string, string>(value, string.Empty);
            }

            return null;
        }

        internal static T? TryGetEnumValue<T>(AnalyzerConfigOptionsWrapper analyzerConfigOptions, string key)
            where T : struct, System.Enum
        {
            var value = TryGetStringValue(analyzerConfigOptions, key)?.Trim();
            return ParseEnum<T>(value);
        }

        internal static ImmutableArray<T>? TryGetEnumListValue<T>(AnalyzerConfigOptionsWrapper analyzerConfigOptions, string key)
            where T : struct, System.Enum
        {
            var list = TryGetStringListValue(analyzerConfigOptions, key);
            if (list is null)
            {
                return null;
            }

            var result = ImmutableArray.CreateBuilder<T>();
            foreach (var item in list.Value)
            {
                if (ParseEnum<T>(item) is { } parsed)
                {
                    result.Add(parsed);
                }
            }

            return result.ToImmutable();
        }

        internal static ImmutableArray<string>? TryGetStringListValue(AnalyzerConfigOptionsWrapper analyzerConfigOptions, string key, bool allowExplicitUnset = true)
        {
            if (analyzerConfigOptions.TryGetValue(key, out var value))
            {
                if (allowExplicitUnset && value == "unset")
                {
                    return null;
                }

                return value.Split(',').Select(static x => x.Trim()).ToImmutableArray();
            }

            return null;
        }

        private static T? ParseEnum<T>(string value)
            where T : struct, System.Enum
        {
            if (!string.IsNullOrEmpty(value)
                && !char.IsDigit(value[0])
                && System.Enum.TryParse<T>(value, ignoreCase: true, out var parsed)
                && System.Enum.IsDefined(typeof(T), parsed))
            {
                return parsed;
            }

            return null;
        }
    }
}
