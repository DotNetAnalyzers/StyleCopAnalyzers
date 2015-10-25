// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

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
    /// The name of a public or internal field in C# does not begin with an upper-case letter.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs when the name of a public or internal field begins with a lower-case
    /// letter. Public or internal fields must being with an upper-case letter.</para>
    ///
    /// <para>If the field or variable name is intended to match the name of an item associated with Win32 or COM, and
    /// thus needs to start with a lower-case letter, place the field or variable within a special <c>NativeMethods</c>
    /// class. A <c>NativeMethods</c> class is any class which contains a name ending in <c>NativeMethods</c>, and is
    /// intended as a placeholder for Win32 or COM wrappers. StyleCop will ignore this violation if the item is placed
    /// within a <c>NativeMethods</c> class.</para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class SA1307AccessibleFieldsMustBeginWithUpperCaseLetter : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1307AccessibleFieldsMustBeginWithUpperCaseLetter"/>
        /// analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1307";
        private const string Title = "Accessible fields must begin with upper-case letter";
        private const string MessageFormat = "Field '{0}' must begin with upper-case letter";
        private const string Description = "The name of a public or internal field in C# does not begin with an upper-case letter.";
        private const string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1307.md";

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
            // To improve performance we are looking for the field instead of the declarator directly. That way we don't get called for local variables.
            FieldDeclarationSyntax declaration = (FieldDeclarationSyntax)context.Node;
            if (declaration.Declaration != null)
            {
                if (declaration.Modifiers.Any(SyntaxKind.ConstKeyword))
                {
                    // These are reported as SA1303.
                    return;
                }

                if (declaration.Modifiers.Any(SyntaxKind.PublicKeyword) || declaration.Modifiers.Any(SyntaxKind.InternalKeyword))
                {
                    foreach (VariableDeclaratorSyntax declarator in declaration.Declaration.Variables)
                    {
                        string name = declarator.Identifier.ToString();

                        if (!string.IsNullOrEmpty(name)
                            && char.IsLower(name[0])
                            && !NamedTypeHelpers.IsContainedInNativeMethodsClass(declaration))
                        {
                            context.ReportDiagnostic(Diagnostic.Create(Descriptor, declarator.Identifier.GetLocation(), name));
                        }
                    }
                }
            }
        }
    }
}
