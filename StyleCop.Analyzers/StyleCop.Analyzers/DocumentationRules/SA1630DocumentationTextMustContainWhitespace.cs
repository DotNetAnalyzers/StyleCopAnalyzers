// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.DocumentationRules
{
    using System.Collections.Immutable;
    using System.Diagnostics.CodeAnalysis;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// A section of the XML header documentation for a C# element does not contain any whitespace between words.
    /// </summary>
    /// <remarks>
    /// <para>This diagnostic is not implemented in StyleCopAnalyzers.</para>
    ///
    /// <para>C# syntax provides a mechanism for inserting documentation for classes and elements directly into the
    /// code, through the use of XML documentation headers.</para>
    ///
    /// <para>A violation of this rule occurs when part of the documentation does contain any whitespace between words.
    /// This can indicate poorly written or poorly formatted documentation. For example:</para>
    ///
    /// <code language="csharp">
    /// /// &lt;summary&gt;
    /// /// Joinsnames
    /// /// &lt;/summary&gt;
    /// /// &lt;param name="firstName"&gt;First&lt;/param&gt;
    /// /// &lt;param name="lastName"&gt;Last&lt;/param&gt;
    /// /// &lt;returns&gt;Name&lt;/returns&gt;
    /// public string JoinNames(string firstName, string lastName)
    /// {
    ///     ...
    /// }
    /// </code>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    [NoDiagnostic("This diagnostic was determined to be too subjective and/or misleading to developers.")]
    [NoCodeFix("Cannot generate documentation")]
    internal class SA1630DocumentationTextMustContainWhitespace : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1630DocumentationTextMustContainWhitespace"/> analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1630";
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(DocumentationResources.SA1630Title), DocumentationResources.ResourceManager, typeof(DocumentationResources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(DocumentationResources.SA1630MessageFormat), DocumentationResources.ResourceManager, typeof(DocumentationResources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(DocumentationResources.SA1630Description), DocumentationResources.ResourceManager, typeof(DocumentationResources));
        private const string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1630.md";

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.DocumentationRules, DiagnosticSeverity.Warning, AnalyzerConstants.DisabledByDefault, Description, HelpLink, WellKnownDiagnosticTags.NotConfigurable);

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
