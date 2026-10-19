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
        if (File.Exists(path) && Path.GetExtension(path) == ".json")
        {
            return [JsonSerializer.Deserialize<Input>(File.ReadAllText(path)) ?? throw new InvalidOperationException("Invalid saved input.")];
        }

        IEnumerable<string> files = Directory.Exists(path) ? Directory.GetFiles(path, "*.cs", SearchOption.AllDirectories).Order(StringComparer.Ordinal) : [path];
        return files.Select(file => new Input(
            File.ReadAllText(file),
            Path.GetFileName(file),
            File.Exists(Path.ChangeExtension(file, ".json")) ? File.ReadAllText(Path.ChangeExtension(file, ".json")) : null)).ToList();
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
}
