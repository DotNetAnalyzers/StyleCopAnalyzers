// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.MaintainabilityRules
{
    using System;
    using System.Collections.Generic;
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
    /// A C# code file contains more than one unique type.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs when a C# file contains more than one type. To increase long-term
    /// maintainability of the code-base, each type should be placed in its own file, and file names should reflect the
    /// name of the type within the file.</para>
    ///
    /// <para>It is possible to configure which kind of types this rule should affect.
    /// By default, it allows delegates, enums, structs and interfaces to be placed together with a class.</para>
    ///
    /// <para>It is also possible to place multiple parts of the same partial type within the same file.</para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class SA1402FileMayOnlyContainASingleType : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1402FileMayOnlyContainASingleType"/> analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1402";
        private const string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1402.md";
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(MaintainabilityResources.SA1402Title), MaintainabilityResources.ResourceManager, typeof(MaintainabilityResources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(MaintainabilityResources.SA1402MessageFormat), MaintainabilityResources.ResourceManager, typeof(MaintainabilityResources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(MaintainabilityResources.SA1402Description), MaintainabilityResources.ResourceManager, typeof(MaintainabilityResources));

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.MaintainabilityRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

        private static readonly Action<SyntaxTreeAnalysisContext, StyleCopSettings> SyntaxTreeAction = HandleSyntaxTree;

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

        private static void HandleSyntaxTree(SyntaxTreeAnalysisContext context, StyleCopSettings settings)
        {
            var syntaxRoot = context.Tree.GetRoot(context.CancellationToken);

            var typeNodes = GetTopLevelTypeDeclarations(syntaxRoot, settings);

            string suffix;
            var fileName = FileNameHelpers.GetFileNameAndSuffix(context.Tree.FilePath, out suffix);
            var preferredTypeNode = typeNodes.FirstOrDefault(n => FileNameHelpers.GetConventionalFileName(n, settings.DocumentationRules.FileNamingConvention) == fileName) ?? typeNodes.FirstOrDefault();

            if (preferredTypeNode == null)
            {
                return;
            }

            var foundTypeName = NamedTypeHelpers.GetNameOrIdentifier(preferredTypeNode);
            var isPartialType = NamedTypeHelpers.IsPartialDeclaration(preferredTypeNode);

            foreach (var typeNode in typeNodes)
            {
                if (typeNode == preferredTypeNode || (isPartialType && foundTypeName == NamedTypeHelpers.GetNameOrIdentifier(typeNode)))
                {
                    continue;
                }

                var location = NamedTypeHelpers.GetNameOrIdentifierLocation(typeNode);
                if (location != null)
                {
                    context.ReportDiagnostic(Diagnostic.Create(Descriptor, location));
                }
            }
        }

        private static IEnumerable<MemberDeclarationSyntax> GetTopLevelTypeDeclarations(SyntaxNode root, StyleCopSettings settings)
        {
            var allTypeDeclarations = root.DescendantNodes(descendIntoChildren: node => ContainsTopLevelTypeDeclarations(node)).OfType<MemberDeclarationSyntax>().ToList();
            var relevantTypeDeclarations = allTypeDeclarations.Where(x => IsRelevantType(x, settings)).ToList();
            return relevantTypeDeclarations;
        }

        private static bool ContainsTopLevelTypeDeclarations(SyntaxNode node)
        {
            return node.IsKind(SyntaxKind.CompilationUnit) || node.IsKind(SyntaxKind.NamespaceDeclaration) || node.IsKind(SyntaxKindEx.FileScopedNamespaceDeclaration);
        }

        private static bool IsRelevantType(SyntaxNode node, StyleCopSettings settings)
        {
            var topLevelTypes = settings.MaintainabilityRules.TopLevelTypes;
            var isRelevant = false;

            switch (node.Kind())
            {
            case SyntaxKind.ClassDeclaration:
            case SyntaxKindEx.RecordDeclaration:
                isRelevant = topLevelTypes.Contains(TopLevelType.Class);
                break;
            case SyntaxKind.InterfaceDeclaration:
                isRelevant = topLevelTypes.Contains(TopLevelType.Interface);
                break;
            case SyntaxKind.StructDeclaration:
            case SyntaxKindEx.RecordStructDeclaration:
                isRelevant = topLevelTypes.Contains(TopLevelType.Struct);
                break;
            case SyntaxKind.EnumDeclaration:
                isRelevant = topLevelTypes.Contains(TopLevelType.Enum);
                break;
            case SyntaxKind.DelegateDeclaration:
                isRelevant = topLevelTypes.Contains(TopLevelType.Delegate);
                break;
            }

            return isRelevant;
        }
    }
}
