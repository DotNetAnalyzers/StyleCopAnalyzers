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
        public const string spacingRuleId = "IfSpacing";
        //Each analyzer needs a public id to identify each DiagnosticDescriptor and subsequently fix diagnostics in CodeFixProvider.cs.

        internal static DiagnosticDescriptor Rule = new DiagnosticDescriptor(id: spacingRuleId, defaultSeverity: DiagnosticSeverity.Warning, isEnabledByDefault: true, title: "Enter a title for this diagnostic", messageFormat: "Enter a message to be displayed with this diagnostic", category: "Enter a category for this diagnostic");
        //If the analyzer finds an issue, it will report the DiagnosticDescriptor rule.
        //id: Identifies each rule. Same as the public constant declared above.
        //defaultSeverity: Is set to DiagnosticSeverity.[severity] where severity can be Error, Warning, Hidden or Info
        //isEnabledByDefault: Determines whether the analyzer is enabled by default or if the user must manually enable it. Generally set to true.

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                return ImmutableArray.Create(Rule);
                //This array contains all the diagnostics that can be shown to the user.
            }
        }

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeIfStatement, SyntaxKind.IfStatement);
            //Calls the AnalyzeIfStatement method to perform analysis whenever there is a change to a SyntaxNode of kind IfStatementSyntax.
        }

        private void AnalyzeIfStatement(SyntaxNodeAnalysisContext context)
        {
            var ifStatement = (IfStatementSyntax)context.Node;
            //The SyntaxNode found by the Initialize method should be cast to the expected type. Here, this type is IfStatementSyntax.

            var ifKeyword = ifStatement.IfKeyword;
            //This statement navigates down the syntax tree one level to extract the 'if' keyword.

            if (ifKeyword.HasTrailingTrivia)
            {
                var trailingTrivia = ifKeyword.TrailingTrivia.Last();

                if (trailingTrivia.Kind() == SyntaxKind.WhitespaceTrivia)
                {
                    if (trailingTrivia.ToString() == " ")
                    {
                        return;
                        //If the analyzer is satisfied that there is only a single whitespace between 'if' and '(', you can return from this method.
                        //If not, remain in AnalyzeIfStatement in order to report the diagnostic.
                    }
                }
            }

            var openParen = ifStatement.OpenParenToken;
            //Extracts the opening parenthesis of the if statement condition.

            var startDiagnosticSpan = ifKeyword.Span.Start;
            //Determines the start of the span of the diagnostic that will be reported, ie the start of the red squiggle.

            var endDiagnosticSpan = openParen.Span.Start;
            //Determines the end of the span of the diagnostic that will be reported.

            var diagnosticSpan = TextSpan.FromBounds(startDiagnosticSpan, endDiagnosticSpan);
            //The span is the range of intergers that define the position of the characters the red squiggle will underline.

            var diagnosticLocation = Location.Create(ifStatement.SyntaxTree, diagnosticSpan);
            //Uses the span created above to create a location for the diagnostic "red squiggle" to appear within the syntax tree passed in as an argument.

            var diagnostic = Diagnostic.Create(Rule, diagnosticLocation);
            //Holds the diagnostic and all necessary information to be reported.

            context.ReportDiagnostic(diagnostic);
            //Sends diagnostic information to the IDE to be shown to the user.
        }
    }
}
