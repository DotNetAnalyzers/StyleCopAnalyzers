﻿// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Editing;

namespace Microsoft.CodeAnalysis.Analyzers.MetaAnalyzers.CodeFixes
{
    public abstract class ApplyDiagnosticAnalyzerAttributeFix : CodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(DiagnosticIds.MissingDiagnosticAnalyzerAttributeRuleId); }
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
            var token = root.FindToken(context.Span.Start);
            if (!token.Span.IntersectsWith(context.Span))
            {
                return;
            }

            var generator = SyntaxGenerator.GetGenerator(context.Document);
            var classDecl = generator.GetDeclaration(token.Parent);
            if (classDecl == null)
            {
                return;
            }

            // Register fixes.

            // 1) Apply C# DiagnosticAnalyzerAttribute.
            var title = string.Format(CodeAnalysisDiagnosticsResources.ApplyDiagnosticAnalyzerAttribute_1, LanguageNames.CSharp);
            AddFix(title, context, root, classDecl, generator, LanguageNames.CSharp);

            // 2) Apply VB DiagnosticAnalyzerAttribute.
            title = string.Format(CodeAnalysisDiagnosticsResources.ApplyDiagnosticAnalyzerAttribute_1, LanguageNames.VisualBasic);
            AddFix(title, context, root, classDecl, generator, LanguageNames.VisualBasic);

            // 3) Apply both C# and VB DiagnosticAnalyzerAttributes.
            title = string.Format(CodeAnalysisDiagnosticsResources.ApplyDiagnosticAnalyzerAttribute_2, LanguageNames.CSharp, LanguageNames.VisualBasic);
            AddFix(title, context, root, classDecl, generator, LanguageNames.CSharp, LanguageNames.VisualBasic);
        }

        protected abstract SyntaxNode ParseExpression(string expression);

        private void AddFix(string codeFixTitle, CodeFixContext context, SyntaxNode root, SyntaxNode classDecl, SyntaxGenerator generator, params string[] languages)
        {
            var fix = new MyCodeAction(
                codeFixTitle,
                c => GetFix(context.Document, root, classDecl, generator, languages));
            context.RegisterCodeFix(fix, context.Diagnostics);
        }

        private Task<Document> GetFix(Document document, SyntaxNode root, SyntaxNode classDecl, SyntaxGenerator generator, params string[] languages)
        {
            var languageNamesFullName = typeof(LanguageNames).FullName;
            var arguments = new SyntaxNode[languages.Length];

            for (int i = 0; i < languages.Length; i++)
            {
                var language = languages[i] == LanguageNames.CSharp ? nameof(LanguageNames.CSharp) : nameof(LanguageNames.VisualBasic);
                var expressionToParse = languageNamesFullName + "." + language;
                var parsedExpression = ParseExpression(expressionToParse);
                arguments[i] = generator.AttributeArgument(parsedExpression);
            }

            SyntaxNode attribute = generator.Attribute(DiagnosticAnalyzerCorrectnessAnalyzer.DiagnosticAnalyzerAttributeFullName, arguments);
            var newClassDecl = generator.AddAttributes(classDecl, attribute);
            var newRoot = root.ReplaceNode(classDecl, newClassDecl);
            return Task.FromResult(document.WithSyntaxRoot(newRoot));
        }

        public sealed override FixAllProvider GetFixAllProvider()
        {
            return WellKnownFixAllProviders.BatchFixer;
        }

        private class MyCodeAction : DocumentChangeAction
        {
            public MyCodeAction(string title, Func<CancellationToken, Task<Document>> createChangedDocument)
                : base(title, createChangedDocument)
            {
            }
        }
    }
}
