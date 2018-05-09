// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.DocumentationRules
{
    using System;
    using System.Collections.Immutable;
    using System.IO;
    using System.Linq;
    using StyleCop.Analyzers.Helpers;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.Settings.ObjectModel;

    /// <summary>
    /// The name of a C# code file does not match the first type declared in the file. For generics
    /// that are defined as Class1&lt;T&gt; the name of the file needs to be Class1{T}.cs or Class1`1
    /// depending on the fileNamingConvention setting.
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class SA1649FileNameMustMatchTypeName : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1649FileNameMustMatchTypeName"/>
        /// analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1649";

        /// <summary>
        /// The key used for passing the expected file name to the code fix provider.
        /// </summary>
        internal const string ExpectedFileNameKey = "ExpectedFileName";

        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(DocumentationResources.SA1649Title), DocumentationResources.ResourceManager, typeof(DocumentationResources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(DocumentationResources.SA1649MessageFormat), DocumentationResources.ResourceManager, typeof(DocumentationResources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(DocumentationResources.SA1649Description), DocumentationResources.ResourceManager, typeof(DocumentationResources));
        private static readonly string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1649.md";

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.DocumentationRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

        private static readonly Action<SyntaxTreeAnalysisContext, StyleCopSettings> SyntaxTreeAction = Analyzer.HandleSyntaxTree;

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
            ImmutableArray.Create(Descriptor);

        /// <inheritdoc/>
        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            context.RegisterSyntaxTreeAction(SyntaxTreeAction);
        }

        private static class Analyzer
        {
            public static void HandleSyntaxTree(SyntaxTreeAnalysisContext context, StyleCopSettings settings)
            {
                var syntaxRoot = context.Tree.GetRoot(context.CancellationToken);

                var firstTypeDeclaration = GetFirstTypeDeclaration(syntaxRoot);
                if (firstTypeDeclaration == null)
                {
                    return;
                }

                if (firstTypeDeclaration.Modifiers.Any(SyntaxKind.PartialKeyword))
                {
                    return;
                }

                string suffix;
                var fileName = FileNameHelpers.GetFileNameAndSuffix(context.Tree.FilePath, out suffix);
                var expectedFileName = FileNameHelpers.GetConventionalFileName(firstTypeDeclaration, settings.DocumentationRules.FileNamingConvention);

                if (string.Compare(fileName, expectedFileName, StringComparison.OrdinalIgnoreCase) != 0)
                {
                    if (settings.DocumentationRules.FileNamingConvention == FileNamingConvention.StyleCop
                        && string.Compare(fileName, FileNameHelpers.GetSimpleFileName(firstTypeDeclaration), StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        return;
                    }

                    var properties = ImmutableDictionary.Create<string, string>()
                        .Add(ExpectedFileNameKey, expectedFileName + suffix);

                    context.ReportDiagnostic(Diagnostic.Create(Descriptor, firstTypeDeclaration.Identifier.GetLocation(), properties));
                }
            }

            private static TypeDeclarationSyntax GetFirstTypeDeclaration(SyntaxNode root)
            {
                return root.DescendantNodes(descendIntoChildren: node => node.IsKind(SyntaxKind.CompilationUnit) || node.IsKind(SyntaxKind.NamespaceDeclaration))
                    .OfType<TypeDeclarationSyntax>()
                    .FirstOrDefault();
            }
        }
    }
}
