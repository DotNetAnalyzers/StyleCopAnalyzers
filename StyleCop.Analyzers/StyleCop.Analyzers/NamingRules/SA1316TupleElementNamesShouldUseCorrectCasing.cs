// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.NamingRules
{
    using System;
    using System.Collections.Immutable;
    using System.Diagnostics;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.Helpers;
    using StyleCop.Analyzers.Lightup;
    using StyleCop.Analyzers.Settings.ObjectModel;

    /// <summary>
    /// Field names within a tuple declaration should have the correct casing.
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class SA1316TupleElementNamesShouldUseCorrectCasing : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1316TupleElementNamesShouldUseCorrectCasing"/> analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1316";

        /// <summary>
        /// The key used to signal the fixed tuple element name to the code fix.
        /// </summary>
        internal const string ExpectedTupleElementNameKey = "ExpectedTupleElementName";

        private const string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1316.md";
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(NamingResources.SA1316Title), NamingResources.ResourceManager, typeof(NamingResources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(NamingResources.SA1316MessageFormat), NamingResources.ResourceManager, typeof(NamingResources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(NamingResources.SA1316Description), NamingResources.ResourceManager, typeof(NamingResources));

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.NamingRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

        private static readonly Action<SyntaxNodeAnalysisContext, StyleCopSettings> TupleTypeAction = HandleTupleTypeAction;
        private static readonly Action<SyntaxNodeAnalysisContext, StyleCopSettings> TupleExpressionAction = HandleTupleExpressionAction;

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
                context.RegisterSyntaxNodeAction(TupleTypeAction, SyntaxKindEx.TupleType);
                context.RegisterSyntaxNodeAction(TupleExpressionAction, SyntaxKindEx.TupleExpression);
            });
        }

        private static void HandleTupleTypeAction(SyntaxNodeAnalysisContext context, StyleCopSettings settings)
        {
            if (!context.SupportsTuples())
            {
                return;
            }

            var tupleType = (TupleTypeSyntaxWrapper)context.Node;

            foreach (var tupleElement in tupleType.Elements)
            {
                CheckTupleElement(context, settings, tupleElement);
            }
        }

        private static void HandleTupleExpressionAction(SyntaxNodeAnalysisContext context, StyleCopSettings settings)
        {
            if (!context.SupportsInferredTupleElementNames())
            {
                return;
            }

            if (!settings.NamingRules.IncludeInferredTupleElementNames)
            {
                return;
            }

            var tupleExpression = (TupleExpressionSyntaxWrapper)context.Node;
            foreach (var argument in tupleExpression.Arguments)
            {
                var inferredMemberName = SyntaxFactsEx.TryGetInferredMemberName(argument.NameColon?.Name ?? argument.Expression);
                if (inferredMemberName != null)
                {
                    CheckName(context, settings, tupleElement: null, inferredMemberName, argument.Expression.GetLocation(), false);
                }
            }
        }

        private static void CheckTupleElement(SyntaxNodeAnalysisContext context, StyleCopSettings settings, TupleElementSyntaxWrapper tupleElement)
        {
            if (tupleElement.Identifier == default)
            {
                return;
            }

            CheckName(context, settings, tupleElement.SyntaxNode, tupleElement.Identifier.ValueText, tupleElement.Identifier.GetLocation(), true);
        }

        private static void CheckName(SyntaxNodeAnalysisContext context, StyleCopSettings settings, SyntaxNode tupleElement, string tupleElementName, Location location, bool prepareCodeFix)
        {
            if (tupleElementName == "_")
            {
                return;
            }

            var firstCharacterIsLower = char.IsLower(tupleElementName[0]);

            bool reportDiagnostic;
            char fixedFirstChar;

            switch (settings.NamingRules.TupleElementNameCasing)
            {
            case TupleElementNameCase.PascalCase:
                reportDiagnostic = firstCharacterIsLower;
                fixedFirstChar = char.ToUpper(tupleElementName[0]);
                break;

            default:
                reportDiagnostic = !firstCharacterIsLower;
                fixedFirstChar = char.ToLower(tupleElementName[0]);
                break;
            }

            if (!reportDiagnostic)
            {
                return;
            }

            if (!CanTupleElementNameBeChanged(context, tupleElement))
            {
                return;
            }

            var diagnosticProperties = ImmutableDictionary.CreateBuilder<string, string>();

            if (prepareCodeFix)
            {
                var fixedName = fixedFirstChar + tupleElementName.Substring(1);
                diagnosticProperties.Add(ExpectedTupleElementNameKey, fixedName);
            }

            context.ReportDiagnostic(Diagnostic.Create(Descriptor, location, diagnosticProperties.ToImmutableDictionary()));
        }

        /// <summary>
        /// When overriding a base class or implementing an interface, the compiler requires the names to match
        /// the original definition. This method checks if we are allowed to change the name of the specified tuple element.
        /// </summary>
        private static bool CanTupleElementNameBeChanged(SyntaxNodeAnalysisContext context, SyntaxNode tupleElement)
        {
            var memberDeclaration = GetAncestorMemberDeclaration(tupleElement);
            if (memberDeclaration == null)
            {
                return true;
            }

            if (IsOverrideOrExplicitInterfaceImplementation(memberDeclaration))
            {
                return false;
            }

            var symbol = context.SemanticModel.GetDeclaredSymbol(memberDeclaration);
            if (NamedTypeHelpers.IsImplementingAnInterfaceMember(symbol))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Returns the ancestor <see cref="MemberDeclarationSyntax"/>, if the node is part of its "signature",
        /// otherwise returns null.
        /// </summary>
        private static MemberDeclarationSyntax GetAncestorMemberDeclaration(SyntaxNode node)
        {
            // NOTE: Avoiding a simple FirstAncestorOrSelf<MemberDeclarationSyntax>() call here, since
            // that would also return true for e.g. tuple variable declarations inside a method body.
            while (node != null)
            {
                switch (node.Kind())
                {
                case SyntaxKind.MethodDeclaration:
                case SyntaxKind.PropertyDeclaration:
                case SyntaxKind.EventDeclaration:
                case SyntaxKind.IndexerDeclaration:
                    return (MemberDeclarationSyntax)node;

                case SyntaxKind.Parameter:
                case SyntaxKind.ParameterList:
                case SyntaxKindEx.TupleElement:
                case SyntaxKind.TypeArgumentList:
                case SyntaxKind when node is TypeSyntax:
                    node = node.Parent;
                    break;

                default:
                    return null;
                }
            }

            return null;
        }

        private static bool IsOverrideOrExplicitInterfaceImplementation(MemberDeclarationSyntax memberDeclaration)
        {
            bool result;

            switch (memberDeclaration.Kind())
            {
            case SyntaxKind.MethodDeclaration:
                var methodDeclaration = (MethodDeclarationSyntax)memberDeclaration;
                result =
                    methodDeclaration.Modifiers.Any(SyntaxKind.OverrideKeyword) ||
                    methodDeclaration.ExplicitInterfaceSpecifier != null;
                break;

            case SyntaxKind.PropertyDeclaration:
            case SyntaxKind.EventDeclaration:
            case SyntaxKind.IndexerDeclaration:
                var basePropertyDeclaration = (BasePropertyDeclarationSyntax)memberDeclaration;
                result =
                    basePropertyDeclaration.Modifiers.Any(SyntaxKind.OverrideKeyword) ||
                    basePropertyDeclaration.ExplicitInterfaceSpecifier != null;
                break;

            default:
                Debug.Assert(false, $"Unhandled type {memberDeclaration.Kind()}");
                result = false;
                break;
            }

            return result;
        }
    }
}
