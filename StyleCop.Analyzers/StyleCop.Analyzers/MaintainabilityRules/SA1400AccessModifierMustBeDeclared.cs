// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.MaintainabilityRules
{
    using System.Collections.Immutable;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// The access modifier for a C# element has not been explicitly defined.
    /// </summary>
    /// <remarks>
    /// <para>C# allows elements to be defined without an access modifier. Depending upon the type of element, C# will
    /// automatically assign an access level to the element in this case.</para>
    ///
    /// <para>This rule requires an access modifier to be explicitly defined for every element. This removes the need
    /// for the reader to make assumptions about the code, improving the readability of the code.</para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SA1400AccessModifierMustBeDeclared : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1400AccessModifierMustBeDeclared"/> analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1400";
        private const string Title = "Access modifier must be declared";
        private const string MessageFormat = "Element '{0}' must declare an access modifier";
        private const string Description = "The access modifier for a C# element has not been explicitly defined.";
        private const string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1400.md";

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.MaintainabilityRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

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
            context.RegisterSyntaxNodeActionHonorExclusions(HandleBaseTypeDeclarationSyntax, SyntaxKind.ClassDeclaration);
            context.RegisterSyntaxNodeActionHonorExclusions(HandleBaseTypeDeclarationSyntax, SyntaxKind.InterfaceDeclaration);
            context.RegisterSyntaxNodeActionHonorExclusions(HandleBaseTypeDeclarationSyntax, SyntaxKind.EnumDeclaration);
            context.RegisterSyntaxNodeActionHonorExclusions(HandleBaseTypeDeclarationSyntax, SyntaxKind.StructDeclaration);
            context.RegisterSyntaxNodeActionHonorExclusions(HandleDelegateDeclarationSyntax, SyntaxKind.DelegateDeclaration);
            context.RegisterSyntaxNodeActionHonorExclusions(HandleEventDeclarationSyntax, SyntaxKind.EventDeclaration);
            context.RegisterSyntaxNodeActionHonorExclusions(HandleMethodDeclarationSyntax, SyntaxKind.MethodDeclaration);
            context.RegisterSyntaxNodeActionHonorExclusions(HandlePropertyDeclarationSyntax, SyntaxKind.PropertyDeclaration);
            context.RegisterSyntaxNodeActionHonorExclusions(HandleBaseFieldDeclarationSyntax, SyntaxKind.EventFieldDeclaration);
            context.RegisterSyntaxNodeActionHonorExclusions(HandleBaseFieldDeclarationSyntax, SyntaxKind.FieldDeclaration);
            context.RegisterSyntaxNodeActionHonorExclusions(HandleOperatorDeclarationSyntax, SyntaxKind.OperatorDeclaration);
            context.RegisterSyntaxNodeActionHonorExclusions(HandleConversionOperatorDeclarationSyntax, SyntaxKind.ConversionOperatorDeclaration);
            context.RegisterSyntaxNodeActionHonorExclusions(HandleIndexerDeclarationSyntax, SyntaxKind.IndexerDeclaration);
            context.RegisterSyntaxNodeActionHonorExclusions(HandleConstructorDeclarationSyntax, SyntaxKind.ConstructorDeclaration);
        }

        private static void HandleBaseTypeDeclarationSyntax(SyntaxNodeAnalysisContext context)
        {
            var syntax = (BaseTypeDeclarationSyntax)context.Node;
            CheckAccessModifiers(context, syntax.Identifier, syntax.Modifiers);
        }

        private static void HandleDelegateDeclarationSyntax(SyntaxNodeAnalysisContext context)
        {
            var syntax = (DelegateDeclarationSyntax)context.Node;
            CheckAccessModifiers(context, syntax.Identifier, syntax.Modifiers);
        }

        private static void HandleEventDeclarationSyntax(SyntaxNodeAnalysisContext context)
        {
            var syntax = (EventDeclarationSyntax)context.Node;
            if (syntax.ExplicitInterfaceSpecifier != null)
            {
                return;
            }

            if (syntax.Parent.IsKind(SyntaxKind.InterfaceDeclaration))
            {
                return;
            }

            CheckAccessModifiers(context, syntax.Identifier, syntax.Modifiers);
        }

        private static void HandleMethodDeclarationSyntax(SyntaxNodeAnalysisContext context)
        {
            var syntax = (MethodDeclarationSyntax)context.Node;
            if (syntax.ExplicitInterfaceSpecifier != null)
            {
                return;
            }

            if (syntax.Parent.IsKind(SyntaxKind.InterfaceDeclaration))
            {
                return;
            }

            CheckAccessModifiers(context, syntax.Identifier, syntax.Modifiers);
        }

        private static void HandlePropertyDeclarationSyntax(SyntaxNodeAnalysisContext context)
        {
            var syntax = (PropertyDeclarationSyntax)context.Node;
            if (syntax.ExplicitInterfaceSpecifier != null)
            {
                return;
            }

            if (syntax.Parent.IsKind(SyntaxKind.InterfaceDeclaration))
            {
                return;
            }

            CheckAccessModifiers(context, syntax.Identifier, syntax.Modifiers);
        }

        private static void HandleBaseFieldDeclarationSyntax(SyntaxNodeAnalysisContext context)
        {
            var syntax = (BaseFieldDeclarationSyntax)context.Node;
            if (syntax.Parent.IsKind(SyntaxKind.InterfaceDeclaration))
            {
                // this can occur for event field declarations
                return;
            }

            VariableDeclarationSyntax declarationSyntax = syntax.Declaration;
            if (declarationSyntax == null)
            {
                return;
            }

            VariableDeclaratorSyntax declarator = declarationSyntax.Variables.FirstOrDefault(i => !i.Identifier.IsMissing);
            if (declarator == null)
            {
                return;
            }

            CheckAccessModifiers(context, declarator.Identifier, syntax.Modifiers, declarator);
        }

        private static void HandleOperatorDeclarationSyntax(SyntaxNodeAnalysisContext context)
        {
            var syntax = (OperatorDeclarationSyntax)context.Node;
            CheckAccessModifiers(context, syntax.OperatorToken, syntax.Modifiers);
        }

        private static void HandleConversionOperatorDeclarationSyntax(SyntaxNodeAnalysisContext context)
        {
            var syntax = (ConversionOperatorDeclarationSyntax)context.Node;
            CheckAccessModifiers(context, syntax.Type.GetLastToken(), syntax.Modifiers);
        }

        private static void HandleIndexerDeclarationSyntax(SyntaxNodeAnalysisContext context)
        {
            var syntax = (IndexerDeclarationSyntax)context.Node;
            if (syntax.ExplicitInterfaceSpecifier != null)
            {
                return;
            }

            if (syntax.Parent.IsKind(SyntaxKind.InterfaceDeclaration))
            {
                return;
            }

            CheckAccessModifiers(context, syntax.ThisKeyword, syntax.Modifiers);
        }

        private static void HandleConstructorDeclarationSyntax(SyntaxNodeAnalysisContext context)
        {
            var syntax = (ConstructorDeclarationSyntax)context.Node;
            CheckAccessModifiers(context, syntax.Identifier, syntax.Modifiers);
        }

        private static void CheckAccessModifiers(SyntaxNodeAnalysisContext context, SyntaxToken identifier, SyntaxTokenList modifiers, SyntaxNode declarationNode = null)
        {
            if (identifier.IsMissing)
            {
                return;
            }

            foreach (SyntaxToken token in modifiers)
            {
                switch (token.Kind())
                {
                case SyntaxKind.PublicKeyword:
                case SyntaxKind.ProtectedKeyword:
                case SyntaxKind.InternalKeyword:
                case SyntaxKind.PrivateKeyword:
                    return;

                case SyntaxKind.StaticKeyword:
                    if (context.Node is ConstructorDeclarationSyntax)
                    {
                        return;
                    }

                    break;

                case SyntaxKind.PartialKeyword:
                    // the access modifier might be declared on another part, which isn't handled at this time
                    return;

                default:
                    break;
                }
            }

            // missing access modifier
            ISymbol symbol = context.SemanticModel.GetDeclaredSymbol(declarationNode ?? context.Node, context.CancellationToken);
            string name = symbol?.Name;
            if (string.IsNullOrEmpty(name))
            {
                return;
            }

            context.ReportDiagnostic(Diagnostic.Create(Descriptor, identifier.GetLocation(), name));
        }
    }
}
