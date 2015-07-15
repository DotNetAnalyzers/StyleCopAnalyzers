/*
    This is the code fix for the analyzer you just wrote. It is written for you, and will replace whatever is between 'if' and '(' with a single space.
    There is a comment inside the 'FixableDiagnosticIds' property that needs to be addressed. Once you have done that, the code fix will work. Feel free
    to explore the code and experiment with it.
*/

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
using Microsoft.CodeAnalysis.Editing;
using Microsoft.CodeAnalysis.Rename;
using Microsoft.CodeAnalysis.Text;

namespace NewAnalyzerTemplate
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(NewAnalyzerTemplateCodeFixProvider)), Shared]
    public class NewAnalyzerTemplateCodeFixProvider : CodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds
        {
            get
            {
                /*
                    Replace the line below with:
                        return ImmutableArray.Create( [name of your analyzer class].[name of your diagnostic id]
                */
                
                 return ImmutableArray<string>.Empty;
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