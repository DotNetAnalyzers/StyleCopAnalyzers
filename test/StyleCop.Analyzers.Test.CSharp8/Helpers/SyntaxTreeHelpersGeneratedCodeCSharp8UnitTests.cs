// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp8.Helpers;

using System.Collections.Immutable;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using StyleCop.Analyzers.Helpers;
using Xunit;

/// <summary>
/// Verifies <see cref="SyntaxTreeHelpers.IsGeneratedDocument"/> honors <c>generated_code</c> analyzer-config
/// overrides ahead of filename heuristics.
/// </summary>
public class SyntaxTreeHelpersGeneratedCodeCSharp8UnitTests
{
    [Fact]
    public void GeneratedCodeFalseOverridesGeneratedFileName()
    {
        SyntaxTree tree = CSharpSyntaxTree.ParseText(
            "class C { }",
            path: "/0/TestType.g.cs");
        AnalyzerOptions options = CreateOptions("""
            root = true

            [*.g.cs]
            generated_code = false
            """);

        Assert.False(tree.IsGeneratedDocument(options, CancellationToken.None));
    }

    [Fact]
    public void GeneratedCodeTrueOverridesOrdinaryFileName()
    {
        SyntaxTree tree = CSharpSyntaxTree.ParseText(
            "class C { }",
            path: "/0/Other.cs");
        AnalyzerOptions options = CreateOptions("""
            root = true

            [Other.cs]
            generated_code = true
            """);

        Assert.True(tree.IsGeneratedDocument(options, CancellationToken.None));
    }

    private static AnalyzerOptions CreateOptions(string editorConfig)
    {
        AnalyzerConfigSet analyzerConfigSet = AnalyzerConfigSet.Create(
            new[] { AnalyzerConfig.Parse(SourceText.From(editorConfig), "/.editorconfig") });
        return new AnalyzerOptions(
            ImmutableArray<AdditionalText>.Empty,
            new TestAnalyzerConfigOptionsProvider(analyzerConfigSet));
    }

    private sealed class TestAnalyzerConfigOptions : AnalyzerConfigOptions
    {
        private readonly AnalyzerConfigOptionsResult analyzerConfigOptionsResult;

        public TestAnalyzerConfigOptions(AnalyzerConfigOptionsResult analyzerConfigOptionsResult)
        {
            this.analyzerConfigOptionsResult = analyzerConfigOptionsResult;
        }

        public override bool TryGetValue(string key, out string value)
        {
            return this.analyzerConfigOptionsResult.AnalyzerOptions.TryGetValue(key, out value!);
        }
    }

    private sealed class TestAnalyzerConfigOptionsProvider : AnalyzerConfigOptionsProvider
    {
        private readonly AnalyzerConfigSet analyzerConfigSet;

        public TestAnalyzerConfigOptionsProvider(AnalyzerConfigSet analyzerConfigSet)
        {
            this.analyzerConfigSet = analyzerConfigSet;
        }

        public override AnalyzerConfigOptions GetOptions(SyntaxTree tree)
            => new TestAnalyzerConfigOptions(this.analyzerConfigSet.GetOptionsForSourcePath(tree.FilePath));

        public override AnalyzerConfigOptions GetOptions(AdditionalText textFile)
            => new TestAnalyzerConfigOptions(this.analyzerConfigSet.GetOptionsForSourcePath(textFile.Path));
    }
}
