// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.NamingRules
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
    /// The name of a variable in C# does not begin with a lower-case letter.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs when the name of a variable does not begin with a lower-case letter.</para>
    ///
    /// <para>If the variable name is intended to match the name of an item associated with Win32 or COM, and thus needs
    /// to begin with an upper-case letter, place the variable within a special <c>NativeMethods</c> class. A
    /// <c>NativeMethods</c> class is any class which contains a name ending in <c>NativeMethods</c>, and is intended as
    /// a placeholder for Win32 or COM wrappers. StyleCop will ignore this violation if the item is placed within a
    /// <c>NativeMethods</c> class.</para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class SA1312VariableNamesMustBeginWithLowerCaseLetter : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1312VariableNamesMustBeginWithLowerCaseLetter"/> analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1312";
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(NamingResources.SA1312Title), NamingResources.ResourceManager, typeof(NamingResources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(NamingResources.SA1312MessageFormat), NamingResources.ResourceManager, typeof(NamingResources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(NamingResources.SA1312Description), NamingResources.ResourceManager, typeof(NamingResources));
        private const string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1312.md";

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.NamingRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

        private static readonly Action<SyntaxNodeAnalysisContext> VariableDeclarationAction = HandleVariableDeclaration;
        private static readonly Action<SyntaxNodeAnalysisContext> CatchDeclarationAction = HandleCatchDeclaration;
        private static readonly Action<SyntaxNodeAnalysisContext> QueryContinuationAction = HandleQueryContinuation;
        private static readonly Action<SyntaxNodeAnalysisContext> FromClauseAction = HandleFromClause;
        private static readonly Action<SyntaxNodeAnalysisContext> LetClauseAction = HandleLetClause;
        private static readonly Action<SyntaxNodeAnalysisContext> JoinClauseAction = HandleJoinClause;
        private static readonly Action<SyntaxNodeAnalysisContext> JoinIntoClauseAction = HandleJoinIntoClause;
        private static readonly Action<SyntaxNodeAnalysisContext> ForEachStatementAction = HandleForEachStatement;
        private static readonly Action<SyntaxNodeAnalysisContext> SingleVariableDesignationAction = HandleSingleVariableDesignation;

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
            ImmutableArray.Create(Descriptor);

        /// <inheritdoc/>
        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            context.RegisterSyntaxNodeAction(VariableDeclarationAction, SyntaxKind.VariableDeclaration);
            context.RegisterSyntaxNodeAction(CatchDeclarationAction, SyntaxKind.CatchDeclaration);
            context.RegisterSyntaxNodeAction(QueryContinuationAction, SyntaxKind.QueryContinuation);
            context.RegisterSyntaxNodeAction(FromClauseAction, SyntaxKind.FromClause);
            context.RegisterSyntaxNodeAction(LetClauseAction, SyntaxKind.LetClause);
            context.RegisterSyntaxNodeAction(JoinClauseAction, SyntaxKind.JoinClause);
            context.RegisterSyntaxNodeAction(JoinIntoClauseAction, SyntaxKind.JoinIntoClause);
            context.RegisterSyntaxNodeAction(ForEachStatementAction, SyntaxKind.ForEachStatement);
            context.RegisterSyntaxNodeAction(SingleVariableDesignationAction, SyntaxKindEx.SingleVariableDesignation);
        }

        private static void HandleVariableDeclaration(SyntaxNodeAnalysisContext context)
        {
            VariableDeclarationSyntax syntax = (VariableDeclarationSyntax)context.Node;
            if (syntax.Parent.IsKind(SyntaxKind.FieldDeclaration)
                || syntax.Parent.IsKind(SyntaxKind.EventFieldDeclaration))
            {
                // This diagnostic is only for local variables.
                return;
            }

            LocalDeclarationStatementSyntax parentDeclaration = syntax.Parent as LocalDeclarationStatementSyntax;
            if (parentDeclaration?.IsConst ?? false)
            {
                // this diagnostic does not apply to locals constants
                return;
            }

            foreach (VariableDeclaratorSyntax variableDeclarator in syntax.Variables)
            {
                if (variableDeclarator == null)
                {
                    continue;
                }

                var identifier = variableDeclarator.Identifier;
                CheckIdentifier(context, identifier);
            }
        }

        private static void HandleCatchDeclaration(SyntaxNodeAnalysisContext context)
        {
            CheckIdentifier(context, ((CatchDeclarationSyntax)context.Node).Identifier);
        }

        private static void HandleQueryContinuation(SyntaxNodeAnalysisContext context)
        {
            CheckIdentifier(context, ((QueryContinuationSyntax)context.Node).Identifier);
        }

        private static void HandleFromClause(SyntaxNodeAnalysisContext context)
        {
            CheckIdentifier(context, ((FromClauseSyntax)context.Node).Identifier);
        }

        private static void HandleLetClause(SyntaxNodeAnalysisContext context)
        {
            CheckIdentifier(context, ((LetClauseSyntax)context.Node).Identifier);
        }

        private static void HandleJoinClause(SyntaxNodeAnalysisContext context)
        {
            CheckIdentifier(context, ((JoinClauseSyntax)context.Node).Identifier);
        }

        private static void HandleJoinIntoClause(SyntaxNodeAnalysisContext context)
        {
            CheckIdentifier(context, ((JoinIntoClauseSyntax)context.Node).Identifier);
        }

        private static void HandleForEachStatement(SyntaxNodeAnalysisContext context)
        {
            CheckIdentifier(context, ((ForEachStatementSyntax)context.Node).Identifier);
        }

        private static void HandleSingleVariableDesignation(SyntaxNodeAnalysisContext context)
        {
            CheckIdentifier(context, ((SingleVariableDesignationSyntaxWrapper)context.Node).Identifier);
        }

        private static void CheckIdentifier(SyntaxNodeAnalysisContext context, SyntaxToken identifier)
        {
            if (identifier.IsMissing)
            {
                return;
            }

            string name = identifier.ValueText;
            if (string.IsNullOrEmpty(name) || char.IsLower(name[0]))
            {
                return;
            }

            if (NamedTypeHelpers.IsContainedInNativeMethodsClass(identifier.Parent))
            {
                return;
            }

            // Variable names should begin with lower-case letter
            context.ReportDiagnostic(Diagnostic.Create(Descriptor, identifier.GetLocation(), name));
        }
    }
}
