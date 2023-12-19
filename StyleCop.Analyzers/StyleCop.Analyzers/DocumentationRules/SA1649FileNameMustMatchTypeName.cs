// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.DocumentationRules
{
    using System;
    using System.Collections.Immutable;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.Helpers;
    using StyleCop.Analyzers.Lightup;
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

        private const string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1649.md";
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(DocumentationResources.SA1649Title), DocumentationResources.ResourceManager, typeof(DocumentationResources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(DocumentationResources.SA1649MessageFormat), DocumentationResources.ResourceManager, typeof(DocumentationResources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(DocumentationResources.SA1649Description), DocumentationResources.ResourceManager, typeof(DocumentationResources));

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

            context.RegisterCompilationStartAction(context =>
            {
                context.RegisterSyntaxTreeAction(SyntaxTreeAction);
            });
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

                var modifiers = (firstTypeDeclaration as BaseTypeDeclarationSyntax)?.Modifiers ?? SyntaxFactory.TokenList();
                if (modifiers.Any(SyntaxKind.PartialKeyword))
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

                    var identifier = (firstTypeDeclaration as BaseTypeDeclarationSyntax)?.Identifier
                        ?? ((DelegateDeclarationSyntax)firstTypeDeclaration).Identifier;
                    context.ReportDiagnostic(Diagnostic.Create(Descriptor, identifier.GetLocation(), properties));
                }
            }

            private static MemberDeclarationSyntax GetFirstTypeDeclaration(SyntaxNode root)
            {
                // Prefer to find the first type which is a true TypeDeclarationSyntax
                MemberDeclarationSyntax firstTypeDeclaration = root.DescendantNodes(descendIntoChildren: node => node.IsKind(SyntaxKind.CompilationUnit) || node.IsKind(SyntaxKind.NamespaceDeclaration) || node.IsKind(SyntaxKindEx.FileScopedNamespaceDeclaration))
                    .OfType<TypeDeclarationSyntax>()
                    .FirstOrDefault();

                // If no TypeDeclarationSyntax is found, expand the search to any type declaration as long as only one
                // is present
                var expandedTypeDeclarations = root.DescendantNodes(descendIntoChildren: node => node.IsKind(SyntaxKind.CompilationUnit) || node.IsKind(SyntaxKind.NamespaceDeclaration) || node.IsKind(SyntaxKindEx.FileScopedNamespaceDeclaration))
                    .OfType<MemberDeclarationSyntax>()
                    .Where(node => node is BaseTypeDeclarationSyntax || node.IsKind(SyntaxKind.DelegateDeclaration))
                    .ToList();
                if (expandedTypeDeclarations.Count == 1)
                {
                    firstTypeDeclaration = expandedTypeDeclarations[0];
                }

                return firstTypeDeclaration;
            }
        }
    }
}
