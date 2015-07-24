// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;
using Microsoft.CodeAnalysis.Simplification;

namespace AsyncPackage
{
    /// <summary>
    /// Codefix changes the synchronous operations to it's asynchronous equivalent. 
    /// </summary>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = BlockingAsyncAnalyzer.BlockingAsyncId), Shared]
    public class BlockingAsyncCodeFix : CodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(BlockingAsyncAnalyzer.BlockingAsyncId); }
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            // Find the type declaration identified by the diagnostic.
            //var parent = root.FindToken(diagnosticSpan.Start).Parent;
            //var invocation = parent.FirstAncestorOrSelf<InvocationExpressionSyntax>();

            var memberAccessNode = root.FindToken(diagnosticSpan.Start).Parent.FirstAncestorOrSelf<MemberAccessExpressionSyntax>();

            var semanticmodel = await context.Document.GetSemanticModelAsync(context.CancellationToken).ConfigureAwait(false);

            var method = semanticmodel.GetEnclosingSymbol(memberAccessNode.SpanStart) as IMethodSymbol;

            if (method != null && method.IsAsync)
            {
                var invokeMethod = semanticmodel.GetSymbolInfo(memberAccessNode).Symbol as IMethodSymbol;

                if (invokeMethod != null)
                {
                    var invocation = memberAccessNode.FirstAncestorOrSelf<InvocationExpressionSyntax>();

                    if (memberAccessNode.Name.Identifier.Text.Equals("Wait"))
                    {
                        var name = memberAccessNode.Name.Identifier.Text;

                        // Register a code action that will invoke the fix.
                        context.RegisterCodeFix(
                            new CodeActionChangetoAwaitAsync("Change synchronous operation to asynchronous counterpart",
                                                             c => ChangetoAwaitAsync(context.Document, invocation, name, c)),
                            diagnostic);
                        return;
                    }

                    if (memberAccessNode.Name.Identifier.Text.Equals("GetAwaiter"))
                    {
                        // Register a code action that will invoke the fix.
                        context.RegisterCodeFix(
                            new CodeActionChangetoAwaitGetAwaiterAsync("Change synchronous operation to asynchronous counterpart",
                                                                       c => ChangetoAwaitGetAwaiterAsync(context.Document, invocation, c)),
                            diagnostic);
                        return;
                    }

                    if (memberAccessNode.Name.Identifier.Text.Equals("Result"))
                    {
                        
                    }

                    if (memberAccessNode.Name.Identifier.Text.Equals("WaitAny"))
                    {
                        var name = memberAccessNode.Name.Identifier.Text;

                        // Register a code action that will invoke the fix.
                        context.RegisterCodeFix(
                            new CodeActionToDelayWhenAnyWhenAllAsync("Change synchronous operation to asynchronous counterpart",
                                                                     c => ToDelayWhenAnyWhenAllAsync(context.Document, invocation, name, c)),
                            diagnostic);
                        return;
                    }

                    if (memberAccessNode.Name.Identifier.Text.Equals("WaitAll"))
                    {
                        var name = memberAccessNode.Name.Identifier.Text;

                        // Register a code action that will invoke the fix.
                        context.RegisterCodeFix(
                            new CodeActionToDelayWhenAnyWhenAllAsync("Change synchronous operation to asynchronous counterpart",
                                                                     c => ToDelayWhenAnyWhenAllAsync(context.Document, invocation, name, c)),
                            diagnostic);
                        return;
                    }

                    if (memberAccessNode.Name.Identifier.Text.Equals("Sleep"))
                    {
                        var name = memberAccessNode.Name.Identifier.Text;

                        // Register a code action that will invoke the fix.
                        context.RegisterCodeFix(
                            new CodeActionToDelayWhenAnyWhenAllAsync("Change synchronous operation to asynchronous counterpart",
                                                                     c => ToDelayWhenAnyWhenAllAsync(context.Document, invocation, name, c)),
                            diagnostic);
                        return;
                    }
                }

                var property = semanticmodel.GetSymbolInfo(memberAccessNode).Symbol as IPropertySymbol;

                if (property != null && memberAccessNode.Name.Identifier.Text.Equals("Result"))
                {
                    var name = memberAccessNode.Name.Identifier.Text;

                    // Register a code action that will invoke the fix.
                    context.RegisterCodeFix(
                        new CodeActionChangetoAwaitAsync("Change synchronous operation to asynchronous counterpart",
                                                         c => ChangeToAwaitAsync(context.Document, memberAccessNode, name, c)),
                        diagnostic);
                    return;
                }
            }
        }

        private async Task<Document> ToDelayWhenAnyWhenAllAsync(Document document, InvocationExpressionSyntax invocation, string name, CancellationToken cancellationToken)
        {
            var simpleExpression = SyntaxFactory.ParseName("");
            if (name.Equals("WaitAny"))
            {
                simpleExpression = SyntaxFactory.ParseName("System.Threading.Tasks.Task.WhenAny").WithAdditionalAnnotations(Simplifier.Annotation);
            }
            else if (name.Equals("WaitAll"))
            {
                simpleExpression = SyntaxFactory.ParseName("System.Threading.Tasks.Task.WhenAll").WithAdditionalAnnotations(Simplifier.Annotation);
            }
            else if (name.Equals("Sleep"))
            {
                simpleExpression = SyntaxFactory.ParseName("System.Threading.Tasks.Task.Delay").WithAdditionalAnnotations(Simplifier.Annotation);
            }

            SyntaxNode oldExpression = invocation;
            var expression = invocation.WithExpression(simpleExpression).WithLeadingTrivia(invocation.GetLeadingTrivia()).WithTrailingTrivia(invocation.GetTrailingTrivia());

            var newExpression = SyntaxFactory.AwaitExpression(expression.WithLeadingTrivia(SyntaxFactory.Space)).WithTrailingTrivia(invocation.GetTrailingTrivia()).WithLeadingTrivia(invocation.GetLeadingTrivia());

            var oldroot = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            var newroot = oldroot.ReplaceNode(oldExpression, newExpression);

            var newDocument = document.WithSyntaxRoot(newroot);

            return newDocument;
        }

        private async Task<Document> ChangetoAwaitAsync(Document document, InvocationExpressionSyntax invocation, string name, CancellationToken cancellationToken)
        {
            SyntaxNode oldExpression = invocation;
            SyntaxNode newExpression = null;

            if (name.Equals("Wait"))
            {
                var expression = (invocation.Expression as MemberAccessExpressionSyntax).Expression;
                newExpression = SyntaxFactory.AwaitExpression(expression).WithAdditionalAnnotations(Formatter.Annotation);
            }

            var oldroot = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            var newroot = oldroot.ReplaceNode(oldExpression, newExpression);

            var newDocument = document.WithSyntaxRoot(newroot);

            return newDocument;
        }

        private async Task<Document> ChangeToAwaitAsync(Document document, MemberAccessExpressionSyntax access, string name, CancellationToken cancellationToken)
        {
            SyntaxNode oldExpression = access;
            SyntaxNode newExpression = null;

            if (name.Equals("Result"))
            {
                newExpression = SyntaxFactory.AwaitExpression(access.Expression).WithAdditionalAnnotations(Formatter.Annotation);
            }

            if (newExpression != null)
            {
                var oldroot = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
                var newroot = oldroot.ReplaceNode(oldExpression, newExpression);

                document = document.WithSyntaxRoot(newroot);
            }

            return document;
        }

        private async Task<Document> ChangetoAwaitGetAwaiterAsync(Document document, InvocationExpressionSyntax invocation, CancellationToken cancellationTkn)
        {
            SyntaxNode expression = invocation;
            while (!(expression is ExpressionStatementSyntax))
            {
                expression = expression.Parent;
            }

            var oldExpression = expression as ExpressionStatementSyntax;
            var awaitedInvocation = SyntaxFactory.AwaitExpression(invocation.WithLeadingTrivia(SyntaxFactory.Space)).WithLeadingTrivia(invocation.GetLeadingTrivia());
            var newExpression = oldExpression.WithExpression(awaitedInvocation);

            var oldroot = await document.GetSyntaxRootAsync(cancellationTkn).ConfigureAwait(false);
            var newroot = oldroot.ReplaceNode(oldExpression, newExpression);
            var newDocument = document.WithSyntaxRoot(newroot);

            return newDocument;
        }

        public sealed override FixAllProvider GetFixAllProvider()
        {
            return null;
        }

        private class CodeActionToDelayWhenAnyWhenAllAsync : CodeAction
        {
            private Func<CancellationToken, Task<Document>> _generateDocument;
            private string _title;

            public CodeActionToDelayWhenAnyWhenAllAsync(string title, Func<CancellationToken, Task<Document>> generateDocument)
            {
                _title = title;
                _generateDocument = generateDocument;
            }

            public override string Title { get { return _title; } }

            protected override Task<Document> GetChangedDocumentAsync(CancellationToken cancellationToken)
            {
                return _generateDocument(cancellationToken);
            }

            public override string EquivalenceKey
            {
                get
                {
                    return nameof(CodeActionToDelayWhenAnyWhenAllAsync);
                }
            }
        }

        private class CodeActionChangetoAwaitAsync : CodeAction
        {
            private Func<CancellationToken, Task<Document>> _generateDocument;
            private string _title;

            public CodeActionChangetoAwaitAsync(string title, Func<CancellationToken, Task<Document>> generateDocument)
            {
                _title = title;
                _generateDocument = generateDocument;
            }

            public override string Title { get { return _title; } }

            protected override Task<Document> GetChangedDocumentAsync(CancellationToken cancellationToken)
            {
                return _generateDocument(cancellationToken);
            }

            public override string EquivalenceKey
            {
                get
                {
                    return nameof(CodeActionChangetoAwaitAsync);
                }
            }
        }

        private class CodeActionChangetoAwaitGetAwaiterAsync : CodeAction
        {
            private Func<CancellationToken, Task<Document>> _generateDocument;
            private string _title;

            public CodeActionChangetoAwaitGetAwaiterAsync(string title, Func<CancellationToken, Task<Document>> generateDocument)
            {
                _title = title;
                _generateDocument = generateDocument;
            }

            public override string Title { get { return _title; } }

            protected override Task<Document> GetChangedDocumentAsync(CancellationToken cancellationToken)
            {
                return _generateDocument(cancellationToken);
            }

            public override string EquivalenceKey
            {
                get
                {
                    return nameof(CodeActionChangetoAwaitGetAwaiterAsync);
                }
            }
        }
    }
}
