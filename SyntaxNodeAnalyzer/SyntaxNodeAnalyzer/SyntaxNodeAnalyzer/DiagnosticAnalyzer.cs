// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;

namespace SyntaxNodeAnalyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SyntaxNodeAnalyzerAnalyzer : DiagnosticAnalyzer
    {
        // Each analyzer needs a public id to identify each DiagnosticDescriptor and subsequently fix diagnostics in CodeFixProvider.cs
        public const string spacingRuleId = "IfSpacing001";

        // If the analyzer finds an issue, it will report the DiagnosticDescriptor rule
        internal static DiagnosticDescriptor Rule = new DiagnosticDescriptor(id: spacingRuleId, defaultSeverity: DiagnosticSeverity.Warning, isEnabledByDefault: true, title: "Enter a title for this diagnostic", messageFormat: "Enter a message to be displayed with this diagnostic", category: "Enter a category for this diagnostic");
        // id: Identifies each rule. Same as the public constant declared above
        // defaultSeverity: Is set to DiagnosticSeverity.[severity] where severity can be Error, Warning, Hidden or Info, but can only be Error or Warning for the purposes of this tutorial
        // isEnabledByDefault: Determines whether the analyzer is enabled by default or if the user must manually enable it. Generally set to true

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                // This array contains all the diagnostics that can be shown to the user
                return ImmutableArray.Create(Rule);
            }
        }

        public override void Initialize(AnalysisContext context)
        {
            // Calls the method (first argument) to perform analysis whenever this is a change to a SyntaxNode of kind IfStatement
            context.RegisterSyntaxNodeAction(AnalyzeIfStatement, SyntaxKind.IfStatement);
        }

        // This method, which is the method that is registered within Initialize, performs the analysis of the Syntax Tree when an IfStatementSyntax Node is found. If the analysis finds an error, a diagnostic is reported
        // In this tutorial, this method will walk through the Syntax Tree seen in IfSyntaxTree.jpg and determine if the if-statement being analyzed has the correct spacing
        private void AnalyzeIfStatement(SyntaxNodeAnalysisContext context)
        {
            // The SyntaxNode found by the Initialize method should be cast to the expected type. Here, this type is IfStatementSyntax
            var ifStatement = (IfStatementSyntax)context.Node;

            // This statement navigates down the syntax tree one level to extract the 'if' keyword
            var ifKeyword = ifStatement.IfKeyword;

            // Checks if there is any trailing trivia (eg spaces or comments) associated with the if-keyword.
            if (ifKeyword.HasTrailingTrivia)
            {
                // Checks that there is only one piece of trailing trivia.
                if (ifKeyword.TrailingTrivia.Count == 1)
                {
                    var trailingTrivia = ifKeyword.TrailingTrivia.First();

                    // Checks that the single trailing trivia is of kind whitespace (as opposed to a comment for example).
                    if (trailingTrivia.IsKind(SyntaxKind.WhitespaceTrivia))
                    {
                        // Finally, this statement checks that the trailing trivia is one single space.
                        if (trailingTrivia.ToString() == " ")
                        {
                            // If the analyzer is satisfied that there is only a single space between 'if' and '(', it will return from this method without reporting a diagnostic
                            return;
                        }
                    }
                }
            }

            // Extracts the opening parenthesis of the if-statement condition
            var openParen = ifStatement.OpenParenToken;

            // Determines the start of the span of the diagnostic that will be reported, ie the start of the squiggle
            var startDiagnosticSpan = ifKeyword.SpanStart;

            // Determines the end of the span of the diagnostic that will be reported
            var endDiagnosticSpan = openParen.SpanStart;

            // The span is the range of integers that define the position of the characters the red squiggle will underline
            var diagnosticSpan = TextSpan.FromBounds(startDiagnosticSpan, endDiagnosticSpan);

            // Uses the span created above to create a location for the diagnostic squiggle to appear within the syntax tree passed in as an argument
            var diagnosticLocation = Location.Create(ifStatement.SyntaxTree, diagnosticSpan);

            // Holds the diagnostic and all necessary information to be reported
            var diagnostic = Diagnostic.Create(Rule, diagnosticLocation);

            // Sends diagnostic information to the IDE to be shown to the user
            context.ReportDiagnostic(diagnostic);
        }
    }
}
