// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Settings;

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

/// <summary>
/// Edits existing <strong>stylecop.json</strong>, <strong>.editorconfig</strong> and <strong>.globalconfig</strong>
/// text to set a boolean setting while leaving comments and formatting intact.
/// </summary>
internal static class SettingsFileEditor
{
    private const string IndentUnit = "  ";

    /// <summary>
    /// Sets <c>settings.&lt;section&gt;.&lt;name&gt;</c> to <see langword="true"/> in a <strong>stylecop.json</strong> file.
    /// </summary>
    /// <param name="json">The current file content.</param>
    /// <param name="section">The name of the settings section.</param>
    /// <param name="name">The name of the setting.</param>
    /// <returns>The updated content, or <see langword="null"/> if the file could not be understood.</returns>
    public static string? SetJsonTrue(string json, string section, string name)
    {
        var rootStart = SkipTrivia(json, 0);
        if (rootStart >= json.Length || json[rootStart] != '{')
        {
            return null;
        }

        var root = ParseObject(json, rootStart);
        if (root == null)
        {
            return null;
        }

        var newLine = json.Contains("\r\n") ? "\r\n" : "\n";
        string BuildProperty(string indent) => indent + Quote(name) + ": true";
        string BuildSection(string indent) => BuildObject(indent, section, BuildProperty(indent + IndentUnit), newLine);

        var settingsMember = root.Find("settings");
        if (settingsMember == null)
        {
            return InsertMember(json, root, newLine, indent => BuildObject(indent, "settings", BuildSection(indent + IndentUnit), newLine));
        }

        var settings = ParseMemberObject(json, settingsMember);
        if (settings == null)
        {
            return null;
        }

        var sectionMember = settings.Find(section);
        if (sectionMember == null)
        {
            return InsertMember(json, settings, newLine, BuildSection);
        }

        var sectionObject = ParseMemberObject(json, sectionMember);
        if (sectionObject == null)
        {
            return null;
        }

        var nameMember = sectionObject.Find(name);
        if (nameMember == null)
        {
            return InsertMember(json, sectionObject, newLine, BuildProperty);
        }

        return json.Substring(0, nameMember.ValueStart) + "true" + json.Substring(nameMember.ValueEnd);
    }

    /// <summary>
    /// Sets an <strong>.editorconfig</strong> or <strong>.globalconfig</strong> key to <see langword="true"/>.
    /// </summary>
    /// <param name="text">The current file content.</param>
    /// <param name="key">The key to set.</param>
    /// <param name="isGlobalConfig"><see langword="true"/> for a <strong>.globalconfig</strong> file, whose
    /// properties precede any section; <see langword="false"/> for an <strong>.editorconfig</strong> file, in which
    /// the key is added to a section that applies to C# files.</param>
    /// <returns>The updated content.</returns>
    public static string SetEditorConfigTrue(string text, string key, bool isGlobalConfig)
    {
        var newLine = text.Contains("\r\n") ? "\r\n" : "\n";
        var lines = new List<string>(text.Replace("\r\n", "\n").Split('\n'));
        if (lines.Count > 0 && lines[lines.Count - 1].Length == 0)
        {
            lines.RemoveAt(lines.Count - 1);
        }

        var keyLine = key + " = true";
        var existing = new Regex("^\\s*" + Regex.Escape(key) + "\\s*[=:]", RegexOptions.IgnoreCase);
        for (var i = 0; i < lines.Count; i++)
        {
            if (existing.IsMatch(lines[i]))
            {
                lines[i] = LeadingWhitespace(lines[i]) + keyLine;
                return string.Join(newLine, lines) + newLine;
            }
        }

        var headers = new List<int>();
        for (var i = 0; i < lines.Count; i++)
        {
            if (IsSectionHeader(lines[i]))
            {
                headers.Add(i);
            }
        }

        int sectionStart;
        int sectionEnd;
        if (isGlobalConfig)
        {
            sectionStart = -1;
            sectionEnd = headers.Count > 0 ? headers[0] : lines.Count;
        }
        else
        {
            sectionStart = -1;
            sectionEnd = -1;
            for (var h = 0; h < headers.Count; h++)
            {
                if (AppliesToCSharp(lines[headers[h]]))
                {
                    sectionStart = headers[h];
                    sectionEnd = h + 1 < headers.Count ? headers[h + 1] : lines.Count;
                    break;
                }
            }

            if (sectionStart < 0)
            {
                while (lines.Count > 0 && lines[lines.Count - 1].Trim().Length == 0)
                {
                    lines.RemoveAt(lines.Count - 1);
                }

                if (lines.Count > 0)
                {
                    lines.Add(string.Empty);
                }

                lines.Add("[*.cs]");
                lines.Add(keyLine);
                return string.Join(newLine, lines) + newLine;
            }
        }

        var insertAt = sectionEnd;
        while (insertAt - 1 > sectionStart && lines[insertAt - 1].Trim().Length == 0)
        {
            insertAt--;
        }

        lines.Insert(insertAt, keyLine);
        return string.Join(newLine, lines) + newLine;
    }

