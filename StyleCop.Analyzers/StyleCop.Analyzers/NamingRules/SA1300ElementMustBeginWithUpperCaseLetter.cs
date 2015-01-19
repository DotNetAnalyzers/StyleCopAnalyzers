namespace StyleCop.Analyzers.NamingRules
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.Helpers;

    /// <summary>
    /// The name of a C# element does not begin with an upper-case letter.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs when the names of certain types of elements do not begin with an
    /// upper-case letter. The following types of elements should use an upper-case letter as the first letter of the
    /// element name: namespaces, classes, enums, structs, delegates, events, methods, and properties.</para>
    ///
    /// <para>In addition, any field which is public, internal, or marked with the const attribute should begin with an
    /// upper-case letter. Non-private readonly fields must also be named using an upper-case letter.</para>
    ///
    /// <para>If the field or variable name is intended to match the name of an item associated with Win32 or COM, and
    /// thus needs to begin with a lower-case letter, place the field or variable within a special <c>NativeMethods</c>
    /// class. A <c>NativeMethods</c> class is any class which contains a name ending in <c>NativeMethods</c>, and is
    /// intended as a placeholder for Win32 or COM wrappers. StyleCop will ignore this violation if the item is placed
    /// within a <c>NativeMethods</c> class.</para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SA1300ElementMustBeginWithUpperCaseLetter : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "SA1300";
        internal const string Title = "Element must begin with upper-case letter";
        internal const string MessageFormat = "Element '{0}' must begin with an uppercase letter";
        internal const string Category = "StyleCop.CSharp.NamingRules";
        internal const string Description = "The name of a C# element does not begin with an upper-case letter.";
        internal const string HelpLink = "http://www.stylecop.com/docs/SA1300.html";

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
            // Note: Interfaces are handled by SA1302
            // Note: Fields are handled by SA1303 through SA1311

            context.RegisterSyntaxNodeAction(HandleNamespaceDeclarationSyntax, SyntaxKind.NamespaceDeclaration);
            context.RegisterSyntaxNodeAction(HandleClassDeclarationSyntax, SyntaxKind.ClassDeclaration);
            context.RegisterSyntaxNodeAction(HandleEnumDeclarationSyntax, SyntaxKind.EnumDeclaration);
            context.RegisterSyntaxNodeAction(HandleStructDeclarationSyntax, SyntaxKind.StructDeclaration);
            context.RegisterSyntaxNodeAction(HandleDelegateDeclarationSyntax, SyntaxKind.DelegateDeclaration);
            context.RegisterSyntaxNodeAction(HandleEventDeclarationSyntax, SyntaxKind.EventDeclaration);
            context.RegisterSyntaxNodeAction(HandleEventFieldDeclarationSyntax, SyntaxKind.EventFieldDeclaration);
            context.RegisterSyntaxNodeAction(HandleMethodDeclarationSyntax, SyntaxKind.MethodDeclaration);
            context.RegisterSyntaxNodeAction(HandlePropertyDeclarationSyntax, SyntaxKind.PropertyDeclaration);
        }

        private void HandleNamespaceDeclarationSyntax(SyntaxNodeAnalysisContext context)
        {
            NameSyntax nameSyntax = ((NamespaceDeclarationSyntax)context.Node).Name;
            CheckNameSyntax(context, nameSyntax);
        }

        private void CheckNameSyntax(SyntaxNodeAnalysisContext context, NameSyntax nameSyntax)
        {
            if (nameSyntax == null || nameSyntax.IsMissing)
                return;

            QualifiedNameSyntax qualifiedNameSyntax = nameSyntax as QualifiedNameSyntax;
            if (qualifiedNameSyntax != null)
            {
                CheckNameSyntax(context, qualifiedNameSyntax.Left);
                CheckNameSyntax(context, qualifiedNameSyntax.Right);
                return;
            }

            SimpleNameSyntax simpleNameSyntax = nameSyntax as SimpleNameSyntax;
            if (simpleNameSyntax != null)
            {
                CheckElementNameToken(context, simpleNameSyntax.Identifier);
                return;
            }

            // TODO: any other cases?
        }

        private void HandleClassDeclarationSyntax(SyntaxNodeAnalysisContext context)
        {
            CheckElementNameToken(context, ((ClassDeclarationSyntax)context.Node).Identifier);
        }

        private void HandleEnumDeclarationSyntax(SyntaxNodeAnalysisContext context)
        {
            CheckElementNameToken(context, ((EnumDeclarationSyntax)context.Node).Identifier);
        }

        private void HandleStructDeclarationSyntax(SyntaxNodeAnalysisContext context)
        {
            CheckElementNameToken(context, ((StructDeclarationSyntax)context.Node).Identifier);
        }

        private void HandleDelegateDeclarationSyntax(SyntaxNodeAnalysisContext context)
        {
            CheckElementNameToken(context, ((DelegateDeclarationSyntax)context.Node).Identifier);
        }

        private void HandleEventDeclarationSyntax(SyntaxNodeAnalysisContext context)
        {
            CheckElementNameToken(context, ((EventDeclarationSyntax)context.Node).Identifier);
        }

        private void HandleEventFieldDeclarationSyntax(SyntaxNodeAnalysisContext context)
        {
            EventFieldDeclarationSyntax eventFieldDeclarationSyntax = (EventFieldDeclarationSyntax)context.Node;
            VariableDeclarationSyntax variableDeclarationSyntax = eventFieldDeclarationSyntax.Declaration;
            if (variableDeclarationSyntax == null || variableDeclarationSyntax.IsMissing)
                return;

            foreach (var declarator in variableDeclarationSyntax.Variables)
            {
                if (declarator == null || declarator.IsMissing)
                    continue;

                CheckElementNameToken(context, declarator.Identifier);
            }
        }

        private void HandleMethodDeclarationSyntax(SyntaxNodeAnalysisContext context)
        {
            CheckElementNameToken(context, ((MethodDeclarationSyntax)context.Node).Identifier);
        }

        private void HandlePropertyDeclarationSyntax(SyntaxNodeAnalysisContext context)
        {
            CheckElementNameToken(context, ((PropertyDeclarationSyntax)context.Node).Identifier);
        }

        private void CheckElementNameToken(SyntaxNodeAnalysisContext context, SyntaxToken identifier)
        {
            if (identifier.IsMissing)
                return;

            if (string.IsNullOrEmpty(identifier.Text))
                return;

            if (char.IsUpper(identifier.Text[0]))
                return;

            if (NamedTypeHelpers.IsContainedInNativeMethodsClass(context.Node))
                return;

            context.ReportDiagnostic(Diagnostic.Create(Descriptor, identifier.GetLocation(), identifier.Text));
        }
    }
}
