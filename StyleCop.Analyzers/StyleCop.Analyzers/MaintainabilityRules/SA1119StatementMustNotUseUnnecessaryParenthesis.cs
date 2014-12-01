namespace StyleCop.Analyzers.MaintainabilityRules
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// A C# statement contains parenthesis which are unnecessary and should be removed.
    /// </summary>
    /// <remarks>
    /// <para>It is possible in C# to insert parenthesis around virtually any type of expression, statement, or clause,
    /// and in many situations use of parenthesis can greatly improve the readability of the code. However, excessive
    /// use of parenthesis can have the opposite effect, making it more difficult to read and maintain the code.</para>
    ///
    /// <para>A violation of this rule occurs when parenthesis are used in situations where they provide no practical
    /// value. Typically, this happens anytime the parenthesis surround an expression which does not strictly require
    /// the use of parenthesis, and the parenthesis expression is located at the root of a statement. For example, the
    /// following lines of code all contain unnecessary parenthesis which will result in violations of this rule:</para>
    ///
    /// <code language="csharp">
    /// int x = (5 + b);
    /// string y = (this.Method()).ToString();
    /// return (x.Value);
    /// </code>
    ///
    /// <para>In each of these statements, the extra parenthesis can be removed without sacrificing the readability of
    /// the code:</para>
    ///
    /// <code language="csharp">
    /// int x = 5 + b;
    /// string y = this.Method().ToString();
    /// return x.Value;
    /// </code>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SA1119StatementMustNotUseUnnecessaryParenthesis : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "SA1119";
        internal const string Title = "A C# statement contains parenthesis which are unnecessary and should be removed.";
        internal const string MessageFormat = "TODO: Message format";
        internal const string Category = "StyleCop.CSharp.MaintainabilityRules";
        internal const string Description = "A C# statement contains parenthesis which are unnecessary and should be removed.";
        internal const string HelpLink = "http://www.stylecop.com/docs/SA1119.html";

        public static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, AnalyzerConstants.DisabledNoTests, Description, HelpLink, customTags: new[] { WellKnownDiagnosticTags.Unnecessary });

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
