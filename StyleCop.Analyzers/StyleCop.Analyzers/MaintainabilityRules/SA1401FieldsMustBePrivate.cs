// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.MaintainabilityRules
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// A field within a C# class has an access modifier other than private.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs whenever a field in a class is given non-private access. For
    /// maintainability reasons, properties should always be used as the mechanism for exposing fields outside of a
    /// class, and fields should always be declared with private access. This allows the internal implementation of the
    /// property to change over time without changing the interface of the class.</para>
    ///
    /// <para>Fields located within C# structs are allowed to have any access level.</para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    [NoCodeFix("The \"Encapsulate Field\" fix is provided by Visual Studio.")]
    internal class SA1401FieldsMustBePrivate : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1401FieldsMustBePrivate"/> analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1401";
        private const string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1401.md";
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(MaintainabilityResources.SA1401Title), MaintainabilityResources.ResourceManager, typeof(MaintainabilityResources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(MaintainabilityResources.SA1401MessageFormat), MaintainabilityResources.ResourceManager, typeof(MaintainabilityResources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(MaintainabilityResources.SA1401Description), MaintainabilityResources.ResourceManager, typeof(MaintainabilityResources));

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.MaintainabilityRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

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

        private static class Analyzer
        {
            public static void AnalyzeField(SymbolAnalysisContext symbolAnalysisContext)
            {
                var fieldDeclarationSyntax = (IFieldSymbol)symbolAnalysisContext.Symbol;
                if (!IsFieldPrivate(fieldDeclarationSyntax) &&
                    !IsStaticReadonly(fieldDeclarationSyntax) &&
                    IsParentAClass(fieldDeclarationSyntax) &&
                    !fieldDeclarationSyntax.IsConst)
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

            private static bool IsFieldPrivate(IFieldSymbol fieldDeclarationSyntax)
            {
                return fieldDeclarationSyntax.DeclaredAccessibility == Accessibility.Private;
            }

            private static bool IsStaticReadonly(IFieldSymbol fieldDeclarationSyntax)
            {
                return fieldDeclarationSyntax.IsStatic && fieldDeclarationSyntax.IsReadOnly;
            }

            private static bool IsParentAClass(IFieldSymbol fieldDeclarationSyntax)
            {
                if (fieldDeclarationSyntax.ContainingSymbol != null &&
                    fieldDeclarationSyntax.ContainingSymbol.Kind == SymbolKind.NamedType)
                {
                    return ((ITypeSymbol)fieldDeclarationSyntax.ContainingSymbol).TypeKind == TypeKind.Class;
                }

                return false;
            }
        }
    }
}
