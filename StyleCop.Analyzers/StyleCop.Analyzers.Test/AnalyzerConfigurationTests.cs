// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.Test
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Microsoft.CodeAnalysis.Testing;
    using Microsoft.CodeAnalysis.Testing.Verifiers;
    using Xunit;

    public class AnalyzerConfigurationTests
    {
        public static IEnumerable<object[]> AllAnalyzers
        {
            get
            {
                foreach (var type in typeof(AnalyzerCategory).Assembly.DefinedTypes)
                {
                    if (type.GetCustomAttributes(typeof(DiagnosticAnalyzerAttribute), true).Any())
                    {
                        yield return new object[] { type };
                    }
                }
            }
        }

        [Theory]
        [MemberData(nameof(AllAnalyzers))]
        public async Task TestEmptySourceAsync(Type analyzerType)
        {
            await new CSharpTest(analyzerType)
            {
                TestCode = string.Empty,
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(AllAnalyzers))]
        public void TestHelpLink(Type analyzerType)
        {
            var analyzer = (DiagnosticAnalyzer)Activator.CreateInstance(analyzerType);
            foreach (var diagnostic in analyzer.SupportedDiagnostics)
            {
                if (diagnostic.DefaultSeverity == DiagnosticSeverity.Hidden && diagnostic.CustomTags.Contains(WellKnownDiagnosticTags.NotConfigurable))
                {
                    // This diagnostic will never appear in the UI
                    continue;
                }

                string expected = $"https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/{diagnostic.Id}.md";
                Assert.Equal(expected, diagnostic.HelpLinkUri);
            }
        }

        private class CSharpTest : CodeFixTest<XUnitVerifier>
        {
            private readonly Type analyzerType;

            public CSharpTest(Type analyzerType)
            {
                this.analyzerType = analyzerType;
            }

            public override string Language => LanguageNames.CSharp;

            public override Type SyntaxKindType => typeof(SyntaxKind);

            protected override string DefaultFileExt => "cs";

            protected override CompilationOptions CreateCompilationOptions()
                => new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, allowUnsafe: true);

            protected override ParseOptions CreateParseOptions()
                => new CSharpParseOptions(LanguageVersion.CSharp6);

            protected override IEnumerable<CodeFixProvider> GetCodeFixProviders()
                => new CodeFixProvider[0];

            protected override IEnumerable<DiagnosticAnalyzer> GetDiagnosticAnalyzers()
                => new[] { (DiagnosticAnalyzer)Activator.CreateInstance(this.analyzerType) };
        }
    }
}
