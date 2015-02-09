namespace StyleCop.Analyzers.MaintainabilityRules
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// A C# file contains code which is unnecessary and can be removed without changing the overall logic of the code.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs when the file contains code which can be removed without changing the
    /// overall logic of the code.</para>
    ///
    /// <para>For example, the following try-catch statement could be removed completely since the try and catch blocks
    /// are both empty.</para>
    ///
    /// <code language="csharp">
    /// try
    /// {
    /// }
    /// catch (Exception ex)
    /// {
    /// }
    /// </code>
    ///
    /// <para>The try-finally statement below does contain code within the try block, but it does not contain any catch
    /// blocks, and the finally block is empty. Thus, the try-finally is not performing any useful function and can be
    /// removed.</para>
    ///
    /// <code language="csharp">
    /// try
    /// {
    ///     this.Method();
    /// }
    /// finally
    /// {
    /// }
    /// </code>
    ///
    /// <para>As a final example, the unsafe statement below is empty, and thus provides no value.</para>
    ///
    /// <code language="csharp">
    /// unsafe
    /// {
    /// }
    /// </code>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SA1409RemoveUnnecessaryCode : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "SA1409";
        private const string Title = "Remove unnecessary code";
        private const string MessageFormat = "TODO: Message format";
        private const string Category = "StyleCop.CSharp.MaintainabilityRules";
        private const string Description = "A C# file contains code which is unnecessary and can be removed without changing the overall logic of the code.";
        private const string HelpLink = "http://www.stylecop.com/docs/SA1409.html";

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, AnalyzerConstants.DisabledNoTests, Description, HelpLink, customTags: new[] { WellKnownDiagnosticTags.Unnecessary });

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
