// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.ReadabilityRules
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;

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
            context.RegisterCompilationStartAction(HandleCompilationStart);
        }

        private static void HandleCompilationStart(CompilationStartAnalysisContext context)
        {
            context.RegisterSyntaxNodeActionHonorExclusions(HandleUsingDeclaration, SyntaxKind.UsingDirective);
        }

        private static void HandleUsingDeclaration(SyntaxNodeAnalysisContext context)
        {
            var usingDirective = (UsingDirectiveSyntax)context.Node;
            CheckUsingDeclaration(context, usingDirective);
        }

        private static void CheckUsingDeclaration(SyntaxNodeAnalysisContext context, UsingDirectiveSyntax usingDirective)
        {
            // Usings outside of a namepsace are always qualified.
            if (usingDirective.Parent is NamespaceDeclarationSyntax && usingDirective.StaticKeyword.IsKind(SyntaxKind.None))
            {
                string usingString = usingDirective.Name.ToString();

                // Check for global qualified namespaces.
                if (usingString.IndexOf("::", StringComparison.Ordinal) < 0)
                {
                    SymbolInfo symbolInfo = context.SemanticModel.GetSymbolInfo(usingDirective.Name, context.CancellationToken);
                    if (symbolInfo.Symbol != null && (symbolInfo.Symbol.Kind == SymbolKind.Namespace || symbolInfo.Symbol.Kind == SymbolKind.NamedType))
                    {
                        string symbolString = symbolInfo.Symbol.ToString();
                        if (symbolString != usingString)
                        {
                            if (symbolInfo.Symbol.Kind == SymbolKind.NamedType)
                            {
                                context.ReportDiagnostic(Diagnostic.Create(DescriptorType, usingDirective.GetLocation(), symbolString));
                            }
                            else
                            {
                                context.ReportDiagnostic(Diagnostic.Create(DescriptorNamespace, usingDirective.GetLocation(), symbolString));
                            }
                        }
                    }
                }
            }
        }
    }
}