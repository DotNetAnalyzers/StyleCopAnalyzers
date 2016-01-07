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
    /// code, through the use of XML documentation headers. For an introduction to these headers and a description of
    /// the header syntax, see the following article:
    /// <see href="http://msdn.microsoft.com/en-us/magazine/cc302121.aspx">XML Comments Let You Build Documentation
    /// Directly From Your Visual Studio .NET Source Files</see>.</para>
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
        private const string Title = "Enumeration items must be documented";
        private const string MessageFormat = "Enumeration items must be documented";
        private const string Description = "An item within a C# enumeration is missing an Xml documentation header.";
        private const string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1602.md";

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.DocumentationRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

        private static readonly Action<CompilationStartAnalysisContext> CompilationStartAction = HandleCompilationStart;
        private static readonly Action<SyntaxNodeAnalysisContext, StyleCopSettings> EnumMemberDeclarationAction = Analyzer.HandleEnumMemberDeclaration;

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
            context.RegisterSyntaxNodeActionHonorExclusions(EnumMemberDeclarationAction, SyntaxKind.EnumMemberDeclaration);
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
                if (NeedsComment(settings.DocumentationRules, declaration.Kind(), declaration.Parent.Kind(), declaredAccessibility, effectiveAccessibility))
                {
                    if (!XmlCommentHelper.HasDocumentation(declaration))
                    {
                        context.ReportDiagnostic(Diagnostic.Create(Descriptor, declaration.Identifier.GetLocation()));
                    }
                }
            }

            private static bool NeedsComment(DocumentationSettings documentationSettings, SyntaxKind syntaxKind, SyntaxKind parentSyntaxKind, Accessibility declaredAccessibility, Accessibility effectiveAccessibility)
            {
                if (documentationSettings.DocumentInterfaces
                    && (syntaxKind == SyntaxKind.InterfaceDeclaration || parentSyntaxKind == SyntaxKind.InterfaceDeclaration))
                {
                    // DocumentInterfaces => all interfaces must be documented
                    return true;
                }

                if (documentationSettings.DocumentPrivateElements)
                {
                    // DocumentPrivateMembers => everything except declared private fields must be documented
                    return true;
                }

                switch (effectiveAccessibility)
                {
                case Accessibility.Public:
                case Accessibility.Protected:
                case Accessibility.ProtectedOrInternal:
                    // These items are part of the exposed API surface => document if configured
                    return documentationSettings.DocumentExposedElements;

                case Accessibility.ProtectedAndInternal:
                case Accessibility.Internal:
                    // These items are part of the internal API surface => document if configured
                    return documentationSettings.DocumentInternalElements;

                case Accessibility.NotApplicable:
                case Accessibility.Private:
                default:
                    return false;
                }
            }
        }
    }
}
