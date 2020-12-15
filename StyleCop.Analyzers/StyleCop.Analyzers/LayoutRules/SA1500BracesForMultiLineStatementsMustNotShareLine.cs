// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.LayoutRules
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
        private const string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1500.md";
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(LayoutResources.SA1500Title), LayoutResources.ResourceManager, typeof(LayoutResources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(LayoutResources.SA1500MessageFormat), LayoutResources.ResourceManager, typeof(LayoutResources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(LayoutResources.SA1500Description), LayoutResources.ResourceManager, typeof(LayoutResources));

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
            if (openBraceToken.IsKind(SyntaxKind.None) || closeBraceToken.IsKind(SyntaxKind.None))
            {
                return;
            }

            bool checkCloseBrace = true;
            int openBraceTokenLine = openBraceToken.GetLine();

            if (openBraceTokenLine == closeBraceToken.GetLine())
            {
                if (context.Node.IsKind(SyntaxKind.ArrayInitializerExpression))
                {
                    switch (context.Node.Parent.Kind())
                    {
                    case SyntaxKind.EqualsValueClause:
                        if (((EqualsValueClauseSyntax)context.Node.Parent).EqualsToken.GetLine() == openBraceTokenLine)
                        {
                            return;
                        }

                        break;

                    case SyntaxKind.ArrayCreationExpression:
                        if (((ArrayCreationExpressionSyntax)context.Node.Parent).NewKeyword.GetLine() == openBraceTokenLine)
                        {
                            return;
                        }

                        break;

                    case SyntaxKind.ImplicitArrayCreationExpression:
                        if (((ImplicitArrayCreationExpressionSyntax)context.Node.Parent).NewKeyword.GetLine() == openBraceTokenLine)
                        {
                            return;
                        }

                        break;

                    case SyntaxKind.StackAllocArrayCreationExpression:
                        if (((StackAllocArrayCreationExpressionSyntax)context.Node.Parent).StackAllocKeyword.GetLine() == openBraceTokenLine)
                        {
                            return;
                        }

                        break;

                    case SyntaxKindEx.ImplicitStackAllocArrayCreationExpression:
                        if (((ImplicitStackAllocArrayCreationExpressionSyntaxWrapper)context.Node.Parent).StackAllocKeyword.GetLine() == openBraceTokenLine)
                        {
                            return;
                        }

                        break;

                    case SyntaxKind.ArrayInitializerExpression:
                        if (!InitializerExpressionSharesLine((InitializerExpressionSyntax)context.Node))
                        {
                            return;
                        }

                        checkCloseBrace = false;
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
                        if (((AccessorDeclarationSyntax)context.Node.Parent).Keyword.GetLine() == openBraceTokenLine)
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

        private static bool InitializerExpressionSharesLine(InitializerExpressionSyntax node)
        {
            var parent = (InitializerExpressionSyntax)node.Parent;
            var index = parent.Expressions.IndexOf(node);

            return (index > 0) && (parent.Expressions[index - 1].GetEndLine() == parent.Expressions[index].GetLine());
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
