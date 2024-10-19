// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.SpecialRules
{
    using System;
    using System.Collections.Immutable;
    using System.Linq;
    using LightJson.Serialization;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// The <em>stylecop.json</em> settings file could not be loaded due to a deserialization failure.
    /// </summary>
    [NoCodeFix("No automatic code fix is possible for general JSON syntax errors.")]
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class SA0002InvalidSettingsFile : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA0002InvalidSettingsFile"/> analyzer.
        /// </summary>
        public const string DiagnosticId = "SA0002";
        private const string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA0002.md";
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(SpecialResources.SA0002Title), SpecialResources.ResourceManager, typeof(SpecialResources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(SpecialResources.SA0002MessageFormat), SpecialResources.ResourceManager, typeof(SpecialResources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(SpecialResources.SA0002Description), SpecialResources.ResourceManager, typeof(SpecialResources));

        private static readonly DiagnosticDescriptor Descriptor =
#pragma warning disable RS1033 // Define diagnostic description correctly (Description ends with formatted exception text)
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.SpecialRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink, customTags: new[] { "CompilationEnd" });
#pragma warning restore RS1033 // Define diagnostic description correctly

        private static readonly Action<CompilationAnalysisContext> CompilationAction = HandleCompilation;

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
            ImmutableArray.Create(Descriptor);

        /// <inheritdoc/>
        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            context.RegisterCompilationAction(CompilationAction);
        }

        private static void HandleCompilation(CompilationAnalysisContext context)
        {
            var firstSyntaxTree = context.Compilation.SyntaxTrees.FirstOrDefault();
            if (firstSyntaxTree is null)
            {
                return;
            }

            try
            {
                context.GetStyleCopSettings(firstSyntaxTree, DeserializationFailureBehavior.ThrowException, context.CancellationToken);
            }
            catch (Exception ex) when (ex is JsonParseException || ex is InvalidSettingsException)
            {
                context.ReportDiagnostic(Diagnostic.Create(Descriptor, Location.None, ex.Message));
            }
        }
    }
}
