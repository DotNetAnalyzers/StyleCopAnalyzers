﻿// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.MaintainabilityRules
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// TODO.
    /// </summary>
    /// <remarks>
    /// <para>TODO.</para>
    ///
    /// <para>A violation of this rule occurs when unnecessary parenthesis have been used in an attribute constructor.
    /// For example:</para>
    ///
    /// <code language="csharp">
    /// [Serializable()]
    /// </code>
    /// <para>The parenthesis are unnecessary and should be removed:</para>
    /// <code language="csharp">
    /// [Serializable]
    /// </code>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class SA1411AttributeConstructorMustNotUseUnnecessaryParenthesis : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the
        /// <see cref="SA1411AttributeConstructorMustNotUseUnnecessaryParenthesis"/> analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1411";
        private const string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1411.md";
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(MaintainabilityResources.SA1411Title), MaintainabilityResources.ResourceManager, typeof(MaintainabilityResources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(MaintainabilityResources.SA1411MessageFormat), MaintainabilityResources.ResourceManager, typeof(MaintainabilityResources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(MaintainabilityResources.SA1411Description), MaintainabilityResources.ResourceManager, typeof(MaintainabilityResources));

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.MaintainabilityRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink, WellKnownDiagnosticTags.Unnecessary);

        private static readonly Action<SyntaxNodeAnalysisContext> AttributeArgumentListAction = HandleAttributeArgumentList;

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
            ImmutableArray.Create(Descriptor);

        /// <inheritdoc/>
        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            context.RegisterSyntaxNodeAction(AttributeArgumentListAction, SyntaxKind.AttributeArgumentList);
        }

        private static void HandleAttributeArgumentList(SyntaxNodeAnalysisContext context)
        {
            AttributeArgumentListSyntax syntax = (AttributeArgumentListSyntax)context.Node;
            if (syntax.Arguments.Count != 0)
            {
                return;
            }

            // Attribute constructor should not use unnecessary parenthesis
            context.ReportDiagnostic(Diagnostic.Create(Descriptor, syntax.GetLocation()));
        }
    }
}
