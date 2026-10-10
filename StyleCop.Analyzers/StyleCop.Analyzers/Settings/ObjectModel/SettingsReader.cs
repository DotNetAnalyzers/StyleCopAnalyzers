// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.Settings.ObjectModel;

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using LightJson;
using StyleCop.Analyzers.Lightup;

/// <summary>
/// Reads settings from every supported source in precedence order: <strong>stylecop.json</strong>, then the
/// <strong>.editorconfig</strong>/<strong>.globalconfig</strong> keys, in the order given. All settings must be
/// read through this type so that every setting is available from every source.
/// </summary>
internal readonly struct SettingsReader
{
    private readonly JsonObject json;
    private readonly AnalyzerConfigOptionsWrapper options;

    public SettingsReader(JsonObject json, AnalyzerConfigOptionsWrapper options)
    {
        this.json = json;
        this.options = options;
    }

    public bool? GetBoolean(string jsonKey, string editorConfigKey)
    {
        if (this.TryGetJson(jsonKey, out var kvp))
        {
            return kvp.ToBooleanValue();
        }

        return AnalyzerConfigHelper.TryGetBooleanValue(this.options, editorConfigKey);
    }

    public int? GetInt32(string jsonKey, string editorConfigKey)
    {
        if (this.TryGetJson(jsonKey, out var kvp))
        {
            return kvp.ToInt32Value();
        }

        return AnalyzerConfigHelper.TryGetInt32Value(this.options, editorConfigKey);
    }

    public string GetString(string jsonKey, string editorConfigKey)
    {
        if (this.TryGetJson(jsonKey, out var kvp))
        {
            return kvp.ToStringValue();
        }

        return AnalyzerConfigHelper.TryGetStringValue(this.options, editorConfigKey);
    }

    public string GetMultiLineString(string jsonKey, string editorConfigKey, string fallbackEditorConfigKey = null)
    {
        if (this.TryGetJson(jsonKey, out var kvp))
        {
            return kvp.ToStringValue();
        }

        return AnalyzerConfigHelper.TryGetMultiLineStringValue(this.options, editorConfigKey)
            ?? (fallbackEditorConfigKey is null ? null : AnalyzerConfigHelper.TryGetMultiLineStringValue(this.options, fallbackEditorConfigKey));
    }

    public T? GetEnum<T>(string jsonKey, string editorConfigKey)
        where T : struct, Enum
    {
        if (this.TryGetJson(jsonKey, out var kvp))
        {
            return kvp.ToEnumValue<T>();
        }

        return AnalyzerConfigHelper.TryGetEnumValue<T>(this.options, editorConfigKey);
    }

    public T? GetMapped<T>(string jsonKey, Func<KeyValuePair<string, JsonValue>, T> fromJson, string editorConfigKey, Func<string, T?> fromEditorConfig)
        where T : struct
    {
        if (this.TryGetJson(jsonKey, out var kvp))
        {
            return fromJson(kvp);
        }

        var value = AnalyzerConfigHelper.TryGetStringValue(this.options, editorConfigKey);
        return value is null ? null : fromEditorConfig(value);
    }

    public ImmutableArray<T>? GetEnumList<T>(string jsonKey, string editorConfigKey)
        where T : struct, Enum
    {
        if (this.TryGetJson(jsonKey, out var kvp))
        {
            kvp.AssertIsArray();
            var builder = ImmutableArray.CreateBuilder<T>();
            foreach (var value in kvp.Value.AsJsonArray)
            {
                builder.Add(value.ToEnumValue<T>(jsonKey));
            }

            return builder.ToImmutable();
        }

        return AnalyzerConfigHelper.TryGetEnumListValue<T>(this.options, editorConfigKey);
    }

    public ImmutableArray<string>? GetStringList(string jsonKey, string editorConfigKey, Func<string, bool> filter = null)
    {
        ImmutableArray<string> result;
        if (this.TryGetJson(jsonKey, out var kvp))
        {
            kvp.AssertIsArray();
            var builder = ImmutableArray.CreateBuilder<string>();
            foreach (var value in kvp.Value.AsJsonArray)
            {
                builder.Add(value.ToStringValue(jsonKey));
            }

            result = builder.ToImmutable();
        }
        else if (AnalyzerConfigHelper.TryGetStringListValue(this.options, editorConfigKey) is { } list)
        {
            result = list;
        }
        else
        {
            return null;
        }

        if (filter is null)
        {
            return result;
        }

        var filtered = ImmutableArray.CreateBuilder<string>();
        foreach (var item in result)
        {
            if (filter(item))
            {
                filtered.Add(item);
            }
        }

        return filtered.ToImmutable();
    }

    /// <summary>
    /// Reads a set of named values: a JSON object, or <strong>.editorconfig</strong> keys of the form
    /// <c>&lt;prefix&gt;&lt;name&gt;</c>.
    /// </summary>
    /// <param name="jsonKey">The key in the <strong>stylecop.json</strong> section.</param>
    /// <param name="editorConfigPrefix">The <strong>.editorconfig</strong> key prefix, including the trailing dot.</param>
    /// <param name="isValidName">Determines whether a name is accepted; other names are skipped.</param>
    /// <returns>The named values, or <see langword="null"/> if no source provides any.</returns>
    public ImmutableDictionary<string, string> GetStringMap(string jsonKey, string editorConfigPrefix, Func<string, bool> isValidName)
    {
        ImmutableDictionary<string, string>.Builder map = null;
        if (this.TryGetJson(jsonKey, out var kvp))
        {
            kvp.AssertIsObject();
            map = ImmutableDictionary.CreateBuilder<string, string>();
            foreach (var child in kvp.Value.AsJsonObject)
            {
                if (isValidName(child.Key))
                {
                    map.Add(child.Key, child.ToStringValue());
                }
            }

            return map.ToImmutable();
        }

        foreach (var key in this.options.GetKeys())
        {
            if (!key.StartsWith(editorConfigPrefix, StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            var name = key.Substring(editorConfigPrefix.Length);
            var value = AnalyzerConfigHelper.TryGetMultiLineStringValue(this.options, key);
            if (value is not null && isValidName(name))
            {
                map ??= ImmutableDictionary.CreateBuilder<string, string>();
                map[name] = value;
            }
        }

        return map?.ToImmutable();
    }

    private bool TryGetJson(string jsonKey, out KeyValuePair<string, JsonValue> kvp)
    {
        if (this.json is not null && this.json.ContainsKey(jsonKey))
        {
            kvp = new KeyValuePair<string, JsonValue>(jsonKey, this.json[jsonKey]);
            return true;
        }

        kvp = default;
        return false;
    }
}
