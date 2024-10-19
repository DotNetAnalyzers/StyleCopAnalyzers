﻿// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.DocumentationRules
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// A section within the XML documentation header for a C# element contains blank lines.
    /// </summary>
    /// <remarks>
    /// <para>C# syntax provides a mechanism for inserting documentation for classes and elements directly into the
    /// code, through the use of XML documentation headers.</para>
    ///
    /// <para>A violation of this rule occurs when the documentation header contains one or more blank lines within a
    /// section of documentation. For example:</para>
    ///
    /// <code language="csharp">
    /// /// &lt;summary&gt;
    /// /// Joins a first name and a last name together into a single string.
    /// ///
    /// /// Uses a simple form of string concatenation.
    /// /// &lt;/summary&gt;
    /// /// &lt;param name="firstName"&gt;The first name to join.&lt;/param&gt;
    /// /// &lt;param name="lastName"&gt;The last name to join.&lt;/param&gt;
    /// /// &lt;returns&gt;The joined names.&lt;/returns&gt;
    /// public string JoinNames(string firstName, string lastName)
    /// {
    ///     return firstName + " " + lastName;
    /// }
    /// </code>
    ///
    /// <para>Rather than inserting blank lines into the documentation, use the <c>&lt;para&gt;</c> tag to denote
    /// paragraphs. For example:</para>
    ///
    /// <code language="csharp">
    /// /// &lt;summary&gt;
    /// /// &lt;para&gt;
    /// /// Joins a first name and a last name together into a single string.
    /// /// &lt;/para&gt;&lt;para&gt;
    /// /// Uses a simple form of string concatenation.
    /// /// &lt;/para&gt;
    /// /// &lt;/summary&gt;
    /// /// &lt;param name="firstName"&gt;The first name to join.&lt;/param&gt;
    /// /// &lt;param name="lastName"&gt;The last name to join.&lt;/param&gt;
    /// /// &lt;returns&gt;The joined names.&lt;/returns&gt;
    /// public string JoinNames(string firstName, string lastName)
    /// {
    ///     return firstName + " " + lastName;
    /// }
    /// </code>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class SA1644DocumentationHeadersMustNotContainBlankLines : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1644DocumentationHeadersMustNotContainBlankLines"/>
        /// analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1644";
        private const string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1644.md";
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(DocumentationResources.SA1644Title), DocumentationResources.ResourceManager, typeof(DocumentationResources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(DocumentationResources.SA1644MessageFormat), DocumentationResources.ResourceManager, typeof(DocumentationResources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(DocumentationResources.SA1644Description), DocumentationResources.ResourceManager, typeof(DocumentationResources));

        private static readonly DiagnosticDescriptor Descriptor =
#pragma warning disable RS2000 // Add analyzer diagnostic IDs to analyzer release.
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.DocumentationRules, DiagnosticSeverity.Warning, AnalyzerConstants.DisabledByDefault, Description, HelpLink, WellKnownDiagnosticTags.NotConfigurable);
#pragma warning restore RS2000 // Add analyzer diagnostic IDs to analyzer release.

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
            ImmutableArray.Create(Descriptor);

        /// <inheritdoc/>
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
