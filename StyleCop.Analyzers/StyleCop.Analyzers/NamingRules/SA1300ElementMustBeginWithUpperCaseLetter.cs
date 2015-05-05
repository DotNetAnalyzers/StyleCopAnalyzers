namespace StyleCop.Analyzers.NamingRules
{
    using System;
    using System.Collections.Immutable;
    using System.Linq;
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
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1300ElementMustBeginWithUpperCaseLetter"/> analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1300";
        private const string Title = "Element must begin with upper-case letter";
        private const string MessageFormat = "Element '{0}' must begin with an uppercase letter";
        private const string Category = "StyleCop.CSharp.NamingRules";
        private const string Description = "The name of a C# element does not begin with an upper-case letter.";
        private const string HelpLink = "http://www.stylecop.com/docs/SA1300.html";

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, true, Description, HelpLink);

        private static readonly ImmutableArray<DiagnosticDescriptor> SupportedDiagnosticsValue =
            ImmutableArray.Create(Descriptor);

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                return SupportedDiagnosticsValue;
            }
        }

        /// <inheritdoc/>
        public override void Initialize(AnalysisContext context)
        {
            // Note: Interfaces are handled by SA1302
            // Note: Fields are handled by SA1303 through SA1311

            context.RegisterSyntaxNodeActionHonorExclusions(this.HandleNamespaceDeclarationSyntax, SyntaxKind.NamespaceDeclaration);
            context.RegisterSyntaxNodeActionHonorExclusions(this.HandleClassDeclarationSyntax, SyntaxKind.ClassDeclaration);
            context.RegisterSyntaxNodeActionHonorExclusions(this.HandleEnumDeclarationSyntax, SyntaxKind.EnumDeclaration);
            context.RegisterSyntaxNodeActionHonorExclusions(this.HandleStructDeclarationSyntax, SyntaxKind.StructDeclaration);
            context.RegisterSyntaxNodeActionHonorExclusions(this.HandleDelegateDeclarationSyntax, SyntaxKind.DelegateDeclaration);
            context.RegisterSyntaxNodeActionHonorExclusions(this.HandleEventDeclarationSyntax, SyntaxKind.EventDeclaration);
            context.RegisterSyntaxNodeActionHonorExclusions(this.HandleEventFieldDeclarationSyntax, SyntaxKind.EventFieldDeclaration);
            context.RegisterSyntaxNodeActionHonorExclusions(this.HandleMethodDeclarationSyntax, SyntaxKind.MethodDeclaration);
            context.RegisterSyntaxNodeActionHonorExclusions(this.HandlePropertyDeclarationSyntax, SyntaxKind.PropertyDeclaration);
        }

        private void HandleNamespaceDeclarationSyntax(SyntaxNodeAnalysisContext context)
        {
            NameSyntax nameSyntax = ((NamespaceDeclarationSyntax)context.Node).Name;
            this.CheckNameSyntax(context, nameSyntax);
        }

        private void CheckNameSyntax(SyntaxNodeAnalysisContext context, NameSyntax nameSyntax)
        {
            if (nameSyntax == null || nameSyntax.IsMissing)
            {
                return;
            }

            QualifiedNameSyntax qualifiedNameSyntax = nameSyntax as QualifiedNameSyntax;
            if (qualifiedNameSyntax != null)
            {
                this.CheckNameSyntax(context, qualifiedNameSyntax.Left);
                this.CheckNameSyntax(context, qualifiedNameSyntax.Right);
                return;
            }

            SimpleNameSyntax simpleNameSyntax = nameSyntax as SimpleNameSyntax;
            if (simpleNameSyntax != null)
            {
                this.CheckElementNameToken(context, simpleNameSyntax.Identifier);
                return;
            }

            // TODO: any other cases?
        }

        private void HandleClassDeclarationSyntax(SyntaxNodeAnalysisContext context)
        {
            this.CheckElementNameToken(context, ((ClassDeclarationSyntax)context.Node).Identifier);
        }

        private void HandleEnumDeclarationSyntax(SyntaxNodeAnalysisContext context)
        {
            this.CheckElementNameToken(context, ((EnumDeclarationSyntax)context.Node).Identifier);
        }

        private void HandleStructDeclarationSyntax(SyntaxNodeAnalysisContext context)
        {
            this.CheckElementNameToken(context, ((StructDeclarationSyntax)context.Node).Identifier);
        }

        private void HandleDelegateDeclarationSyntax(SyntaxNodeAnalysisContext context)
        {
            this.CheckElementNameToken(context, ((DelegateDeclarationSyntax)context.Node).Identifier);
        }

        private void HandleEventDeclarationSyntax(SyntaxNodeAnalysisContext context)
        {
            var eventDeclaration = (EventDeclarationSyntax)context.Node;
            if (eventDeclaration.Modifiers.Any(SyntaxKind.OverrideKeyword))
            {
                // Don't analyze an overridden event.
                return;
            }

            this.CheckElementNameToken(context, eventDeclaration.Identifier);
        }

        private void HandleEventFieldDeclarationSyntax(SyntaxNodeAnalysisContext context)
        {
            EventFieldDeclarationSyntax eventFieldDeclarationSyntax = (EventFieldDeclarationSyntax)context.Node;
            VariableDeclarationSyntax variableDeclarationSyntax = eventFieldDeclarationSyntax.Declaration;
            if (variableDeclarationSyntax == null || variableDeclarationSyntax.IsMissing)
            {
                return;
            }

            foreach (var declarator in variableDeclarationSyntax.Variables)
            {
                if (declarator == null || declarator.IsMissing)
                {
                    continue;
                }

                this.CheckElementNameToken(context, declarator.Identifier);
            }
        }

        private void HandleMethodDeclarationSyntax(SyntaxNodeAnalysisContext context)
        {
            var methodDeclaration = (MethodDeclarationSyntax)context.Node;
            if (methodDeclaration.Modifiers.Any(SyntaxKind.OverrideKeyword))
            {
                // Don't analyze an overridden method.
                return;
            }

            this.CheckElementNameToken(context, methodDeclaration.Identifier);
        }

        private void HandlePropertyDeclarationSyntax(SyntaxNodeAnalysisContext context)
        {
            var propertyDeclaration = (PropertyDeclarationSyntax)context.Node;
            if (propertyDeclaration.Modifiers.Any(SyntaxKind.OverrideKeyword))
            {
                // Don't analyze an overridden property.
                return;
            }

            this.CheckElementNameToken(context, propertyDeclaration.Identifier);
        }

        private void CheckElementNameToken(SyntaxNodeAnalysisContext context, SyntaxToken identifier)
        {
            if (identifier.IsMissing)
            {
                return;
            }

            if (string.IsNullOrEmpty(identifier.ValueText))
            {
                return;
            }

            /* This code uses char.IsLower(...) instead of !char.IsUpper(...) for all of the following reasons:
             *  1. Foreign languages may not have upper case variants for certain characters
             *  2. This diagnostic appears targeted for "English" identifiers.
             *
             * See DotNetAnalyzers/StyleCopAnalyzers#369 for additional information:
             * https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/369
             */
            if (!char.IsLower(identifier.ValueText[0]) && identifier.ValueText[0] != '_')
            {
                return;
            }

            if (NamedTypeHelpers.IsContainedInNativeMethodsClass(context.Node))
            {
                return;
            }

            var symbolInfo = context.SemanticModel.GetDeclaredSymbol(identifier.Parent);
            if (symbolInfo != null && NamedTypeHelpers.IsImplementingAnInterfaceMember(symbolInfo))
            {
                return;
            }

            context.ReportDiagnostic(Diagnostic.Create(Descriptor, identifier.GetLocation(), identifier.ValueText));
        }
    }
}
