namespace StyleCop.Analyzers.DocumentationRules
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// The file header at the top of a C# code file does not contain a filled-in summary tag.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs when the file header at the top of a C# file does not contain a valid
    /// <c>summary</c> tag. This rule is disabled by default.</para>
    ///
    /// <para>For example:</para>
    ///
    /// <code language="csharp">
    /// //-----------------------------------------------------------------------
    /// // &lt;copyright file="Widget.cs" company="My Company"&gt;
    /// //     Custom company copyright tag.
    /// // &lt;/copyright&gt;
    /// //-----------------------------------------------------------------------
    /// </code>
    ///
    /// <para>If this rule is enabled, the file header should contain a summary tag. For example:</para>
    ///
    /// <code>
    /// //-----------------------------------------------------------------------
    /// // &lt;copyright file="Widget.cs" company="My Company"&gt;
    /// //     Custom company copyright tag.
    /// // &lt;/copyright&gt;
    /// // &lt;summary&gt;This is the Widget class.&lt;/summary&gt;
    /// //-----------------------------------------------------------------------
    /// </code>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SA1639FileHeaderMustHaveSummary : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1639FileHeaderMustHaveSummary"/> analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1639";
        private const string Title = "File header must have summary";
        private const string MessageFormat = "TODO: Message format";
        private const string Category = "StyleCop.CSharp.DocumentationRules";
        private const string Description = "The file header at the top of a C# code file does not contain a filled-in summary tag.";
        private const string HelpLink = "http://www.stylecop.com/docs/SA1639.html";

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, AnalyzerConstants.DisabledNoTests, Description, HelpLink);

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
            // TODO: Implement analysis
        }
    }
}
