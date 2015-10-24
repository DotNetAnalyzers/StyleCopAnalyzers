// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.DocumentationRules
{
    using System;
    using System.Collections.Immutable;
    using Helpers;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// <c>&lt;inheritdoc&gt;</c> has been used on an element that doesn't inherit from a base class or implement an
    /// interface.
    /// </summary>
    /// <remarks>
    /// <para>Verifies that an <c>inheritdoc</c> tag is not used when the class or interface does not inherit from a
    /// base class or interface.</para>
    ///
    /// <para>A violation of this rule occurs when the element having the <c>inheritdoc</c> tag doesn't inherit from a
    /// base case or implement an interface.</para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class SA1648InheritDocMustBeUsedWithInheritingClass : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1648InheritDocMustBeUsedWithInheritingClass"/> analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1648";
        private const string Title = "inheritdoc must be used with inheriting class";
        private const string MessageFormat = "inheritdoc must be used with inheriting class";
        private const string Description = "<inheritdoc> has been used on an element that doesn't inherit from a base class or implement an interface.";
        private const string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1648.md";

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.DocumentationRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

        private static readonly ImmutableArray<SyntaxKind> HandledTypeLikeDeclarationKinds =
            ImmutableArray.Create(SyntaxKind.ClassDeclaration, SyntaxKind.StructDeclaration, SyntaxKind.InterfaceDeclaration, SyntaxKind.EnumDeclaration, SyntaxKind.DelegateDeclaration);

        private static readonly ImmutableArray<SyntaxKind> MemberDeclarationKinds =
            ImmutableArray.Create(SyntaxKind.ConstructorDeclaration, SyntaxKind.EventDeclaration, SyntaxKind.MethodDeclaration, SyntaxKind.PropertyDeclaration, SyntaxKind.EventFieldDeclaration, SyntaxKind.FieldDeclaration, SyntaxKind.IndexerDeclaration);

        private static readonly Action<CompilationStartAnalysisContext> CompilationStartAction = HandleCompilationStart;
        private static readonly Action<SyntaxNodeAnalysisContext> BaseTypeLikeDeclarationAction = HandleBaseTypeLikeDeclaration;
        private static readonly Action<SyntaxNodeAnalysisContext> MemberDeclarationAction = HandleMemberDeclaration;

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
            ImmutableArray.Create(Descriptor);

        /// <inheritdoc/>
        public override void Initialize(AnalysisContext context)
        {
            context.RegisterCompilationStartAction(CompilationStartAction);
        }

        private static void HandleCompilationStart(CompilationStartAnalysisContext context)
        {
            context.RegisterSyntaxNodeActionHonorExclusions(BaseTypeLikeDeclarationAction, HandledTypeLikeDeclarationKinds);
            context.RegisterSyntaxNodeActionHonorExclusions(MemberDeclarationAction, MemberDeclarationKinds);
        }

        private static void HandleBaseTypeLikeDeclaration(SyntaxNodeAnalysisContext context)
        {
            BaseTypeDeclarationSyntax baseType = context.Node as BaseTypeDeclarationSyntax;

            // baseType can be null here if we are looking at a delegate declaration
            if (baseType != null && baseType.BaseList != null && baseType.BaseList.Types.Any())
            {
                return;
            }

            DocumentationCommentTriviaSyntax documentation = context.Node.GetDocumentationCommentTriviaSyntax();

            XmlNodeSyntax inheritDocElement = documentation?.Content.GetFirstXmlElement(XmlCommentHelper.InheritdocXmlTag);

            if (inheritDocElement != null)
            {
                context.ReportDiagnostic(Diagnostic.Create(Descriptor, inheritDocElement.GetLocation()));
            }
        }

        private static void HandleMemberDeclaration(SyntaxNodeAnalysisContext context)
        {
            MemberDeclarationSyntax memberSyntax = (MemberDeclarationSyntax)context.Node;

            var modifiers = memberSyntax.GetModifiers();

            if (modifiers.Any(SyntaxKind.OverrideKeyword))
            {
                return;
            }

            DocumentationCommentTriviaSyntax documentation = memberSyntax.GetDocumentationCommentTriviaSyntax();

            XmlNodeSyntax inheritDocElement = documentation?.Content.GetFirstXmlElement(XmlCommentHelper.InheritdocXmlTag);

            if (inheritDocElement != null)
            {
                ISymbol declaredSymbol = context.SemanticModel.GetDeclaredSymbol(memberSyntax, context.CancellationToken);
                if (declaredSymbol == null && memberSyntax.IsKind(SyntaxKind.EventFieldDeclaration))
                {
                    var eventFieldDeclarationSyntax = (EventFieldDeclarationSyntax)memberSyntax;
                    VariableDeclaratorSyntax firstVariable = eventFieldDeclarationSyntax.Declaration?.Variables.FirstOrDefault();
                    if (firstVariable != null)
                    {
                        declaredSymbol = context.SemanticModel.GetDeclaredSymbol(firstVariable, context.CancellationToken);
                    }
                }

                // If we don't have a declared symbol we have some kind of field declaration. A field can not override or
                // implement anything so we want to report a diagnostic.
                if (declaredSymbol == null || !NamedTypeHelpers.IsImplementingAnInterfaceMember(declaredSymbol))
                {
                    context.ReportDiagnostic(Diagnostic.Create(Descriptor, inheritDocElement.GetLocation()));
                }
            }
        }
    }
}
