namespace StyleCop.Analyzers.OrderingRules
{
    using System;
    using System.Collections.Immutable;
    using System.Linq;
    using System.Threading;

    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;

    using StyleCop.Analyzers.Helpers;
    using System.Collections.Generic;


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
        private const string MessageFormat = "Using directive for '{0}' must appear before directive for '{1}'";
        private const string Category = "StyleCop.CSharp.OrderingRules";
        private const string Description = "The using directives within a C# code file are not sorted alphabetically by namespace.";
        private const string HelpLink = "http://www.stylecop.com/docs/SA1210.html";

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

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
            var usingsDirectivesWithoutAliasAndStatic = usings.Where(ExcludeAliasAndStaticUsingDirectives);
            var systemUsingDirectives = usingsDirectivesWithoutAliasAndStatic.Where(directive => directive.IsSystemUsingDirective());
            var usingDirectives = usingsDirectivesWithoutAliasAndStatic.Where(directive => !directive.IsSystemUsingDirective());

            CheckOrderAndReportDiagnostic(context, usingDirectives.ToList());
            CheckOrderAndReportDiagnostic(context, systemUsingDirectives.ToList());
        }

        private static void CheckOrderAndReportDiagnostic(SyntaxNodeAnalysisContext context, IList<UsingDirectiveSyntax> usings)
        {
            if (usings.Count <= 1)
            {
                return;
            }

            var properPositionsOfUsingDirectives = usings.OrderBy(directive => directive.Name.ToUnaliasedString()).Select((directive, idx) => new { Directive = directive, Index = idx }).ToDictionary(item => item.Directive, item => item.Index);
            var usingsPositions = new List<int>(usings.Count - 1);

            for (int i = 1; i < usings.Count; i++)
            {
                usingsPositions.Add(properPositionsOfUsingDirectives[usings[i - 1]]);
                var currentUsingDirectiveProperPosition = properPositionsOfUsingDirectives[usings[i]];

                var isAnyUsingDirectiveNameBeforeWrongOrdered = usingsPositions.Any(position => position > currentUsingDirectiveProperPosition);
                if (isAnyUsingDirectiveNameBeforeWrongOrdered)
                {
                    var positionOfUsingThatShouldBeBefore = usingsPositions.Where(value => value > currentUsingDirectiveProperPosition).Min();
                    var indexOfUsingThatShouldBeBefore = usingsPositions.IndexOf(positionOfUsingThatShouldBeBefore);

                    context.ReportDiagnostic(Diagnostic.Create(Descriptor, usings[i].GetLocation(), usings[i].Name.ToUnaliasedString(), usings[indexOfUsingThatShouldBeBefore].Name.ToUnaliasedString()));
                }
            }
        }

        private static bool ExcludeAliasAndStaticUsingDirectives(UsingDirectiveSyntax usingDirective) => usingDirective.Alias == null && usingDirective.StaticKeyword.IsKind(SyntaxKind.None);
    }
}
