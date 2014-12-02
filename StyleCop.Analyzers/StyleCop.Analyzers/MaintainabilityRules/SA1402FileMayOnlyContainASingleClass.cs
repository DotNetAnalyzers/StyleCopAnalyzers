namespace StyleCop.Analyzers.MaintainabilityRules
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// A C# code file contains more than one unique class.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs when a C# file contains more than one class. To increase long-term
    /// maintainability of the code-base, each class should be placed in its own file, and file names should reflect the
    /// name of the class within the file.</para>
    ///
    /// <para>It is possible to place other supporting elements within the same file as the class, such as delegates,
    /// enums, etc., if they are related to the class.</para>
    ///
    /// <para>It is also possible to place multiple parts of the same partial class within the same file.</para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SA1402FileMayOnlyContainASingleClass : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "SA1402";
        internal const string Title = "A C# code file contains more than one unique class.";
        internal const string MessageFormat = "TODO: Message format";
        internal const string Category = "StyleCop.CSharp.MaintainabilityRules";
        internal const string Description = "A C# code file contains more than one unique class.";
        internal const string HelpLink = "http://www.stylecop.com/docs/SA1402.html";

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
