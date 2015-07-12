﻿namespace StyleCop.Analyzers.DocumentationRules
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// The element documentation for the element contains one or more spelling mistakes or unrecognized words.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs when the element documentation contains spelling mistakes:</para>
    ///
    /// <code language="csharp">
    /// /// &lt;summary&gt;
    /// /// Joinsnames
    /// /// &lt;/summary&gt;
    /// /// &lt;param name="firstName"&gt;The first name.&lt;/param&gt;
    /// /// &lt;param name="lastName"&gt;The last name.&lt;/param&gt;
    /// /// &lt;returns&gt;Name&lt;/returns&gt;
    /// public string JoinNames(string firstName, string lastName)
    /// {
    ///     ...
    /// }
    /// </code>
    ///
    /// <para>The spelling is checked using the culture specified in the Settings. StyleCop file (and en-US by
    /// default).</para>
    ///
    /// <para>In this example the word Joinsnames in the summary element is misspelled. Either correct the spelling,
    /// insert any C# names in <c>&lt;c&gt;</c> <c>&lt;/c&gt;</c> elements, suppress the violation, or add the
    /// Joinsnames to a CustomDictionary.xml file.</para>
    ///
    /// <list type="bullet">
    /// <item>CustomDictionary.xml files can contain words that the spelling checker does not normally recognise.</item>
    /// <item>The CustomDictionary.xml file should be placed in the same folder as the StyleCop.dll and the Rules. That
    /// folder (and all subfolders) are checked for the dictionary files.</item>
    /// <item>StyleCop loads CustomDictionary.xml, CustomDictionary.en-GB.xml and then CustomDictionary.en.xml (where
    /// en-GB is the culture specified in the Settings.StyleCop file).</item>
    /// <item>StyleCop also loads custom.dic, custom.en-GB.dic and then custom.en.dic (where en-GB is the culture
    /// specified in the Settings.StyleCop file).</item>
    /// <item>Recognized words can also be added to the Settings.StyleCop file using the Settings Editor on the spelling
    /// tab.</item>
    /// <item>Attribute values in the documentation xml of the element are not checked for spelling.</item>
    /// <item>Any text inside <c> </c> or <code> </code> elements is also ignored.</item>
    /// <item>Any text starting with and ending with '$' or starting and ending with '$$' is also ignored. i.e.
    /// $$(thtp kthpo kthpk)$$.</item>
    /// </list>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SA1650ElementDocumentationMustBeSpelledCorrectly : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1650ElementDocumentationMustBeSpelledCorrectly"/>
        /// analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1650";
        private const string Title = "Element documentation must be spelled correctly";
        private const string MessageFormat = "TODO: Message format";
        private const string Description = "The element documentation for the element contains one or more spelling mistakes or unrecognized words.";
        private const string HelpLink = "http://www.stylecop.com/docs/SA1650.html";

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.DocumentationRules, DiagnosticSeverity.Warning, AnalyzerConstants.DisabledNoTests, Description, HelpLink);

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
