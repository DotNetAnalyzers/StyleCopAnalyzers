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

    /// <summary>
    /// The name of a static readonly field does not begin with an upper-case letter.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs when the name of a static readonly field begins with a lower-case
    /// letter.</para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class SA1311StaticReadonlyFieldsMustBeginWithUpperCaseLetter : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1311StaticReadonlyFieldsMustBeginWithUpperCaseLetter"/>
        /// analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1311";
        private const string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1311.md";
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(NamingResources.SA1311Title), NamingResources.ResourceManager, typeof(NamingResources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(NamingResources.SA1311MessageFormat), NamingResources.ResourceManager, typeof(NamingResources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(NamingResources.SA1311Description), NamingResources.ResourceManager, typeof(NamingResources));

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.NamingRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

        private static readonly Action<SyntaxNodeAnalysisContext> FieldDeclarationAction = HandleFieldDeclaration;

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
            ImmutableArray.Create(Descriptor);

        /// <inheritdoc/>
        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            context.RegisterSyntaxNodeAction(FieldDeclarationAction, SyntaxKind.FieldDeclaration);
        }

        private static void HandleFieldDeclaration(SyntaxNodeAnalysisContext context)
        {
            var fieldDeclaration = (FieldDeclarationSyntax)context.Node;

            if (!fieldDeclaration.Modifiers.Any(SyntaxKind.StaticKeyword) ||
               !fieldDeclaration.Modifiers.Any(SyntaxKind.ReadOnlyKeyword))
            {
                return;
            }

            var variables = fieldDeclaration.Declaration?.Variables;
            if (variables == null)
            {
                return;
            }

            foreach (VariableDeclaratorSyntax variableDeclarator in variables.Value)
            {
                if (variableDeclarator == null)
                {
                    continue;
                }

                var identifier = variableDeclarator.Identifier;
                if (identifier.IsMissing)
                {
                    continue;
                }

                string name = identifier.ValueText;
                if (string.IsNullOrEmpty(name) || !char.IsLower(name[0]))
                {
                    continue;
                }

                context.ReportDiagnostic(Diagnostic.Create(Descriptor, identifier.GetLocation()));
            }
        }
    }
}
