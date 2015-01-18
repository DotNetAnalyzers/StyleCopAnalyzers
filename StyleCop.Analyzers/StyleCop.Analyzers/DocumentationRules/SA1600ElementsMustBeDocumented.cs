namespace StyleCop.Analyzers.DocumentationRules
{
    using System.Linq;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Helpers;
    using System;

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
    public class SA1600ElementsMustBeDocumented : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "SA1600";
        internal const string Title = "Elements must be documented";
        internal const string MessageFormat = "Elements must be documented";
        internal const string Category = "StyleCop.CSharp.Documentation";
        internal const string Description = "A C# code element is missing a documentation header.";
        internal const string HelpLink = "http://www.stylecop.com/docs/SA1600.html";

        public static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, true, Description, HelpLink);

        private static readonly ImmutableArray<DiagnosticDescriptor> _supportedDiagnostics =
            ImmutableArray.Create(Descriptor);

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                return _supportedDiagnostics;
            }
        }

        /// <inheritdoc/>
        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(HandleTypeDeclaration, SyntaxKind.ClassDeclaration);
            context.RegisterSyntaxNodeAction(HandleTypeDeclaration, SyntaxKind.StructDeclaration);
            context.RegisterSyntaxNodeAction(HandleTypeDeclaration, SyntaxKind.InterfaceDeclaration);
            context.RegisterSyntaxNodeAction(HandleTypeDeclaration, SyntaxKind.EnumDeclaration);
            context.RegisterSyntaxNodeAction(HandleMethodDeclaration, SyntaxKind.MethodDeclaration);
            context.RegisterSyntaxNodeAction(HandleConstructorDeclaration, SyntaxKind.ConstructorDeclaration);
            context.RegisterSyntaxNodeAction(HandleDestructorDeclaration, SyntaxKind.DestructorDeclaration);
            context.RegisterSyntaxNodeAction(HandlePropertyDeclaration, SyntaxKind.PropertyDeclaration);
            context.RegisterSyntaxNodeAction(HandleIndexerDeclaration, SyntaxKind.IndexerDeclaration);
            context.RegisterSyntaxNodeAction(HandleFieldDeclaration, SyntaxKind.FieldDeclaration);
            context.RegisterSyntaxNodeAction(HandleDelegateDeclaration, SyntaxKind.DelegateDeclaration);
            context.RegisterSyntaxNodeAction(HandleEventDeclaration, SyntaxKind.EventDeclaration);
            context.RegisterSyntaxNodeAction(HandleEventFieldDeclaration, SyntaxKind.EventFieldDeclaration);
        }

        private void HandleTypeDeclaration(SyntaxNodeAnalysisContext context)
        {
            BaseTypeDeclarationSyntax declaration = context.Node as BaseTypeDeclarationSyntax;

            bool isNestedInClassOrStruct = IsNestedInClassOrStructRecursive(declaration.Parent);

            if (declaration != null && NeedsComment(declaration.Modifiers, isNestedInClassOrStruct ? SyntaxKind.PrivateKeyword : SyntaxKind.InternalKeyword))
            {
                if (!XmlCommentHelper.HasDocumentation(declaration))
                {
                    context.ReportDiagnostic(Diagnostic.Create(Descriptor, declaration.Identifier.GetLocation()));
                }
            }
        }

        private void HandleMethodDeclaration(SyntaxNodeAnalysisContext context)
        {
            MethodDeclarationSyntax declaration = context.Node as MethodDeclarationSyntax;
            SyntaxKind defaultVisibility = SyntaxKind.PrivateKeyword;

            if (IsDeclaredInInterface(declaration))
            {
                defaultVisibility = SyntaxKind.PublicKeyword;
            }

            if (declaration != null && NeedsComment(declaration.Modifiers, defaultVisibility))
            {
                if (!XmlCommentHelper.HasDocumentation(declaration))
                {
                    context.ReportDiagnostic(Diagnostic.Create(Descriptor, declaration.Identifier.GetLocation()));
                }
            }
        }

        private void HandleConstructorDeclaration(SyntaxNodeAnalysisContext context)
        {
            ConstructorDeclarationSyntax declaration = context.Node as ConstructorDeclarationSyntax;

            if (declaration != null && NeedsComment(declaration.Modifiers, SyntaxKind.PrivateKeyword))
            {
                if (!XmlCommentHelper.HasDocumentation(declaration))
                {
                    context.ReportDiagnostic(Diagnostic.Create(Descriptor, declaration.Identifier.GetLocation()));
                }
            }
        }

        private void HandleDestructorDeclaration(SyntaxNodeAnalysisContext context)
        {
            DestructorDeclarationSyntax declaration = context.Node as DestructorDeclarationSyntax;

            if (declaration != null)
            {
                if (!XmlCommentHelper.HasDocumentation(declaration))
                {
                    context.ReportDiagnostic(Diagnostic.Create(Descriptor, declaration.Identifier.GetLocation()));
                }
            }
        }

        private void HandlePropertyDeclaration(SyntaxNodeAnalysisContext context)
        {
            PropertyDeclarationSyntax declaration = context.Node as PropertyDeclarationSyntax;
            SyntaxKind defaultVisibility = SyntaxKind.PrivateKeyword;

            if (IsDeclaredInInterface(declaration))
            {
                defaultVisibility = SyntaxKind.PublicKeyword;
            }

            if (declaration != null && NeedsComment(declaration.Modifiers, defaultVisibility))
            {
                if (!XmlCommentHelper.HasDocumentation(declaration))
                { 
                    context.ReportDiagnostic(Diagnostic.Create(Descriptor, declaration.Identifier.GetLocation()));
                }
            }
        }



        private void HandleIndexerDeclaration(SyntaxNodeAnalysisContext context)
        {
            IndexerDeclarationSyntax declaration = context.Node as IndexerDeclarationSyntax;
            SyntaxKind defaultVisibility = SyntaxKind.PrivateKeyword;

            if (IsDeclaredInInterface(declaration))
            {
                defaultVisibility = SyntaxKind.PublicKeyword;
            }

            if (declaration != null && NeedsComment(declaration.Modifiers, defaultVisibility))
            {
                if (!XmlCommentHelper.HasDocumentation(declaration))
                {
                    context.ReportDiagnostic(Diagnostic.Create(Descriptor, declaration.ThisKeyword.GetLocation()));
                }
            }
        }

        private void HandleFieldDeclaration(SyntaxNodeAnalysisContext context)
        {
            FieldDeclarationSyntax declaration = context.Node as FieldDeclarationSyntax;
            var variableDeclaration = declaration?.Declaration;

            if (variableDeclaration != null && NeedsComment(declaration.Modifiers, SyntaxKind.PrivateKeyword))
            {
                if (!XmlCommentHelper.HasDocumentation(declaration))
                {
                    var locations = variableDeclaration.Variables.Select(v => v.Identifier.GetLocation()).ToArray();
                    foreach (var location in locations)
                    {
                        context.ReportDiagnostic(Diagnostic.Create(Descriptor, location));
                    }
                }
            }
        }

        private void HandleDelegateDeclaration(SyntaxNodeAnalysisContext context)
        {
            DelegateDeclarationSyntax declaration = context.Node as DelegateDeclarationSyntax;

            bool isNestedInClassOrStruct = IsNestedInClassOrStructRecursive(declaration.Parent);

            if (declaration != null && NeedsComment(declaration.Modifiers, isNestedInClassOrStruct ? SyntaxKind.PrivateKeyword : SyntaxKind.InternalKeyword))
            {
                if (!XmlCommentHelper.HasDocumentation(declaration))
                {
                    context.ReportDiagnostic(Diagnostic.Create(Descriptor, declaration.Identifier.GetLocation()));
                }
            }
        }

        private void HandleEventDeclaration(SyntaxNodeAnalysisContext context)
        {
            EventDeclarationSyntax declaration = context.Node as EventDeclarationSyntax;
            SyntaxKind defaultVisibility = SyntaxKind.PrivateKeyword;

            if (IsDeclaredInInterface(declaration))
            {
                defaultVisibility = SyntaxKind.PublicKeyword;
            }

            if (declaration != null && NeedsComment(declaration.Modifiers, defaultVisibility))
            {
                if (!XmlCommentHelper.HasDocumentation(declaration))
                {
                    context.ReportDiagnostic(Diagnostic.Create(Descriptor, declaration.Identifier.GetLocation()));
                }
            }
        }

        private void HandleEventFieldDeclaration(SyntaxNodeAnalysisContext context)
        {
            EventFieldDeclarationSyntax declaration = context.Node as EventFieldDeclarationSyntax;
            var variableDeclaration = declaration?.Declaration;

            if (variableDeclaration != null && NeedsComment(declaration.Modifiers, SyntaxKind.PrivateKeyword))
            {
                if (!XmlCommentHelper.HasDocumentation(declaration))
                {
                    var locations = variableDeclaration.Variables.Select(v => v.Identifier.GetLocation()).ToArray();
                    foreach (var location in locations)
                    {
                        context.ReportDiagnostic(Diagnostic.Create(Descriptor, location));
                    }
                }
            }
        }

        private bool NeedsComment(SyntaxTokenList modifiers, SyntaxKind defaultModifier)
        {

            return (modifiers.Any(SyntaxKind.PublicKeyword)
                || modifiers.Any(SyntaxKind.ProtectedKeyword)
                || modifiers.Any(SyntaxKind.InternalKeyword)
                || defaultModifier == SyntaxKind.PublicKeyword
                || defaultModifier == SyntaxKind.ProtectedKeyword
                || defaultModifier == SyntaxKind.InternalKeyword)
                // Ignore partial classes because they get reported as SA1601
                && !modifiers.Any(SyntaxKind.PartialKeyword);
        }

        private bool IsNestedInClassOrStructRecursive(SyntaxNode parent)
        {
            if (parent == null)
            {
                return false;
            }
            if (parent is BaseTypeDeclarationSyntax)
            {
                return true;
            }
            return IsNestedInClassOrStructRecursive(parent.Parent);
        }

        private bool IsDeclaredInInterface(SyntaxNode parent)
        {
            if (parent == null)
            {
                return false;
            }
            if (parent is InterfaceDeclarationSyntax)
            {
                return true;
            }
            return IsDeclaredInInterface(parent.Parent);
        }
    }
}
