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
    using StyleCop.Analyzers.Settings.ObjectModel;

    /// <summary>
    /// The using directives within a C# code file are not sorted alphabetically by namespace.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs when the using directives are not sorted alphabetically by namespace.
    /// Sorting the using directives alphabetically makes the code cleaner and easier to read, and can help make it
    /// easier to identify the namespaces that are being used by the code. The <see cref="System"/> namespaces are an
    /// exception to this rule and will always precede all other namespaces. See
    /// <see cref="SA1208SystemUsingDirectivesMustBePlacedBeforeOtherUsingDirectives"/> for more details.</para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class SA1210UsingDirectivesMustBeOrderedAlphabeticallyByNamespace : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the
        /// <see cref="SA1210UsingDirectivesMustBeOrderedAlphabeticallyByNamespace"/> analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1210";
        private const string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1210.md";
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(OrderingResources.SA1210Title), OrderingResources.ResourceManager, typeof(OrderingResources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(OrderingResources.SA1210MessageFormat), OrderingResources.ResourceManager, typeof(OrderingResources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(OrderingResources.SA1210Description), OrderingResources.ResourceManager, typeof(OrderingResources));

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.OrderingRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

        private static readonly Action<SyntaxNodeAnalysisContext, StyleCopSettings> CompilationUnitAction = HandleCompilationUnit;
        private static readonly Action<SyntaxNodeAnalysisContext, StyleCopSettings> NamespaceDeclarationAction = HandleNamespaceDeclaration;

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

        private static void HandleCompilationUnit(SyntaxNodeAnalysisContext context, StyleCopSettings settings)
        {
            var compilationUnit = (CompilationUnitSyntax)context.Node;

            ProcessUsings(context, settings.OrderingRules, compilationUnit.Usings);
        }

        private static void HandleNamespaceDeclaration(SyntaxNodeAnalysisContext context, StyleCopSettings settings)
        {
            var namespaceDeclaration = (NamespaceDeclarationSyntax)context.Node;

            ProcessUsings(context, settings.OrderingRules, namespaceDeclaration.Usings);
        }

        private static void ProcessUsings(SyntaxNodeAnalysisContext context, OrderingSettings orderingSettings, SyntaxList<UsingDirectiveSyntax> usings)
        {
            var usingDirectives = new List<UsingDirectiveSyntax>();
            var systemUsingDirectives = new List<UsingDirectiveSyntax>();

            foreach (var usingDirective in usings)
            {
                if (usingDirective.IsPrecededByPreprocessorDirective())
                {
                    CheckIncorrectlyOrderedUsingsAndReportDiagnostic(context, usingDirectives);
                    CheckIncorrectlyOrderedUsingsAndReportDiagnostic(context, systemUsingDirectives);
                    usingDirectives.Clear();
                    systemUsingDirectives.Clear();
                }

                if (IsAliasOrStaticUsingDirective(usingDirective))
                {
                    continue;
                }

                if (usingDirective.HasNamespaceAliasQualifier()
                    || !usingDirective.IsSystemUsingDirective()
                    || !orderingSettings.SystemUsingDirectivesFirst)
                {
                    usingDirectives.Add(usingDirective);
                }
                else
                {
                    systemUsingDirectives.Add(usingDirective);
                }
            }

            CheckIncorrectlyOrderedUsingsAndReportDiagnostic(context, usingDirectives);
            CheckIncorrectlyOrderedUsingsAndReportDiagnostic(context, systemUsingDirectives);
        }

        private static void CheckIncorrectlyOrderedUsingsAndReportDiagnostic(SyntaxNodeAnalysisContext context, IEnumerable<UsingDirectiveSyntax> usings)
        {
            UsingDirectiveSyntax previousUsingDirective = null;

            foreach (var directive in usings)
            {
                if (previousUsingDirective != null)
                {
                    if (NameSyntaxHelpers.Compare(previousUsingDirective.Name, directive.Name) > 0)
                    {
                        context.ReportDiagnostic(Diagnostic.Create(Descriptor, previousUsingDirective.GetLocation()));
                    }
                }

                previousUsingDirective = directive;
            }
        }

        private static bool IsAliasOrStaticUsingDirective(UsingDirectiveSyntax usingDirective) => usingDirective.Alias != null || usingDirective.StaticKeyword.IsKind(SyntaxKind.StaticKeyword);
    }
}
