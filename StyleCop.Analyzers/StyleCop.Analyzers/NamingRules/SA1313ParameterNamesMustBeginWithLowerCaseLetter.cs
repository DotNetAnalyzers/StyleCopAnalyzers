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
    /// The name of a parameter in C# does not begin with a lower-case letter.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs when the name of a parameter does not begin with a lower-case letter.</para>
    ///
    /// <para>An exception to this rule is made for lambda parameters named <c>_</c>. These parameters are often used to
    /// designate a placeholder parameter which is not actually used in the body of the lambda expression.</para>
    ///
    /// <para>If the parameter name is intended to match the name of an item associated with Win32 or COM, and thus
    /// needs to begin with an upper-case letter, place the parameter within a special <c>NativeMethods</c> class. A
    /// <c>NativeMethods</c> class is any class which contains a name ending in <c>NativeMethods</c>, and is intended as
    /// a placeholder for Win32 or COM wrappers. StyleCop will ignore this violation if the item is placed within a
    /// <c>NativeMethods</c> class.</para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class SA1313ParameterNamesMustBeginWithLowerCaseLetter : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1313ParameterNamesMustBeginWithLowerCaseLetter"/> analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1313";
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(NamingResources.SA1313Title), NamingResources.ResourceManager, typeof(NamingResources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(NamingResources.SA1313MessageFormat), NamingResources.ResourceManager, typeof(NamingResources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(NamingResources.SA1313Description), NamingResources.ResourceManager, typeof(NamingResources));
        private static readonly string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1313.md";

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.NamingRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

        private static readonly Action<CompilationStartAnalysisContext> CompilationStartAction = HandleCompilationStart;
        private static readonly Action<SyntaxNodeAnalysisContext> ParameterAction = HandleParameter;

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
            context.RegisterSyntaxNodeActionHonorExclusions(ParameterAction, SyntaxKind.Parameter);
        }

        private static void HandleParameter(SyntaxNodeAnalysisContext context)
        {
            ParameterSyntax syntax = (ParameterSyntax)context.Node;
            if (NamedTypeHelpers.IsContainedInNativeMethodsClass(syntax))
            {
                return;
            }

            var identifier = syntax.Identifier;
            if (identifier.IsMissing)
            {
                return;
            }

            string name = identifier.ValueText;
            if (string.IsNullOrEmpty(name) || char.IsLower(name[0]))
            {
                return;
            }

            if (name == "_" && IsInLambda(syntax))
            {
                return;
            }

            if (NameMatchesAbstraction(syntax, context.SemanticModel))
            {
                return;
            }

            // Parameter names must begin with lower-case letter
            context.ReportDiagnostic(Diagnostic.Create(Descriptor, identifier.GetLocation(), name));
        }

        private static bool IsInLambda(ParameterSyntax syntax)
        {
            if (syntax.Parent.IsKind(SyntaxKind.SimpleLambdaExpression))
            {
                return true;
            }

            if (syntax.Parent.Parent.IsKind(SyntaxKind.ParenthesizedLambdaExpression)
                || syntax.Parent.Parent.IsKind(SyntaxKind.AnonymousMethodExpression))
            {
                return true;
            }

            return false;
        }

        private static bool NameMatchesAbstraction(ParameterSyntax syntax, SemanticModel semanticModel)
        {
            var parameterList = syntax.Parent as ParameterListSyntax;
            if (parameterList == null)
            {
                // This occurs for simple lambda expressions (without parentheses)
                return false;
            }

            var index = parameterList.Parameters.IndexOf(syntax);
            var declaringMember = syntax.Parent.Parent;

            if (!declaringMember.IsKind(SyntaxKind.MethodDeclaration))
            {
                return false;
            }

            var methodDeclaration = (MethodDeclarationSyntax)declaringMember;
            var methodSymbol = semanticModel.GetDeclaredSymbol(methodDeclaration);

            if (methodSymbol.IsOverride)
            {
                if (methodSymbol.OverriddenMethod.Parameters[index].Name == syntax.Identifier.ValueText)
                {
                    return true;
                }
            }
            else
            {
                var containingType = methodSymbol.ContainingType;
                if (containingType == null)
                {
                    return false;
                }

                var implementedInterfaces = containingType.Interfaces;
                foreach (var implementedInterface in implementedInterfaces)
                {
                    foreach (var member in implementedInterface.GetMembers(methodSymbol.Name).OfType<IMethodSymbol>())
                    {
                        if (containingType.FindImplementationForInterfaceMember(member) == methodSymbol)
                        {
                            return member.Parameters[index].Name == syntax.Identifier.ValueText;
                        }
                    }
                }
            }

            return false;
        }
    }
}
