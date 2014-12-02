namespace StyleCop.Analyzers.ReadabilityRules
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// The closing parenthesis or bracket in a call to a C# method or indexer, or the declaration of a method or
    /// indexer, is not placed on the same line as the opening bracket when the element does not take any parameters.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs when a method or indexer does not take any parameters and the closing
    /// bracket of a call or declaration for the method or indexer is not placed on the same line as the opening
    /// bracket. The following example shows correct placement of the closing parenthesis:</para>
    /// <code language="csharp">
    /// public string GetName()
    /// {
    ///     return this.name.Trim();
    /// }
    /// </code>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SA1112ClosingParenthesisMustBeOnLineOfOpeningParenthesis : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "SA1112";
        internal const string Title = "The closing parenthesis or bracket in a call to a C# method or indexer, or the declaration of a method or indexer, is not placed on the same line as the opening bracket when the element does not take any parameters.";
        internal const string MessageFormat = "TODO: Message format";
        internal const string Category = "StyleCop.CSharp.ReadabilityRules";
        internal const string Description = "The closing parenthesis or bracket in a call to a C# method or indexer, or the declaration of a method or indexer, is not placed on the same line as the opening bracket when the element does not take any parameters.";
        internal const string HelpLink = "http://www.stylecop.com/docs/SA1112.html";

        public static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, AnalyzerConstants.DisabledNoTests, Description, HelpLink);

        private static readonly ImmutableArray<DiagnosticDescriptor> _supportedDiagnostics =
            ImmutableArray.Create(Descriptor);

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                return _supportedDiagnostics;
            }
        }

        /// <inheritdoc/>
        public override void Initialize(AnalysisContext context)
        {
            // TODO: Implement analysis
        }
    }
}
