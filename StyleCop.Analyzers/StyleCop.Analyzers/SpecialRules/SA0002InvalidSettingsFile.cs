﻿// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.SpecialRules
{
    using System;
    using System.Collections.Immutable;
    using System.Globalization;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Newtonsoft.Json;

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
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.SpecialRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

        private static readonly Action<CompilationAnalysisContext> CompilationAction = HandleCompilation;

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
            ImmutableArray.Create(Descriptor);

        /// <inheritdoc/>
        public override void Initialize(AnalysisContext context)
        {
            context.RegisterCompilationAction(CompilationAction);
        }

        private static void HandleCompilation(CompilationAnalysisContext context)
        {
            try
            {
                SettingsHelper.GetStyleCopSettings(context.Options, DeserializationFailureBehavior.ThrowException, context.CancellationToken);
            }
            catch (JsonException ex)
            {
                string details = ex.Message;
                string completeDescription = string.Format(Description.ToString(CultureInfo.CurrentCulture), details);

                var completeDescriptor = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.SpecialRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, completeDescription, HelpLink);
                context.ReportDiagnostic(Diagnostic.Create(completeDescriptor, Location.None));
            }
        }
    }
}
