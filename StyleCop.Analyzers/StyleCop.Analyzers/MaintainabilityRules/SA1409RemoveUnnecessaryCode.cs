// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.MaintainabilityRules
{
    using System.Collections.Immutable;
    using System.Diagnostics.CodeAnalysis;
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
        private const string Title = "Remove unnecessary code";
        private const string MessageFormat = "TODO: Message format";
        private const string Description = "A C# file contains code which is unnecessary and can be removed without changing the overall logic of the code.";
        private const string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1409.md";

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.MaintainabilityRules, DiagnosticSeverity.Warning, AnalyzerConstants.DisabledByDefault, Description, HelpLink, WellKnownDiagnosticTags.NotConfigurable);

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
