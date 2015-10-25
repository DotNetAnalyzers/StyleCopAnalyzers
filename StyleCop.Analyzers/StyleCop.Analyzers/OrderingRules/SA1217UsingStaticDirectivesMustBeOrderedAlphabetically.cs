// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.OrderingRules
{
    using System;
    using System.Collections.Immutable;
    using System.Globalization;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.Helpers;

    /// <summary>
    /// A static using directive is positioned at the wrong location.
    /// </summary>
    /// <remarks>
    /// <para>
    /// A violation of this rule occurs when a using static directive is placed before a normal or an alias using directive.
    /// Placing the using static directives together below normal and alias using-directives can make the code cleaner and easier to read,
    /// and can help make it easier to identify the static members used throughout the code.
    /// </para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class SA1217UsingStaticDirectivesMustBeOrderedAlphabetically : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1217UsingStaticDirectivesMustBeOrderedAlphabetically"/> analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1217";
        private const string Title = "Using static directives must be ordered alphabetically";
        private const string MessageFormat = "The using static directive for '{0}' must appear after the using static directive for '{1}'";
        private const string Description = "All using static directives must be ordered alphabetically.";
        private const string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1217.md";

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
            var compilationUnit = (CompilationUnitSyntax)context.Node;
            CheckUsingDeclarations(context, compilationUnit.Usings);
        }

        private static void HandleNamespaceDeclaration(SyntaxNodeAnalysisContext context)
        {
            var namespaceDirective = (NamespaceDeclarationSyntax)context.Node;
            CheckUsingDeclarations(context, namespaceDirective.Usings);
        }

        private static void CheckUsingDeclarations(SyntaxNodeAnalysisContext context, SyntaxList<UsingDirectiveSyntax> usingDirectives)
        {
            UsingDirectiveSyntax lastStaticUsingDirective = null;

            foreach (var usingDirective in usingDirectives)
            {
                if (usingDirective.IsPrecededByPreprocessorDirective())
                {
                    lastStaticUsingDirective = null;
                }

                if (usingDirective.StaticKeyword.IsKind(SyntaxKind.StaticKeyword))
                {
                    if (lastStaticUsingDirective != null)
                    {
                        var firstName = lastStaticUsingDirective.Name.ToNormalizedString();
                        var secondName = usingDirective.Name.ToNormalizedString();

                        if (CultureInfo.InvariantCulture.CompareInfo.Compare(firstName, secondName, CompareOptions.IgnoreCase | CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreWidth) > 0)
                        {
                            context.ReportDiagnostic(Diagnostic.Create(Descriptor, lastStaticUsingDirective.GetLocation(), new[] { firstName, secondName }));
                            return;
                        }
                    }

                    lastStaticUsingDirective = usingDirective;
                }
            }
        }
    }
}
