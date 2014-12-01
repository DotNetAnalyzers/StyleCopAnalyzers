namespace StyleCop.Analyzers.ReadabilityRules
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// The <see cref="Nullable{T}"/> type has been defined not using the C# shorthand. For example,
    /// <c>Nullable&lt;DateTime&gt;</c> has been used instead of the preferred <c>DateTime?</c>.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs whenever the <see cref="Nullable{T}"/> type has been defined without using
    /// the shorthand C# style.</para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SA1125UseShorthandForNullableTypes : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "SA1125";
        internal const string Title = "The Nullable<T> type has been defined not using the C# shorthand. For example, Nullable<DateTime> has been used instead of the preferred DateTime?";
        internal const string MessageFormat = "TODO: Message format";
        internal const string Category = "StyleCop.CSharp.ReadabilityRules";
        internal const string Description = "The Nullable<T> type has been defined not using the C# shorthand. For example, Nullable<DateTime> has been used instead of the preferred DateTime?";
        internal const string HelpLink = "http://www.stylecop.com/docs/SA1125.html";

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
