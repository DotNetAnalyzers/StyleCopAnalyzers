// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.MaintainabilityRules
{
    using System;
    using System.Collections.Concurrent;
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
        private const string Title = "Fields must be private";
        private const string MessageFormat = "Field must be private";
        private const string Description = "A field within a C# class has an access modifier other than private.";
        private const string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1401.md";

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
            Analyzer analyzer = new Analyzer(context.Compilation.GetOrCreateGeneratedDocumentCache());
            context.RegisterSymbolAction(analyzer.AnalyzeField, SymbolKind.Field);
        }

        private sealed class Analyzer
        {
            private readonly ConcurrentDictionary<SyntaxTree, bool> generatedHeaderCache;

            public Analyzer(ConcurrentDictionary<SyntaxTree, bool> generatedHeaderCache)
            {
                this.generatedHeaderCache = generatedHeaderCache;
            }

            public void AnalyzeField(SymbolAnalysisContext symbolAnalysisContext)
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

                        if (location.SourceTree.IsGeneratedDocument(this.generatedHeaderCache, symbolAnalysisContext.CancellationToken))
                        {
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
