// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

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
        private const string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1211.md";
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(OrderingResources.SA1211Title), OrderingResources.ResourceManager, typeof(OrderingResources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(OrderingResources.SA1211MessageFormat), OrderingResources.ResourceManager, typeof(OrderingResources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(OrderingResources.SA1211Description), OrderingResources.ResourceManager, typeof(OrderingResources));

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.OrderingRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

        private static readonly Action<SyntaxNodeAnalysisContext> CompilationUnitAction = HandleCompilationUnit;
        private static readonly Action<SyntaxNodeAnalysisContext> NamespaceDeclarationAction = HandleNamespaceDeclaration;

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
            ImmutableArray.Create(Descriptor);

        /// <inheritdoc/>
        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            context.RegisterSyntaxNodeAction(CompilationUnitAction, SyntaxKind.CompilationUnit);
            context.RegisterSyntaxNodeAction(NamespaceDeclarationAction, SyntaxKind.NamespaceDeclaration);
        }

        private static void HandleCompilationUnit(SyntaxNodeAnalysisContext context)
        {
            var compilationUnit = (CompilationUnitSyntax)context.Node;
            HandleUsingDirectives(context, compilationUnit.Usings);
        }

        private static void HandleNamespaceDeclaration(SyntaxNodeAnalysisContext context)
        {
            var namespaceDeclaration = (NamespaceDeclarationSyntax)context.Node;
            HandleUsingDirectives(context, namespaceDeclaration.Usings);
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

                        // Using alias directive for '{currentAliasName}' should appear before using alias directive for '{prevAliasName}'
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
