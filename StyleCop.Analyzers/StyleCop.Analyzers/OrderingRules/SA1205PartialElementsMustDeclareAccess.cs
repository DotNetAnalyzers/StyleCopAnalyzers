﻿namespace StyleCop.Analyzers.OrderingRules
{
    using System.Collections.Immutable;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// The partial element does not have an access modifier defined. StyleCop may not be able to determine the correct
    /// placement of the elements in the file. Please declare an access modifier for all partial elements.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs when the partial elements does not have an access modifier defined.</para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SA1205PartialElementsMustDeclareAccess : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1205PartialElementsMustDeclareAccess"/> analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1205";
        private const string Title = "Partial elements must declare access";
        private const string MessageFormat = "Partial elements must declare an access modifier";
        private const string Description = "The partial element does not have an access modifier defined. StyleCop may not be able to determine the correct placement of the elements in the file. Please declare an access modifier for all partial elements.";
        private const string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1205.md";

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.OrderingRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

        private static readonly ImmutableArray<DiagnosticDescriptor> SupportedDiagnosticsValue =
            ImmutableArray.Create(Descriptor);

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                return SupportedDiagnosticsValue;
            }
        }

        /// <inheritdoc/>
        public override void Initialize(AnalysisContext context)
        {
            context.RegisterCompilationStartAction(HandleCompilationStart);
        }

        private static void HandleCompilationStart(CompilationStartAnalysisContext context)
        {
            context.RegisterSyntaxNodeActionHonorExclusions(HandleElementDeclaration, SyntaxKind.ClassDeclaration, SyntaxKind.StructDeclaration, SyntaxKind.InterfaceDeclaration);
        }

        private static void HandleElementDeclaration(SyntaxNodeAnalysisContext context)
        {
            var typeDeclarationNode = (TypeDeclarationSyntax)context.Node;

            if (ContainsModifier(typeDeclarationNode.Modifiers, SyntaxKind.PartialKeyword))
            {
                if (!ContainsModifier(typeDeclarationNode.Modifiers, SyntaxKind.PublicKeyword) &&
                    !ContainsModifier(typeDeclarationNode.Modifiers, SyntaxKind.InternalKeyword))
                {
                    context.ReportDiagnostic(Diagnostic.Create(Descriptor, typeDeclarationNode.Identifier.GetLocation()));
                }
            }
        }

        private static bool ContainsModifier(SyntaxTokenList modifiers, SyntaxKind expectedKeyword)
        {
            return modifiers.Any(modifier => modifier.Kind() == expectedKeyword);
        }
    }
}
