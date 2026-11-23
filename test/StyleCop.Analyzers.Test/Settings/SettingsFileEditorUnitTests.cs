// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.Settings;

using StyleCop.Analyzers.Settings;
using Xunit;

public class SettingsFileEditorUnitTests
{
    [Theory]
    [InlineData("{}", "\"settings\"", "\"compatibility\"", "true")]
    [InlineData("{ \"settings\": {} }", "\"compatibility\"", "\"multiTargetsLegacyFrameworks\": true", null)]
    [InlineData("{ \"settings\": { \"compatibility\": { \"multiTargetsLegacyFrameworks\": false } } }", "\"multiTargetsLegacyFrameworks\": true", null, null)]
    [InlineData("{ // c\n \"settings\": { \"documentationRules\": { \"a\": 1, }, }, }", "\"compatibility\"", "\"a\": 1", "true")]
    public void TestSetJsonTrue(string json, string expected1, string? expected2, string? expected3)
    {
        var result = SettingsFileEditor.SetJsonTrue(json, "compatibility", "multiTargetsLegacyFrameworks");

        Assert.NotNull(result);
        Assert.Contains(expected1, result);
        if (expected2 != null)
        {
            Assert.Contains(expected2, result);
        }

        if (expected3 != null)
        {
            Assert.Contains(expected3, result);
        }
    }

    [Fact]
    public void TestSetJsonTrueMalformed()
    {
        Assert.Null(SettingsFileEditor.SetJsonTrue("{ \"settings\": ", "compatibility", "multiTargetsLegacyFrameworks"));
    }

    [Fact]
    public void TestSetEditorConfigReplacesExistingKey()
    {
        const string Key = "stylecop.compatibility.multiTargetsLegacyFrameworks";
        var result = SettingsFileEditor.SetEditorConfigTrue("[*.cs]\r\n" + Key + " = false\r\nother = 1\r\n", Key, isGlobalConfig: false);

        Assert.Equal("[*.cs]\r\n" + Key + " = true\r\nother = 1\r\n", result);
    }

    [Fact]
    public void TestSetEditorConfigInsertsIntoCSharpSection()
    {
        const string Key = "stylecop.compatibility.multiTargetsLegacyFrameworks";
        var result = SettingsFileEditor.SetEditorConfigTrue("root = true\n\n[*.cs]\nindent_size = 4\n", Key, isGlobalConfig: false);

        Assert.Contains("[*.cs]", result);
        Assert.Single(System.Text.RegularExpressions.Regex.Matches(result, "multiTargetsLegacyFrameworks = true"));
        Assert.True(result.IndexOf(Key) > result.IndexOf("[*.cs]"));
    }

    [Fact]
    public void TestSetEditorConfigAppendsSectionAndGlobalConfigInsertsBeforeSections()
    {
        const string Key = "stylecop.compatibility.multiTargetsLegacyFrameworks";
        var appended = SettingsFileEditor.SetEditorConfigTrue("root = true\n", Key, isGlobalConfig: false);
        Assert.True(appended.IndexOf("[*.cs]") < appended.IndexOf(Key));

        var global = SettingsFileEditor.SetEditorConfigTrue("is_global = true\n\n[/a.cs]\nx = 1\n", Key, isGlobalConfig: true);
        Assert.True(global.IndexOf(Key) < global.IndexOf("[/a.cs]"));
    }
}
