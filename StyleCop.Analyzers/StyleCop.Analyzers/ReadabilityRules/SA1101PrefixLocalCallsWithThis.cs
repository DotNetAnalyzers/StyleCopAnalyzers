namespace StyleCop.Analyzers.ReadabilityRules
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// A call to an instance member of the local class or a base class is not prefixed with ‘this.’, within a C# code
    /// file.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs whenever the code contains a call to an instance member of the local class
    /// or a base class which is not prefixed with <c>this.</c>. An exception to this rule occurs when there is a local
    /// override of a base class member, and the code intends to call the base class member directly, bypassing the
    /// local override. In this case the call can be prefixed with <c>base.</c> rather than <c>this.</c>.</para>
    ///
    /// <para>By default, StyleCop disallows the use of underscores or <c>m_</c> to mark local class fields, in favor of
    /// the <c>this.</c> prefix. The advantage of using <c>this.</c> is that it applies equally to all element types
    /// including methods, properties, etc., and not just fields, making all calls to class members instantly
    /// recognizable, regardless of which editor is being used to view the code. Another advantage is that it creates a
    /// quick, recognizable differentiation between instance members and static members, which are not prefixed.</para>
    ///
    /// <para>A final advantage of using the <c>this.</c> prefix is that typing <c>this.</c> will cause Visual Studio to
    /// show the IntelliSense pop-up, making it quick and easy for the developer to choose the class member to
    /// call.</para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SA1101PrefixLocalCallsWithThis : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "SA1101";
        internal const string Title = "A call to an instance member of the local class or a base class is not prefixed with 'this.', within a C# code file.";
        internal const string MessageFormat = "TODO: Message format";
        internal const string Category = "StyleCop.CSharp.ReadabilityRules";
        internal const string Description = "A call to an instance member of the local class or a base class is not prefixed with 'this.', within a C# code file.";
        internal const string HelpLink = "http://www.stylecop.com/docs/SA1101.html";

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
