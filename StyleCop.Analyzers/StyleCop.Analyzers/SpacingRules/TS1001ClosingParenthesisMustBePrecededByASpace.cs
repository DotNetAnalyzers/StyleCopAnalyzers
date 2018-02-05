// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.SpacingRules
{
   using System;
   using System.Collections.Immutable;
   using System.Linq;
   using Microsoft.CodeAnalysis;
   using Microsoft.CodeAnalysis.CSharp;
   using Microsoft.CodeAnalysis.CSharp.Syntax;
   using Microsoft.CodeAnalysis.Diagnostics;
   using SpacingRules;
   using StyleCop.Analyzers.Helpers;

   /// <summary>
   /// An opening parenthesis within a C# statement is not spaced correctly.
   /// </summary>
   /// <remarks>
   /// <para>An opening parenthesis should not be followed by whitespace, unless it is the last character on the
   /// line.</para>
   /// </remarks>
   [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class TS1001ClosingParenthesisMustBePrecededByASpace : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "TS1001";
        private const string Title = "Closing parenthesis must be preceded by a space :)";
        private const string Description = "A closing parenthesis within a C# statement should be preceded by a space :)";
        private const string HelpLink = "https://github.com/TechSmith/CamtasiaWin/wiki/Automated-Code-Standards";

        private const string MessageNotFollowed = "Closing parenthesis must be preceded by a space. :)";

        private static readonly Action<CompilationStartAnalysisContext> CompilationStartAction = HandleCompilationStart;
        private static readonly Action<SyntaxTreeAnalysisContext> SyntaxTreeAction = HandleSyntaxTree;

        /// <summary>
        /// Gets the diagnostic descriptor for an opening parenthesis that must not be followed by whitespace.
        /// </summary>
        /// <value>The diagnostic descriptor for an opening parenthesis that must not be followed by whitespace.</value>
        public static DiagnosticDescriptor DescriptorNotFollowed { get; }
            = new DiagnosticDescriptor(DiagnosticId, Title, MessageNotFollowed, AnalyzerCategory.SpacingRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
            ImmutableArray.Create( DescriptorNotFollowed );

        /// <inheritdoc/>
        public override void Initialize(AnalysisContext context)
        {
            context.RegisterCompilationStartAction(CompilationStartAction);
        }

        private static void HandleCompilationStart(CompilationStartAnalysisContext context)
        {
            context.RegisterSyntaxTreeActionHonorExclusions(SyntaxTreeAction);
        }

        private static void HandleSyntaxTree(SyntaxTreeAnalysisContext context)
        {
            SyntaxNode root = context.Tree.GetCompilationUnitRoot(context.CancellationToken);
            foreach (var token in root.DescendantTokens(descendIntoTrivia: true).Where(t => t.IsKind(SyntaxKind.CloseParenToken)))
            {
                HandleCloseParenToken(context, token);
            }
        }

        private static void HandleCloseParenToken( SyntaxTreeAnalysisContext context, SyntaxToken token)
        {
            if (token.IsMissing)
            {
                return;
            }

            if ( token.Parent.Kind() == SyntaxKind.CastExpression )
            {
               return; // We don't care whether you have spaces or not when casting
            }

            if (!token.IsPrecededByWhitespace())
            {
               // If previous token is an opening parentheses then this is OK to me :)
               var prevToken = token.GetPreviousToken();

               if ( prevToken.IsKind(SyntaxKind.OpenParenToken))
               {
                  return;
               }

               context.ReportDiagnostic(Diagnostic.Create(DescriptorNotFollowed, token.GetLocation(), TokenSpacingProperties.InsertPreceding));
            }
        }
    }
}
