namespace StyleCop.Analyzers.OrderingRules
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// An element within a C# code file is out of order within regard to access level, in relation to other elements in
    /// the code.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs when the code elements within a file do not follow a standard ordering
    /// scheme based on access level.</para>
    ///
    /// <para>To comply with this rule, adjacent elements of the same type must be positioned in the following order by
    /// access level:</para>
    ///
    /// <list type="bullet">
    /// <item>public</item>
    /// <item>internal</item>
    /// <item>protected internal</item>
    /// <item>protected</item>
    /// <item>private</item>
    /// </list>
    ///
    /// <para>Complying with a standard ordering scheme based on access level can increase the readability and
    /// maintainability of the file and make it easier to identify the public interface that is being exposed from a
    /// class.</para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SA1202ElementsMustBeOrderedByAccess : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "SA1202";
        private const string Title = "Elements must be ordered by access";
        private const string MessageFormat = "TODO: Message format";
        private const string Category = "StyleCop.CSharp.OrderingRules";
        private const string Description = "An element within a C# code file is out of order within regard to access level, in relation to other elements in the code.";
        private const string HelpLink = "http://www.stylecop.com/docs/SA1202.html";

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
