// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.OrderingRules
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.Helpers;

    /// <summary>
    /// A using-alias directive is positioned before a regular using directive.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs when a using-alias directive is placed before a normal using directive.
    /// Using-alias directives have special behavior which can alter the meaning of the rest of the code within the file
    /// or namespace. Placing the using-alias directives together below all other using-directives can make the code
    /// cleaner and easier to read, and can help make it easier to identify the types used throughout the code.</para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class SA1209UsingAliasDirectivesMustBePlacedAfterOtherUsingDirectives : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the
        /// <see cref="SA1209UsingAliasDirectivesMustBePlacedAfterOtherUsingDirectives"/> analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1209";
        private const string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1209.md";
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(OrderingResources.SA1209Title), OrderingResources.ResourceManager, typeof(OrderingResources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(OrderingResources.SA1209MessageFormat), OrderingResources.ResourceManager, typeof(OrderingResources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(OrderingResources.SA1209Description), OrderingResources.ResourceManager, typeof(OrderingResources));

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.OrderingRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

        private static readonly Action<SyntaxNodeAnalysisContext> CompilationUnitAction = HandleCompilationUnit;
        private static readonly Action<SyntaxNodeAnalysisContext> NamespaceDeclarationAction = HandleNamespaceDeclaration;

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
            ImmutableArray.Create(Descriptor);

        /// <inheritdoc/>
        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            context.RegisterSyntaxNodeAction(CompilationUnitAction, SyntaxKind.CompilationUnit);
            context.RegisterSyntaxNodeAction(NamespaceDeclarationAction, SyntaxKind.NamespaceDeclaration);
        }

        private static void HandleCompilationUnit(SyntaxNodeAnalysisContext context)
        {
            var compilationUnit = (CompilationUnitSyntax)context.Node;

            ProcessUsingsAndReportDiagnostic(compilationUnit.Usings, context);
        }

        private static void HandleNamespaceDeclaration(SyntaxNodeAnalysisContext context)
        {
            var namespaceDeclaration = (NamespaceDeclarationSyntax)context.Node;

            ProcessUsingsAndReportDiagnostic(namespaceDeclaration.Usings, context);
        }

        private static void ProcessUsingsAndReportDiagnostic(SyntaxList<UsingDirectiveSyntax> usings, SyntaxNodeAnalysisContext context)
        {
            for (int i = 0; i < usings.Count; i++)
            {
                var usingDirective = usings[i];
                var notLastUsingDirective = i + 1 < usings.Count;
                if (usingDirective.Alias != null && notLastUsingDirective)
                {
                    var nextUsingDirective = usings[i + 1];
                    if (nextUsingDirective.Alias == null && nextUsingDirective.StaticKeyword.IsKind(SyntaxKind.None) && !nextUsingDirective.IsPrecededByPreprocessorDirective())
                    {
                        context.ReportDiagnostic(Diagnostic.Create(Descriptor, usingDirective.GetLocation()));
                    }
                }
            }
        }
    }
}
