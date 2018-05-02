// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.LayoutRules
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.Helpers;

    /// <summary>
    /// The opening or closing brace within a C# statement, element, or expression is not placed on its own line.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs when the opening or closing brace within a statement, element, or
    /// expression is not placed on its own line. For example:</para>
    ///
    /// <code language="cs">
    /// public object Method()
    /// {
    ///   lock (this) {
    ///     return this.value;
    ///   }
    /// }
    /// </code>
    ///
    /// <para>When StyleCop checks this code, a violation of this rule will occur because the opening brace of the lock
    /// statement is placed on the same line as the lock keyword, rather than being placed on its own line, as
    /// follows:</para>
    ///
    /// <code language="cs">
    /// public object Method()
    /// {
    ///   lock (this)
    ///   {
    ///     return this.value;
    ///   }
    /// }
    /// </code>
    ///
    /// <para>A violation will also occur if the closing brace shares a line with other code. For example:</para>
    ///
    /// <code language="cs">
    /// public object Method()
    /// {
    ///   lock (this)
    ///   {
    ///     return this.value; }
    /// }
    /// </code>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class SA1500BracesForMultiLineStatementsMustNotShareLine : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the
        /// <see cref="SA1500BracesForMultiLineStatementsMustNotShareLine"/> analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1500";
        private const string Title = "Braces for multi-line statements should not share line";
        private const string MessageFormat = "Braces for multi-line statements should not share line";
        private const string Description = "The opening or closing brace within a C# statement, element, or expression is not placed on its own line.";
        private const string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1500.md";

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.LayoutRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

        private static readonly Action<SyntaxNodeAnalysisContext> NamespaceDeclarationAction = HandleNamespaceDeclaration;
        private static readonly Action<SyntaxNodeAnalysisContext> BaseTypeDeclarationAction = HandleBaseTypeDeclaration;
        private static readonly Action<SyntaxNodeAnalysisContext> AccessorListAction = HandleAccessorList;
        private static readonly Action<SyntaxNodeAnalysisContext> BlockAction = HandleBlock;
        private static readonly Action<SyntaxNodeAnalysisContext> SwitchStatementAction = HandleSwitchStatement;
        private static readonly Action<SyntaxNodeAnalysisContext> InitializerExpressionAction = HandleInitializerExpression;
        private static readonly Action<SyntaxNodeAnalysisContext> AnonymousObjectCreationExpressionAction = HandleAnonymousObjectCreationExpression;

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
            ImmutableArray.Create(Descriptor);

        /// <inheritdoc/>
        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            context.RegisterSyntaxNodeAction(NamespaceDeclarationAction, SyntaxKind.NamespaceDeclaration);
            context.RegisterSyntaxNodeAction(BaseTypeDeclarationAction, SyntaxKinds.BaseTypeDeclaration);
            context.RegisterSyntaxNodeAction(AccessorListAction, SyntaxKind.AccessorList);
            context.RegisterSyntaxNodeAction(BlockAction, SyntaxKind.Block);
            context.RegisterSyntaxNodeAction(SwitchStatementAction, SyntaxKind.SwitchStatement);
            context.RegisterSyntaxNodeAction(InitializerExpressionAction, SyntaxKinds.InitializerExpression);
            context.RegisterSyntaxNodeAction(AnonymousObjectCreationExpressionAction, SyntaxKind.AnonymousObjectCreationExpression);
        }

        private static void HandleNamespaceDeclaration(SyntaxNodeAnalysisContext context)
        {
            var syntax = (NamespaceDeclarationSyntax)context.Node;
            CheckBraces(context, syntax.OpenBraceToken, syntax.CloseBraceToken);
        }

        private static void HandleBaseTypeDeclaration(SyntaxNodeAnalysisContext context)
        {
            var syntax = (BaseTypeDeclarationSyntax)context.Node;
            CheckBraces(context, syntax.OpenBraceToken, syntax.CloseBraceToken);
        }

        private static void HandleAccessorList(SyntaxNodeAnalysisContext context)
        {
            var syntax = (AccessorListSyntax)context.Node;
            CheckBraces(context, syntax.OpenBraceToken, syntax.CloseBraceToken);
        }

        private static void HandleBlock(SyntaxNodeAnalysisContext context)
        {
            var syntax = (BlockSyntax)context.Node;
            CheckBraces(context, syntax.OpenBraceToken, syntax.CloseBraceToken);
        }

        private static void HandleSwitchStatement(SyntaxNodeAnalysisContext context)
        {
            var syntax = (SwitchStatementSyntax)context.Node;
            CheckBraces(context, syntax.OpenBraceToken, syntax.CloseBraceToken);
        }

        private static void HandleInitializerExpression(SyntaxNodeAnalysisContext context)
        {
            var syntax = (InitializerExpressionSyntax)context.Node;
            CheckBraces(context, syntax.OpenBraceToken, syntax.CloseBraceToken);
        }

        private static void HandleAnonymousObjectCreationExpression(SyntaxNodeAnalysisContext context)
        {
            var syntax = (AnonymousObjectCreationExpressionSyntax)context.Node;
            CheckBraces(context, syntax.OpenBraceToken, syntax.CloseBraceToken);
        }

        private static void CheckBraces(SyntaxNodeAnalysisContext context, SyntaxToken openBraceToken, SyntaxToken closeBraceToken)
        {
            bool checkCloseBrace = true;

            if (GetStartLine(openBraceToken) == GetStartLine(closeBraceToken))
            {
                if (context.Node.IsKind(SyntaxKind.ArrayInitializerExpression))
                {
                    switch (context.Node.Parent.Kind())
                    {
                    case SyntaxKind.EqualsValueClause:
                        if (GetStartLine(((EqualsValueClauseSyntax)context.Node.Parent).EqualsToken) == GetStartLine(openBraceToken))
                        {
                            return;
                        }

                        break;

                    case SyntaxKind.ArrayCreationExpression:
                        if (GetStartLine(((ArrayCreationExpressionSyntax)context.Node.Parent).NewKeyword) == GetStartLine(openBraceToken))
                        {
                            return;
                        }

                        break;

                    case SyntaxKind.ImplicitArrayCreationExpression:
                        if (GetStartLine(((ImplicitArrayCreationExpressionSyntax)context.Node.Parent).NewKeyword) == GetStartLine(openBraceToken))
                        {
                            return;
                        }

                        break;

                    default:
                        break;
                    }
                }
                else
                {
                    switch (context.Node.Parent.Kind())
                    {
                    case SyntaxKind.GetAccessorDeclaration:
                    case SyntaxKind.SetAccessorDeclaration:
                    case SyntaxKind.AddAccessorDeclaration:
                    case SyntaxKind.RemoveAccessorDeclaration:
                    case SyntaxKind.UnknownAccessorDeclaration:
                        if (GetStartLine(((AccessorDeclarationSyntax)context.Node.Parent).Keyword) == GetStartLine(openBraceToken))
                        {
                            // reported as SA1504, if at all
                            return;
                        }

                        checkCloseBrace = false;
                        break;

                    default:
                        // reported by SA1501 or SA1502
                        return;
                    }
                }
            }

            CheckBraceToken(context, openBraceToken);
            if (checkCloseBrace)
            {
                CheckBraceToken(context, closeBraceToken);
            }
        }

        private static int GetStartLine(SyntaxToken token)
        {
            return token.GetLineSpan().StartLinePosition.Line;
        }

        private static void CheckBraceToken(SyntaxNodeAnalysisContext context, SyntaxToken token)
        {
            if (token.IsMissing)
            {
                return;
            }

            int line = token.GetLineSpan().StartLinePosition.Line;

            SyntaxToken previousToken = token.GetPreviousToken(includeZeroWidth: true);
            if (!previousToken.IsMissing)
            {
                if (previousToken.GetLineSpan().StartLinePosition.Line == line)
                {
                    context.ReportDiagnostic(Diagnostic.Create(Descriptor, token.GetLocation()));

                    // no need to report more than one instance for this token
                    return;
                }
            }

            SyntaxToken nextToken = token.GetNextToken(includeZeroWidth: true);
            if (!nextToken.IsMissing)
            {
                switch (nextToken.Kind())
                {
                case SyntaxKind.CloseParenToken:
                case SyntaxKind.CommaToken:
                case SyntaxKind.SemicolonToken:
                case SyntaxKind.DotToken:
                    // these are allowed to appear on the same line
                    return;

                case SyntaxKind.EndOfFileToken:
                    // last token of this file
                    return;

                default:
                    break;
                }

                if (nextToken.GetLineSpan().StartLinePosition.Line == line)
                {
                    context.ReportDiagnostic(Diagnostic.Create(Descriptor, token.GetLocation()));
                }
            }
        }
    }
}
