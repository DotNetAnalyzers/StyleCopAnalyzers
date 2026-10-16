// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

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

        private static readonly string HelpLink = HelpLinkHelper.GetHelpLink(DiagnosticId);
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

                var firstTypeDeclaration = GetFirstTypeDeclaration(syntaxRoot, settings);
                if (firstTypeDeclaration == null)
                {
                    return;
                }

                if (((CompilationUnitSyntax)syntaxRoot).Members.Any(SyntaxKind.GlobalStatement))
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
                var expectedFileName = FileNameHelpers.GetConventionalFileNameWithDecodedIdentifier(firstTypeDeclaration, settings.DocumentationRules.FileNamingConvention);

                if (string.Compare(fileName, expectedFileName, StringComparison.OrdinalIgnoreCase) != 0)
                {
                    if (settings.DocumentationRules.FileNamingConvention == FileNamingConvention.StyleCop
                        && string.Compare(fileName, FileNameHelpers.GetIdentifierValueText(firstTypeDeclaration), StringComparison.OrdinalIgnoreCase) == 0)
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

            private static MemberDeclarationSyntax? GetFirstTypeDeclaration(SyntaxNode root, StyleCopSettings settings)
            {
                MemberDeclarationSyntax? firstTypeDeclaration = null;
                var topLevelTypes = settings.MaintainabilityRules.TopLevelTypes;
                foreach (var typeDeclaration in root.DescendantNodes(descendIntoChildren: node =>
                    node.IsKind(SyntaxKind.CompilationUnit)
                    || node.IsKind(SyntaxKind.NamespaceDeclaration)
                    || node.IsKind(SyntaxKindEx.FileScopedNamespaceDeclaration)
                    || node is BaseTypeDeclarationSyntax).OfType<MemberDeclarationSyntax>())
                {
                    if (!(typeDeclaration is BaseTypeDeclarationSyntax) && !typeDeclaration.IsKind(SyntaxKind.DelegateDeclaration))
                    {
                        continue;
                    }

                    if (!HasName(typeDeclaration))
                    {
                        continue;
                    }

                    if (firstTypeDeclaration == null)
                    {
                        firstTypeDeclaration = typeDeclaration;
                    }

                    if (IsTopLevelType(typeDeclaration)
                        && IsConfiguredTopLevelType(typeDeclaration, topLevelTypes))
                    {
                        return typeDeclaration;
                    }
                }

                return firstTypeDeclaration;
            }

            private static bool IsTopLevelType(MemberDeclarationSyntax node)
            {
                return node.Parent.IsKind(SyntaxKind.CompilationUnit)
                    || node.Parent.IsKind(SyntaxKind.NamespaceDeclaration)
                    || node.Parent.IsKind(SyntaxKindEx.FileScopedNamespaceDeclaration);
            }

            private static bool IsConfiguredTopLevelType(MemberDeclarationSyntax node, ImmutableArray<TopLevelType> topLevelTypes)
            {
                switch (node.Kind())
                {
                case SyntaxKind.ClassDeclaration:
                case SyntaxKindEx.RecordDeclaration:
                    return topLevelTypes.Contains(TopLevelType.Class);
                case SyntaxKind.InterfaceDeclaration:
                    return topLevelTypes.Contains(TopLevelType.Interface);
                case SyntaxKind.StructDeclaration:
                case SyntaxKindEx.RecordStructDeclaration:
                case SyntaxKindEx.UnionDeclaration:
                    return topLevelTypes.Contains(TopLevelType.Struct);
                case SyntaxKind.EnumDeclaration:
                    return topLevelTypes.Contains(TopLevelType.Enum);
                case SyntaxKind.DelegateDeclaration:
                    return topLevelTypes.Contains(TopLevelType.Delegate);
                default:
                    return false;
                }
            }

            private static bool HasName(MemberDeclarationSyntax node)
            {
                // A C# 14 extension block is a type declaration without an identifier, so there is no type name for
                // the file name to match.
                return !(node is BaseTypeDeclarationSyntax typeDeclaration)
                    || !typeDeclaration.Identifier.IsKind(SyntaxKind.None);
            }
        }
    }
}