    private static bool IsSectionHeader(string line)
    {
        var trimmed = line.Trim();
        return trimmed.Length >= 2 && trimmed[0] == '[' && trimmed[trimmed.Length - 1] == ']';
    }

    private static bool AppliesToCSharp(string header)
    {
        var glob = header.Trim();
        glob = glob.Substring(1, glob.Length - 2);
        return glob == "*"
            || glob.Contains("*.cs")
            || Regex.IsMatch(glob, "\\*\\.\\{[^}]*\\bcs\\b[^}]*\\}");
    }

    private static string LeadingWhitespace(string line)
    {
        var length = 0;
        while (length < line.Length && (line[length] == ' ' || line[length] == '\t'))
        {
            length++;
        }

        return line.Substring(0, length);
    }

    private static string Quote(string value) => "\"" + value + "\"";

    private static string BuildObject(string indent, string name, string innerMembers, string newLine)
        => indent + Quote(name) + ": {" + newLine + innerMembers + newLine + indent + "}";

    private static string InsertMember(string text, JsonObjectInfo target, string newLine, Func<string, string> buildMember)
    {
        if (target.Members.Count > 0)
        {
            var indent = LeadingWhitespace(LineOf(text, target.Members[0].KeyStart));
            if (target.CommaAfterLastMember >= 0)
            {
                return text.Insert(target.CommaAfterLastMember + 1, newLine + buildMember(indent) + ",");
            }

            return text.Insert(target.Members[target.Members.Count - 1].ValueEnd, "," + newLine + buildMember(indent));
        }

        var openIndent = LeadingWhitespace(LineOf(text, target.OpenBrace));
        var memberText = buildMember(openIndent + IndentUnit);
        var inner = text.Substring(target.OpenBrace + 1, target.CloseBrace - target.OpenBrace - 1);
        if (inner.Trim().Length == 0)
        {
            return text.Substring(0, target.OpenBrace + 1) + newLine + memberText + newLine + openIndent + text.Substring(target.CloseBrace);
        }

        return text.Insert(target.OpenBrace + 1, newLine + memberText);
    }

    private static string LineOf(string text, int position)
    {
        var start = text.LastIndexOf('\n', Math.Max(position - 1, 0)) + 1;
        if (position == 0)
        {
            start = 0;
        }

        var end = text.IndexOf('\n', position);
        return end < 0 ? text.Substring(start) : text.Substring(start, end - start);
    }

    private static JsonObjectInfo? ParseMemberObject(string text, JsonMemberInfo member)
        => member.ValueStart < text.Length && text[member.ValueStart] == '{' ? ParseObject(text, member.ValueStart) : null;

