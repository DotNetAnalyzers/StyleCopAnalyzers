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
    using StyleCop.Analyzers.SpacingRules;

    /// <summary>
    /// An opening curly bracket within a C# element, statement, or expression is followed by a blank line.
    /// </summary>
    /// <remarks>
    /// <para>To improve the readability of the code, StyleCop requires blank lines in certain situations, and prohibits
    /// blank lines in other situations. This results in a consistent visual pattern across the code, which can improve
    /// recognition and readability of unfamiliar code.</para>
    ///
    /// <para>A violation of this rule occurs when an opening curly bracket is followed by a blank line. For
    /// example:</para>
    ///
    /// <code>
    /// public bool Enabled
    /// {
    ///
    ///     get
    ///     {
    ///
    ///         return this.enabled;
    ///     }
    /// }
    /// </code>
    ///
    /// <para>The code above would generate two instances of this violation, since there are two places where opening
    /// curly brackets are followed by blank lines.</para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class SA1505OpeningCurlyBracketsMustNotBeFollowedByBlankLine : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1505OpeningCurlyBracketsMustNotBeFollowedByBlankLine"/>
        /// analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1505";
        private const string Title = "Opening curly brackets must not be followed by blank line";
        private const string MessageFormat = "An opening curly bracket must not be followed by a blank line.";
        private const string Description = "An opening curly bracket within a C# element, statement, or expression is followed by a blank line.";
        private const string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1505.md";

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.LayoutRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

        private static readonly ImmutableArray<SyntaxKind> BaseTypeDeclarationKinds =
            ImmutableArray.Create(SyntaxKind.ClassDeclaration, SyntaxKind.StructDeclaration, SyntaxKind.InterfaceDeclaration, SyntaxKind.EnumDeclaration);

        private static readonly ImmutableArray<SyntaxKind> InitializerExpressionKinds =
            ImmutableArray.Create(SyntaxKind.ObjectInitializerExpression, SyntaxKind.CollectionInitializerExpression, SyntaxKind.ArrayInitializerExpression, SyntaxKind.ComplexElementInitializerExpression);

        private static readonly Action<CompilationStartAnalysisContext> CompilationStartAction = HandleCompilationStart;
        private static readonly Action<SyntaxNodeAnalysisContext> BlockAction = HandleBlock;
        private static readonly Action<SyntaxNodeAnalysisContext> InitializerExpressionAction = HandleInitializerExpression;
        private static readonly Action<SyntaxNodeAnalysisContext> AnonymousObjectCreationExpressionAction = HandleAnonymousObjectCreationExpression;
        private static readonly Action<SyntaxNodeAnalysisContext> SwitchStatementAction = HandleSwitchStatement;
        private static readonly Action<SyntaxNodeAnalysisContext> NamespaceDeclarationAction = HandleNamespaceDeclaration;
        private static readonly Action<SyntaxNodeAnalysisContext> BaseTypeDeclarationAction = HandleBaseTypeDeclaration;
        private static readonly Action<SyntaxNodeAnalysisContext> AccessorListAction = HandleAccessorList;

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
            context.RegisterSyntaxNodeActionHonorExclusions(BlockAction, SyntaxKind.Block);
            context.RegisterSyntaxNodeActionHonorExclusions(InitializerExpressionAction, InitializerExpressionKinds);
            context.RegisterSyntaxNodeActionHonorExclusions(AnonymousObjectCreationExpressionAction, SyntaxKind.AnonymousObjectCreationExpression);
            context.RegisterSyntaxNodeActionHonorExclusions(SwitchStatementAction, SyntaxKind.SwitchStatement);
            context.RegisterSyntaxNodeActionHonorExclusions(NamespaceDeclarationAction, SyntaxKind.NamespaceDeclaration);
            context.RegisterSyntaxNodeActionHonorExclusions(BaseTypeDeclarationAction, BaseTypeDeclarationKinds);
            context.RegisterSyntaxNodeActionHonorExclusions(AccessorListAction, SyntaxKind.AccessorList);
        }

        private static void HandleBlock(SyntaxNodeAnalysisContext context)
        {
            var block = (BlockSyntax)context.Node;
            AnalyzeOpenBrace(context, block.OpenBraceToken);
        }

        private static void HandleInitializerExpression(SyntaxNodeAnalysisContext context)
        {
            var expression = (InitializerExpressionSyntax)context.Node;
            AnalyzeOpenBrace(context, expression.OpenBraceToken);
        }

        private static void HandleAnonymousObjectCreationExpression(SyntaxNodeAnalysisContext context)
        {
            var expression = (AnonymousObjectCreationExpressionSyntax)context.Node;
            AnalyzeOpenBrace(context, expression.OpenBraceToken);
        }

        private static void HandleSwitchStatement(SyntaxNodeAnalysisContext context)
        {
            var switchStatement = (SwitchStatementSyntax)context.Node;
            AnalyzeOpenBrace(context, switchStatement.OpenBraceToken);
        }

        private static void HandleNamespaceDeclaration(SyntaxNodeAnalysisContext context)
        {
            var namespaceDeclaration = (NamespaceDeclarationSyntax)context.Node;
            AnalyzeOpenBrace(context, namespaceDeclaration.OpenBraceToken);
        }

        private static void HandleBaseTypeDeclaration(SyntaxNodeAnalysisContext context)
        {
            var typeDeclaration = (BaseTypeDeclarationSyntax)context.Node;
            AnalyzeOpenBrace(context, typeDeclaration.OpenBraceToken);
        }

        private static void HandleAccessorList(SyntaxNodeAnalysisContext context)
        {
            var accessorList = (AccessorListSyntax)context.Node;
            AnalyzeOpenBrace(context, accessorList.OpenBraceToken);
        }

        private static void AnalyzeOpenBrace(SyntaxNodeAnalysisContext context, SyntaxToken openBraceToken)
        {
            var nextToken = openBraceToken.GetNextToken();
            if (nextToken.IsMissingOrDefault())
            {
                return;
            }

            if ((GetLine(nextToken) - GetLine(openBraceToken)) < 2)
            {
                // there will be no blank lines when the opening brace and the following token are not at least two lines apart.
                return;
            }

            var separatingTrivia = TriviaHelper.MergeTriviaLists(openBraceToken.TrailingTrivia, nextToken.LeadingTrivia);

            // skip everything until the first end of line (as this is considered part of the line of the opening brace)
            var startIndex = 0;
            while ((startIndex < separatingTrivia.Count) && !separatingTrivia[startIndex].IsKind(SyntaxKind.EndOfLineTrivia))
            {
                startIndex++;
            }

            startIndex = (startIndex == separatingTrivia.Count) ? 0 : startIndex + 1;

            var done = false;
            var eolCount = 0;
            for (var i = startIndex; !done && (i < separatingTrivia.Count); i++)
            {
                switch (separatingTrivia[i].Kind())
                {
                case SyntaxKind.WhitespaceTrivia:
                    break;
                case SyntaxKind.EndOfLineTrivia:
                    eolCount++;
                    break;
                default:
                    done = true;
                    break;
                }
            }

            if (eolCount > 0)
            {
                context.ReportDiagnostic(Diagnostic.Create(Descriptor, openBraceToken.GetLocation()));
            }
        }

        private static int GetLine(SyntaxToken token)
        {
            return token.GetLineSpan().StartLinePosition.Line;
        }
    }
}
