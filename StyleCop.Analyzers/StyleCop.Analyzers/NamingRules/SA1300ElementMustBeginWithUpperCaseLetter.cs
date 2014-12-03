namespace StyleCop.Analyzers.NamingRules
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// The name of a C# element does not begin with an upper-case letter.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs when the names of certain types of elements do not begin with an
    /// upper-case letter. The following types of elements should use an upper-case letter as the first letter of the
    /// element name: namespaces, classes, enums, structs, delegates, events, methods, and properties.</para>
    ///
    /// <para>In addition, any field which is public, internal, or marked with the const attribute should begin with an
    /// upper-case letter. Non-private readonly fields must also be named using an upper-case letter.</para>
    ///
    /// <para>If the field or variable name is intended to match the name of an item associated with Win32 or COM, and
    /// thus needs to begin with a lower-case letter, place the field or variable within a special <c>NativeMethods</c>
    /// class. A <c>NativeMethods</c> class is any class which contains a name ending in <c>NativeMethods</c>, and is
    /// intended as a placeholder for Win32 or COM wrappers. StyleCop will ignore this violation if the item is placed
    /// within a <c>NativeMethods</c> class.</para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SA1300ElementMustBeginWithUpperCaseLetter : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "SA1300";
        internal const string Title = "Element Must Begin With Upper Case Letter";
        internal const string MessageFormat = "TODO: Message format";
        internal const string Category = "StyleCop.CSharp.NamingRules";
        internal const string Description = "The name of a C# element does not begin with an upper-case letter.";
        internal const string HelpLink = "http://www.stylecop.com/docs/SA1300.html";

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
