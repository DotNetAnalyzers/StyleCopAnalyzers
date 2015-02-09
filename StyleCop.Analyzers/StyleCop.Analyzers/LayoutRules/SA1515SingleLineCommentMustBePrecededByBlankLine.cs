namespace StyleCop.Analyzers.LayoutRules
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// A single-line comment within C# code is not preceded by a blank line.
    /// </summary>
    /// <remarks>
    /// <para>To improve the readability of the code, StyleCop requires blank lines in certain situations, and prohibits
    /// blank lines in other situations. This results in a consistent visual pattern across the code, which can improve
    /// recognition and readability of unfamiliar code.</para>
    ///
    /// <para>A violation of this rule occurs when a single-line comment is not preceded by a blank line. For
    /// example:</para>
    ///
    /// <code language="csharp">
    /// public bool Enabled
    /// {
    ///     get
    ///     {
    ///         Console.WriteLine("Getting the enabled flag.");
    ///         // Return the value of the 'enabled' field.
    ///         return this.enabled;
    ///     }
    /// }
    /// </code>
    ///
    /// <para>The code above would generate an instance of this violation, since the single-line comment is not preceded
    /// by a blank line.</para>
    ///
    /// <para>An exception to this rule occurs when the single-line comment is the first item within its scope. In this
    /// case, the comment should not be preceded by a blank line. For example:</para>
    ///
    /// <code language="csharp">
    /// public bool Enabled
    /// {
    ///     get
    ///     {
    ///         // Return the value of the 'enabled' field.
    ///         return this.enabled;
    ///     }
    /// }
    /// </code>
    ///
    /// <para>In the code above, the comment is the first item within its scope, and thus it should not be preceded by a
    /// blank line.</para>
    ///
    /// <para>If the comment is being used to comment out a line of code, begin the comment with four forward slashes
    /// rather than two. This will cause StyleCop to ignore this violation. For example:</para>
    ///
    /// <code language="csharp">
    /// public bool Enabled
    /// {
    ///     get
    ///     {
    ///         Console.WriteLine("Getting the enabled flag.");
    ///         ////return false;
    ///         return this.enabled;
    ///     }
    /// }
    /// </code>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SA1515SingleLineCommentMustBePrecededByBlankLine : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "SA1515";
        private const string Title = "Single-line comment must be preceded by blank line";
        private const string MessageFormat = "TODO: Message format";
        private const string Category = "StyleCop.CSharp.LayoutRules";
        private const string Description = "A single-line comment within C# code is not preceded by a blank line.";
        private const string HelpLink = "http://www.stylecop.com/docs/SA1515.html";

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
            // TODO: Implement analysis
        }
    }
}
