// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.DocumentationRules
{
    using System.Collections.Immutable;
    using System.Diagnostics.CodeAnalysis;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// An included XML documentation file does not exist.
    /// </summary>
    /// <remarks>
    /// <para>C# syntax provides a mechanism for inserting documentation for classes and elements directly into the
    /// code, through the use of XML documentation headers.</para>
    ///
    /// <para>As an alternative to authoring documentation directly within the code file, it is possible to place
    /// documentation for multiple elements within a separate XML file, and then reference a section of that file within
    /// an element's documentation header. This causes the compiler to import the documentation for that element from
    /// the included document. For example:</para>
    ///
    /// <code language="csharp">
    /// /// &lt;include file="IncludedDocumentation.xml" path="root/EnabledMethodDocs" /&gt;
    /// public bool Enabled(bool true)
    /// {
    /// }
    /// </code>
    ///
    /// <para>A violation of this rule occurs when the included file does not exist at the given location, or cannot be
    /// loaded.</para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    [NoDiagnostic("This is already handled by the compiler with warning CS1589.")]
    internal class SA1645IncludedDocumentationFileDoesNotExist : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1645IncludedDocumentationFileDoesNotExist"/> analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1645";
        private const string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1645.md";
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(DocumentationResources.SA1645Title), DocumentationResources.ResourceManager, typeof(DocumentationResources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(DocumentationResources.SA1645MessageFormat), DocumentationResources.ResourceManager, typeof(DocumentationResources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(DocumentationResources.SA1645Description), DocumentationResources.ResourceManager, typeof(DocumentationResources));

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
