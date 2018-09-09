// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.MaintainabilityRules
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// A internal field within a C# class is not a internal auto property.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs whenever a internal field in a class is not a auto property. For
    /// maintainability reasons, properties should always be used as the mechanism for exposing fields outside of a
    /// class, and fields should always be declared with private access. This allows the internal implementation of the
    /// property to change over time without changing the interface of the class.</para>
    ///
    /// <para>Fields located within C# structs are allowed to have any access level.</para>
    /// </remarks>
    // TODO: Fix NoCodeFix to suggest the proper change for this (change to auto property suggestion).
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    [NoCodeFix("The \"Encapsulate Field\" fix is provided by Visual Studio.")]
    internal class SA1414InternalFieldMustBeAutoProperty : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1401FieldsMustBePrivate"/> analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1414";
        private const string Title = "Internal fields should be auto properties.";
        private const string MessageFormat = "A internal field should be made into a auto property.";
        private const string Description = "A internal field within a C# class is not a internal auto property.";
        private const string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1414.md";

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.MaintainabilityRules, DiagnosticSeverity.Warning, AnalyzerConstants.DisabledByDefault, Description, HelpLink);

        private static readonly Action<SymbolAnalysisContext> AnalyzeFieldAction = Analyzer.AnalyzeField;

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
            ImmutableArray.Create(Descriptor);

        /// <inheritdoc/>
        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            context.RegisterSymbolAction(AnalyzeFieldAction, SymbolKind.Field);
        }

        private class Analyzer
        {
            internal static void AnalyzeField(SymbolAnalysisContext symbolAnalysisContext)
            {
                var fieldDeclarationSyntax = (IFieldSymbol)symbolAnalysisContext.Symbol;
                if ((IsInternal(fieldDeclarationSyntax)
                    || IsProtectedInternal(fieldDeclarationSyntax))
                    && (!IsStatic(fieldDeclarationSyntax)
                    || !IsConst(fieldDeclarationSyntax)))
                {
                    foreach (var location in symbolAnalysisContext.Symbol.Locations)
                    {
                        if (!location.IsInSource)
                        {
                            // assume symbols not defined in a source document are "out of reach"
                            return;
                        }
                    }

                    symbolAnalysisContext.ReportDiagnostic(Diagnostic.Create(Descriptor, fieldDeclarationSyntax.Locations[0]));
                }
            }

            private static bool IsStatic(IFieldSymbol fieldDeclarationSyntax)
            {
                return fieldDeclarationSyntax.IsStatic;
            }

            private static bool IsConst(IFieldSymbol fieldDeclarationSyntax)
            {
                return fieldDeclarationSyntax.IsConst;
            }

            private static bool IsProtectedInternal(IFieldSymbol fieldDeclarationSyntax)
            {
                return fieldDeclarationSyntax.DeclaredAccessibility == Accessibility.ProtectedAndInternal;
            }

            private static bool IsInternal(IFieldSymbol fieldDeclarationSyntax)
            {
                return fieldDeclarationSyntax.DeclaredAccessibility == Accessibility.Internal;
            }
        }
    }
}
