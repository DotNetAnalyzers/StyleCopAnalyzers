// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.DocumentationRules
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.Helpers;
    using StyleCop.Analyzers.Settings.ObjectModel;

    /// <summary>
    /// An item within a C# enumeration is missing an XML documentation header.
    /// </summary>
    /// <remarks>
    /// <para>C# syntax provides a mechanism for inserting documentation for classes and elements directly into the
    /// code, through the use of XML documentation headers.</para>
    ///
    /// <para>A violation of this rule occurs when an item within an enumeration is missing a header. For
    /// example:</para>
    ///
    /// <code>
    /// /// &lt;summary&gt;
    /// /// Types of animals.
    /// /// &lt;/summary&gt;
    /// public enum Animals
    /// {
    ///     Dog,
    ///     Cat,
    ///     Horse
    /// }
    /// </code>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class SA1602EnumerationItemsMustBeDocumented : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1602EnumerationItemsMustBeDocumented"/> analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1602";
        private const string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1602.md";
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(DocumentationResources.SA1602Title), DocumentationResources.ResourceManager, typeof(DocumentationResources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(DocumentationResources.SA1602MessageFormat), DocumentationResources.ResourceManager, typeof(DocumentationResources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(DocumentationResources.SA1602Description), DocumentationResources.ResourceManager, typeof(DocumentationResources));

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.DocumentationRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

        private static readonly Action<SyntaxNodeAnalysisContext, StyleCopSettings> EnumMemberDeclarationAction = Analyzer.HandleEnumMemberDeclaration;

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
            ImmutableArray.Create(Descriptor);

        /// <inheritdoc/>
        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            context.RegisterSyntaxNodeAction(EnumMemberDeclarationAction, SyntaxKind.EnumMemberDeclaration);
        }

        private static class Analyzer
        {
            public static void HandleEnumMemberDeclaration(SyntaxNodeAnalysisContext context, StyleCopSettings settings)
            {
                if (context.GetDocumentationMode() == DocumentationMode.None)
                {
                    return;
                }

                EnumMemberDeclarationSyntax declaration = (EnumMemberDeclarationSyntax)context.Node;
                Accessibility declaredAccessibility = declaration.GetDeclaredAccessibility();
                Accessibility effectiveAccessibility = declaration.GetEffectiveAccessibility(context.SemanticModel, context.CancellationToken);
                if (SA1600ElementsMustBeDocumented.NeedsComment(settings.DocumentationRules, declaration.Kind(), declaration.Parent.Kind(), declaredAccessibility, effectiveAccessibility))
                {
                    if (!XmlCommentHelper.HasDocumentation(declaration))
                    {
                        context.ReportDiagnostic(Diagnostic.Create(Descriptor, declaration.Identifier.GetLocation()));
                    }
                }
            }
        }
    }
}
