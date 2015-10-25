// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.OrderingRules
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.Helpers;

    /// <summary>
    /// The using-alias directives within a C# code file are not sorted alphabetically by alias name.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs when the using-alias directives are not sorted alphabetically by alias
    /// name. Sorting the using-alias directives alphabetically can make the code cleaner and easier to read, and can
    /// help make it easier to identify the namespaces that are being used by the code.</para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class SA1211UsingAliasDirectivesMustBeOrderedAlphabeticallyByAliasName : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the
        /// <see cref="SA1211UsingAliasDirectivesMustBeOrderedAlphabeticallyByAliasName"/> analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1211";
        private const string Title = "Using alias directives must be ordered alphabetically by alias name";
        private const string MessageFormat = "Using alias directive for '{0}' must appear before using alias directive for '{1}'";
        private const string Description = "The using-alias directives within a C# code file are not sorted alphabetically by alias name.";
        private const string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1211.md";

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.OrderingRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

        private static readonly Action<CompilationStartAnalysisContext> CompilationStartAction = HandleCompilationStart;
        private static readonly Action<SyntaxNodeAnalysisContext> CompilationUnitAction = HandleCompilationUnit;
        private static readonly Action<SyntaxNodeAnalysisContext> NamespaceDeclarationAction = HandleNamespaceDeclaration;

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
            context.RegisterSyntaxNodeActionHonorExclusions(CompilationUnitAction, SyntaxKind.CompilationUnit);
            context.RegisterSyntaxNodeActionHonorExclusions(NamespaceDeclarationAction, SyntaxKind.NamespaceDeclaration);
        }

        private static void HandleCompilationUnit(SyntaxNodeAnalysisContext context)
        {
            var compilationUnit = context.Node as CompilationUnitSyntax;
            if (compilationUnit != null)
            {
                HandleUsingDirectives(context, compilationUnit.Usings);
            }
        }

        private static void HandleNamespaceDeclaration(SyntaxNodeAnalysisContext context)
        {
            var namespaceDeclaration = context.Node as NamespaceDeclarationSyntax;
            if (namespaceDeclaration != null)
            {
                HandleUsingDirectives(context, namespaceDeclaration.Usings);
            }
        }

        private static void HandleUsingDirectives(SyntaxNodeAnalysisContext context, SyntaxList<UsingDirectiveSyntax> usingDirectives)
        {
            if (usingDirectives.Count == 0)
            {
                return;
            }

            var usingAliasNames = new List<string>();
            UsingDirectiveSyntax prevAliasUsingDirective = null;

            foreach (var usingDirective in usingDirectives)
            {
                if (usingDirective.IsPrecededByPreprocessorDirective())
                {
                    usingAliasNames.Clear();
                    prevAliasUsingDirective = null;
                }

                // only interested in using alias directives
                if (usingDirective.Alias?.Name?.IsMissing != false)
                {
                    continue;
                }

                string currentAliasName = usingDirective.Alias.Name.Identifier.ValueText;
                if (prevAliasUsingDirective != null)
                {
                    string currentLowerInvariant = currentAliasName.ToLowerInvariant();
                    string prevAliasName = prevAliasUsingDirective.Alias.Name.Identifier.ValueText;
                    if (string.CompareOrdinal(prevAliasName.ToLowerInvariant(), currentLowerInvariant) > 0)
                    {
                        // Find alias before which should be placed current alias
                        foreach (string aliasName in usingAliasNames)
                        {
                            if (string.CompareOrdinal(aliasName.ToLowerInvariant(), currentLowerInvariant) > 0)
                            {
                                prevAliasName = aliasName;
                                break;
                            }
                        }

                        // Using alias directive for '{currentAliasName}' must appear before using alias directive for '{prevAliasName}'
                        context.ReportDiagnostic(Diagnostic.Create(Descriptor, usingDirective.GetLocation(), currentAliasName, prevAliasName));
                        return;
                    }
                }

                usingAliasNames.Add(currentAliasName);
                prevAliasUsingDirective = usingDirective;
            }
        }
    }
}
