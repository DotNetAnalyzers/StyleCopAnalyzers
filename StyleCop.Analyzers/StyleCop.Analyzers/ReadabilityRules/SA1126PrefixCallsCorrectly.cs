﻿// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.ReadabilityRules
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// A call to a member is not prefixed with the <c>this.</c>, <c>base.</c>, <c>object.</c> or <c>typename.</c>
    /// prefix to indicate the intended method call, within a C# code file.
    /// </summary>
    /// <remarks>
    /// <para>This diagnostic is not implemented in StyleCopAnalyzers.</para>
    ///
    /// <para>A violation of this rule occurs whenever the code contains a call to a member which is not prefixed
    /// correctly.</para>
    ///
    /// <para>In some case from source code analysis we cannot be sure which prefix is required. It could be
    /// <c>this</c>, <c>base</c>, <c>object</c>, the typename of the class we're in, or one of our base classes.</para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    [NoDiagnostic("Roslyn provides the ability to create well-defined rules for specific use cases, removing the need for this vaguely-worded diagnostic.")]
    internal class SA1126PrefixCallsCorrectly : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1126PrefixCallsCorrectly"/> analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1126";
        private const string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1126.md";
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(ReadabilityResources.SA1126Title), ReadabilityResources.ResourceManager, typeof(ReadabilityResources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(ReadabilityResources.SA1126MessageFormat), ReadabilityResources.ResourceManager, typeof(ReadabilityResources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(ReadabilityResources.SA1126Description), ReadabilityResources.ResourceManager, typeof(ReadabilityResources));

        private static readonly DiagnosticDescriptor Descriptor =
#pragma warning disable RS2000 // Add analyzer diagnostic IDs to analyzer release.
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.ReadabilityRules, DiagnosticSeverity.Warning, AnalyzerConstants.DisabledByDefault, Description, HelpLink, WellKnownDiagnosticTags.NotConfigurable);
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
