// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.OrderingRules
{
    using System;
    using System.Collections.Immutable;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.Helpers;

    /// <summary>
    /// The partial element does not have an access modifier defined.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs when the partial elements does not have an access modifier defined.</para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class SA1205PartialElementsMustDeclareAccess : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1205PartialElementsMustDeclareAccess"/> analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1205";
        private const string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1205.md";
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(OrderingResources.SA1205Title), OrderingResources.ResourceManager, typeof(OrderingResources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(OrderingResources.SA1205MessageFormat), OrderingResources.ResourceManager, typeof(OrderingResources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(OrderingResources.SA1205Description), OrderingResources.ResourceManager, typeof(OrderingResources));

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.OrderingRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

        private static readonly Action<SyntaxNodeAnalysisContext> TypeDeclarationAction = HandleTypeDeclaration;

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
            ImmutableArray.Create(Descriptor);

        /// <inheritdoc/>
        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            context.RegisterSyntaxNodeAction(TypeDeclarationAction, SyntaxKinds.TypeDeclaration);
        }

        private static void HandleTypeDeclaration(SyntaxNodeAnalysisContext context)
        {
            var typeDeclarationNode = (TypeDeclarationSyntax)context.Node;

            if (typeDeclarationNode.Modifiers.Any(SyntaxKind.PartialKeyword))
            {
                if (!typeDeclarationNode.Modifiers.Any(SyntaxKind.PublicKeyword)
                    && !typeDeclarationNode.Modifiers.Any(SyntaxKind.InternalKeyword)
                    && !typeDeclarationNode.Modifiers.Any(SyntaxKind.ProtectedKeyword)
                    && !typeDeclarationNode.Modifiers.Any(SyntaxKind.PrivateKeyword))
                {
                    context.ReportDiagnostic(Diagnostic.Create(Descriptor, typeDeclarationNode.Identifier.GetLocation()));
                }
            }
        }
    }
}
