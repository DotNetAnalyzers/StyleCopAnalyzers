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
        // In this tutorial, FixableDiagnosticIds should return the diagnostic ID defined in DiagnosticAnalyzer.cs.
        // However, it is possible to provide fixes for diagnostics coming from other analyzers.
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
            // This statement gets the top of the syntax tree of the file on which the analyzer is running.
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            // This tutorial only includes one diagnostic, therefore it suffices to take the first diagnostic from the context.
            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            // The node this code fix aims to fix is of type 'IfStatementSyntax'.
            // The following statement extracts the node of type IfStatementSyntax closest to the reported diagnostic's location.
            var ifStatement = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<IfStatementSyntax>().First();

            // The IfStatementSyntax node is now passed to a method, FixSpacingAsync, that will generate the suggested new code.
            context.RegisterCodeFix(
                CodeAction.Create("Fix spacing", c => FixSpacingAsync(context.Document, ifStatement, c)),
                diagnostic);
        }

        private async Task<Document> FixSpacingAsync(Document document, IfStatementSyntax ifStatement, CancellationToken c)
        {
            // This method will generate a new if-statement node with a single space between the if-keyword and the opening parenthesis.
            var generator = SyntaxGenerator.GetGenerator(document);
        
            // The new if-statement will need to retain the same statements as the old if-statement.
            var ifBlock = ifStatement.Statement as BlockSyntax;
            var ifBlockStatements = ifBlock.Statements;

            // The new if-statement should retain the formatting of the trivia before and after the if-statement.
            // The following statements extract the original trivia.
            var ifKeyword = ifStatement.IfKeyword;
            var leadingTrivia = ifKeyword.LeadingTrivia;
            var closeBrace = ifBlock.CloseBraceToken;
            var trailingTrivia = closeBrace.TrailingTrivia;

            // If-statements generated using SyntaxGenerator are formatted with the desired spacing by default so this is not specified explicitly.
            var newIfStatement = generator.IfStatement(ifStatement.Condition, ifBlockStatements).WithLeadingTrivia(leadingTrivia).WithTrailingTrivia(trailingTrivia);

            // This statement gets the top of the syntax tree so that the old if-statement node can be replaced with the new one.
            var root = await document.GetSyntaxRootAsync();

            // A new root is created with the old if-statement replaced by the new one.
            var newRoot = root.ReplaceNode(ifStatement, newIfStatement);

            // A new document with the new root is returned.
            var newDocument = document.WithSyntaxRoot(newRoot);
            return newDocument;
    }
}
}