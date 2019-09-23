// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.MaintainabilityRules
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// A call to <see cref="O:System.Diagnostics.Debug.Fail"/> in C# code does not include a descriptive message.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs when the code contains a call to
    /// <see cref="O:System.Diagnostics.Debug.Fail"/> which does not provide a description for the end-user. For
    /// example, the following call includes a description message:</para>
    ///
    /// <code language="csharp">
    /// Debug.Fail("The code should never reach this point.");
    /// </code>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    [NoCodeFix("No message is available for Debug.Fail")]
    internal class SA1406DebugFailMustProvideMessageText : SystemDiagnosticsDebugDiagnosticBase
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1406DebugFailMustProvideMessageText"/> analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1406";
        private const string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1406.md";
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(MaintainabilityResources.SA1406Title), MaintainabilityResources.ResourceManager, typeof(MaintainabilityResources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(MaintainabilityResources.SA1406MessageFormat), MaintainabilityResources.ResourceManager, typeof(MaintainabilityResources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(MaintainabilityResources.SA1406Description), MaintainabilityResources.ResourceManager, typeof(MaintainabilityResources));

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.MaintainabilityRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

        private static readonly Action<SyntaxNodeAnalysisContext> InvocationExpressionAction = HandleInvocationExpression;

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
            ImmutableArray.Create(Descriptor);

        /// <inheritdoc/>
        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            context.RegisterSyntaxNodeAction(InvocationExpressionAction, SyntaxKind.InvocationExpression);
        }

        private static void HandleInvocationExpression(SyntaxNodeAnalysisContext context)
        {
            // Debug.Fail is not available in a portable library. So no nameof(Debug.Fail) here
            HandleInvocationExpression(context, "Fail", 0, Descriptor);
        }
    }
}
