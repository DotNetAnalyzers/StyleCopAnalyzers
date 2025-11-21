// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.DocumentationRules
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// An <c>include</c> tag within an XML documentation header does not contain valid file and path attribute.
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
    /// <para>A violation of this rule occurs when the include tag is missing a file or path attribute, or contains an
    /// improperly formatted file or path attribute.</para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    [NoDiagnostic("This is already handled by the compiler with warning CS1590.")]
    [NoCodeFix("Cannot generate documentation")]
    internal class SA1647IncludeNodeDoesNotContainValidFileAndPath : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1647IncludeNodeDoesNotContainValidFileAndPath"/>
        /// analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1647";
        private const string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1647.md";
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(DocumentationResources.SA1647Title), DocumentationResources.ResourceManager, typeof(DocumentationResources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(DocumentationResources.SA1647MessageFormat), DocumentationResources.ResourceManager, typeof(DocumentationResources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(DocumentationResources.SA1647Description), DocumentationResources.ResourceManager, typeof(DocumentationResources));

        private static readonly DiagnosticDescriptor Descriptor =
#pragma warning disable IDE0079 // Remove unnecessary suppression
#pragma warning disable RS2000 // Add analyzer diagnostic IDs to analyzer release.
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.DocumentationRules, DiagnosticSeverity.Warning, AnalyzerConstants.DisabledByDefault, Description, HelpLink, WellKnownDiagnosticTags.NotConfigurable);
#pragma warning restore RS2000 // Add analyzer diagnostic IDs to analyzer release.
#pragma warning restore IDE0079 // Remove unnecessary suppression

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
            ImmutableArray.Create(Descriptor);

        /// <inheritdoc/>
#pragma warning disable IDE0079 // Remove unnecessary suppression
#pragma warning disable RS1025 // Configure generated code analysis
#pragma warning disable RS1026 // Enable concurrent execution
        public override void Initialize(AnalysisContext context)
#pragma warning restore RS1026 // Enable concurrent execution
#pragma warning restore RS1025 // Configure generated code analysis
#pragma warning restore IDE0079 // Remove unnecessary suppression
        {
            // This diagnostic is not implemented (by design) in StyleCopAnalyzers.
        }
    }
}
