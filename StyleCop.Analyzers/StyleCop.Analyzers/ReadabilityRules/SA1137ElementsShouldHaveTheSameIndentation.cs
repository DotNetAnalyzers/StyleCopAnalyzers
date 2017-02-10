﻿// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.ReadabilityRules
{
    using System;
    using System.Collections.Immutable;
    using System.Linq;
    using Helpers;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class SA1137ElementsShouldHaveTheSameIndentation : DiagnosticAnalyzer
    {
        public const string ExpectedIndentationKey = "ExpectedIndentation";

        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1137ElementsShouldHaveTheSameIndentation"/> analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1137";
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(ReadabilityResources.SA1137Title), ReadabilityResources.ResourceManager, typeof(ReadabilityResources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(ReadabilityResources.SA1137MessageFormat), ReadabilityResources.ResourceManager, typeof(ReadabilityResources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(ReadabilityResources.SA1137Description), ReadabilityResources.ResourceManager, typeof(ReadabilityResources));
        private static readonly string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1137.md";

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.ReadabilityRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

        private static readonly Action<SyntaxNodeAnalysisContext> CompilationUnitAction = HandleCompilationUnit;
        private static readonly Action<SyntaxNodeAnalysisContext> NamespaceDeclarationAction = HandleNamespaceDeclaration;
        private static readonly Action<SyntaxNodeAnalysisContext> TypeDeclarationAction = HandleTypeDeclaration;
        private static readonly Action<SyntaxNodeAnalysisContext> EnumDeclarationAction = HandleEnumDeclaration;
        private static readonly Action<SyntaxNodeAnalysisContext> MethodDeclarationAction = HandleMethodDeclaration;
        private static readonly Action<SyntaxNodeAnalysisContext> AccessorListAction = HandleAccessorList;
        private static readonly Action<SyntaxNodeAnalysisContext> VariableDeclarationAction = HandleVariableDeclaration;
        private static readonly Action<SyntaxNodeAnalysisContext> TypeParameterListAction = HandleTypeParameterList;
        private static readonly Action<SyntaxNodeAnalysisContext> BaseParameterListAction = HandleBaseParameterList;
        private static readonly Action<SyntaxNodeAnalysisContext> BaseArgumentListAction = HandleBaseArgumentList;
        private static readonly Action<SyntaxNodeAnalysisContext> AttributeListAction = HandleAttributeList;
        private static readonly Action<SyntaxNodeAnalysisContext> AttributeArgumentListAction = HandleAttributeArgumentList;
        private static readonly Action<SyntaxNodeAnalysisContext> BlockAction = HandleBlock;
        private static readonly Action<SyntaxNodeAnalysisContext> SwitchStatementAction = HandleSwitchStatement;
        private static readonly Action<SyntaxNodeAnalysisContext> InitializerExpressionAction = HandleInitializerExpression;
        private static readonly Action<SyntaxNodeAnalysisContext> AnonymousObjectCreationExpressionAction = HandleAnonymousObjectCreationExpression;

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
            ImmutableArray.Create(Descriptor);

        /// <inheritdoc/>
        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            context.RegisterSyntaxNodeAction(CompilationUnitAction, SyntaxKind.CompilationUnit);
            context.RegisterSyntaxNodeAction(NamespaceDeclarationAction, SyntaxKind.NamespaceDeclaration);
            context.RegisterSyntaxNodeAction(TypeDeclarationAction, SyntaxKinds.TypeDeclaration);
            context.RegisterSyntaxNodeAction(EnumDeclarationAction, SyntaxKind.EnumDeclaration);
            context.RegisterSyntaxNodeAction(MethodDeclarationAction, SyntaxKind.MethodDeclaration);
            context.RegisterSyntaxNodeAction(AccessorListAction, SyntaxKind.AccessorList);
            context.RegisterSyntaxNodeAction(VariableDeclarationAction, SyntaxKind.VariableDeclaration);
            context.RegisterSyntaxNodeAction(TypeParameterListAction, SyntaxKind.TypeParameterList);
            context.RegisterSyntaxNodeAction(BaseParameterListAction, SyntaxKinds.BaseParameterList);
            context.RegisterSyntaxNodeAction(BaseArgumentListAction, SyntaxKinds.BaseArgumentList);
            context.RegisterSyntaxNodeAction(AttributeListAction, SyntaxKind.AttributeList);
            context.RegisterSyntaxNodeAction(AttributeArgumentListAction, SyntaxKind.AttributeArgumentList);
            context.RegisterSyntaxNodeAction(BlockAction, SyntaxKind.Block);
            context.RegisterSyntaxNodeAction(SwitchStatementAction, SyntaxKind.SwitchStatement);
            context.RegisterSyntaxNodeAction(InitializerExpressionAction, SyntaxKinds.InitializerExpression);
            context.RegisterSyntaxNodeAction(AnonymousObjectCreationExpressionAction, SyntaxKind.AnonymousObjectCreationExpression);
        }

        private static void HandleCompilationUnit(SyntaxNodeAnalysisContext context)
        {
            var compilationUnit = (CompilationUnitSyntax)context.Node;

            var elements = ImmutableList.CreateBuilder<SyntaxNode>();

            elements.AddRange(compilationUnit.Externs);
            elements.AddRange(compilationUnit.Usings);
            elements.AddRange(compilationUnit.AttributeLists);
            AddMembersAndAttributes(elements, compilationUnit.Members);

            CheckElements(context, elements.ToImmutable());
        }

        private static void HandleNamespaceDeclaration(SyntaxNodeAnalysisContext context)
        {
            var namespaceDeclaration = (NamespaceDeclarationSyntax)context.Node;

            var elements = ImmutableList.CreateBuilder<SyntaxNode>();

            elements.AddRange(namespaceDeclaration.Externs);
            elements.AddRange(namespaceDeclaration.Usings);
            AddMembersAndAttributes(elements, namespaceDeclaration.Members);

            CheckElements(context, elements.ToImmutable());
        }

        private static void HandleTypeDeclaration(SyntaxNodeAnalysisContext context)
        {
            var typeDeclaration = (TypeDeclarationSyntax)context.Node;

            CheckElements(context, typeDeclaration.ConstraintClauses);

            var elements = ImmutableList.CreateBuilder<SyntaxNode>();
            AddMembersAndAttributes(elements, typeDeclaration.Members);
            CheckElements(context, elements.ToImmutable());
        }

        private static void HandleEnumDeclaration(SyntaxNodeAnalysisContext context)
        {
            var enumDeclaration = (EnumDeclarationSyntax)context.Node;

            var elements = ImmutableList.CreateBuilder<SyntaxNode>();
            foreach (EnumMemberDeclarationSyntax enumMemberDeclaration in enumDeclaration.Members)
            {
                elements.AddRange(enumMemberDeclaration.AttributeLists);
                elements.Add(enumMemberDeclaration);
            }

            CheckElements(context, elements.ToImmutable());
        }

        private static void HandleMethodDeclaration(SyntaxNodeAnalysisContext context)
        {
            var methodDeclaration = (MethodDeclarationSyntax)context.Node;

            CheckElements(context, methodDeclaration.ConstraintClauses);
        }

        private static void HandleAccessorList(SyntaxNodeAnalysisContext context)
        {
            var accessorList = (AccessorListSyntax)context.Node;

            var elements = ImmutableList.CreateBuilder<SyntaxNode>();
            AddMembersAndAttributes(elements, accessorList.Accessors);
            CheckElements(context, elements.ToImmutable());
        }

        private static void HandleVariableDeclaration(SyntaxNodeAnalysisContext context)
        {
            var variableDeclaration = (VariableDeclarationSyntax)context.Node;
            CheckElements(context, variableDeclaration.Variables);
        }

        private static void HandleTypeParameterList(SyntaxNodeAnalysisContext context)
        {
            var typeParameterList = (TypeParameterListSyntax)context.Node;

            var elements = ImmutableList.CreateBuilder<SyntaxNode>();
            AddMembersAndAttributes(elements, typeParameterList.Parameters);
            CheckElements(context, elements.ToImmutable());
        }

        private static void HandleBaseParameterList(SyntaxNodeAnalysisContext context)
        {
            var baseParameterList = (BaseParameterListSyntax)context.Node;

            var elements = ImmutableList.CreateBuilder<SyntaxNode>();
            AddMembersAndAttributes(elements, baseParameterList.Parameters);
            CheckElements(context, elements.ToImmutable());
        }

        private static void HandleBaseArgumentList(SyntaxNodeAnalysisContext context)
        {
            var baseArgumentList = (BaseArgumentListSyntax)context.Node;

            CheckElements(context, baseArgumentList.Arguments);
        }

        private static void HandleAttributeList(SyntaxNodeAnalysisContext context)
        {
            var attributeList = (AttributeListSyntax)context.Node;

            CheckElements(context, attributeList.Attributes);
        }

        private static void HandleAttributeArgumentList(SyntaxNodeAnalysisContext context)
        {
            var attributeArgumentList = (AttributeArgumentListSyntax)context.Node;

            CheckElements(context, attributeArgumentList.Arguments);
        }

        private static void HandleBlock(SyntaxNodeAnalysisContext context)
        {
            var block = (BlockSyntax)context.Node;

            var statements = ImmutableList.CreateBuilder<StatementSyntax>();
            var labeledStatements = ImmutableList.CreateBuilder<StatementSyntax>();

            foreach (var statement in block.Statements)
            {
                StatementSyntax statementToAlign = statement;
                while (statementToAlign.IsKind(SyntaxKind.LabeledStatement))
                {
                    labeledStatements.Add(statementToAlign);
                    statementToAlign = ((LabeledStatementSyntax)statementToAlign).Statement;
                }

                statements.Add(statementToAlign);
            }

            CheckElements(context, statements.ToImmutable());
            CheckElements(context, labeledStatements.ToImmutable());
        }

        private static void HandleSwitchStatement(SyntaxNodeAnalysisContext context)
        {
            var switchStatement = (SwitchStatementSyntax)context.Node;

            var labels = ImmutableList.CreateBuilder<SwitchLabelSyntax>();
            var statements = ImmutableList.CreateBuilder<StatementSyntax>();
            var labeledStatements = ImmutableList.CreateBuilder<StatementSyntax>();
            foreach (SwitchSectionSyntax switchSection in switchStatement.Sections)
            {
                labels.AddRange(switchSection.Labels);
                foreach (var statement in switchSection.Statements)
                {
                    StatementSyntax statementToAlign = statement;
                    while (statementToAlign.IsKind(SyntaxKind.LabeledStatement))
                    {
                        labeledStatements.Add(statementToAlign);
                        statementToAlign = ((LabeledStatementSyntax)statementToAlign).Statement;
                    }

                    statements.Add(statementToAlign);
                }
            }

            CheckElements(context, labels.ToImmutable());
            CheckElements(context, statements.ToImmutable());
            CheckElements(context, labeledStatements.ToImmutable());
        }

        private static void HandleInitializerExpression(SyntaxNodeAnalysisContext context)
        {
            var initializerExpression = (InitializerExpressionSyntax)context.Node;

            CheckElements(context, initializerExpression.Expressions);
        }

        private static void HandleAnonymousObjectCreationExpression(SyntaxNodeAnalysisContext context)
        {
            var anonymousObjectCreationExpression = (AnonymousObjectCreationExpressionSyntax)context.Node;

            CheckElements(context, anonymousObjectCreationExpression.Initializers);
        }

        private static void AddMembersAndAttributes<T>(ImmutableList<SyntaxNode>.Builder elements, SeparatedSyntaxList<T> members)
            where T : SyntaxNode
        {
            foreach (SyntaxNode member in members)
            {
                AddMemberAndAttributes(elements, member);
            }
        }

        private static void AddMembersAndAttributes<T>(ImmutableList<SyntaxNode>.Builder elements, SyntaxList<T> members)
            where T : SyntaxNode
        {
            foreach (SyntaxNode member in members)
            {
                AddMemberAndAttributes(elements, member);
            }
        }

        private static void AddMemberAndAttributes(ImmutableList<SyntaxNode>.Builder elements, SyntaxNode member)
        {
            switch (member.Kind())
            {
            case SyntaxKind.ClassDeclaration:
            case SyntaxKind.StructDeclaration:
            case SyntaxKind.InterfaceDeclaration:
            case SyntaxKind.EnumDeclaration:
                elements.AddRange(((BaseTypeDeclarationSyntax)member).AttributeLists);
                break;

            case SyntaxKind.FieldDeclaration:
            case SyntaxKind.EventFieldDeclaration:
                elements.AddRange(((BaseFieldDeclarationSyntax)member).AttributeLists);
                break;

            case SyntaxKind.PropertyDeclaration:
            case SyntaxKind.EventDeclaration:
            case SyntaxKind.IndexerDeclaration:
                elements.AddRange(((BasePropertyDeclarationSyntax)member).AttributeLists);
                break;

            case SyntaxKind.MethodDeclaration:
            case SyntaxKind.ConstructorDeclaration:
            case SyntaxKind.DestructorDeclaration:
            case SyntaxKind.OperatorDeclaration:
            case SyntaxKind.ConversionOperatorDeclaration:
                elements.AddRange(((BaseMethodDeclarationSyntax)member).AttributeLists);
                break;

            case SyntaxKind.GetAccessorDeclaration:
            case SyntaxKind.SetAccessorDeclaration:
            case SyntaxKind.AddAccessorDeclaration:
            case SyntaxKind.RemoveAccessorDeclaration:
            case SyntaxKind.UnknownAccessorDeclaration:
                elements.AddRange(((AccessorDeclarationSyntax)member).AttributeLists);
                break;

            case SyntaxKind.TypeParameter:
                elements.AddRange(((TypeParameterSyntax)member).AttributeLists);
                break;

            case SyntaxKind.Parameter:
                elements.AddRange(((ParameterSyntax)member).AttributeLists);
                break;

            default:
                break;
            }

            elements.Add(member);
        }

        private static void CheckElements<T>(SyntaxNodeAnalysisContext context, SyntaxList<T> elements)
            where T : SyntaxNode
        {
            if (elements.Count < 2)
            {
                return;
            }

            CheckElements(context, elements.ToImmutableList());
        }

        private static void CheckElements<T>(SyntaxNodeAnalysisContext context, SeparatedSyntaxList<T> elements)
            where T : SyntaxNode
        {
            if (elements.Count < 2)
            {
                return;
            }

            CheckElements(context, elements.ToImmutableList());
        }

        private static void CheckElements<T>(SyntaxNodeAnalysisContext context, ImmutableList<T> elements)
            where T : SyntaxNode
        {
            if (elements.Count < 2)
            {
                return;
            }

            elements = elements.RemoveAll(
                element =>
                {
                    SyntaxToken firstToken = GetFirstTokenForAnalysis(element);
                    return firstToken.IsMissingOrDefault() || !firstToken.IsFirstInLine(allowNonWhitespaceTrivia: false);
                });

            if (elements.Count < 2)
            {
                return;
            }

            // Try to reorder the list so the first item is not an attribute list. This element will establish the
            // expected indentation for the entire collection.
            int desiredFirst = elements.FindIndex(x => !x.IsKind(SyntaxKind.AttributeList));
            if (desiredFirst > 0)
            {
                T newFirstElement = elements[desiredFirst];
                elements = elements.RemoveAt(desiredFirst).Insert(0, newFirstElement);
            }

            bool first = true;
            string expectedIndentation = null;
            foreach (T element in elements)
            {
                SyntaxToken firstToken = GetFirstTokenForAnalysis(element);
                SyntaxTrivia indentationTrivia = firstToken.LeadingTrivia.LastOrDefault();
                string indentation = indentationTrivia.IsKind(SyntaxKind.WhitespaceTrivia) ? indentationTrivia.ToString() : string.Empty;

                if (first)
                {
                    expectedIndentation = indentation;
                    first = false;
                    continue;
                }

                if (string.Equals(expectedIndentation, indentation, StringComparison.Ordinal))
                {
                    // This handles the case where elements are indented properly
                    continue;
                }

                Location location;
                if (indentation.Length == 0)
                {
                    location = firstToken.GetLocation();
                }
                else
                {
                    location = indentationTrivia.GetLocation();
                }

                ImmutableDictionary<string, string> properties = ImmutableDictionary.Create<string, string>().SetItem(ExpectedIndentationKey, expectedIndentation);
                context.ReportDiagnostic(Diagnostic.Create(Descriptor, location, properties));
            }
        }

        private static SyntaxToken GetFirstTokenForAnalysis(SyntaxNode node)
        {
            SyntaxToken firstToken = node.GetFirstToken();
            if (!node.IsKind(SyntaxKind.AttributeList))
            {
                while (firstToken.IsKind(SyntaxKind.OpenBracketToken)
                    && firstToken.Parent.IsKind(SyntaxKind.AttributeList))
                {
                    // Skip over the attribute list since it's not the focus of this check
                    firstToken = firstToken.Parent.GetLastToken().GetNextToken();
                }
            }

            return firstToken;
        }
    }
}
