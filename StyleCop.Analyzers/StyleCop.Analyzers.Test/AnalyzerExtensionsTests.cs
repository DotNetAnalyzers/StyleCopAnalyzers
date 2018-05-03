// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test
{
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.SpecialRules;
    using TestHelper;
    using Xunit;

    public class AnalyzerExtensionsTests : DiagnosticVerifier
    {
        private bool invokedBlockCallback;
        private bool invokedMethodDeclarationCallback;

        [Fact]
        public async Task TestCompilationCallbackWithSettingsAsync()
        {
            string testCode = @"
class TypeName
{
    void MethodName()
    {
    }
}
";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            Assert.True(this.invokedBlockCallback);
            Assert.True(this.invokedMethodDeclarationCallback);
        }

        protected override IEnumerable<DiagnosticAnalyzer> GetCSharpDiagnosticAnalyzers()
        {
            yield return new CompilationStartDiagnosticAnalyzer { AnalyzerExtensionsTests = this };
        }

        /// <summary>
        /// Note that I wanted to just extend <see cref="SA0002InvalidSettingsFile"/>, but errors in the meta analyzers
        /// were resulting in AD0001 during the build.
        /// </summary>
        [DiagnosticAnalyzer(LanguageNames.CSharp)]
        internal class CompilationStartDiagnosticAnalyzer : DiagnosticAnalyzer
        {
            public const string DiagnosticId = "SA0002";
            private const string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA0002.md";
            private static readonly LocalizableString Title = new LocalizableResourceString(nameof(SpecialResources.SA0002Title), SpecialResources.ResourceManager, typeof(SpecialResources));
            private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(SpecialResources.SA0002MessageFormat), SpecialResources.ResourceManager, typeof(SpecialResources));
            private static readonly LocalizableString Description = new LocalizableResourceString(nameof(SpecialResources.SA0002Description), SpecialResources.ResourceManager, typeof(SpecialResources));

            private static readonly DiagnosticDescriptor Descriptor =
                new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.SpecialRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

            /// <inheritdoc/>
            public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
                ImmutableArray.Create(Descriptor);

            public AnalyzerExtensionsTests AnalyzerExtensionsTests
            {
                get;
                set;
            }

            public override void Initialize(AnalysisContext context)
            {
                context.RegisterCompilationStartAction(
                    compilationStartContext =>
                    {
                        compilationStartContext.RegisterSyntaxNodeAction(
                            (syntaxNodeContext, settings) =>
                            {
                                Assert.IsAssignableFrom<BlockSyntax>(syntaxNodeContext.Node);
                                Assert.NotNull(settings);
                                this.AnalyzerExtensionsTests.invokedBlockCallback = true;
                            },
                            SyntaxKind.Block);
                        compilationStartContext.RegisterSyntaxNodeAction(
                            (syntaxNodeContext, settings) =>
                            {
                                Assert.IsAssignableFrom<MethodDeclarationSyntax>(syntaxNodeContext.Node);
                                Assert.NotNull(settings);
                                this.AnalyzerExtensionsTests.invokedMethodDeclarationCallback = true;
                            },
                            ImmutableArray.Create(SyntaxKind.MethodDeclaration));
                    });
            }
        }
    }
}
