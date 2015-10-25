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
        private const string Title = "Debug.Fail must provide message text";
        private const string MessageFormat = "Debug.Fail must provide message text";
        private const string Description = "A call to Debug.Fail in C# code does not include a descriptive message.";
        private const string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1406.md";

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.MaintainabilityRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

        private static readonly Action<CompilationStartAnalysisContext> CompilationStartAction = HandleCompilationStart;
        private static readonly Action<SyntaxNodeAnalysisContext> InvocationExpressionAction = HandleInvocationExpression;

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
            ImmutableArray.Create(Descriptor);

        /// <inheritdoc/>
        public override void Initialize(AnalysisContext context)
        {
            context.RegisterCompilationStartAction(CompilationStartAction);
        }

        private static void HandleCompilationStart(CompilationStartAnalysisContext context)
        {
            context.RegisterSyntaxNodeActionHonorExclusions(InvocationExpressionAction, SyntaxKind.InvocationExpression);
        }

        private static void HandleInvocationExpression(SyntaxNodeAnalysisContext context)
        {
            // Debug.Fail is not available in a portable library. So no nameof(Debug.Fail) here
            HandleMethodCall(context, "Fail", 0, Descriptor);
        }
    }
}
