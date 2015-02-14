namespace StyleCop.Analyzers.LayoutRules
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// The opening and closing curly brackets for a C# statement have been omitted.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs when the opening and closing curly brackets for a statement have been
    /// omitted. In C#, some types of statements may optionally include curly brackets. Examples include <c>if</c>,
    /// <c>while</c>, and <c>for</c> statements. For example, an if-statement may be written without curly
    /// brackets:</para>
    ///
    /// <code language="csharp">
    /// if (true)
    ///     return this.value;
    /// </code>
    ///
    /// <para>Although this is legal in C#, StyleCop always requires the curly brackets to be present, to increase the
    /// readability and maintainability of the code.</para>
    ///
    /// <para>When the curly brackets are omitted, it is possible to introduce an error in the code by inserting an
    /// additional statement beneath the if-statement. For example:</para>
    ///
    /// <code language="csharp">
    /// if (true)
    ///     this.value = 2;
    ///     return this.value;
    /// </code>
    ///
    /// <para>Glancing at this code, it appears as if both the assignment statement and the return statement are
    /// children of the if-statement. In fact, this is not true. Only the assignment statement is a child of the
    /// if-statement, and the return statement will always execute regardless of the outcome of the if-statement.</para>
    ///
    /// <para>StyleCop always requires the opening and closing curly brackets to be present, to prevent these kinds of
    /// errors:</para>
    ///
    /// <code language="csharp">
    /// if (true)
    /// {
    ///     this.value = 2;
    ///     return this.value;
    /// }
    /// </code>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SA1503CurlyBracketsMustNotBeOmitted : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1503CurlyBracketsMustNotBeOmitted"/> analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1503";
        private const string Title = "Curly brackets must not be omitted";
        private const string MessageFormat = "TODO: Message format";
        private const string Category = "StyleCop.CSharp.LayoutRules";
        private const string Description = "The opening and closing curly brackets for a C# statement have been omitted.";
        private const string HelpLink = "http://www.stylecop.com/docs/SA1503.html";

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
