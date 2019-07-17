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
    using StyleCop.Analyzers.Settings.ObjectModel;

    /// <summary>
    /// The opening and closing braces for a multi-line C# statement have been omitted.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs when the opening and closing braces for a multi-line statement have been
    /// omitted. In C#, some types of statements may optionally include braces. Examples include <c>if</c>,
    /// <c>while</c>, and <c>for</c> statements. For example, an if-statement may be written without braces:</para>
    ///
    /// <code language="csharp">
    /// if (true)
    ///     return
    ///         this.value;
    /// </code>
    ///
    /// <para>Although this is legal in C#, StyleCop requires the braces to be present when the statement spans multiple
    /// lines, to increase the readability and maintainability of the code.</para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class SA1519BracesMustNotBeOmittedFromMultiLineChildStatement : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1519BracesMustNotBeOmittedFromMultiLineChildStatement"/> analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1519";

        /// <summary>
        /// The diagnostic descriptor for the <see cref="SA1519BracesMustNotBeOmittedFromMultiLineChildStatement"/> analyzer.
        /// </summary>
        public static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.LayoutRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(LayoutResources.SA1519Title), LayoutResources.ResourceManager, typeof(LayoutResources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(LayoutResources.SA1519MessageFormat), LayoutResources.ResourceManager, typeof(LayoutResources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(LayoutResources.SA1519Description), LayoutResources.ResourceManager, typeof(LayoutResources));
        private const string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1519.md";

        private static readonly Action<SyntaxNodeAnalysisContext> IfStatementAction = HandleIfStatement;
        private static readonly Action<SyntaxNodeAnalysisContext> DoStatementAction = HandleDoStatement;
        private static readonly Action<SyntaxNodeAnalysisContext> WhileStatementAction = HandleWhileStatement;
        private static readonly Action<SyntaxNodeAnalysisContext> ForStatementAction = HandleForStatement;
        private static readonly Action<SyntaxNodeAnalysisContext> ForEachStatementAction = HandleForEachStatement;
        private static readonly Action<SyntaxNodeAnalysisContext, StyleCopSettings> UsingStatementAction = HandleUsingStatement;

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
            ImmutableArray.Create(Descriptor);

        /// <inheritdoc/>
        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            context.RegisterSyntaxNodeAction(IfStatementAction, SyntaxKind.IfStatement);
            context.RegisterSyntaxNodeAction(DoStatementAction, SyntaxKind.DoStatement);
            context.RegisterSyntaxNodeAction(WhileStatementAction, SyntaxKind.WhileStatement);
            context.RegisterSyntaxNodeAction(ForStatementAction, SyntaxKind.ForStatement);
            context.RegisterSyntaxNodeAction(ForEachStatementAction, SyntaxKind.ForEachStatement);
            context.RegisterSyntaxNodeAction(UsingStatementAction, SyntaxKind.UsingStatement);
        }

        private static void HandleIfStatement(SyntaxNodeAnalysisContext context)
        {
            var ifStatement = (IfStatementSyntax)context.Node;

            CheckChildStatement(context, ifStatement.Statement);

            if (ifStatement.Else != null)
            {
                // an 'else' directly followed by an 'if' should not trigger this diagnostic.
                if (!ifStatement.Else.Statement.IsKind(SyntaxKind.IfStatement))
                {
                    CheckChildStatement(context, ifStatement.Else.Statement);
                }
            }
        }

        private static void HandleDoStatement(SyntaxNodeAnalysisContext context)
        {
            var doStatement = (DoStatementSyntax)context.Node;
            CheckChildStatement(context, doStatement.Statement);
        }

        private static void HandleWhileStatement(SyntaxNodeAnalysisContext context)
        {
            var whileStatement = (WhileStatementSyntax)context.Node;
            CheckChildStatement(context, whileStatement.Statement);
        }

        private static void HandleForStatement(SyntaxNodeAnalysisContext context)
        {
            var forStatement = (ForStatementSyntax)context.Node;
            CheckChildStatement(context, forStatement.Statement);
        }

        private static void HandleForEachStatement(SyntaxNodeAnalysisContext context)
        {
            var forEachStatement = (ForEachStatementSyntax)context.Node;
            CheckChildStatement(context, forEachStatement.Statement);
        }

        private static void HandleUsingStatement(SyntaxNodeAnalysisContext context, StyleCopSettings settings)
        {
            var usingStatement = (UsingStatementSyntax)context.Node;

            if (settings.LayoutRules.AllowConsecutiveUsings && (usingStatement.Statement is UsingStatementSyntax))
            {
                return;
            }

            CheckChildStatement(context, usingStatement.Statement);
        }

        private static void CheckChildStatement(SyntaxNodeAnalysisContext context, StatementSyntax childStatement)
        {
            if (childStatement is BlockSyntax)
            {
                return;
            }

            FileLinePositionSpan lineSpan = childStatement.GetLineSpan();
            if (lineSpan.StartLinePosition.Line == lineSpan.EndLinePosition.Line)
            {
                return;
            }

            context.ReportDiagnostic(Diagnostic.Create(Descriptor, childStatement.GetLocation()));
        }
    }
}
