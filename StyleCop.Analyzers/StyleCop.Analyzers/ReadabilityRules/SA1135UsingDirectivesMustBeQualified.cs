// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.ReadabilityRules
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;

    using StyleCop.Analyzers.Helpers;

    /// <summary>
    /// A using directive is not qualified.
    /// </summary>
    /// <remarks>
    /// <para>
    /// A violation of this rule occurs when a using directive is contained within a namespace and is not qualified.
    /// </para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class SA1135UsingDirectivesMustBeQualified : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1135UsingDirectivesMustBeQualified"/> analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1135";
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(ReadabilityResources.SA1135Title), ReadabilityResources.ResourceManager, typeof(ReadabilityResources));
        private static readonly LocalizableString MessageFormatNamespace = new LocalizableResourceString(nameof(ReadabilityResources.SA1135MessageFormatNamespace), ReadabilityResources.ResourceManager, typeof(ReadabilityResources));
        private static readonly LocalizableString MessageFormatType = new LocalizableResourceString(nameof(ReadabilityResources.SA1135MessageFormatType), ReadabilityResources.ResourceManager, typeof(ReadabilityResources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(ReadabilityResources.SA1135Description), ReadabilityResources.ResourceManager, typeof(ReadabilityResources));
        private static readonly string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1135.md";

        public static DiagnosticDescriptor DescriptorNamespace { get; } =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormatNamespace, AnalyzerCategory.ReadabilityRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

        public static DiagnosticDescriptor DescriptorType { get; } =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormatType, AnalyzerCategory.ReadabilityRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
            ImmutableArray.Create(DescriptorNamespace);

        /// <inheritdoc/>
        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            context.RegisterCompilationStartAction(HandleCompilationStart);
        }

        private static void HandleCompilationStart(CompilationStartAnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(HandleUsingDeclaration, SyntaxKind.UsingDirective);
        }

        private static void HandleUsingDeclaration(SyntaxNodeAnalysisContext context)
        {
            var usingDirective = (UsingDirectiveSyntax)context.Node;
            CheckUsingDeclaration(context, usingDirective);
        }

        private static void CheckUsingDeclaration(SyntaxNodeAnalysisContext context, UsingDirectiveSyntax usingDirective)
        {
            if (!usingDirective.Parent.IsKind(SyntaxKind.NamespaceDeclaration))
            {
                // Usings outside of a namespace are always qualified.
                return;
            }

            if (!usingDirective.StaticKeyword.IsKind(SyntaxKind.None))
            {
                // using static types is not considered.
                return;
            }

            if (usingDirective.HasNamespaceAliasQualifier())
            {
                // global qualified namespaces are OK.
                return;
            }

            var symbol = context.SemanticModel.GetSymbolInfo(usingDirective.Name, context.CancellationToken).Symbol;
            if (symbol == null)
            {
                // if there is no symbol, do not proceed.
                return;
            }

            string symbolString = symbol.ToString();
            string usingString = usingDirective.Name.ToString();
            if ((symbolString != usingString) && !usingDirective.StartsWithAlias(context.SemanticModel, context.CancellationToken))
            {
                switch (symbol.Kind)
                {
                case SymbolKind.Namespace:
                    context.ReportDiagnostic(Diagnostic.Create(DescriptorNamespace, usingDirective.GetLocation(), symbolString));
                    break;

                case SymbolKind.NamedType:
                    var containingNamespace = ((NamespaceDeclarationSyntax)usingDirective.Parent).Name.ToString();
                    if (containingNamespace != symbol.ContainingNamespace.ToString())
                    {
                        context.ReportDiagnostic(Diagnostic.Create(DescriptorType, usingDirective.GetLocation(), symbolString));
                    }

                    break;
                }
            }
        }
    }
}
