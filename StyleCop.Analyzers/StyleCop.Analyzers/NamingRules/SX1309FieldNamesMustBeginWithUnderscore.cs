﻿// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
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

    /// <summary>
    /// A field name in C# does not begin with an underscore.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs when a field name does not begin with an underscore.</para>
    ///
    /// <para>This rule is an alternative to rule <see cref="SA1309FieldNamesMustNotBeginWithUnderscore"/> for
    /// development teams who prefer to prefix fields with an underscore.</para>
    ///
    /// <para>If the field or variable name is intended to match the name of an item associated with Win32 or COM, and
    /// thus needs to not begin with an underscore, place the field or variable within a special <c>NativeMethods</c>
    /// class. A <c>NativeMethods</c> class is any class which contains a name ending in <c>NativeMethods</c>, and is
    /// intended as a placeholder for Win32 or COM wrappers. StyleCop will ignore this violation if the item is placed
    /// within a <c>NativeMethods</c> class.</para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class SX1309FieldNamesMustBeginWithUnderscore : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SX1309FieldNamesMustBeginWithUnderscore"/> analyzer.
        /// </summary>
        public const string DiagnosticId = "SX1309";
        private const string Title = "Field names must begin with underscore";
        private const string MessageFormat = "Field '{0}' must begin with an underscore";
        private const string Description = "A field name in C# does not begin with an underscore.";
        private const string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SX1309.md";

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.NamingRules, DiagnosticSeverity.Warning, AnalyzerConstants.DisabledAlternative, Description, HelpLink);

        private static readonly Action<SyntaxNodeAnalysisContext> FieldDeclarationAction = HandleFieldDeclaration;

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
            ImmutableArray.Create(Descriptor);

        /// <inheritdoc/>
        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            context.RegisterSyntaxNodeAction(FieldDeclarationAction, SyntaxKind.FieldDeclaration);
        }

        private static void HandleFieldDeclaration(SyntaxNodeAnalysisContext context)
        {
            FieldDeclarationSyntax syntax = (FieldDeclarationSyntax)context.Node;
            foreach (SyntaxToken token in syntax.Modifiers)
            {
                switch (token.Kind())
                {
                case SyntaxKind.StaticKeyword:
                case SyntaxKind.ConstKeyword:
                    // This analyzer only looks at instance fields.
                    return;

                case SyntaxKind.InternalKeyword:
                case SyntaxKind.ProtectedKeyword:
                case SyntaxKind.PublicKeyword:
                    // This analyzer only looks at private fields.
                    return;

                default:
                    break;
                }
            }

            if (NamedTypeHelpers.IsContainedInNativeMethodsClass(syntax))
            {
                return;
            }

            var variables = syntax.Declaration?.Variables;
            if (variables == null)
            {
                return;
            }

            foreach (VariableDeclaratorSyntax variableDeclarator in variables.Value)
            {
                if (variableDeclarator == null)
                {
                    continue;
                }

                var identifier = variableDeclarator.Identifier;
                if (identifier.IsMissing)
                {
                    continue;
                }

                if (identifier.ValueText.StartsWith("_", StringComparison.Ordinal))
                {
                    continue;
                }

                // Field '{name}' must begin with an underscore
                string name = identifier.ValueText;
                context.ReportDiagnostic(Diagnostic.Create(Descriptor, identifier.GetLocation(), name));
            }
        }
    }
}
