// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.NamingRules;

using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using StyleCop.Analyzers.Helpers;
using StyleCop.Analyzers.Lightup;

/// <summary>
/// An identifier name contains letters other than the basic Latin letters A-Z and a-z.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
internal class SA1317IdentifierShouldBeNamedOnlyWithLatinLetters : DiagnosticAnalyzer
{
    /// <summary>
    /// The ID for diagnostics produced by the <see cref="SA1317IdentifierShouldBeNamedOnlyWithLatinLetters"/> analyzer.
    /// </summary>
    public const string DiagnosticId = "SA1317";
    private static readonly string HelpLink = HelpLinkHelper.GetHelpLink(DiagnosticId);
    private static readonly LocalizableString Title = new LocalizableResourceString(nameof(NamingResources.SA1317Title), NamingResources.ResourceManager, typeof(NamingResources));
    private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(NamingResources.SA1317MessageFormat), NamingResources.ResourceManager, typeof(NamingResources));
    private static readonly LocalizableString Description = new LocalizableResourceString(nameof(NamingResources.SA1317Description), NamingResources.ResourceManager, typeof(NamingResources));

    private static readonly DiagnosticDescriptor Descriptor =
        new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.NamingRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

    private static readonly Action<SyntaxNodeAnalysisContext> DeclarationAction = HandleDeclaration;
    private static readonly Action<SyntaxNodeAnalysisContext> NamespaceDeclarationAction = HandleNamespaceDeclaration;

    /// <inheritdoc/>
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
        ImmutableArray.Create(Descriptor);

    /// <inheritdoc/>
    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        context.RegisterSyntaxNodeAction(NamespaceDeclarationAction, SyntaxKinds.BaseNamespaceDeclaration);
        context.RegisterSyntaxNodeAction(DeclarationAction, SyntaxKinds.BaseTypeDeclaration);
        context.RegisterSyntaxNodeAction(
            DeclarationAction,
            SyntaxKind.ConstructorDeclaration,
            SyntaxKind.DestructorDeclaration,
            SyntaxKind.MethodDeclaration,
            SyntaxKind.PropertyDeclaration,
            SyntaxKind.EventDeclaration,
            SyntaxKind.EnumMemberDeclaration,
            SyntaxKind.DelegateDeclaration,
            SyntaxKind.VariableDeclarator,
            SyntaxKind.Parameter,
            SyntaxKind.TypeParameter,
            SyntaxKind.ForEachStatement,
            SyntaxKind.CatchDeclaration,
            SyntaxKind.FromClause,
            SyntaxKind.LetClause,
            SyntaxKind.JoinClause,
            SyntaxKind.JoinIntoClause,
            SyntaxKind.QueryContinuation,
            SyntaxKind.LabeledStatement,
            SyntaxKindEx.LocalFunctionStatement,
            SyntaxKindEx.SingleVariableDesignation,
            SyntaxKindEx.TupleElement);
    }

    private static void HandleNamespaceDeclaration(SyntaxNodeAnalysisContext context)
    {
        CheckNamespaceName(context, ((BaseNamespaceDeclarationSyntaxWrapper)context.Node).Name);
    }

    private static void CheckNamespaceName(SyntaxNodeAnalysisContext context, NameSyntax name)
    {
        if (name is QualifiedNameSyntax qualifiedName)
        {
            CheckNamespaceName(context, qualifiedName.Left);
            CheckIdentifierName(context, qualifiedName.Right.Identifier);
        }
        else if (name is SimpleNameSyntax simpleName)
        {
            CheckIdentifierName(context, simpleName.Identifier);
        }
    }

    private static void HandleDeclaration(SyntaxNodeAnalysisContext context)
    {
        SyntaxToken identifier;
        switch (context.Node)
        {
        case BaseTypeDeclarationSyntax declaration:
            identifier = declaration.Identifier;
            break;
        case MethodDeclarationSyntax declaration:
            identifier = declaration.Identifier;
            break;
        case ConstructorDeclarationSyntax declaration:
            identifier = declaration.Identifier;
            break;
        case DestructorDeclarationSyntax declaration:
            identifier = declaration.Identifier;
            break;
        case PropertyDeclarationSyntax declaration:
            identifier = declaration.Identifier;
            break;
        case EventDeclarationSyntax declaration:
            identifier = declaration.Identifier;
            break;
        case EnumMemberDeclarationSyntax declaration:
            identifier = declaration.Identifier;
            break;
        case DelegateDeclarationSyntax declaration:
            identifier = declaration.Identifier;
            break;
        case VariableDeclaratorSyntax declaration:
            identifier = declaration.Identifier;
            break;
        case ParameterSyntax declaration:
            identifier = declaration.Identifier;
            break;
        case TypeParameterSyntax declaration:
            identifier = declaration.Identifier;
            break;
        case ForEachStatementSyntax declaration:
            identifier = declaration.Identifier;
            break;
        case CatchDeclarationSyntax declaration:
            identifier = declaration.Identifier;
            break;
        case FromClauseSyntax declaration:
            identifier = declaration.Identifier;
            break;
        case LetClauseSyntax declaration:
            identifier = declaration.Identifier;
            break;
        case JoinClauseSyntax declaration:
            identifier = declaration.Identifier;
            break;
        case JoinIntoClauseSyntax declaration:
            identifier = declaration.Identifier;
            break;
        case QueryContinuationSyntax declaration:
            identifier = declaration.Identifier;
            break;
        case LabeledStatementSyntax declaration:
            identifier = declaration.Identifier;
            break;
        default:
            switch (context.Node.Kind())
            {
            case SyntaxKindEx.LocalFunctionStatement:
                identifier = ((LocalFunctionStatementSyntaxWrapper)context.Node).Identifier;
                break;
            case SyntaxKindEx.SingleVariableDesignation:
                identifier = ((SingleVariableDesignationSyntaxWrapper)context.Node).Identifier;
                break;
            case SyntaxKindEx.TupleElement:
                identifier = ((TupleElementSyntaxWrapper)context.Node).Identifier;
                break;
            default:
                throw new InvalidOperationException("Unexpected declaration kind.");
            }

            break;
        }

        CheckIdentifierName(context, identifier);
    }

    private static void CheckIdentifierName(SyntaxNodeAnalysisContext context, SyntaxToken identifier)
    {
        string name = identifier.ValueText;
        if (string.IsNullOrEmpty(name))
        {
            return;
        }

        for (int i = 0; i < name.Length; i++)
        {
            if (name[i] > 127 && char.IsLetter(name[i]))
            {
                context.ReportDiagnostic(Diagnostic.Create(Descriptor, identifier.GetLocation(), name, i));
            }
        }
    }
}
