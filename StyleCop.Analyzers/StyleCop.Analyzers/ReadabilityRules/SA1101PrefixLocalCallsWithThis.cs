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
    using StyleCop.Analyzers.Helpers;
    using StyleCop.Analyzers.Lightup;

    /// <summary>
    /// A call to an instance member of the local class or a base class is not prefixed with ‘this.’, within a C# code
    /// file.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs whenever the code contains a call to an instance member of the local class
    /// or a base class which is not prefixed with <c>this.</c>. An exception to this rule occurs when there is a local
    /// override of a base class member, and the code intends to call the base class member directly, bypassing the
    /// local override. In this case the call can be prefixed with <c>base.</c> rather than <c>this.</c>.</para>
    ///
    /// <para>By default, StyleCop disallows the use of underscores or <c>m_</c> to mark local class fields, in favor of
    /// the <c>this.</c> prefix. The advantage of using <c>this.</c> is that it applies equally to all element types
    /// including methods, properties, etc., and not just fields, making all calls to class members instantly
    /// recognizable, regardless of which editor is being used to view the code. Another advantage is that it creates a
    /// quick, recognizable differentiation between instance members and static members, which are not prefixed.</para>
    ///
    /// <para>A final advantage of using the <c>this.</c> prefix is that typing <c>this.</c> will cause Visual Studio to
    /// show the IntelliSense pop-up, making it quick and easy for the developer to choose the class member to
    /// call.</para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class SA1101PrefixLocalCallsWithThis : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1101PrefixLocalCallsWithThis"/> analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1101";
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(ReadabilityResources.SA1101Title), ReadabilityResources.ResourceManager, typeof(ReadabilityResources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(ReadabilityResources.SA1101MessageFormat), ReadabilityResources.ResourceManager, typeof(ReadabilityResources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(ReadabilityResources.SA1101Description), ReadabilityResources.ResourceManager, typeof(ReadabilityResources));
        private const string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1101.md";

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.ReadabilityRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

        private static readonly Action<SyntaxNodeAnalysisContext> MemberAccessExpressionAction = HandleMemberAccessExpression;
        private static readonly Action<SyntaxNodeAnalysisContext> SimpleNameAction = HandleSimpleName;

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
            ImmutableArray.Create(Descriptor);

        /// <inheritdoc/>
        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            context.RegisterSyntaxNodeAction(MemberAccessExpressionAction, SyntaxKind.SimpleMemberAccessExpression);
            context.RegisterSyntaxNodeAction(SimpleNameAction, SyntaxKinds.SimpleName);
        }

        /// <summary>
        /// <see cref="SyntaxKind.SimpleMemberAccessExpression"/> is handled separately so only <c>X</c> is evaluated in
        /// the expression <c>X.Y.Z.A.B.C</c>.
        /// </summary>
        /// <param name="context">The analysis context for a <see cref="SyntaxNode"/>.</param>
        private static void HandleMemberAccessExpression(SyntaxNodeAnalysisContext context)
        {
            MemberAccessExpressionSyntax syntax = (MemberAccessExpressionSyntax)context.Node;
            IdentifierNameSyntax nameExpression = syntax.Expression as IdentifierNameSyntax;
            HandleIdentifierNameImpl(context, nameExpression);
        }

        private static void HandleSimpleName(SyntaxNodeAnalysisContext context)
        {
            switch (context.Node?.Parent?.Kind() ?? SyntaxKind.None)
            {
            case SyntaxKind.SimpleMemberAccessExpression:
                // this is handled separately
                return;

            case SyntaxKind.MemberBindingExpression:
            case SyntaxKind.NameColon:
            case SyntaxKind.PointerMemberAccessExpression:
                // this doesn't need to be handled
                return;

            case SyntaxKind.QualifiedCref:
            case SyntaxKind.NameMemberCref:
                // documentation comments don't use 'this.'
                return;

            case SyntaxKind.SimpleAssignmentExpression:
                if (((AssignmentExpressionSyntax)context.Node.Parent).Left == context.Node
                    && (context.Node.Parent.Parent?.IsKind(SyntaxKind.ObjectInitializerExpression) ?? true))
                {
                    /* Handle 'X' in:
                     *   new TypeName() { X = 3 }
                     */
                    return;
                }

                break;

            case SyntaxKind.NameEquals:
                if (((NameEqualsSyntax)context.Node.Parent).Name != context.Node)
                {
                    break;
                }

                switch (context.Node?.Parent?.Parent?.Kind() ?? SyntaxKind.None)
                {
                case SyntaxKind.AttributeArgument:
                case SyntaxKind.AnonymousObjectMemberDeclarator:
                    return;

                default:
                    break;
                }

                break;

            case SyntaxKind.Argument when IsPartOfConstructorInitializer((SimpleNameSyntax)context.Node):
                // constructor invocations cannot contain this.
                return;

            default:
                break;
            }

            HandleIdentifierNameImpl(context, (SimpleNameSyntax)context.Node);
        }

        private static void HandleIdentifierNameImpl(SyntaxNodeAnalysisContext context, SimpleNameSyntax nameExpression)
        {
            if (nameExpression == null)
            {
                return;
            }

            if (!HasThis(nameExpression))
            {
                return;
            }

            SymbolInfo symbolInfo = context.SemanticModel.GetSymbolInfo(nameExpression, context.CancellationToken);
            ImmutableArray<ISymbol> symbolsToAnalyze;
            if (symbolInfo.Symbol != null)
            {
                symbolsToAnalyze = ImmutableArray.Create(symbolInfo.Symbol);
            }
            else if (symbolInfo.CandidateReason == CandidateReason.MemberGroup)
            {
                // analyze the complete set of candidates, and use 'this.' if it applies to all
                symbolsToAnalyze = symbolInfo.CandidateSymbols;
            }
            else
            {
                return;
            }

            foreach (ISymbol symbol in symbolsToAnalyze)
            {
                if (symbol is ITypeSymbol)
                {
                    return;
                }

                if (symbol.IsStatic)
                {
                    return;
                }

                if (!(symbol.ContainingSymbol is ITypeSymbol))
                {
                    // covers local variables, parameters, etc.
                    return;
                }

                if (symbol is IMethodSymbol methodSymbol)
                {
                    switch (methodSymbol.MethodKind)
                    {
                    case MethodKind.Constructor:
                    case MethodKindEx.LocalFunction:
                        return;

                    default:
                        break;
                    }
                }

                // This is a workaround for:
                // - https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/1501
                // - https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/2093
                // and can be removed when the underlying bug in roslyn is resolved
                if (nameExpression.Parent is MemberAccessExpressionSyntax)
                {
                    var memberAccessSymbol = context.SemanticModel.GetSymbolInfo(nameExpression.Parent, context.CancellationToken).Symbol;

                    switch (memberAccessSymbol?.Kind)
                    {
                    case null:
                        break;

                    case SymbolKind.Field:
                    case SymbolKind.Method:
                    case SymbolKind.Property:
                        if (memberAccessSymbol.IsStatic && (memberAccessSymbol.ContainingType.Name == symbol.Name))
                        {
                            return;
                        }

                        break;
                    }
                }

                // End of workaround
            }

            // Prefix local calls with this
            context.ReportDiagnostic(Diagnostic.Create(Descriptor, nameExpression.GetLocation()));
        }

        private static bool HasThis(SyntaxNode node)
        {
            for (; node != null; node = node.Parent)
            {
                switch (node.Kind())
                {
                case SyntaxKind.ClassDeclaration:
                case SyntaxKind.InterfaceDeclaration:
                case SyntaxKind.StructDeclaration:
                case SyntaxKind.DelegateDeclaration:
                case SyntaxKind.EnumDeclaration:
                case SyntaxKind.NamespaceDeclaration:
                    return false;

                case SyntaxKind.FieldDeclaration:
                case SyntaxKind.EventFieldDeclaration:
                    return false;

                case SyntaxKind.EventDeclaration:
                case SyntaxKind.IndexerDeclaration:
                    var basePropertySyntax = (BasePropertyDeclarationSyntax)node;
                    return !basePropertySyntax.Modifiers.Any(SyntaxKind.StaticKeyword);

                case SyntaxKind.PropertyDeclaration:
                    var propertySyntax = (PropertyDeclarationSyntax)node;
                    return !propertySyntax.Modifiers.Any(SyntaxKind.StaticKeyword)
                        && propertySyntax.Initializer == null;

                case SyntaxKind.MultiLineDocumentationCommentTrivia:
                case SyntaxKind.SingleLineDocumentationCommentTrivia:
                    return false;

                case SyntaxKind.ConstructorDeclaration:
                case SyntaxKind.DestructorDeclaration:
                case SyntaxKind.MethodDeclaration:
                    var baseMethodSyntax = (BaseMethodDeclarationSyntax)node;
                    return !baseMethodSyntax.Modifiers.Any(SyntaxKind.StaticKeyword);

                case SyntaxKind.Attribute:
                    return false;

                default:
                    continue;
                }
            }

            return false;
        }

        private static bool IsPartOfConstructorInitializer(SyntaxNode node)
        {
            for (; node != null; node = node.Parent)
            {
                switch (node.Kind())
                {
                case SyntaxKind.ThisConstructorInitializer:
                case SyntaxKind.BaseConstructorInitializer:
                    return true;
                }
            }

            return false;
        }
    }
}
