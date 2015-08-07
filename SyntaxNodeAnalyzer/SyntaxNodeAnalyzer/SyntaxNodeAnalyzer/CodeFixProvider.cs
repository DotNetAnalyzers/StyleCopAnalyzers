// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Rename;
using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis.Editing;

namespace SyntaxNodeAnalyzer
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SyntaxNodeAnalyzerCodeFixProvider)), Shared]
    public class SyntaxNodeAnalyzerCodeFixProvider : CodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds
        {
            get
            {
                return ImmutableArray.Create(SyntaxNodeAnalyzerAnalyzer.spacingRuleId);
            }
        }

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            var ifStatement = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<IfStatementSyntax>().First();

            context.RegisterCodeFix(
                CodeAction.Create("Fix spacing", c => FixSpacingAsync(context.Document, ifStatement, c)),
                diagnostic);
        }

        private async Task<Document> FixSpacingAsync(Document document, IfStatementSyntax ifStatement, CancellationToken c)
        {
            var generator = SyntaxGenerator.GetGenerator(document);

            var ifKeyword = ifStatement.IfKeyword;
            var ifBlock = ifStatement.Statement as BlockSyntax;
            var closeBrace = ifBlock.CloseBraceToken;
            var ifBlockStatements = ifBlock.Statements;
 
            var leadingTrivia = ifKeyword.LeadingTrivia;
            var trailingTrivia = closeBrace.TrailingTrivia;
            var newIfStatement = generator.IfStatement(ifStatement.Condition, ifBlockStatements).WithLeadingTrivia(leadingTrivia).WithTrailingTrivia(trailingTrivia);
      
            var root = await document.GetSyntaxRootAsync();
            var newRoot = root.ReplaceNode(ifStatement, newIfStatement);
            var newDocument = document.WithSyntaxRoot(newRoot);
            return newDocument;
        }
    }
}