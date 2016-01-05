// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
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
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.ReadabilityRules, DiagnosticSeverity.Warning, AnalyzerConstants.DisabledNoTests, Description, HelpLink);

        private static readonly Action<CompilationStartAnalysisContext> CompilationStartAction = HandleCompilationStart;
        private static readonly Action<SyntaxNodeAnalysisContext> CompilationUnitAction = HandleCompilationUnit;
        private static readonly Action<SyntaxNodeAnalysisContext> NamespaceDeclarationAction = HandleNamespaceDeclaration;
        private static readonly Action<SyntaxNodeAnalysisContext> BaseTypeDeclarationAction = HandleBaseTypeDeclaration;
        private static readonly Action<SyntaxNodeAnalysisContext> TypeDeclarationAction = HandleTypeDeclaration;
        private static readonly Action<SyntaxNodeAnalysisContext> EnumDeclarationAction = HandleEnumDeclaration;
        private static readonly Action<SyntaxNodeAnalysisContext> EnumMemberDeclarationAction = HandleEnumMemberDeclaration;
        private static readonly Action<SyntaxNodeAnalysisContext> BaseFieldDeclarationAction = HandleBaseFieldDeclaration;
        private static readonly Action<SyntaxNodeAnalysisContext> BaseMethodDeclarationAction = HandleBaseMethodDeclaration;
        private static readonly Action<SyntaxNodeAnalysisContext> MethodDeclarationAction = HandleMethodDeclaration;
        private static readonly Action<SyntaxNodeAnalysisContext> BasePropertyDeclarationAction = HandleBasePropertyDeclaration;
        private static readonly Action<SyntaxNodeAnalysisContext> AccessorListAction = HandleAccessorList;
        private static readonly Action<SyntaxNodeAnalysisContext> AccessorDeclarationAction = HandleAccessorDeclaration;
        private static readonly Action<SyntaxNodeAnalysisContext> VariableDeclarationAction = HandleVariableDeclaration;
        private static readonly Action<SyntaxNodeAnalysisContext> TypeParameterListAction = HandleTypeParameterList;
        private static readonly Action<SyntaxNodeAnalysisContext> TypeParameterAction = HandleTypeParameter;
        private static readonly Action<SyntaxNodeAnalysisContext> BaseParameterListAction = HandleBaseParameterList;
        private static readonly Action<SyntaxNodeAnalysisContext> ParameterAction = HandleParameter;
        private static readonly Action<SyntaxNodeAnalysisContext> BaseArgumentListAction = HandleBaseArgumentList;
        private static readonly Action<SyntaxNodeAnalysisContext> AttributeListAction = HandleAttributeList;
        private static readonly Action<SyntaxNodeAnalysisContext> AttributeArgumentListAction = HandleAttributeArgumentList;
        private static readonly Action<SyntaxNodeAnalysisContext> BlockAction = HandleBlock;
        private static readonly Action<SyntaxNodeAnalysisContext> SwitchStatementAction = HandleSwitchStatement;
        private static readonly Action<SyntaxNodeAnalysisContext> InitializerExpressionAction = HandleInitializerExpression;

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
            context.RegisterSyntaxNodeActionHonorExclusions(CompilationUnitAction, SyntaxKind.CompilationUnit);
            context.RegisterSyntaxNodeActionHonorExclusions(NamespaceDeclarationAction, SyntaxKind.NamespaceDeclaration);
            context.RegisterSyntaxNodeActionHonorExclusions(BaseTypeDeclarationAction, SyntaxKinds.BaseTypeDeclaration);
            context.RegisterSyntaxNodeActionHonorExclusions(TypeDeclarationAction, SyntaxKinds.TypeDeclaration);
            context.RegisterSyntaxNodeActionHonorExclusions(EnumDeclarationAction, SyntaxKind.EnumDeclaration);
            context.RegisterSyntaxNodeActionHonorExclusions(EnumMemberDeclarationAction, SyntaxKind.EnumMemberDeclaration);
            context.RegisterSyntaxNodeActionHonorExclusions(BaseFieldDeclarationAction, SyntaxKinds.BaseFieldDeclaration);
            context.RegisterSyntaxNodeActionHonorExclusions(BaseMethodDeclarationAction, SyntaxKinds.BaseMethodDeclaration);
            context.RegisterSyntaxNodeActionHonorExclusions(MethodDeclarationAction, SyntaxKind.MethodDeclaration);
            context.RegisterSyntaxNodeActionHonorExclusions(BasePropertyDeclarationAction, SyntaxKinds.BasePropertyDeclaration);
            context.RegisterSyntaxNodeActionHonorExclusions(AccessorListAction, SyntaxKind.AccessorList);
            context.RegisterSyntaxNodeActionHonorExclusions(AccessorDeclarationAction, SyntaxKinds.AccessorDeclaration);
            context.RegisterSyntaxNodeActionHonorExclusions(VariableDeclarationAction, SyntaxKind.VariableDeclaration);
            context.RegisterSyntaxNodeActionHonorExclusions(TypeParameterListAction, SyntaxKind.TypeParameterList);
            context.RegisterSyntaxNodeActionHonorExclusions(TypeParameterAction, SyntaxKind.TypeParameter);
            context.RegisterSyntaxNodeActionHonorExclusions(BaseParameterListAction, SyntaxKinds.BaseParameterList);
            context.RegisterSyntaxNodeActionHonorExclusions(ParameterAction, SyntaxKind.Parameter);
            context.RegisterSyntaxNodeActionHonorExclusions(BaseArgumentListAction, SyntaxKinds.BaseArgumentList);
            context.RegisterSyntaxNodeActionHonorExclusions(AttributeListAction, SyntaxKind.AttributeList);
            context.RegisterSyntaxNodeActionHonorExclusions(AttributeArgumentListAction, SyntaxKind.AttributeArgumentList);
            context.RegisterSyntaxNodeActionHonorExclusions(BlockAction, SyntaxKind.Block);
            context.RegisterSyntaxNodeActionHonorExclusions(SwitchStatementAction, SyntaxKind.SwitchStatement);
            context.RegisterSyntaxNodeActionHonorExclusions(InitializerExpressionAction, SyntaxKinds.InitializerExpression);
        }

        private static void HandleCompilationUnit(SyntaxNodeAnalysisContext context)
        {
            var compilationUnit = (CompilationUnitSyntax)context.Node;

            var elements = ImmutableList.CreateBuilder<SyntaxNode>();

            elements.AddRange(compilationUnit.Externs);
            elements.AddRange(compilationUnit.Usings);
            elements.AddRange(compilationUnit.AttributeLists);
            elements.AddRange(compilationUnit.Members);

            CheckElements(context, elements.ToImmutable());
        }

        private static void HandleNamespaceDeclaration(SyntaxNodeAnalysisContext context)
        {
            var namespaceDeclaration = (NamespaceDeclarationSyntax)context.Node;

            var elements = ImmutableList.CreateBuilder<SyntaxNode>();

            elements.AddRange(namespaceDeclaration.Externs);
            elements.AddRange(namespaceDeclaration.Usings);
            elements.AddRange(namespaceDeclaration.Members);

            CheckElements(context, elements.ToImmutable());
        }

        private static void HandleBaseTypeDeclaration(SyntaxNodeAnalysisContext context)
        {
            var baseTypeDeclaration = (BaseTypeDeclarationSyntax)context.Node;
            CheckAttributeLists(context, baseTypeDeclaration.AttributeLists, baseTypeDeclaration);
        }

        private static void HandleTypeDeclaration(SyntaxNodeAnalysisContext context)
        {
            var typeDeclaration = (TypeDeclarationSyntax)context.Node;

            CheckElements(context, typeDeclaration.Members);
        }

        private static void HandleEnumDeclaration(SyntaxNodeAnalysisContext context)
        {
            var enumDeclaration = (EnumDeclarationSyntax)context.Node;

            CheckElements(context, enumDeclaration.Members);
        }

        private static void HandleEnumMemberDeclaration(SyntaxNodeAnalysisContext context)
        {
            var enumMemberDeclaration = (EnumMemberDeclarationSyntax)context.Node;

            CheckAttributeLists(context, enumMemberDeclaration.AttributeLists, enumMemberDeclaration);
        }

        private static void HandleBaseFieldDeclaration(SyntaxNodeAnalysisContext context)
        {
            var baseFieldDeclaration = (BaseFieldDeclarationSyntax)context.Node;

            CheckAttributeLists(context, baseFieldDeclaration.AttributeLists, baseFieldDeclaration);
        }

        private static void HandleBaseMethodDeclaration(SyntaxNodeAnalysisContext context)
        {
            var baseMethodDeclaration = (BaseMethodDeclarationSyntax)context.Node;

            CheckAttributeLists(context, baseMethodDeclaration.AttributeLists, baseMethodDeclaration);
        }

        private static void HandleMethodDeclaration(SyntaxNodeAnalysisContext context)
        {
            var methodDeclaration = (MethodDeclarationSyntax)context.Node;

            CheckElements(context, methodDeclaration.ConstraintClauses);
        }

        private static void HandleBasePropertyDeclaration(SyntaxNodeAnalysisContext context)
        {
            var basePropertyDeclaration = (BasePropertyDeclarationSyntax)context.Node;

            CheckAttributeLists(context, basePropertyDeclaration.AttributeLists, basePropertyDeclaration);
        }

        private static void HandleAccessorList(SyntaxNodeAnalysisContext context)
        {
            var accessorList = (AccessorListSyntax)context.Node;

            CheckElements(context, accessorList.Accessors);
        }

        private static void HandleAccessorDeclaration(SyntaxNodeAnalysisContext context)
        {
            var accessorDeclaration = (AccessorDeclarationSyntax)context.Node;

            CheckAttributeLists(context, accessorDeclaration.AttributeLists, accessorDeclaration);
        }

        private static void HandleVariableDeclaration(SyntaxNodeAnalysisContext context)
        {
            var variableDeclaration = (VariableDeclarationSyntax)context.Node;
            CheckElements(context, variableDeclaration.Variables);
        }

        private static void HandleTypeParameterList(SyntaxNodeAnalysisContext context)
        {
            var typeParameterList = (TypeParameterListSyntax)context.Node;

            CheckElements(context, typeParameterList.Parameters);
        }

        private static void HandleTypeParameter(SyntaxNodeAnalysisContext context)
        {
            var typeParameter = (TypeParameterSyntax)context.Node;

            CheckAttributeLists(context, typeParameter.AttributeLists, typeParameter);
        }

        private static void HandleBaseParameterList(SyntaxNodeAnalysisContext context)
        {
            var baseParameterList = (BaseParameterListSyntax)context.Node;

            CheckElements(context, baseParameterList.Parameters);
        }

        private static void HandleParameter(SyntaxNodeAnalysisContext context)
        {
            var parameter = (ParameterSyntax)context.Node;
            CheckAttributeLists(context, parameter.AttributeLists, parameter);
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

        private static void CheckAttributeLists(SyntaxNodeAnalysisContext context, SyntaxList<AttributeListSyntax> attributeLists, SyntaxNode parent)
        {
            if (attributeLists.Count == 0)
            {
                return;
            }

            var nodes = ImmutableList.CreateBuilder<SyntaxNode>();

            // Placing the parent first in this list causes the analysis to prefer to align attribute lists with their
            // parent syntax, assuming the parent also appears on its own line.
            nodes.Add(parent);
            nodes.AddRange(attributeLists);

            CheckElements(context, nodes.ToImmutable());
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

            elements.RemoveAll(
                element =>
                {
                    SyntaxToken firstToken = element.GetFirstToken();
                    return firstToken.IsMissingOrDefault() || !firstToken.IsFirstInLine();
                });

            if (elements.Count < 2)
            {
                return;
            }

            bool first = true;
            string expectedIndentation = null;
            foreach (T element in elements)
            {
                SyntaxTrivia indentationTrivia = element.GetFirstToken().LeadingTrivia.LastOrDefault();
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
                    location = element.GetFirstToken().GetLocation();
                }
                else
                {
                    location = indentationTrivia.GetLocation();
                }

                ImmutableDictionary<string, string> properties = ImmutableDictionary.Create<string, string>().SetItem(ExpectedIndentationKey, expectedIndentation);
                context.ReportDiagnostic(Diagnostic.Create(Descriptor, location, properties));
            }
        }
    }
}
