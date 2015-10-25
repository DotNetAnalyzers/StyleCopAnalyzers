// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.NamingRules
{
    using System;
    using System.Collections.Immutable;
    using Helpers;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// The name of a field in C# does not begin with a lower-case letter.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs when the name of a field begins with an upper-case letter. Constants,
    /// non-private readonly fields and static readonly fields must always start with an uppercase letter, whilst
    /// private readonly fields must start with a lowercase letter. Also, public or internal fields must always start
    /// with an uppercase letter.</para>
    ///
    /// <para>If the field name is intended to match the name of an item associated with Win32 or COM, and thus needs to
    /// begin with an upper-case letter, place the field within a special <c>NativeMethods</c> class. A
    /// <c>NativeMethods</c> class is any class which contains a name ending in <c>NativeMethods</c>, and is intended as
    /// a placeholder for Win32 or COM wrappers. StyleCop will ignore this violation if the item is placed within a
    /// <c>NativeMethods</c> class.</para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class SA1306FieldNamesMustBeginWithLowerCaseLetter : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1306FieldNamesMustBeginWithLowerCaseLetter"/> analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1306";
        private const string Title = "Field names must begin with lower-case letter";
        private const string MessageFormat = "Field '{0}' must begin with lower-case letter";
        private const string Description = "The name of a field in C# does not begin with a lower-case letter.";
        private const string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1306.md";

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.NamingRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

        private static readonly Action<CompilationStartAnalysisContext> CompilationStartAction = HandleCompilationStart;
        private static readonly Action<SyntaxNodeAnalysisContext> FieldDeclarationAction = HandleFieldDeclaration;

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
            context.RegisterSyntaxNodeActionHonorExclusions(FieldDeclarationAction, SyntaxKind.FieldDeclaration);
        }

        private static void HandleFieldDeclaration(SyntaxNodeAnalysisContext context)
        {
            FieldDeclarationSyntax syntax = (FieldDeclarationSyntax)context.Node;
            if (NamedTypeHelpers.IsContainedInNativeMethodsClass(syntax))
            {
                return;
            }

            if (syntax.Modifiers.Any(SyntaxKind.ConstKeyword))
            {
                // this diagnostic does not apply to constant fields
                return;
            }

            if (syntax.Modifiers.Any(SyntaxKind.PublicKeyword)
                || syntax.Modifiers.Any(SyntaxKind.InternalKeyword))
            {
                // this diagnostic does not apply to public or internal read only fields
                return;
            }

            if (syntax.Modifiers.Any(SyntaxKind.ReadOnlyKeyword)
                && syntax.Modifiers.Any(SyntaxKind.ProtectedKeyword))
            {
                // this diagnostic does not apply to non-private read only fields
                return;
            }

            if (syntax.Modifiers.Any(SyntaxKind.ReadOnlyKeyword)
                && syntax.Modifiers.Any(SyntaxKind.StaticKeyword))
            {
                // this diagnostic does not apply to static read only fields
                return;
            }

            var variables = syntax.Declaration?.Variables;
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
                if (string.IsNullOrEmpty(name) || char.IsLower(name[0]))
                {
                    continue;
                }

                if (name[0] == '_')
                {
                    // `_foo` is handled by SA1309
                    continue;
                }

                // Field names must begin with lower-case letter
                context.ReportDiagnostic(Diagnostic.Create(Descriptor, identifier.GetLocation(), name));
            }
        }
    }
}
