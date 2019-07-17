// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.NamingRules
{
    using System;
    using System.Collections.Immutable;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.Helpers;

    /// <summary>
    /// The name of a constant C# field should begin with an upper-case letter.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs when the name of a field marked with the <c>const</c> attribute does not
    /// begin with an upper-case letter.</para>
    ///
    /// <para>If the field or variable name is intended to match the name of an item associated with Win32 or COM, and
    /// thus needs to begin with a lower-case letter, place the field or variable within a special <c>NativeMethods</c>
    /// class. A <c>NativeMethods</c> class is any class which contains a name ending in <c>NativeMethods</c>, and is
    /// intended as a placeholder for Win32 or COM wrappers. StyleCop will ignore this violation if the item is placed
    /// within a <c>NativeMethods</c> class.</para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class SA1303ConstFieldNamesMustBeginWithUpperCaseLetter : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1303ConstFieldNamesMustBeginWithUpperCaseLetter"/>
        /// analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1303";
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(NamingResources.SA1303Title), NamingResources.ResourceManager, typeof(NamingResources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(NamingResources.SA1303MessageFormat), NamingResources.ResourceManager, typeof(NamingResources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(NamingResources.SA1303Description), NamingResources.ResourceManager, typeof(NamingResources));
        private const string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1303.md";

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.NamingRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

        private static readonly Action<SymbolAnalysisContext> FieldDeclarationAction = Analyzer.HandleFieldDeclaration;

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
            ImmutableArray.Create(Descriptor);

        /// <inheritdoc/>
        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            context.RegisterSymbolAction(FieldDeclarationAction, SymbolKind.Field);
        }

        private static class Analyzer
        {
            public static void HandleFieldDeclaration(SymbolAnalysisContext context)
            {
                if (!(context.Symbol is IFieldSymbol symbol) || !symbol.IsConst || symbol.ContainingType?.TypeKind == TypeKind.Enum)
                {
                    return;
                }

                if (NamedTypeHelpers.IsContainedInNativeMethodsClass(symbol.ContainingType))
                {
                    return;
                }

                /* This code uses char.IsLower(...) instead of !char.IsUpper(...) for all of the following reasons:
                 *  1. Fields starting with `_` should be reported as SA1309 instead of this diagnostic
                 *  2. Foreign languages may not have upper case variants for certain characters
                 *  3. This diagnostic appears targeted for "English" identifiers.
                 *
                 * See DotNetAnalyzers/StyleCopAnalyzers#369 for additional information:
                 * https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/369
                 */
                if (!string.IsNullOrEmpty(symbol.Name) &&
                    char.IsLower(symbol.Name[0]) &&
                    symbol.Locations.Any())
                {
                    foreach (var location in context.Symbol.Locations)
                    {
                        if (!location.IsInSource)
                        {
                            // assume symbols not defined in a source document are "out of reach"
                            return;
                        }
                    }

                    context.ReportDiagnostic(Diagnostic.Create(Descriptor, symbol.Locations[0]));
                }
            }
        }
    }
}
