// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.ReadabilityRules
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// The <see cref="Nullable{T}"/> type has been defined not using the C# shorthand. For example,
    /// <c>Nullable&lt;DateTime&gt;</c> has been used instead of the preferred <c>DateTime?</c>.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs whenever the <see cref="Nullable{T}"/> type has been defined without using
    /// the shorthand C# style.</para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    [NoCodeFix("Provided by Visual Studio")]
    internal class SA1125UseShorthandForNullableTypes : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1125UseShorthandForNullableTypes"/> analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1125";
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(ReadabilityResources.SA1125Title), ReadabilityResources.ResourceManager, typeof(ReadabilityResources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(ReadabilityResources.SA1125MessageFormat), ReadabilityResources.ResourceManager, typeof(ReadabilityResources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(ReadabilityResources.SA1125Description), ReadabilityResources.ResourceManager, typeof(ReadabilityResources));
        private static readonly string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1125.md";

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.ReadabilityRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

        private static readonly Action<CompilationStartAnalysisContext> CompilationStartAction = HandleCompilationStart;
        private static readonly Action<SyntaxNodeAnalysisContext> GenericNameAction = HandleGenericName;

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
            context.RegisterSyntaxNodeActionHonorExclusions(GenericNameAction, SyntaxKind.GenericName);
        }

        private static void HandleGenericName(SyntaxNodeAnalysisContext context)
        {
            GenericNameSyntax genericNameSyntax = (GenericNameSyntax)context.Node;

            if (genericNameSyntax.Identifier.IsMissing || genericNameSyntax.Identifier.Text != "Nullable")
            {
                return;
            }

            // This covers the specific forms in an XML comment which cannot be simplified
            if (genericNameSyntax.Parent is NameMemberCrefSyntax)
            {
                // cref="Nullable{T}"
                return;
            }
            else if (genericNameSyntax.Parent is QualifiedCrefSyntax)
            {
                // cref="Nullable{T}.Value"
                return;
            }
            else if (genericNameSyntax.Parent is QualifiedNameSyntax && genericNameSyntax.Parent.Parent is QualifiedCrefSyntax)
            {
                // cref="System.Nullable{T}.Value"
                return;
            }

            // The shorthand syntax is not available in using directives (covers standard, alias, and static)
            if (genericNameSyntax.FirstAncestorOrSelf<UsingDirectiveSyntax>() != null)
            {
                return;
            }

            // This covers special cases of static and instance member access through the type name. It also covers most
            // special cases for the `nameof` expression.
            if (genericNameSyntax.Parent is MemberAccessExpressionSyntax)
            {
                return;
            }

            // This covers the special case of `nameof(Nullable<int>)`
            if (genericNameSyntax.Parent is ArgumentSyntax)
            {
                return;
            }

            SemanticModel semanticModel = context.SemanticModel;
            INamedTypeSymbol symbol = semanticModel.GetSymbolInfo(genericNameSyntax, context.CancellationToken).Symbol as INamedTypeSymbol;
            if (symbol?.OriginalDefinition?.SpecialType != SpecialType.System_Nullable_T)
            {
                return;
            }

            if (symbol.IsUnboundGenericType)
            {
                // There is never a shorthand syntax for the open generic Nullable<>
                return;
            }

            SyntaxNode locationNode = genericNameSyntax;
            if (genericNameSyntax.Parent is QualifiedNameSyntax)
            {
                locationNode = genericNameSyntax.Parent;
            }

            // Use shorthand for nullable types
            context.ReportDiagnostic(Diagnostic.Create(Descriptor, locationNode.GetLocation()));
        }
    }
}
