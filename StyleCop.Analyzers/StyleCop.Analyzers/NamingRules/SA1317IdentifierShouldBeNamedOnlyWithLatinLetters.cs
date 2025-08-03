// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.NamingRules
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.Settings.ObjectModel;

    /// <summary>
    /// An identifier name contains non-Latin letters.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs when an identifier name contains non-Latin letters.
    /// This is very common mistake to use Cyrillic letter instead of the Latin letter in some countries that use Cyrillic alphabet
    /// (Bulgaria, Russia, Serbia, Macedonia, Ukraine, etc.).
    /// For example, these letters ('a' ('а'), 'o' ('о'), 'e' ('е'), 'k' ('к') and few others) look the same in both Latin and Cyrillic alphabets.</para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class SA1317IdentifierShouldBeNamedOnlyWithLatinLetters : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1317IdentifierShouldBeNamedOnlyWithLatinLetters"/> analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1317";
        private const string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1317.md";
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(NamingResources.SA1317Title), NamingResources.ResourceManager, typeof(NamingResources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(NamingResources.SA1317MessageFormat), NamingResources.ResourceManager, typeof(NamingResources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(NamingResources.SA1317Description), NamingResources.ResourceManager, typeof(NamingResources));

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.NamingRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

        private static readonly Action<SyntaxNodeAnalysisContext, StyleCopSettings> HandleMemberDeclarationIdentifierNameAction = HandleMemberDeclarationIdentifierName;
        private static readonly Action<SyntaxNodeAnalysisContext, StyleCopSettings> HandleVariableDeclarationIdentifierNameAction = HandleVariableDeclarationIdentifierName;
        private static readonly Action<SyntaxNodeAnalysisContext, StyleCopSettings> HandleParameterIdentifierNameAction = HandleParameterIdentifierName;

        private static readonly ImmutableArray<SyntaxKind> MemberDeclarationKinds =
            ImmutableArray.Create(
                SyntaxKind.EventFieldDeclaration,
                SyntaxKind.EventDeclaration,
                SyntaxKind.FieldDeclaration,
                SyntaxKind.ConstructorDeclaration,
                SyntaxKind.DestructorDeclaration,
                SyntaxKind.MethodDeclaration,
                SyntaxKind.NamespaceDeclaration,
                SyntaxKind.PropertyDeclaration,
                SyntaxKind.EnumDeclaration,
                SyntaxKind.EnumMemberDeclaration,
                SyntaxKind.StructDeclaration,
                SyntaxKind.InterfaceDeclaration,
                SyntaxKind.DelegateDeclaration,
                SyntaxKind.ClassDeclaration);

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
            ImmutableArray.Create(Descriptor);

        /// <inheritdoc/>
        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            context.RegisterCompilationStartAction(context =>
            {
                context.RegisterSyntaxNodeAction(HandleMemberDeclarationIdentifierNameAction, MemberDeclarationKinds);
                context.RegisterSyntaxNodeAction(HandleVariableDeclarationIdentifierNameAction, SyntaxKind.VariableDeclaration);
                context.RegisterSyntaxNodeAction(HandleParameterIdentifierNameAction, SyntaxKind.Parameter);
            });
        }

        private static void HandleMemberDeclarationIdentifierName(SyntaxNodeAnalysisContext context, StyleCopSettings settings)
        {
            var syntax = (MemberDeclarationSyntax)context.Node;

            if (syntax is MethodDeclarationSyntax methodDeclaration)
            {
                CheckIdentifierName(context, methodDeclaration.Identifier);
                return;
            }

            if (syntax is ConstructorDeclarationSyntax constructorDeclaration)
            {
                CheckIdentifierName(context, constructorDeclaration.Identifier);
                return;
            }

            if (syntax is DestructorDeclarationSyntax destructorDeclaration)
            {
                CheckIdentifierName(context, destructorDeclaration.Identifier);
                return;
            }

            if (syntax is BaseTypeDeclarationSyntax typeDeclaration)
            {
                CheckIdentifierName(context, typeDeclaration.Identifier);
                return;
            }

            if (syntax is PropertyDeclarationSyntax propertyDeclaration)
            {
                CheckIdentifierName(context, propertyDeclaration.Identifier);
                return;
            }

            if (syntax is DelegateDeclarationSyntax delegateDeclaration)
            {
                CheckIdentifierName(context, delegateDeclaration.Identifier);
                return;
            }

            if (syntax is EnumMemberDeclarationSyntax enumMemberDeclaration)
            {
                CheckIdentifierName(context, enumMemberDeclaration.Identifier);
                return;
            }

            if (syntax is BaseFieldDeclarationSyntax baseFieldDeclarationSyntax)
            {
                var variableDeclarationSyntax = baseFieldDeclarationSyntax.Declaration;
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

                    CheckIdentifierName(context, declarator.Identifier);
                }
            }
        }

        private static void HandleVariableDeclarationIdentifierName(SyntaxNodeAnalysisContext context, StyleCopSettings settings)
        {
            var syntax = (VariableDeclarationSyntax)context.Node;
            if (syntax.Parent.IsKind(SyntaxKind.FieldDeclaration) || syntax.Parent.IsKind(SyntaxKind.EventFieldDeclaration))
            {
                // This diagnostic is only for local variables.
                return;
            }

            foreach (var variableDeclarator in syntax.Variables)
            {
                if (variableDeclarator is not null)
                {
                    CheckIdentifierName(context, variableDeclarator.Identifier);
                }
            }
        }

        private static void HandleParameterIdentifierName(SyntaxNodeAnalysisContext context, StyleCopSettings settings)
        {
            var syntax = (ParameterSyntax)context.Node;

            CheckIdentifierName(context, syntax.Identifier);
        }

        private static void CheckIdentifierName(SyntaxNodeAnalysisContext context, SyntaxToken identifier)
        {
            var name = identifier.Text;

            for (var i = 0; i < name.Length; i++)
            {
                if (char.IsLetter(name[i]) && name[i] is not ((>= 'a' and <= 'z') or (>= 'A' and <= 'Z')))
                {
                    context.ReportDiagnostic(Diagnostic.Create(Descriptor, identifier.GetLocation(), name, i));
                }
            }
        }
    }
}
