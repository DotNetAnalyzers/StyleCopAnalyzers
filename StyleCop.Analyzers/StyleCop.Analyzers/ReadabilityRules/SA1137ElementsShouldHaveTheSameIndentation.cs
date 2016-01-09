// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.ReadabilityRules
{
    using System;
    using System.Collections.Immutable;
    using System.Diagnostics;
    using System.Linq;
    using Helpers;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Settings.ObjectModel;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class SA1137ElementsShouldHaveTheSameIndentation : DiagnosticAnalyzer
    {
        public const string ExpectedIndentationKey = "ExpectedIndentation";

        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1137ElementsShouldHaveTheSameIndentation"/> analyzer.
        /// </summary>
        public const string SA1137DiagnosticId = "SA1137";

        /// <summary>
        /// The ID for diagnostics produced by the SA1138 (Indent elements correctly) analyzer.
        /// </summary>
        public const string SA1138DiagnosticId = "SA1138";

        private static readonly LocalizableString SA1137Title = new LocalizableResourceString(nameof(ReadabilityResources.SA1137Title), ReadabilityResources.ResourceManager, typeof(ReadabilityResources));
        private static readonly LocalizableString SA1137MessageFormat = new LocalizableResourceString(nameof(ReadabilityResources.SA1137MessageFormat), ReadabilityResources.ResourceManager, typeof(ReadabilityResources));
        private static readonly LocalizableString SA1137Description = new LocalizableResourceString(nameof(ReadabilityResources.SA1137Description), ReadabilityResources.ResourceManager, typeof(ReadabilityResources));
        private static readonly string SA1137HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1137.md";

        private static readonly DiagnosticDescriptor SA1137Descriptor =
            new DiagnosticDescriptor(SA1137DiagnosticId, SA1137Title, SA1137MessageFormat, AnalyzerCategory.ReadabilityRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, SA1137Description, SA1137HelpLink);

        private static readonly LocalizableString SA1138Title = new LocalizableResourceString(nameof(ReadabilityResources.SA1138Title), ReadabilityResources.ResourceManager, typeof(ReadabilityResources));
        private static readonly LocalizableString SA1138MessageFormat = new LocalizableResourceString(nameof(ReadabilityResources.SA1138MessageFormat), ReadabilityResources.ResourceManager, typeof(ReadabilityResources));
        private static readonly LocalizableString SA1138Description = new LocalizableResourceString(nameof(ReadabilityResources.SA1138Description), ReadabilityResources.ResourceManager, typeof(ReadabilityResources));
        private static readonly string SA1138HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1138.md";

        private static readonly DiagnosticDescriptor SA1138Descriptor =
            new DiagnosticDescriptor(SA1138DiagnosticId, SA1138Title, SA1138MessageFormat, AnalyzerCategory.ReadabilityRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, SA1138Description, SA1138HelpLink);

        private static readonly Action<CompilationStartAnalysisContext> CompilationStartAction = HandleCompilationStart;
#pragma warning disable RS1008 // Avoid storing per-compilation data into the fields of a diagnostic analyzer.
        private static readonly Action<SyntaxTreeAnalysisContext, Compilation, StyleCopSettings> SyntaxTreeAction = HandleSyntaxTree;
#pragma warning restore RS1008 // Avoid storing per-compilation data into the fields of a diagnostic analyzer.

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
            ImmutableArray.Create(SA1137Descriptor, SA1138Descriptor);

        /// <inheritdoc/>
        public override void Initialize(AnalysisContext context)
        {
            context.RegisterCompilationStartAction(CompilationStartAction);
        }

        private static void HandleCompilationStart(CompilationStartAnalysisContext context)
        {
            context.RegisterSyntaxTreeActionHonorExclusions(SyntaxTreeAction);
        }

        private static void HandleSyntaxTree(SyntaxTreeAnalysisContext context, Compilation compilation, StyleCopSettings settings)
        {
            var walker = new IndentationWalker(context, compilation, settings);
            walker.Visit(context.Tree.GetRoot(context.CancellationToken));
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

        private static void CheckElements<T>(SyntaxTreeAnalysisContext context, Compilation compilation, StyleCopSettings settings, int? indentationLevel, SyntaxList<T> elements)
            where T : SyntaxNode
        {
            if (!elements.Any())
            {
                return;
            }

            if (elements.Count == 1 && compilation.IsAnalyzerSuppressed(SA1138DiagnosticId))
            {
                return;
            }

            CheckElements(context, compilation, settings, indentationLevel, elements.ToImmutableList());
        }

        private static void CheckElements<T>(SyntaxTreeAnalysisContext context, Compilation compilation, StyleCopSettings settings, int? indentationLevel, SeparatedSyntaxList<T> elements)
            where T : SyntaxNode
        {
            if (!elements.Any())
            {
                return;
            }

            if (elements.Count == 1 && compilation.IsAnalyzerSuppressed(SA1138DiagnosticId))
            {
                return;
            }

            CheckElements(context, compilation, settings, indentationLevel, elements.ToImmutableList());
        }

        private static void CheckElements<T>(SyntaxTreeAnalysisContext context, Compilation compilation, StyleCopSettings settings, int? indentationLevel, ImmutableList<T> elements)
            where T : SyntaxNode
        {
            DiagnosticDescriptor descriptor = SA1137Descriptor;

            bool enableAbsoluteIndentationAnalysis = !compilation.IsAnalyzerSuppressed(SA1138DiagnosticId);

            if (elements.IsEmpty || (elements.Count == 1 && !enableAbsoluteIndentationAnalysis))
            {
                return;
            }

            elements = elements.RemoveAll(
                element =>
                {
                    SyntaxToken firstToken = GetFirstTokenForAnalysis(element);
                    return firstToken.IsMissingOrDefault() || !firstToken.IsFirstInLine(allowNonWhitespaceTrivia: false);
                });

            if (elements.IsEmpty || (elements.Count == 1 && !enableAbsoluteIndentationAnalysis))
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
                    if (enableAbsoluteIndentationAnalysis && indentationLevel != null)
                    {
                        descriptor = SA1138Descriptor;
                        if (settings.Indentation.UseTabs)
                        {
                            expectedIndentation = new string('\t', indentationLevel.Value);
                        }
                        else
                        {
                            expectedIndentation = new string(' ', settings.Indentation.IndentationSize * indentationLevel.Value);
                        }
                    }
                    else
                    {
                        expectedIndentation = indentation;
                    }

                    first = false;

                    if (!enableAbsoluteIndentationAnalysis)
                    {
                        continue;
                    }
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
                context.ReportDiagnostic(Diagnostic.Create(descriptor, location, properties));
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

        private class IndentationWalker : CSharpSyntaxWalker
        {
            private readonly SyntaxTreeAnalysisContext context;
            private readonly Compilation compilation;
            private readonly StyleCopSettings settings;

            private int indentationLevel;
            private int unknownIndentationLevel;

            public IndentationWalker(SyntaxTreeAnalysisContext context, Compilation compilation, StyleCopSettings settings)
            {
                this.context = context;
                this.compilation = compilation;
                this.settings = settings;
            }

            private int? IndentationLevel
            {
                get
                {
                    if (this.unknownIndentationLevel > 0)
                    {
                        return null;
                    }

                    return this.indentationLevel;
                }
            }

            private int? BlockIndentation
            {
                get
                {
                    switch (this.settings.Indentation.IndentBlock)
                    {
                    case true:
                        return 1;

                    case false:
                        return 0;

                    default:
                        return null;
                    }
                }
            }

            private int? LabelAdjustment
            {
                get
                {
                    switch (this.settings.Indentation.LabelPositioning)
                    {
                    case LabelPositioning.LeftMost:
                        return -this.IndentationLevel;

                    case LabelPositioning.OneLess:
                        return this.IndentationLevel > 0 ? -1 : 0;

                    case LabelPositioning.NoIndent:
                        return 0;

                    default:
                        // Disable indentation check for labels
                        return null;
                    }
                }
            }

            private int? SwitchStatementAdjustment
            {
                get
                {
                    switch (this.settings.Indentation.IndentSwitchCaseSection)
                    {
                    case true:
                        return 1;

                    case false:
                        return 0;

                    default:
                        return null;
                    }
                }
            }

            public override void DefaultVisit(SyntaxNode node)
            {
                this.unknownIndentationLevel++;
                try
                {
                    base.DefaultVisit(node);
                }
                finally
                {
                    this.unknownIndentationLevel--;
                }
            }

            public override void VisitCompilationUnit(CompilationUnitSyntax node)
            {
                using (this.AdjustIndentation(0))
                {
                    this.AnalyzeCompilationUnit(node);
                    base.VisitCompilationUnit(node);
                }
            }

            public override void VisitNamespaceDeclaration(NamespaceDeclarationSyntax node)
            {
                using (this.AdjustIndentation(1))
                {
                    this.AnalyzeNamespaceDeclaration(node);
                    base.VisitNamespaceDeclaration(node);
                }
            }

            public override void VisitClassDeclaration(ClassDeclarationSyntax node)
            {
                using (this.AdjustIndentation(1))
                {
                    this.AnalyzeTypeDeclaration(node);
                    base.VisitClassDeclaration(node);
                }
            }

            public override void VisitStructDeclaration(StructDeclarationSyntax node)
            {
                using (this.AdjustIndentation(1))
                {
                    this.AnalyzeTypeDeclaration(node);
                    base.VisitStructDeclaration(node);
                }
            }

            public override void VisitInterfaceDeclaration(InterfaceDeclarationSyntax node)
            {
                using (this.AdjustIndentation(1))
                {
                    this.AnalyzeTypeDeclaration(node);
                    base.VisitInterfaceDeclaration(node);
                }
            }

            public override void VisitEnumDeclaration(EnumDeclarationSyntax node)
            {
                using (this.AdjustIndentation(1))
                {
                    this.AnalyzeEnumDeclaration(node);
                    base.VisitEnumDeclaration(node);
                }
            }

            public override void VisitMethodDeclaration(MethodDeclarationSyntax node)
            {
                using (this.AdjustIndentation(1))
                {
                    this.AnalyzeMethodDeclaration(node);
                    base.VisitMethodDeclaration(node);
                }
            }

            public override void VisitAccessorList(AccessorListSyntax node)
            {
                using (this.AdjustIndentation(1))
                {
                    this.AnalyzeAccessorList(node);
                    base.VisitAccessorList(node);
                }
            }

            public override void VisitVariableDeclaration(VariableDeclarationSyntax node)
            {
                this.AnalyzeVariableDeclaration(node);
                base.VisitVariableDeclaration(node);
            }

            public override void VisitTypeParameterList(TypeParameterListSyntax node)
            {
                using (this.AdjustIndentation(0))
                {
                    this.AnalyzeTypeParameterList(node);
                    base.VisitTypeParameterList(node);
                }
            }

            public override void VisitParameterList(ParameterListSyntax node)
            {
                this.AnalyzeBaseParameterList(node);
                base.VisitParameterList(node);
            }

            public override void VisitBracketedParameterList(BracketedParameterListSyntax node)
            {
                this.AnalyzeBaseParameterList(node);
                base.VisitBracketedParameterList(node);
            }

            public override void VisitArgumentList(ArgumentListSyntax node)
            {
                this.AnalyzeBaseArgumentList(node);
                base.VisitArgumentList(node);
            }

            public override void VisitBracketedArgumentList(BracketedArgumentListSyntax node)
            {
                this.AnalyzeBaseArgumentList(node);
                base.VisitBracketedArgumentList(node);
            }

            public override void VisitAttributeList(AttributeListSyntax node)
            {
                using (this.AdjustIndentation(1))
                {
                    this.AnalyzeAttributeList(node);
                    base.VisitAttributeList(node);
                }
            }

            public override void VisitAttributeArgumentList(AttributeArgumentListSyntax node)
            {
                this.AnalyzeAttributeArgumentList(node);
                base.VisitAttributeArgumentList(node);
            }

            public override void VisitBlock(BlockSyntax node)
            {
                int? adjustment;
                if (node.Parent.IsKind(SyntaxKind.Block))
                {
                    // The current indentation level is the level of the block itself
                    adjustment = this.BlockIndentation;
                }
                else
                {
                    // The parent indented by 1; update to match the BlockIndentation value
                    adjustment = this.BlockIndentation - 1;
                }

                using (this.AdjustIndentation(adjustment))
                {
                    this.AnalyzeBlock(node);
                    base.VisitBlock(node);
                }
            }

            public override void VisitSwitchStatement(SwitchStatementSyntax node)
            {
                int? indentationAmount =
                    this.settings.Indentation.IndentSwitchSection == null
                    ? default(int?)
                    : this.settings.Indentation.IndentSwitchSection == true ? 1 : 0;

                using (this.AdjustIndentation(indentationAmount))
                {
                    this.AnalyzeSwitchStatement(node);
                    base.VisitSwitchStatement(node);
                }
            }

            public override void VisitInitializerExpression(InitializerExpressionSyntax node)
            {
                this.AnalyzeInitializerExpression(node);
                base.VisitInitializerExpression(node);
            }

            public override void VisitAnonymousObjectCreationExpression(AnonymousObjectCreationExpressionSyntax node)
            {
                this.AnalyzeAnonymousObjectCreationExpression(node);
                base.VisitAnonymousObjectCreationExpression(node);
            }

            public override void VisitConstructorDeclaration(ConstructorDeclarationSyntax node)
            {
                using (this.AdjustIndentation(1))
                {
                    base.VisitConstructorDeclaration(node);
                }
            }

            public override void VisitDestructorDeclaration(DestructorDeclarationSyntax node)
            {
                using (this.AdjustIndentation(1))
                {
                    base.VisitDestructorDeclaration(node);
                }
            }

            public override void VisitOperatorDeclaration(OperatorDeclarationSyntax node)
            {
                using (this.AdjustIndentation(1))
                {
                    base.VisitOperatorDeclaration(node);
                }
            }

            public override void VisitConversionOperatorDeclaration(ConversionOperatorDeclarationSyntax node)
            {
                using (this.AdjustIndentation(1))
                {
                    base.VisitConversionOperatorDeclaration(node);
                }
            }

            public override void VisitPropertyDeclaration(PropertyDeclarationSyntax node)
            {
                using (this.AdjustIndentation(1))
                {
                    base.VisitPropertyDeclaration(node);
                }
            }

            public override void VisitEventDeclaration(EventDeclarationSyntax node)
            {
                using (this.AdjustIndentation(1))
                {
                    base.VisitEventDeclaration(node);
                }
            }

            public override void VisitIndexerDeclaration(IndexerDeclarationSyntax node)
            {
                using (this.AdjustIndentation(1))
                {
                    base.VisitIndexerDeclaration(node);
                }
            }

            public override void VisitAccessorDeclaration(AccessorDeclarationSyntax node)
            {
                using (this.AdjustIndentation(0))
                {
                    base.VisitAccessorDeclaration(node);
                }
            }

            public override void VisitLabeledStatement(LabeledStatementSyntax node)
            {
                using (this.AdjustIndentation(0))
                {
                    base.VisitLabeledStatement(node);
                }
            }

            public override void VisitCheckedStatement(CheckedStatementSyntax node)
            {
                using (this.AdjustIndentation(1))
                {
                    base.VisitCheckedStatement(node);
                }
            }

            public override void VisitDoStatement(DoStatementSyntax node)
            {
                using (this.AdjustIndentation(1))
                {
                    base.VisitDoStatement(node);
                }
            }

            public override void VisitFixedStatement(FixedStatementSyntax node)
            {
                using (this.AdjustIndentation(1))
                {
                    base.VisitFixedStatement(node);
                }
            }

            public override void VisitForEachStatement(ForEachStatementSyntax node)
            {
                using (this.AdjustIndentation(1))
                {
                    base.VisitForEachStatement(node);
                }
            }

            public override void VisitForStatement(ForStatementSyntax node)
            {
                using (this.AdjustIndentation(1))
                {
                    base.VisitForStatement(node);
                }
            }

            public override void VisitIfStatement(IfStatementSyntax node)
            {
                using (this.AdjustIndentation(1))
                {
                    base.VisitIfStatement(node);
                }
            }

            public override void VisitElseClause(ElseClauseSyntax node)
            {
                // This clause inherits the indentation from the if statement
                using (this.AdjustIndentation(0))
                {
                    base.VisitElseClause(node);
                }
            }

            public override void VisitLockStatement(LockStatementSyntax node)
            {
                using (this.AdjustIndentation(1))
                {
                    base.VisitLockStatement(node);
                }
            }

            public override void VisitUsingStatement(UsingStatementSyntax node)
            {
                // Allow consecutive using statements without nesting indentation.
                using (this.AdjustIndentation(node.Statement.IsKind(SyntaxKind.UsingStatement) ? 0 : 1))
                {
                    base.VisitUsingStatement(node);
                }
            }

            public override void VisitWhileStatement(WhileStatementSyntax node)
            {
                using (this.AdjustIndentation(1))
                {
                    base.VisitWhileStatement(node);
                }
            }

            private void AnalyzeCompilationUnit(CompilationUnitSyntax node)
            {
                var elements = ImmutableList.CreateBuilder<SyntaxNode>();

                elements.AddRange(node.Externs);
                elements.AddRange(node.Usings);
                elements.AddRange(node.AttributeLists);
                AddMembersAndAttributes(elements, node.Members);

                CheckElements(this.context, this.compilation, this.settings, this.IndentationLevel, elements.ToImmutable());
            }

            private void AnalyzeNamespaceDeclaration(NamespaceDeclarationSyntax node)
            {
                var elements = ImmutableList.CreateBuilder<SyntaxNode>();

                elements.AddRange(node.Externs);
                elements.AddRange(node.Usings);
                AddMembersAndAttributes(elements, node.Members);

                CheckElements(this.context, this.compilation, this.settings, this.IndentationLevel, elements.ToImmutable());
            }

            private void AnalyzeTypeDeclaration(TypeDeclarationSyntax node)
            {
                CheckElements(this.context, this.compilation, this.settings, this.IndentationLevel, node.ConstraintClauses);

                var elements = ImmutableList.CreateBuilder<SyntaxNode>();
                AddMembersAndAttributes(elements, node.Members);
                CheckElements(this.context, this.compilation, this.settings, this.IndentationLevel, elements.ToImmutable());
            }

            private void AnalyzeEnumDeclaration(EnumDeclarationSyntax node)
            {
                var elements = ImmutableList.CreateBuilder<SyntaxNode>();
                foreach (EnumMemberDeclarationSyntax enumMemberDeclaration in node.Members)
                {
                    elements.AddRange(enumMemberDeclaration.AttributeLists);
                    elements.Add(enumMemberDeclaration);
                }

                CheckElements(this.context, this.compilation, this.settings, this.IndentationLevel, elements.ToImmutable());
            }

            private void AnalyzeMethodDeclaration(MethodDeclarationSyntax node)
            {
                CheckElements(this.context, this.compilation, this.settings, this.IndentationLevel, node.ConstraintClauses);
            }

            private void AnalyzeAccessorList(AccessorListSyntax node)
            {
                var elements = ImmutableList.CreateBuilder<SyntaxNode>();
                AddMembersAndAttributes(elements, node.Accessors);
                CheckElements(this.context, this.compilation, this.settings, this.IndentationLevel, elements.ToImmutable());
            }

            private void AnalyzeVariableDeclaration(VariableDeclarationSyntax node)
            {
                CheckElements(this.context, this.compilation, this.settings, this.IndentationLevel, node.Variables);
            }

            private void AnalyzeTypeParameterList(TypeParameterListSyntax node)
            {
                var elements = ImmutableList.CreateBuilder<SyntaxNode>();
                AddMembersAndAttributes(elements, node.Parameters);
                CheckElements(this.context, this.compilation, this.settings, this.IndentationLevel, elements.ToImmutable());
            }

            private void AnalyzeBaseParameterList(BaseParameterListSyntax node)
            {
                var elements = ImmutableList.CreateBuilder<SyntaxNode>();
                AddMembersAndAttributes(elements, node.Parameters);
                CheckElements(this.context, this.compilation, this.settings, this.IndentationLevel, elements.ToImmutable());
            }

            private void AnalyzeBaseArgumentList(BaseArgumentListSyntax node)
            {
                CheckElements(this.context, this.compilation, this.settings, this.IndentationLevel, node.Arguments);
            }

            private void AnalyzeAttributeList(AttributeListSyntax node)
            {
                CheckElements(this.context, this.compilation, this.settings, this.IndentationLevel, node.Attributes);
            }

            private void AnalyzeAttributeArgumentList(AttributeArgumentListSyntax node)
            {
                CheckElements(this.context, this.compilation, this.settings, this.IndentationLevel, node.Arguments);
            }

            private void AnalyzeBlock(BlockSyntax node)
            {
                var statements = ImmutableList.CreateBuilder<StatementSyntax>();
                var labeledStatements = ImmutableList.CreateBuilder<StatementSyntax>();

                foreach (var statement in node.Statements)
                {
                    StatementSyntax statementToAlign = statement;
                    while (statementToAlign.IsKind(SyntaxKind.LabeledStatement))
                    {
                        labeledStatements.Add(statementToAlign);
                        statementToAlign = ((LabeledStatementSyntax)statementToAlign).Statement;
                    }

                    statements.Add(statementToAlign);
                }

                CheckElements(this.context, this.compilation, this.settings, this.IndentationLevel, statements.ToImmutable());
                CheckElements(this.context, this.compilation, this.settings, this.IndentationLevel + this.LabelAdjustment, labeledStatements.ToImmutable());
            }

            private void AnalyzeSwitchStatement(SwitchStatementSyntax node)
            {
                var labels = ImmutableList.CreateBuilder<SwitchLabelSyntax>();
                var statements = ImmutableList.CreateBuilder<StatementSyntax>();
                var labeledStatements = ImmutableList.CreateBuilder<StatementSyntax>();
                foreach (SwitchSectionSyntax switchSection in node.Sections)
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

                CheckElements(this.context, this.compilation, this.settings, this.IndentationLevel, labels.ToImmutable());
                CheckElements(this.context, this.compilation, this.settings, this.IndentationLevel + this.SwitchStatementAdjustment, statements.ToImmutable());
                CheckElements(this.context, this.compilation, this.settings, this.IndentationLevel + this.SwitchStatementAdjustment + this.LabelAdjustment, labeledStatements.ToImmutable());
            }

            private void AnalyzeInitializerExpression(InitializerExpressionSyntax node)
            {
                CheckElements(this.context, this.compilation, this.settings, this.IndentationLevel, node.Expressions);
            }

            private void AnalyzeAnonymousObjectCreationExpression(AnonymousObjectCreationExpressionSyntax node)
            {
                CheckElements(this.context, this.compilation, this.settings, this.IndentationLevel, node.Initializers);
            }

            private IndentationAdjuster AdjustIndentation(int? levels)
            {
                return new IndentationAdjuster(this, levels);
            }

            private struct IndentationAdjuster : IDisposable
            {
                private readonly IndentationWalker walker;
                private readonly int? levels;

                public IndentationAdjuster(IndentationWalker walker, int? levels)
                {
                    this.walker = walker;
                    this.levels = levels;

                    if (levels != null)
                    {
                        IncreaseIndentationLevel(walker, levels.Value);
                    }
                }

                public void Dispose()
                {
                    if (this.levels != null)
                    {
                        DecreaseIndentationLevel(this.walker, this.levels.Value);
                    }
                }

                private static void IncreaseIndentationLevel(IndentationWalker walker, int levels)
                {
                    if (walker.IndentationLevel == null)
                    {
                        return;
                    }

                    Debug.Assert(walker.indentationLevel >= 0, "Assertion failed: walker.indentationLevel >= 0");
                    Debug.Assert(walker.unknownIndentationLevel == 0, "Assertion failed: walker.unknownIndentationLevel == 0");
                    walker.indentationLevel += levels;
                    walker.unknownIndentationLevel--;
                }

                private static void DecreaseIndentationLevel(IndentationWalker walker, int levels)
                {
                    if (walker.IndentationLevel == null)
                    {
                        return;
                    }

                    Debug.Assert(walker.indentationLevel >= levels, "Assertion failed: walker.indentationLevel >= levels");
                    Debug.Assert(walker.unknownIndentationLevel == -1, "Assertion failed: walker.unknownIndentationLevel == -1");
                    walker.indentationLevel -= levels;
                    walker.unknownIndentationLevel++;
                }
            }
        }
    }
}
