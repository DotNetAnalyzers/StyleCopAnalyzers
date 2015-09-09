﻿namespace StyleCop.Analyzers.OrderingRules
{
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Globalization;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.Helpers;

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
    public class SA1210UsingDirectivesMustBeOrderedAlphabeticallyByNamespace : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the
        /// <see cref="SA1210UsingDirectivesMustBeOrderedAlphabeticallyByNamespace"/> analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1210";
        private const string Title = "Using directives must be ordered alphabetically by namespace";
        private const string MessageFormat = "Using directives must be ordered alphabetically by the namespaces.";
        private const string Description = "The using directives within a C# code file are not sorted alphabetically by namespace.";
        private const string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1210.md";

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.OrderingRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

        private static readonly ImmutableArray<DiagnosticDescriptor> SupportedDiagnosticsValue =
            ImmutableArray.Create(Descriptor);

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                return SupportedDiagnosticsValue;
            }
        }

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

            ProcessUsings(context, compilationUnit.Usings);
        }

        private static void HandleNamespaceDeclaration(SyntaxNodeAnalysisContext context)
        {
            var namespaceDeclaration = (NamespaceDeclarationSyntax)context.Node;

            ProcessUsings(context, namespaceDeclaration.Usings);
        }

        private static void ProcessUsings(SyntaxNodeAnalysisContext context, SyntaxList<UsingDirectiveSyntax> usings)
        {
            var usingDirectives = new List<UsingDirectiveSyntax>();
            var systemUsingDirectives = new List<UsingDirectiveSyntax>();

            foreach (var usingDirective in usings)
            {
                if (IsAliasOrStaticUsingDirective(usingDirective))
                {
                    continue;
                }

                if (usingDirective.IsPrecededByPreprocessorDirective())
                {
                    CheckIncorrectlyOrderedUsingsAndReportDiagnostic(context, usingDirectives);
                    CheckIncorrectlyOrderedUsingsAndReportDiagnostic(context, systemUsingDirectives);
                    usingDirectives.Clear();
                    systemUsingDirectives.Clear();
                }

                if (usingDirective.HasNamespaceAliasQualifier() || !usingDirective.IsSystemUsingDirective())
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
                    if (CultureInfo.InvariantCulture.CompareInfo.Compare(previousUsingDirective.Name.ToString(), directive.Name.ToString(), CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreWidth) > 0)
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
