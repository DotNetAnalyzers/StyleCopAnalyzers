// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.ReadabilityRules
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// A C# statement contains a region tag between the declaration of the statement and the opening brace of the
    /// statement.
    /// </summary>
    /// <remarks>
    /// <para>This diagnostic is not implemented in StyleCopAnalyzers.</para>
    ///
    /// <para>A violation of this rule occurs when the code contains a region tag in between the declaration and the
    /// opening brace. For example:</para>
    /// <code language="csharp">
    /// if (x != y)
    /// #region
    /// {
    /// }
    /// #endregion
    /// </code>
    /// <para>This will result in the body of the statement being hidden when the region is collapsed.</para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    [NoDiagnostic("This diagnostic is rarely-occurring specialization of SA1123; the latter is now preferred in all cases.")]
    internal class SA1109BlockStatementsMustNotContainEmbeddedRegions : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1109BlockStatementsMustNotContainEmbeddedRegions"/>
        /// analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1109";
        private const string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1109.md";
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(ReadabilityResources.SA1109Title), ReadabilityResources.ResourceManager, typeof(ReadabilityResources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(ReadabilityResources.SA1109MessageFormat), ReadabilityResources.ResourceManager, typeof(ReadabilityResources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(ReadabilityResources.SA1109Description), ReadabilityResources.ResourceManager, typeof(ReadabilityResources));

        private static readonly DiagnosticDescriptor Descriptor =
#pragma warning disable IDE0079 // Remove unnecessary suppression
#pragma warning disable RS2000 // Add analyzer diagnostic IDs to analyzer release.
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.ReadabilityRules, DiagnosticSeverity.Warning, AnalyzerConstants.DisabledByDefault, Description, HelpLink, WellKnownDiagnosticTags.NotConfigurable);
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
