// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.LayoutRules
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.Helpers;

    /// <summary>
    /// Within a C# property, indexer or event, at least one of the child accessors is written on a single line, and at
    /// least one of the child accessors is written across multiple lines.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs when the accessors within a property, indexer or event are not
    /// consistently written on a single line or on multiple lines. This rule is intended to increase the readability of
    /// the code by requiring all of the accessors within an element to be formatted in the same way.</para>
    ///
    /// <para>For example, the following property would generate a violation of this rule, because one accessor is
    /// written on a single line while the other accessor snaps multiple lines.</para>
    ///
    /// <code language="csharp">
    /// public bool Enabled
    /// {
    ///     get { return this.enabled; }
    ///
    ///     set
    ///     {
    ///         this.enabled = value;
    ///     }
    /// }
    /// </code>
    ///
    /// <para>The violation can be avoided by placing both accessors on a single line, or expanding both accessors
    /// across multiple lines:</para>
    ///
    /// <code language="csharp">
    /// public bool Enabled
    /// {
    ///     get { return this.enabled; }
    ///     set { this.enabled = value; }
    /// }
    ///
    /// public bool Enabled
    /// {
    ///     get
    ///     {
    ///         return this.enabled;
    ///     }
    ///
    ///     set
    ///     {
    ///         this.enabled = value;
    ///     }
    /// }
    /// </code>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class SA1504AllAccessorsMustBeSingleLineOrMultiLine : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1504AllAccessorsMustBeSingleLineOrMultiLine"/> analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1504";

        private const string Title = "All accessors must be single-line or multi-line";
        private const string MessageFormat = "All accessors must be single-line or multi-line";
        private const string Description = "Within a C# property, indexer or event, at least one of the child accessors is written on a single line, and at least one of the child accessors is written across multiple lines.";
        private const string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1504.md";

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.LayoutRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

        private static readonly Action<CompilationStartAnalysisContext> CompilationStartAction = HandleCompilationStart;
        private static readonly Action<SyntaxNodeAnalysisContext> AccessorListAction = HandleAccessorList;

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
            context.RegisterSyntaxNodeActionHonorExclusions(AccessorListAction, SyntaxKind.AccessorList);
        }

        private static void HandleAccessorList(SyntaxNodeAnalysisContext context)
        {
            var accessorList = (AccessorListSyntax)context.Node;

            if (accessorList.Accessors.Count < 2)
            {
                return;
            }

            var hasSingleLineAccessor = false;
            var hasMultipleLinesAccessor = false;

            foreach (var accessor in accessorList.Accessors)
            {
                // never report when any accessor has no body.
                if (accessor.Body == null)
                {
                    return;
                }

                var fileLinePositionSpan = accessor.GetLineSpan();
                if (fileLinePositionSpan.StartLinePosition.Line == fileLinePositionSpan.EndLinePosition.Line)
                {
                    hasSingleLineAccessor = true;
                }
                else
                {
                    hasMultipleLinesAccessor = true;
                }
            }

            if (hasSingleLineAccessor && hasMultipleLinesAccessor)
            {
                context.ReportDiagnostic(Diagnostic.Create(Descriptor, accessorList.Accessors.First().Keyword.GetLocation()));
            }
        }
    }
}
