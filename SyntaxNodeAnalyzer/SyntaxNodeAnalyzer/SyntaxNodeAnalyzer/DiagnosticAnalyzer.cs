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
        //If the analyzer finds an issue, it will report the DiagnosticDescriptor rule. Thus, each analyzer must have at least one DiagnosticDescriptor.
        //id: Identifies each rule. Same as the public constant declared above.
        //defaultSeverity: Is set to DiagnosticSeverity.[severity] where severity can be Error, Warning, Hidden or Info
        //isEnabledByDefault: Determines whether the analyzer is enabled by default or if the user must manually enable it. Generally set to true.
        //title: The title for this diagnostic.
        //message: Will be displayed when the diagnostic surfaces
        //category: A category for the diagnostic eg "Syntax" or "Formatting".

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                return ImmutableArray.Create(Rule);
                //The SupportedDiagnostics property returns an ImmutableArray containing all the DiagnosticDescriptors supported by the analyzer.
            }
        }

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeIfStatement, SyntaxKind.IfStatement);
            //Scans for and registers instances of if statement syntax so the analysis in AnalyzeIfStatement can be performed.
        }

        private void AnalyzeIfStatement(SyntaxNodeAnalysisContext context)
        {
            var ifStatement = (IfStatementSyntax)context.Node;
            //The SyntaxNode registered in the Initialize method should be cast to the expected type. Here, this type is IfStatementSyntax.

            var ifKeyword = ifStatement.IfKeyword;
            //This statement navigates down the syntax tree one level to extract the 'if' keyword.

            if (ifKeyword.HasTrailingTrivia)
            //Checks whether the 'if' keyword has trailing trivia.**
            {
                var trailingTrivia = ifKeyword.TrailingTrivia.Last();
                //Extracts the last trailing trivia of ifKeyword.

                if (trailingTrivia.Kind() == SyntaxKind.WhitespaceTrivia)
                //Checks that the trailing trivia of the 'if' keyword is of Kind 'WhitespaceTrivia'.**
                {
                    if (trailingTrivia.ToString() == " ")
                    //Checks that only trailing trivia is a single whitespace.**
                    {
                        return;
                        //If you are satisfied that there is only a single whitespace between 'if' and '(', you can return from this method.
                        //If not, remain in AnalyzeIfStatement in order to report the diagnostic.
                    }
                }
            }

            var openParen = ifStatement.OpenParenToken;
            //Extracts the opening parenthesis of the if statement condition.

            var startDiagnosticSpan = ifKeyword.Span.Start;
            //Determines the start of the span of the diagnostic that will be reported, ie the start of the red squiggle.

            var endDiagnosticSpan = openParen.Span.Start;
            //Determines the end of the span of the diagnostic that will be reported.**

            var diagnosticSpan = TextSpan.FromBounds(startDiagnosticSpan, endDiagnosticSpan);
            //Creates variable to define the span of the diagnostic.

            var diagnosticLocation = Location.Create(ifStatement.SyntaxTree, diagnosticSpan);
            //Uses the span created above to create a location for the diagnostic, ie where the red squiggle will appear.

            var diagnostic = Diagnostic.Create(Rule, diagnosticLocation, Rule.MessageFormat);
            //Holds the diagnostic and all necessary information to be reported.

            context.ReportDiagnostic(diagnostic);
            //Reports the diagnostic.
            //That's all folks! Try out your analyzer by running it and opening any new or existing project containing an if statement.
        }
    }
}
