// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.DocumentationRules
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Microsoft.CodeAnalysis.Text;

    /// <summary>
    /// The project is configured to not parse XML documentation comments.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs when a compilation (project) contains one or more files which are parsed
    /// with the <see cref="DocumentationMode"/> not set to <see cref="DocumentationMode.Diagnose"/>. This most
    /// frequently occurs when the project is configured to not produce an XML documentation file during the
    /// build.</para>
    ///
    /// <para>Each project should be configured to include an XML documentation file with the compiled output.
    /// Otherwise, the semantics of all documentation comments are not checked and comments are likely to contain an
    /// increasing number of errors over time.</para>
    /// </remarks>
    [NoCodeFix("The necessary actions for this code fix are not supported by the analysis infrastructure.")]
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class SA1652EnableXmlDocumentationOutput : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1652EnableXmlDocumentationOutput"/> analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1652";
        private const string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1652.md";
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(DocumentationResources.SA1652Title), DocumentationResources.ResourceManager, typeof(DocumentationResources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(DocumentationResources.SA1652MessageFormat), DocumentationResources.ResourceManager, typeof(DocumentationResources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(DocumentationResources.SA1652Description), DocumentationResources.ResourceManager, typeof(DocumentationResources));

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.DocumentationRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

        private static readonly Action<CompilationStartAnalysisContext> CompilationStartAction = HandleCompilationStart;
        private static readonly Action<SyntaxTreeAnalysisContext> SyntaxTreeAction = HandleSyntaxTree;

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
            context.RegisterSyntaxTreeActionHonorExclusions(SyntaxTreeAction);
        }

        private static void HandleSyntaxTree(SyntaxTreeAnalysisContext context)
        {
            if (context.Tree.Options.DocumentationMode != DocumentationMode.Diagnose)
            {
                context.ReportDiagnostic(Diagnostic.Create(Descriptor, context.Tree.GetLocation(new TextSpan(0, 0))));
            }
        }
    }
}
