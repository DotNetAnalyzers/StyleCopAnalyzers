namespace StyleCop.Analyzers.MaintainabilityRules
{
    using System.Collections.Immutable;
    using System.Text;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Microsoft.CodeAnalysis.Text;

    /// <summary>
    /// Store files as UTF-8 with byte order mark.
    /// </summary>
    /// <remarks>
    /// <para>Storing files in this encoding ensures that the files are always treated the same way by the compiler,
    /// even when compiled on systems with varying default system encodings. In addition,
    /// this encoding is the most widely supported encoding for features like visual diffs on GitHub and other tooling.
    /// This encoding is also the default encoding used when creating new C# source files within Visual Studio.
    /// </para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SA1412StoreFilesAsUtf8 : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the
        /// <see cref="SA1412StoreFilesAsUtf8"/> analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1412";
        private const string Title = "Store files as UTF-8 with byte order mark";
        private const string MessageFormat = "Store files as UTF-8 with byte order mark";
        private const string Description = "Source files should be saved using the UTF-8 encoding with a byte order mark";
        private const string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1412.md";

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.MaintainabilityRules, DiagnosticSeverity.Warning, AnalyzerConstants.DisabledByDefault, Description, HelpLink);

        private static readonly ImmutableArray<DiagnosticDescriptor> SupportedDiagnosticsValue =
            ImmutableArray.Create(Descriptor);

        private static byte[] utf8Preamble = Encoding.UTF8.GetPreamble();

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
            context.RegisterSyntaxTreeActionHonorExclusions(HandleSyntaxTree);
        }

        private static void HandleSyntaxTree(SyntaxTreeAnalysisContext context)
        {
            byte[] preamble = context.Tree.Encoding.GetPreamble();

            if (!IsUtf8Preamble(preamble))
            {
                context.ReportDiagnostic(Diagnostic.Create(Descriptor, Location.Create(context.Tree, TextSpan.FromBounds(0, 0))));
            }
        }

        private static bool IsUtf8Preamble(byte[] preamble)
        {
            if (preamble == null || preamble.Length != utf8Preamble.Length)
            {
                return false;
            }
            else
            {
                for (int i = 0; i < utf8Preamble.Length; i++)
                {
                    if (utf8Preamble[i] != preamble[i])
                    {
                        return false;
                    }
                }

                return true;
            }
        }
    }
}
