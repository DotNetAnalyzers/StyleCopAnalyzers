// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.MaintainabilityRules
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Immutable;
    using System.Diagnostics.CodeAnalysis;
    using Helpers;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// A Code Analysis SuppressMessage attribute does not include a justification.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs when the code contains a Code Analysis
    /// <see cref="SuppressMessageAttribute"/> attribute, but a justification for the suppression has not been provided
    /// within the attribute. Whenever a Code Analysis rule is suppressed, a justification should be provided. This can
    /// increase the long-term maintainability of the code.</para>
    ///
    /// <code language="csharp">
    /// [SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", Justification = "Used during unit testing")]
    /// public bool Enable()
    /// {
    /// }
    /// </code>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class SA1404CodeAnalysisSuppressionMustHaveJustification : DiagnosticAnalyzer
    {
        /// <summary>
        /// The placeholder to insert as part of the code fix.
        /// </summary>
        public const string JustificationPlaceholder = "<Pending>";

        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1404CodeAnalysisSuppressionMustHaveJustification"/>
        /// analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1404";
        private const string Title = "Code analysis suppression must have justification";
        private const string MessageFormat = "Code analysis suppression must have justification";
        private const string Description = "A Code Analysis SuppressMessage attribute does not include a justification.";
        private const string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1404.md";

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.MaintainabilityRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

        private static readonly Action<CompilationStartAnalysisContext> CompilationStartAction = HandleCompilationStart;

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
            AnalyzerInstance instance = new AnalyzerInstance(context.Compilation.GetOrCreateUsingAliasCache());
            context.RegisterSyntaxNodeActionHonorExclusions(instance.HandleAttributeNode, SyntaxKind.Attribute);
        }

        /// <summary>
        /// This class holds analyzer state information for analysis within a particular <see cref="Compilation"/>.
        /// </summary>
        private sealed class AnalyzerInstance
        {
            private readonly ConcurrentDictionary<SyntaxTree, bool> usingAliasCache;

            /// <summary>
            /// A lazily-initialized reference to <see cref="SuppressMessageAttribute"/> within the context of a
            /// particular <see cref="Compilation"/>.
            /// </summary>
            private INamedTypeSymbol suppressMessageAttribute;

            public AnalyzerInstance(ConcurrentDictionary<SyntaxTree, bool> usingAliasCache)
            {
                this.usingAliasCache = usingAliasCache;
            }

            public void HandleAttributeNode(SyntaxNodeAnalysisContext context)
            {
                var attribute = (AttributeSyntax)context.Node;

                // Return fast if the name doesn't match and the file doesn't contain any using alias directives
                if (!attribute.SyntaxTree.ContainsUsingAlias(this.usingAliasCache))
                {
                    SimpleNameSyntax simpleNameSyntax = attribute.Name as SimpleNameSyntax;
                    if (simpleNameSyntax == null)
                    {
                        QualifiedNameSyntax qualifiedNameSyntax = attribute.Name as QualifiedNameSyntax;
                        simpleNameSyntax = qualifiedNameSyntax.Right;
                    }

                    if (simpleNameSyntax.Identifier.ValueText != nameof(SuppressMessageAttribute)
                        && simpleNameSyntax.Identifier.ValueText != "SuppressMessage")
                    {
                        return;
                    }
                }

                SymbolInfo symbolInfo = context.SemanticModel.GetSymbolInfo(attribute);
                ISymbol symbol = symbolInfo.Symbol;
                if (symbol != null)
                {
                    if (this.suppressMessageAttribute == null)
                    {
                        this.suppressMessageAttribute = context.SemanticModel.Compilation.GetTypeByMetadataName(typeof(SuppressMessageAttribute).FullName);
                    }

                    if (symbol.ContainingType == this.suppressMessageAttribute)
                    {
                        foreach (var attributeArgument in attribute.ArgumentList.Arguments)
                        {
                            if (attributeArgument.NameEquals?.Name?.Identifier.ValueText == nameof(SuppressMessageAttribute.Justification))
                            {
                                // Check if the justification is not empty
                                var value = context.SemanticModel.GetConstantValue(attributeArgument.Expression);

                                // If value does not have a value the expression is not constant -> Compilation error
                                if (!value.HasValue || (!string.IsNullOrWhiteSpace(value.Value as string) && (value.Value as string) != JustificationPlaceholder))
                                {
                                    return;
                                }

                                // Empty, Whitespace, placeholder, or null justification provided
                                context.ReportDiagnostic(Diagnostic.Create(Descriptor, attributeArgument.GetLocation()));
                                return;
                            }
                        }

                        // No justification set
                        context.ReportDiagnostic(Diagnostic.Create(Descriptor, attribute.GetLocation()));
                    }
                }
            }
        }
    }
}
