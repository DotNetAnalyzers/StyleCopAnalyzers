// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.DocumentationRules
{
    using System.Collections.Immutable;
    using System.Diagnostics.CodeAnalysis;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// An included XML documentation link contains an invalid path.
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
    /// /// &lt;include file="IncludedDocumentation.xml" path="root/EnabledMethodDocs" /&gt;
    /// public bool Enabled(bool true)
    /// {
    /// }
    /// </code>
    ///
    /// <para>A violation of this rule occurs when the path attribute does not link to a valid path within the included
    /// documentation file.</para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    [NoDiagnostic("This is already handled by the compiler with warning CS1589.")]
    [NoCodeFix("Cannot generate documentation")]
    internal class SA1646IncludedDocumentationXPathDoesNotExist : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1646IncludedDocumentationXPathDoesNotExist"/> analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1646";
        private const string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1646.md";
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(DocumentationResources.SA1646Title), DocumentationResources.ResourceManager, typeof(DocumentationResources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(DocumentationResources.SA1646MessageFormat), DocumentationResources.ResourceManager, typeof(DocumentationResources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(DocumentationResources.SA1646Description), DocumentationResources.ResourceManager, typeof(DocumentationResources));

        private static readonly DiagnosticDescriptor Descriptor =
#pragma warning disable RS2000 // Add analyzer diagnostic IDs to analyzer release.
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.DocumentationRules, DiagnosticSeverity.Warning, AnalyzerConstants.DisabledByDefault, Description, HelpLink, WellKnownDiagnosticTags.NotConfigurable);
#pragma warning restore RS2000 // Add analyzer diagnostic IDs to analyzer release.

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
            ImmutableArray.Create(Descriptor);

        /// <inheritdoc/>
        [ExcludeFromCodeCoverage]
#pragma warning disable RS1025 // Configure generated code analysis
#pragma warning disable RS1026 // Enable concurrent execution
        public override void Initialize(AnalysisContext context)
#pragma warning restore RS1026 // Enable concurrent execution
#pragma warning restore RS1025 // Configure generated code analysis
        {
            // This diagnostic is not implemented (by design) in StyleCopAnalyzers.
        }
    }
}
