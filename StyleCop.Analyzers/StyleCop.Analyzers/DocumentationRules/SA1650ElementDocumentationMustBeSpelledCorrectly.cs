// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.DocumentationRules
{
    using System.Collections.Immutable;
    using System.Diagnostics.CodeAnalysis;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// The element documentation for the element contains one or more spelling mistakes or unrecognized words.
    /// </summary>
    /// <remarks>
    /// <para>This diagnostic is not implemented in StyleCopAnalyzers.</para>
    ///
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
    /// <item><description>CustomDictionary.xml files can contain words that the spelling checker does not normally
    /// recognize.</description></item>
    /// <item><description>The CustomDictionary.xml file should be placed in the same folder as the StyleCop.dll and the
    /// Rules. That folder (and all subfolders) are checked for the dictionary files.</description></item>
    /// <item><description>StyleCop loads CustomDictionary.xml, CustomDictionary.en-GB.xml and then
    /// CustomDictionary.en.xml (where en-GB is the culture specified in the Settings.StyleCop
    /// file).</description></item>
    /// <item><description>StyleCop also loads custom.dic, custom.en-GB.dic and then custom.en.dic (where en-GB is the
    /// culture specified in the Settings.StyleCop file).</description></item>
    /// <item><description>Recognized words can also be added to the Settings.StyleCop file using the Settings Editor on
    /// the spelling tab.</description></item>
    /// <item><description>Attribute values in the documentation xml of the element are not checked for
    /// spelling.</description></item>
    /// <item><description>Any text inside <c>&lt;c&gt;</c> or <c>&lt;code&gt;</c> elements is also
    /// ignored.</description></item>
    /// <item><description>Any text starting with and ending with '$' or starting and ending with '$$' is also ignored.
    /// i.e. $$(thtp kthpo kthpk)$$.</description></item>
    /// </list>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    [NoDiagnostic("This diagnostic has an unacceptable rate of false positives.")]
    internal class SA1650ElementDocumentationMustBeSpelledCorrectly : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1650ElementDocumentationMustBeSpelledCorrectly"/>
        /// analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1650";
        private const string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1650.md";
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(DocumentationResources.SA1650Title), DocumentationResources.ResourceManager, typeof(DocumentationResources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(DocumentationResources.SA1650MessageFormat), DocumentationResources.ResourceManager, typeof(DocumentationResources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(DocumentationResources.SA1650Description), DocumentationResources.ResourceManager, typeof(DocumentationResources));

        private static readonly DiagnosticDescriptor Descriptor =
#pragma warning disable RS2000 // Add analyzer diagnostic IDs to analyzer release.
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.DocumentationRules, DiagnosticSeverity.Warning, AnalyzerConstants.DisabledByDefault, Description, HelpLink, WellKnownDiagnosticTags.NotConfigurable);
#pragma warning restore RS2000 // Add analyzer diagnostic IDs to analyzer release.

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
            ImmutableArray.Create(Descriptor);

        /// <inheritdoc/>
        [ExcludeFromCodeCoverage]
        public override void Initialize(AnalysisContext context)
        {
            // This diagnostic is not implemented (by design) in StyleCopAnalyzers.
        }
    }
}
