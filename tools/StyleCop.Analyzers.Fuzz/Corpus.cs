// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Tools.Fuzz;

using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

internal static class Corpus
{
    private static readonly string[] Insertions =
    [
        " ", "\t", "\n", "\r\n", "/*fuzz*/", "//fuzz\n", "/// <summary>fuzz</summary>\n",
        "#if DEBUG\n", "#endif\n", "#region fuzz\n", "#endregion\n", "#nullable enable\n",
        "(", ")", "{", "}", "[", "]", ";", ",", "?", "!", "=>", "partial ", "static ", "async ", "\"\"\"",
    ];

    internal static List<Input> Load(string path)
    {
        if (File.Exists(path))
        {
            return [LoadFile(path)];
        }

        if (!Directory.Exists(path))
        {
            throw new FileNotFoundException("Corpus path was not found.", path);
        }

        var inputs = new List<Input>();
        LoadDirectory(path, inputs);
        return inputs;
    }

    internal static int Extract(string path, string output)
    {
        Directory.CreateDirectory(output);
        var hashes = new HashSet<string>(StringComparer.Ordinal);
        foreach (var file in Directory.GetFiles(path, "*.cs", SearchOption.AllDirectories).Order(StringComparer.Ordinal))
        {
            if (file.Split(Path.DirectorySeparatorChar).Any(part => part is "bin" or "obj"))
            {
                continue;
            }

            var root = CSharpSyntaxTree.ParseText(File.ReadAllText(file), new CSharpParseOptions(LanguageVersion.Preview)).GetRoot();
            foreach (var literal in root.DescendantNodes().OfType<LiteralExpressionSyntax>().Where(literal => literal.IsKind(SyntaxKind.StringLiteralExpression)))
            {
                var text = literal.Token.ValueText;

                // Complete, markup-free literals only. Assembled snippets need capture at the verifier boundary.
                if (text.Length < 30 || text.Contains("[|", StringComparison.Ordinal) || text.Contains("{|", StringComparison.Ordinal)
                    || !text.Contains('{') || !text.Contains('}')
                    || !CSharpSyntaxTree.ParseText(text).GetRoot().DescendantNodes().OfType<BaseTypeDeclarationSyntax>().Any())
                {
                    continue;
                }

                var hash = Convert.ToHexStringLower(SHA256.HashData(Encoding.UTF8.GetBytes(text)));
                if (hashes.Add(hash))
                {
                    File.WriteAllText(Path.Combine(output, hash + ".cs"), text);
                }
            }
        }

        return hashes.Count;
    }

    internal static Input Mutate(Input input, Random random)
    {
        var source = input.Source;
        var tokens = CSharpSyntaxTree.ParseText(source, new CSharpParseOptions(LanguageVersion.Preview)).GetRoot()
            .DescendantTokens().Where(token => token.Span.Length != 0).ToArray();
        if (tokens.Length == 0)
        {
            return input with { Source = source + Insertions[random.Next(Insertions.Length)] };
        }

        var token = tokens[random.Next(tokens.Length)];
        var insertion = Insertions[random.Next(Insertions.Length)];
        var mutated = random.Next(5) switch
        {
            0 => source.Remove(token.Span.Start, token.Span.Length),
            1 => source.Insert(token.Span.Start, insertion),
            2 => source.Insert(token.Span.End, insertion),
            3 => source[..random.Next(source.Length + 1)],
            _ => source.Insert(token.Span.Start, "/*fuzz*/"),
        };
        return input with { Source = mutated };
    }

    private static void LoadDirectory(string directory, List<Input> inputs)
    {
        // A saved finding is one input. Its generated .cs files are not additional seeds, but ordinary
        // seeds in the same tree keep their filenames and settings sidecars.
        if (TryLoadFindingBundle(directory, out var bundle))
        {
            inputs.Add(bundle);
        }

        foreach (var file in Directory.GetFiles(directory, "*.cs").Order(StringComparer.Ordinal))
        {
            if (!IsGeneratedFindingArtifact(directory, file))
            {
                inputs.Add(LoadSeed(file));
            }
        }

        foreach (var child in Directory.GetDirectories(directory).Order(StringComparer.Ordinal))
        {
            LoadDirectory(child, inputs);
        }
    }

    private static Input LoadFile(string path)
    {
        if (!string.Equals(Path.GetExtension(path), ".json", StringComparison.OrdinalIgnoreCase))
        {
            return LoadSeed(path);
        }

        if (!TryReadSavedInput(path, out var saved))
        {
            throw new InvalidOperationException("Invalid saved input.");
        }

        // Replaying the envelope file still honors a minimized input.cs beside a finding bundle.
        var directory = Path.GetDirectoryName(path) ?? string.Empty;
        var sourcePath = Path.Combine(directory, "input.cs");
        if (string.Equals(Path.GetFileName(path), "input.json", StringComparison.Ordinal)
            && File.Exists(Path.Combine(directory, "finding.json"))
            && File.Exists(sourcePath))
        {
            return saved with { Source = File.ReadAllText(sourcePath) };
        }

        return saved;
    }

    private static bool TryLoadFindingBundle(string directory, out Input input)
    {
        input = null!;
        if (!File.Exists(Path.Combine(directory, "finding.json"))
            || !TryReadSavedInput(Path.Combine(directory, "input.json"), out var saved))
        {
            return false;
        }

        var sourcePath = Path.Combine(directory, "input.cs");
        input = File.Exists(sourcePath) ? saved with { Source = File.ReadAllText(sourcePath) } : saved;
        return true;
    }

    private static bool IsGeneratedFindingArtifact(string directory, string file)
    {
        var name = Path.GetFileName(file);
        return name is "input.cs" or "fixed.cs"
            && File.Exists(Path.Combine(directory, "finding.json"))
            && TryReadSavedInput(Path.Combine(directory, "input.json"), out _);
    }

    private static Input LoadSeed(string file)
    {
        var settingsPath = Path.ChangeExtension(file, ".json");
        return new Input(
            File.ReadAllText(file),
            Path.GetFileName(file),
            File.Exists(settingsPath) ? File.ReadAllText(settingsPath) : null);
    }

    private static bool TryReadSavedInput(string path, out Input input)
    {
        input = null!;
        if (!File.Exists(path))
        {
            return false;
        }

        try
        {
            using var document = JsonDocument.Parse(File.ReadAllText(path));
            if (document.RootElement.ValueKind != JsonValueKind.Object
                || !document.RootElement.TryGetProperty("Source", out var source)
                || source.ValueKind != JsonValueKind.String
                || !document.RootElement.TryGetProperty("Name", out var name)
                || name.ValueKind != JsonValueKind.String)
            {
                return false;
            }

            input = document.RootElement.Deserialize<Input>() ?? throw new InvalidOperationException("Invalid saved input.");
            return true;
        }
        catch (JsonException)
        {
            return false;
        }
    }
}
