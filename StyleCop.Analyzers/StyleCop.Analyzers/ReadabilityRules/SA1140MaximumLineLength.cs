// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.ReadabilityRules
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.Helpers;

    /// <summary>
    /// The C# comment does not contain any comment text.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs whenever the code contains a C# comment which does not contain any
    /// text.</para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class SA1140MaximumLineLength : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1140MaximumLineLength"/> analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1140";
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(ReadabilityResources.SA1140Title), ReadabilityResources.ResourceManager, typeof(ReadabilityResources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(ReadabilityResources.SA1140MessageFormat), ReadabilityResources.ResourceManager, typeof(ReadabilityResources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(ReadabilityResources.SA1140Description), ReadabilityResources.ResourceManager, typeof(ReadabilityResources));
        private static readonly string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1140.md";

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.ReadabilityRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

        private static readonly Action<SyntaxTreeAnalysisContext> SyntaxTreeAction = HandleSyntaxTree;

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
            ImmutableArray.Create(Descriptor);

        /// <inheritdoc/>
        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            context.RegisterSyntaxTreeAction(SyntaxTreeAction);
        }

        private static void HandleSyntaxTree(SyntaxTreeAnalysisContext context)
        {
            var readabilityRules = context.GetStyleCopSettings(context.CancellationToken).ReadabilityRules;

            // Only proceed if rule is enabled
            if (readabilityRules.LineLength > 0)
            {
                SyntaxNode root = context.Tree.GetCompilationUnitRoot(context.CancellationToken);
                var fileText = context.Tree.GetText(context.CancellationToken);
                foreach (var line in fileText.Lines)
                {
                    if ((line.End - line.Start) > readabilityRules.LineLength)
                    {
                        var location = root.SyntaxTree.GetLocation(line.Span);
                        var diagnostic = Diagnostic.Create(Descriptor, location);
                        context.ReportDiagnostic(diagnostic);
                    }
                }
            }
        }
    }
}
