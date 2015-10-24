// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.DocumentationRules
{
    using System;
    using System.Collections.Immutable;
    using System.Linq;
    using Helpers;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Settings.ObjectModel;

    /// <summary>
    /// A C# code element is missing a documentation header.
    /// </summary>
    /// <remarks>
    /// <para>C# syntax provides a mechanism for inserting documentation for classes and elements directly into the
    /// code, through the use of Xml documentation headers. For an introduction to these headers and a description of
    /// the header syntax, see the following article:
    /// <see href="http://msdn.microsoft.com/en-us/magazine/cc302121.aspx"/>.</para>
    ///
    /// <para>A violation of this rule occurs if an element is completely missing a documentation header, or if the
    /// header is empty. In C# the following types of elements can have documentation headers: classes, constructors,
    /// delegates, enums, events, finalizers, indexers, interfaces, methods, properties, and structs.</para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class SA1600ElementsMustBeDocumented : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1600ElementsMustBeDocumented"/> analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1600";
        private const string Title = "Elements must be documented";
        private const string MessageFormat = "Elements must be documented";
        private const string Description = "A C# code element is missing a documentation header.";
        private const string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1600.md";

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.DocumentationRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

        private static readonly ImmutableArray<SyntaxKind> BaseTypeDeclarationKinds =
            ImmutableArray.Create(SyntaxKind.ClassDeclaration, SyntaxKind.StructDeclaration, SyntaxKind.InterfaceDeclaration, SyntaxKind.EnumDeclaration);

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
            Analyzer analyzer = new Analyzer(context.Options);
            context.RegisterSyntaxNodeActionHonorExclusions(analyzer.HandleBaseTypeDeclaration, BaseTypeDeclarationKinds);
            context.RegisterSyntaxNodeActionHonorExclusions(analyzer.HandleMethodDeclaration, SyntaxKind.MethodDeclaration);
            context.RegisterSyntaxNodeActionHonorExclusions(analyzer.HandleConstructorDeclaration, SyntaxKind.ConstructorDeclaration);
            context.RegisterSyntaxNodeActionHonorExclusions(analyzer.HandleDestructorDeclaration, SyntaxKind.DestructorDeclaration);
            context.RegisterSyntaxNodeActionHonorExclusions(analyzer.HandlePropertyDeclaration, SyntaxKind.PropertyDeclaration);
            context.RegisterSyntaxNodeActionHonorExclusions(analyzer.HandleIndexerDeclaration, SyntaxKind.IndexerDeclaration);
            context.RegisterSyntaxNodeActionHonorExclusions(analyzer.HandleFieldDeclaration, SyntaxKind.FieldDeclaration);
            context.RegisterSyntaxNodeActionHonorExclusions(analyzer.HandleDelegateDeclaration, SyntaxKind.DelegateDeclaration);
            context.RegisterSyntaxNodeActionHonorExclusions(analyzer.HandleEventDeclaration, SyntaxKind.EventDeclaration);
            context.RegisterSyntaxNodeActionHonorExclusions(analyzer.HandleEventFieldDeclaration, SyntaxKind.EventFieldDeclaration);
        }

        private class Analyzer
        {
            private readonly DocumentationSettings documentationSettings;

            public Analyzer(AnalyzerOptions options)
            {
                StyleCopSettings settings = options.GetStyleCopSettings();
                this.documentationSettings = settings.DocumentationRules;
            }

            public void HandleBaseTypeDeclaration(SyntaxNodeAnalysisContext context)
            {
                if (context.GetDocumentationMode() != DocumentationMode.Diagnose)
                {
                    return;
                }

                BaseTypeDeclarationSyntax declaration = (BaseTypeDeclarationSyntax)context.Node;
                if (declaration.Modifiers.Any(SyntaxKind.PartialKeyword))
                {
                    // Handled by SA1601
                    return;
                }

                Accessibility declaredAccessibility = declaration.GetDeclaredAccessibility(context.SemanticModel, context.CancellationToken);
                Accessibility effectiveAccessibility = declaration.GetEffectiveAccessibility(context.SemanticModel, context.CancellationToken);
                if (this.NeedsComment(declaration.Kind(), declaration.Parent.Kind(), declaredAccessibility, effectiveAccessibility))
                {
                    if (!XmlCommentHelper.HasDocumentation(declaration))
                    {
                        context.ReportDiagnostic(Diagnostic.Create(Descriptor, declaration.Identifier.GetLocation()));
                    }
                }
            }

            public void HandleMethodDeclaration(SyntaxNodeAnalysisContext context)
            {
                if (context.GetDocumentationMode() != DocumentationMode.Diagnose)
                {
                    return;
                }

                MethodDeclarationSyntax declaration = (MethodDeclarationSyntax)context.Node;

                Accessibility declaredAccessibility = declaration.GetDeclaredAccessibility(context.SemanticModel, context.CancellationToken);
                Accessibility effectiveAccessibility = declaration.GetEffectiveAccessibility(context.SemanticModel, context.CancellationToken);
                if (this.NeedsComment(declaration.Kind(), declaration.Parent.Kind(), declaredAccessibility, effectiveAccessibility))
                {
                    if (!XmlCommentHelper.HasDocumentation(declaration))
                    {
                        context.ReportDiagnostic(Diagnostic.Create(Descriptor, declaration.Identifier.GetLocation()));
                    }
                }
            }

            public void HandleConstructorDeclaration(SyntaxNodeAnalysisContext context)
            {
                if (context.GetDocumentationMode() != DocumentationMode.Diagnose)
                {
                    return;
                }

                ConstructorDeclarationSyntax declaration = (ConstructorDeclarationSyntax)context.Node;

                Accessibility declaredAccessibility = declaration.GetDeclaredAccessibility(context.SemanticModel, context.CancellationToken);
                Accessibility effectiveAccessibility = declaration.GetEffectiveAccessibility(context.SemanticModel, context.CancellationToken);
                if (this.NeedsComment(declaration.Kind(), declaration.Parent.Kind(), declaredAccessibility, effectiveAccessibility))
                {
                    if (!XmlCommentHelper.HasDocumentation(declaration))
                    {
                        context.ReportDiagnostic(Diagnostic.Create(Descriptor, declaration.Identifier.GetLocation()));
                    }
                }
            }

            public void HandleDestructorDeclaration(SyntaxNodeAnalysisContext context)
            {
                if (context.GetDocumentationMode() != DocumentationMode.Diagnose)
                {
                    return;
                }

                DestructorDeclarationSyntax declaration = (DestructorDeclarationSyntax)context.Node;

                Accessibility declaredAccessibility = declaration.GetDeclaredAccessibility(context.SemanticModel, context.CancellationToken);
                Accessibility effectiveAccessibility = declaration.GetEffectiveAccessibility(context.SemanticModel, context.CancellationToken);
                if (this.NeedsComment(declaration.Kind(), declaration.Parent.Kind(), declaredAccessibility, effectiveAccessibility))
                {
                    if (!XmlCommentHelper.HasDocumentation(declaration))
                    {
                        context.ReportDiagnostic(Diagnostic.Create(Descriptor, declaration.Identifier.GetLocation()));
                    }
                }
            }

            public void HandlePropertyDeclaration(SyntaxNodeAnalysisContext context)
            {
                if (context.GetDocumentationMode() != DocumentationMode.Diagnose)
                {
                    return;
                }

                PropertyDeclarationSyntax declaration = (PropertyDeclarationSyntax)context.Node;

                Accessibility declaredAccessibility = declaration.GetDeclaredAccessibility(context.SemanticModel, context.CancellationToken);
                Accessibility effectiveAccessibility = declaration.GetEffectiveAccessibility(context.SemanticModel, context.CancellationToken);
                if (this.NeedsComment(declaration.Kind(), declaration.Parent.Kind(), declaredAccessibility, effectiveAccessibility))
                {
                    if (!XmlCommentHelper.HasDocumentation(declaration))
                    {
                        context.ReportDiagnostic(Diagnostic.Create(Descriptor, declaration.Identifier.GetLocation()));
                    }
                }
            }

            public void HandleIndexerDeclaration(SyntaxNodeAnalysisContext context)
            {
                if (context.GetDocumentationMode() != DocumentationMode.Diagnose)
                {
                    return;
                }

                IndexerDeclarationSyntax declaration = (IndexerDeclarationSyntax)context.Node;

                Accessibility declaredAccessibility = declaration.GetDeclaredAccessibility(context.SemanticModel, context.CancellationToken);
                Accessibility effectiveAccessibility = declaration.GetEffectiveAccessibility(context.SemanticModel, context.CancellationToken);
                if (this.NeedsComment(declaration.Kind(), declaration.Parent.Kind(), declaredAccessibility, effectiveAccessibility))
                {
                    if (!XmlCommentHelper.HasDocumentation(declaration))
                    {
                        context.ReportDiagnostic(Diagnostic.Create(Descriptor, declaration.ThisKeyword.GetLocation()));
                    }
                }
            }

            public void HandleFieldDeclaration(SyntaxNodeAnalysisContext context)
            {
                if (context.GetDocumentationMode() != DocumentationMode.Diagnose)
                {
                    return;
                }

                FieldDeclarationSyntax declaration = (FieldDeclarationSyntax)context.Node;
                var variableDeclaration = declaration.Declaration;

                Accessibility declaredAccessibility = declaration.GetDeclaredAccessibility(context.SemanticModel, context.CancellationToken);
                Accessibility effectiveAccessibility = declaration.GetEffectiveAccessibility(context.SemanticModel, context.CancellationToken);
                if (variableDeclaration != null && this.NeedsComment(declaration.Kind(), declaration.Parent.Kind(), declaredAccessibility, effectiveAccessibility))
                {
                    if (!XmlCommentHelper.HasDocumentation(declaration))
                    {
                        var locations = variableDeclaration.Variables.Select(v => v.Identifier.GetLocation());
                        foreach (var location in locations)
                        {
                            context.ReportDiagnostic(Diagnostic.Create(Descriptor, location));
                        }
                    }
                }
            }

            public void HandleDelegateDeclaration(SyntaxNodeAnalysisContext context)
            {
                if (context.GetDocumentationMode() != DocumentationMode.Diagnose)
                {
                    return;
                }

                DelegateDeclarationSyntax declaration = (DelegateDeclarationSyntax)context.Node;

                Accessibility declaredAccessibility = declaration.GetDeclaredAccessibility(context.SemanticModel, context.CancellationToken);
                Accessibility effectiveAccessibility = declaration.GetEffectiveAccessibility(context.SemanticModel, context.CancellationToken);
                if (this.NeedsComment(declaration.Kind(), declaration.Parent.Kind(), declaredAccessibility, effectiveAccessibility))
                {
                    if (!XmlCommentHelper.HasDocumentation(declaration))
                    {
                        context.ReportDiagnostic(Diagnostic.Create(Descriptor, declaration.Identifier.GetLocation()));
                    }
                }
            }

            public void HandleEventDeclaration(SyntaxNodeAnalysisContext context)
            {
                if (context.GetDocumentationMode() != DocumentationMode.Diagnose)
                {
                    return;
                }

                EventDeclarationSyntax declaration = (EventDeclarationSyntax)context.Node;

                Accessibility declaredAccessibility = declaration.GetDeclaredAccessibility(context.SemanticModel, context.CancellationToken);
                Accessibility effectiveAccessibility = declaration.GetEffectiveAccessibility(context.SemanticModel, context.CancellationToken);
                if (this.NeedsComment(declaration.Kind(), declaration.Parent.Kind(), declaredAccessibility, effectiveAccessibility))
                {
                    if (!XmlCommentHelper.HasDocumentation(declaration))
                    {
                        context.ReportDiagnostic(Diagnostic.Create(Descriptor, declaration.Identifier.GetLocation()));
                    }
                }
            }

            public void HandleEventFieldDeclaration(SyntaxNodeAnalysisContext context)
            {
                if (context.GetDocumentationMode() != DocumentationMode.Diagnose)
                {
                    return;
                }

                EventFieldDeclarationSyntax declaration = (EventFieldDeclarationSyntax)context.Node;
                VariableDeclarationSyntax variableDeclaration = declaration.Declaration;

                Accessibility declaredAccessibility = declaration.GetDeclaredAccessibility(context.SemanticModel, context.CancellationToken);
                Accessibility effectiveAccessibility = declaration.GetEffectiveAccessibility(context.SemanticModel, context.CancellationToken);
                if (variableDeclaration != null && this.NeedsComment(declaration.Kind(), declaration.Parent.Kind(), declaredAccessibility, effectiveAccessibility))
                {
                    if (!XmlCommentHelper.HasDocumentation(declaration))
                    {
                        var locations = variableDeclaration.Variables.Select(v => v.Identifier.GetLocation());
                        foreach (var location in locations)
                        {
                            context.ReportDiagnostic(Diagnostic.Create(Descriptor, location));
                        }
                    }
                }
            }

            private bool NeedsComment(SyntaxKind syntaxKind, SyntaxKind parentSyntaxKind, Accessibility declaredAccessibility, Accessibility effectiveAccessibility)
            {
                if (this.documentationSettings.DocumentInterfaces
                    && (syntaxKind == SyntaxKind.InterfaceDeclaration || parentSyntaxKind == SyntaxKind.InterfaceDeclaration))
                {
                    // DocumentInterfaces => all interfaces must be documented
                    return true;
                }

                if (syntaxKind == SyntaxKind.FieldDeclaration && this.documentationSettings.DocumentPrivateFields)
                {
                    // DocumentPrivateFields => all fields must be documented
                    return true;
                }

                if (this.documentationSettings.DocumentPrivateElements)
                {
                    if (syntaxKind == SyntaxKind.FieldDeclaration && declaredAccessibility == Accessibility.Private)
                    {
                        // Handled by DocumentPrivateFields
                        return false;
                    }

                    // DocumentPrivateMembers => everything except declared private fields must be documented
                    return true;
                }

                switch (effectiveAccessibility)
                {
                case Accessibility.Public:
                case Accessibility.Protected:
                case Accessibility.ProtectedOrInternal:
                    // These items are part of the exposed API surface => document if configured
                    return this.documentationSettings.DocumentExposedElements;

                case Accessibility.ProtectedAndInternal:
                case Accessibility.Internal:
                    // These items are part of the internal API surface => document if configured
                    return this.documentationSettings.DocumentInternalElements;

                case Accessibility.NotApplicable:
                case Accessibility.Private:
                default:
                    return false;
                }
            }
        }
    }
}
