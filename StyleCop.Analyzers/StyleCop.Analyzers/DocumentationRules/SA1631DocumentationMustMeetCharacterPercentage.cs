// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.DocumentationRules
{
    using System.Collections.Immutable;
    using System.Diagnostics.CodeAnalysis;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// A section of the XML header documentation for a C# element does not contain enough alphabetic characters.
    /// </summary>
    /// <remarks>
    /// <para>This diagnostic is not implemented in StyleCopAnalyzers.</para>
    ///
    /// <para>C# syntax provides a mechanism for inserting documentation for classes and elements directly into the
    /// code, through the use of XML documentation headers. For an introduction to these headers and a description of
    /// the header syntax, see the following article:
    /// <see href="http://msdn.microsoft.com/en-us/magazine/cc302121.aspx">XML Comments Let You Build Documentation
    /// Directly From Your Visual Studio .NET Source Files</see>.</para>
    ///
    /// <para>A violation of this rule occurs when part of the documentation does contain enough characters. This rule
    /// is calculated by counting the number of alphabetic characters and numbers within the documentation text, and
    /// comparing it against the number of symbols and other non-alphabetic characters. If the percentage of
    /// non-alphabetic characters is too high, this generally indicates poorly formatted documentation which will be
    /// difficult to read. For example, consider the follow summary documentation:</para>
    ///
    /// <code>
    /// /// &lt;summary&gt;
    /// /// @)$(*A name--------
    /// /// &lt;/summary&gt;
    /// public class Name
    /// {
    ///     ...
    /// }
    /// </code>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    [NoDiagnostic("This diagnostic was determined to be too subjective and/or misleading to developers.")]
    [NoCodeFix("Cannot generate documentation")]
    internal class SA1631DocumentationMustMeetCharacterPercentage : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1631DocumentationMustMeetCharacterPercentage"/>
        /// analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1631";
        private const string Title = "Documentation must meet character percentage";
        private const string MessageFormat = "TODO: Message format";
        private const string Description = "A section of the Xml header documentation for a C# element does not contain enough alphabetic characters.";
        private const string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1631.md";

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
