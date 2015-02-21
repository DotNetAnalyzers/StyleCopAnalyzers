using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace StyleCop.Analyzers.ReadabilityRules
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// The start of the parameter list for a method or indexer call or declaration does not begin on the same line as
    /// the opening bracket, or on the line after the opening bracket.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs when there are one or more blank lines between the opening bracket and the
    /// start of the parameter list. For example:</para>
    /// <code language="csharp">
    /// public string JoinName(
    ///
    ///     string first, string last)
    /// {
    /// }
    /// </code>
    /// <para>The parameter list must begin on the same line as the opening bracket, or on the next line. For
    /// example:</para>
    /// <code language="csharp">
    /// public string JoinName(string first, string last)
    /// {
    /// }
    ///
    /// public string JoinName(
    ///     string first, string last)
    /// {
    /// }
    /// </code>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SA1114ParameterListMustFollowDeclaration : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1114ParameterListMustFollowDeclaration"/> analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1114";
        private const string Title = "Parameter list must follow declaration";
        private const string MessageFormat = "Parameter list must follow declaration.";
        private const string Category = "StyleCop.CSharp.ReadabilityRules";
        private const string Description = "The start of the parameter list for a method or indexer call or declaration does not begin on the same line as the opening bracket, or on the line after the opening bracket.";
        private const string HelpLink = "http://www.stylecop.com/docs/SA1114.html";

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, AnalyzerConstants.DisabledNoTests, Description, HelpLink);

        private static readonly ImmutableArray<DiagnosticDescriptor> supportedDiagnostics =
            ImmutableArray.Create(Descriptor);

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                return supportedDiagnostics;
            }
        }

        /// <inheritdoc/>
        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(this.HandleMethodDeclaration, SyntaxKind.MethodDeclaration);
        }

        private void HandleMethodDeclaration(SyntaxNodeAnalysisContext context)
        {
            var methodDeclaration = (MethodDeclarationSyntax) context.Node;

            var parameterListSyntax = methodDeclaration.ParameterList;
            var openParenToken = parameterListSyntax.OpenParenToken;
            if (openParenToken.IsMissing ||
                parameterListSyntax.IsMissing ||
                !parameterListSyntax.Parameters.Any())
            {
                return;
            }

            var firstParameter = parameterListSyntax.Parameters[0];

            var firstParameterLineSpan = firstParameter.GetLocation().GetLineSpan();
            if (!firstParameterLineSpan.IsValid)
            {
                return;
            }

            var openParenLineSpan = openParenToken.GetLocation().GetLineSpan();
            if (!openParenLineSpan.IsValid)
            {
                return;
            }

            if (openParenLineSpan.EndLinePosition.Line != firstParameterLineSpan.StartLinePosition.Line &&
                openParenLineSpan.EndLinePosition.Line != (firstParameterLineSpan.StartLinePosition.Line - 1))
            {
                context.ReportDiagnostic(Diagnostic.Create(Descriptor, firstParameter.GetLocation()));
            }
        }
    }
}
