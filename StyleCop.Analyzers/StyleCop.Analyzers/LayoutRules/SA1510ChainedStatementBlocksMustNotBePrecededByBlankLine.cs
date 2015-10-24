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
    /// Chained C# statements are separated by a blank line.
    /// </summary>
    /// <remarks>
    /// <para>To improve the readability of the code, StyleCop requires blank lines in certain situations, and prohibits
    /// blank lines in other situations. This results in a consistent visual pattern across the code, which can improve
    /// recognition and readability of unfamiliar code.</para>
    ///
    /// <para>Some types of C# statements can only be used when chained to the bottom of another statement. Examples
    /// include catch and finally statements, which must always be chained to the bottom of a try-statement. Another
    /// example is an else-statement, which must always be chained to the bottom of an if-statement, or to another
    /// else-statement. These types of chained statements must not be separated by a blank line. For example:</para>
    ///
    /// <code language="csharp">
    /// try
    /// {
    ///     this.SomeMethod();
    /// }
    ///
    /// catch (Exception ex)
    /// {
    ///     Console.WriteLine(ex.ToString());
    /// }
    /// </code>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class SA1510ChainedStatementBlocksMustNotBePrecededByBlankLine : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the
        /// <see cref="SA1510ChainedStatementBlocksMustNotBePrecededByBlankLine"/> analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1510";
        private const string Title = "Chained statement blocks must not be preceded by blank line";
        private const string MessageFormat = "'{0}' statement must not be preceded by a blank line";
        private const string Description = "Chained C# statements are separated by a blank line.";
        private const string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1510.md";

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.LayoutRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

        private static readonly Action<CompilationStartAnalysisContext> CompilationStartAction = HandleCompilationStart;
        private static readonly Action<SyntaxNodeAnalysisContext> ElseStatementAction = HandleElseStatement;
        private static readonly Action<SyntaxNodeAnalysisContext> CatchClauseAction = HandleCatchClause;
        private static readonly Action<SyntaxNodeAnalysisContext> FinallyClauseAction = HandleFinallyClause;

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
            context.RegisterSyntaxNodeActionHonorExclusions(ElseStatementAction, SyntaxKind.ElseClause);
            context.RegisterSyntaxNodeActionHonorExclusions(CatchClauseAction, SyntaxKind.CatchClause);
            context.RegisterSyntaxNodeActionHonorExclusions(FinallyClauseAction, SyntaxKind.FinallyClause);
        }

        private static void HandleElseStatement(SyntaxNodeAnalysisContext context)
        {
            var elseClause = (ElseClauseSyntax)context.Node;
            var elseKeyword = elseClause.ElseKeyword;

            if (!elseKeyword.IsPrecededByBlankLines())
            {
                return;
            }

            context.ReportDiagnostic(Diagnostic.Create(Descriptor, elseKeyword.GetLocation(), elseKeyword.ToString()));
        }

        private static void HandleCatchClause(SyntaxNodeAnalysisContext context)
        {
            var catchClause = (CatchClauseSyntax)context.Node;
            var catchKeyword = catchClause.CatchKeyword;

            if (!catchKeyword.IsPrecededByBlankLines())
            {
                return;
            }

            context.ReportDiagnostic(Diagnostic.Create(Descriptor, catchKeyword.GetLocation(), catchKeyword.ToString()));
        }

        private static void HandleFinallyClause(SyntaxNodeAnalysisContext context)
        {
            var finallyClause = (FinallyClauseSyntax)context.Node;
            var finallyKeyword = finallyClause.FinallyKeyword;

            if (!finallyKeyword.IsPrecededByBlankLines())
            {
                return;
            }

            context.ReportDiagnostic(Diagnostic.Create(Descriptor, finallyKeyword.GetLocation(), finallyKeyword.ToString()));
        }
    }
}
