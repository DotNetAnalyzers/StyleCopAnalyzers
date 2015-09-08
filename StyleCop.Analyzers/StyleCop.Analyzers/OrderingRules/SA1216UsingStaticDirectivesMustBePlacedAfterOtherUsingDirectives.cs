﻿namespace StyleCop.Analyzers.OrderingRules
{
    using System.Collections.Immutable;
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
    public class SA1216UsingStaticDirectivesMustBePlacedAfterOtherUsingDirectives : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1216UsingStaticDirectivesMustBePlacedAfterOtherUsingDirectives"/> analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1216";
        private const string Title = "Using static directives must be placed after other using directives";
        private const string MessageFormat = "Using static directives must be placed after other using directives";
        private const string Description = "A using static directive is positioned before a regular or alias using directive.";
        private const string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1216.md";

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.OrderingRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

        private static readonly ImmutableArray<DiagnosticDescriptor> SupportedDiagnosticsValue =
            ImmutableArray.Create(Descriptor);

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => SupportedDiagnosticsValue;

        /// <inheritdoc/>
        public override void Initialize(AnalysisContext context)
        {
            context.RegisterCompilationStartAction(HandleCompilationStart);
        }

        private static void HandleCompilationStart(CompilationStartAnalysisContext context)
        {
            context.RegisterSyntaxNodeActionHonorExclusions(HandleCompilationUnit, SyntaxKind.CompilationUnit);
            context.RegisterSyntaxNodeActionHonorExclusions(HandleNamespaceDeclaration, SyntaxKind.NamespaceDeclaration);
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
                if (usingDirective.StaticKeyword.IsKind(SyntaxKind.StaticKeyword))
                {
                    lastStaticUsingDirective = usingDirective;
                }
                else if (lastStaticUsingDirective != null && !usingDirective.IsPrecededByPreprocessorDirective())
                {
                    // only report a single diagnostic for the last static using directive that is followed by a non-static using directive
                    context.ReportDiagnostic(Diagnostic.Create(Descriptor, lastStaticUsingDirective.GetLocation()));
                    break;
                }
            }
        }
    }
}