    private static JsonObjectInfo? ParseObject(string text, int openBrace)
    {
        var info = new JsonObjectInfo(openBrace);
        var i = openBrace + 1;
        while (true)
        {
            i = SkipTrivia(text, i);
            if (i >= text.Length)
            {
                return null;
            }

            switch (text[i])
            {
            case '}':
                info.CloseBrace = i;
                return info;

            case ',':
                info.CommaAfterLastMember = i;
                i++;
                break;

            case '"':
                var keyStart = i;
                var keyEnd = SkipString(text, i);
                if (keyEnd < 0)
                {
                    return null;
                }

                var key = text.Substring(keyStart + 1, keyEnd - keyStart - 2);
                i = SkipTrivia(text, keyEnd);
                if (i >= text.Length || text[i] != ':')
                {
                    return null;
                }

                i = SkipTrivia(text, i + 1);
                var valueStart = i;
                var valueEnd = SkipValue(text, i);
                if (valueEnd < 0)
                {
                    return null;
                }

                info.Members.Add(new JsonMemberInfo(key, keyStart, valueStart, valueEnd));
                info.CommaAfterLastMember = -1;
                i = valueEnd;
                break;

            default:
                return null;
            }
        }
    }

    private static int SkipTrivia(string text, int i)
    {
        while (i < text.Length)
        {
            if (char.IsWhiteSpace(text[i]))
            {
                i++;
            }
            else if (text[i] == '/' && i + 1 < text.Length && text[i + 1] == '/')
            {
                var end = text.IndexOf('\n', i);
                i = end < 0 ? text.Length : end + 1;
            }
            else if (text[i] == '/' && i + 1 < text.Length && text[i + 1] == '*')
            {
                var end = text.IndexOf("*/", i + 2, StringComparison.Ordinal);
                i = end < 0 ? text.Length : end + 2;
            }
            else
            {
                break;
            }
        }

        return i;
    }

    private static int SkipString(string text, int i)
    {
        i++;
        while (i < text.Length)
        {
            if (text[i] == '\\')
            {
                i += 2;
            }
            else if (text[i] == '"')
            {
                return i + 1;
            }
            else
            {
                i++;
            }
        }

        return -1;
    }

    private static int SkipValue(string text, int i)
    {
        if (i >= text.Length)
        {
            return -1;
        }

        if (text[i] == '"')
        {
            return SkipString(text, i);
        }

        if (text[i] == '{' || text[i] == '[')
        {
            var depth = 0;
            while (i < text.Length)
            {
                var c = text[i];
                if (c == '"')
                {
                    i = SkipString(text, i);
                    if (i < 0)
                    {
                        return -1;
                    }

                    continue;
                }

                if (c == '/' && i + 1 < text.Length && (text[i + 1] == '/' || text[i + 1] == '*'))
                {
                    i = SkipTrivia(text, i);
                    continue;
                }

                if (c == '{' || c == '[')
                {
                    depth++;
                }
                else if (c == '}' || c == ']')
                {
                    depth--;
                    if (depth == 0)
                    {
                        return i + 1;
                    }
                }

                i++;
            }

            return -1;
        }

        while (i < text.Length && text[i] != ',' && text[i] != '}' && text[i] != ']' && text[i] != '/' && !char.IsWhiteSpace(text[i]))
        {
            i++;
        }

        return i;
    }

    private sealed class JsonMemberInfo
    {
        public JsonMemberInfo(string name, int keyStart, int valueStart, int valueEnd)
        {
            this.Name = name;
            this.KeyStart = keyStart;
            this.ValueStart = valueStart;
            this.ValueEnd = valueEnd;
        }

        public string Name { get; }

        public int KeyStart { get; }

        public int ValueStart { get; }

        public int ValueEnd { get; }
    }

    private sealed class JsonObjectInfo
    {
        public JsonObjectInfo(int openBrace)
        {
            this.OpenBrace = openBrace;
        }

        public int OpenBrace { get; }

        public int CloseBrace { get; set; }

        public int CommaAfterLastMember { get; set; } = -1;

        public List<JsonMemberInfo> Members { get; } = new List<JsonMemberInfo>();

        public JsonMemberInfo? Find(string name)
        {
            foreach (var member in this.Members)
            {
                if (member.Name == name)
                {
                    return member;
                }
            }

            return null;
        }
    }
}
