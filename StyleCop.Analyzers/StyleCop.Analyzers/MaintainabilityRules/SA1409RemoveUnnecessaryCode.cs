﻿// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.MaintainabilityRules
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// A C# file contains code which is unnecessary and can be removed without changing the overall logic of the code.
    /// </summary>
    /// <remarks>
    /// <para>This diagnostic is not implemented in StyleCopAnalyzers.</para>
    ///
    /// <para>A violation of this rule occurs when the file contains code which can be removed without changing the
    /// overall logic of the code.</para>
    ///
    /// <para>For example, the following try-catch statement could be removed completely since the try and catch blocks
    /// are both empty.</para>
    ///
    /// <code language="csharp">
    /// try
    /// {
    /// }
    /// catch (Exception ex)
    /// {
    /// }
    /// </code>
    ///
    /// <para>The try-finally statement below does contain code within the try block, but it does not contain any catch
    /// blocks, and the finally block is empty. Thus, the try-finally is not performing any useful function and can be
    /// removed.</para>
    ///
    /// <code language="csharp">
    /// try
    /// {
    ///     this.Method();
    /// }
    /// finally
    /// {
    /// }
    /// </code>
    ///
    /// <para>As a final example, the unsafe statement below is empty, and thus provides no value.</para>
    ///
    /// <code language="csharp">
    /// unsafe
    /// {
    /// }
    /// </code>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    [NoDiagnostic("This diagnostic requires deep semantic analysis which is more suited to a usage-based analysis toolset as opposed to a style-based analysis toolset.")]
    internal class SA1409RemoveUnnecessaryCode : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1409RemoveUnnecessaryCode"/> analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1409";
        private const string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1409.md";
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(MaintainabilityResources.SA1409Title), MaintainabilityResources.ResourceManager, typeof(MaintainabilityResources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(MaintainabilityResources.SA1409MessageFormat), MaintainabilityResources.ResourceManager, typeof(MaintainabilityResources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(MaintainabilityResources.SA1409Description), MaintainabilityResources.ResourceManager, typeof(MaintainabilityResources));

        private static readonly DiagnosticDescriptor Descriptor =
#pragma warning disable RS2000 // Add analyzer diagnostic IDs to analyzer release.
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.MaintainabilityRules, DiagnosticSeverity.Warning, AnalyzerConstants.DisabledByDefault, Description, HelpLink, WellKnownDiagnosticTags.NotConfigurable);
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
