// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.MaintainabilityRules
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

   [DiagnosticAnalyzer(LanguageNames.CSharp)]
   internal class TS1002NumericLiteralsMustHaveSuffix : DiagnosticAnalyzer
   {
      public const string DiagnosticId = "TS1002";
      private const string Title = "Numeric literals must have suffix :)";
      private const string Description = "A numeric literal in C# should have a suffix :)";
      private const string HelpLink = "https://github.com/TechSmith/CamtasiaWin/wiki/Automated-Code-Standards";

      private const string MessageNotFollowed = "A numeric literal in C# should have a suffix :)";

      private static readonly Action<CompilationStartAnalysisContext> CompilationStartAction = HandleCompilationStart;
      private static readonly Action<SyntaxTreeAnalysisContext> SyntaxTreeAction = HandleSyntaxTree;

      /// <summary>
      /// Gets the diagnostic descriptor for an opening parenthesis that must not be followed by whitespace.
      /// </summary>
      /// <value>The diagnostic descriptor for an opening parenthesis that must not be followed by whitespace.</value>
      public static DiagnosticDescriptor DescriptorNotFollowed { get; }
          = new DiagnosticDescriptor( DiagnosticId, Title, MessageNotFollowed, AnalyzerCategory.SpacingRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink );

      /// <inheritdoc/>
      public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
          ImmutableArray.Create( DescriptorNotFollowed );

      /// <inheritdoc/>
      public override void Initialize( AnalysisContext context )
      {
         context.RegisterCompilationStartAction( CompilationStartAction );
      }

      private static void HandleCompilationStart( CompilationStartAnalysisContext context )
      {
         context.RegisterSyntaxTreeActionHonorExclusions( SyntaxTreeAction );
      }

      private static void HandleSyntaxTree( SyntaxTreeAnalysisContext context )
      {
         SyntaxNode root = context.Tree.GetCompilationUnitRoot( context.CancellationToken );
         foreach ( var token in root.DescendantTokens( descendIntoTrivia: true ).Where( t => t.IsKind( SyntaxKind.NumericLiteralToken ) ) )
         {
            HandleNumericLiteralToken( context, token );
         }
      }

      private static void HandleNumericLiteralToken( SyntaxTreeAnalysisContext context, SyntaxToken token )
      {
         // First checking if already has a suffix and in that case it is good :)
         string[] arrSuffixes =
         {
            "L",
            "D",
            "F",
            "U",
            "M",
            "UL"
         };
         foreach ( var suffix in arrSuffixes )
         {
            if ( token.Text.EndsWith(suffix, StringComparison.CurrentCultureIgnoreCase) )
            {
               return; // Already has a suffix; so good :)
            }
         }

         // Does it have a decimal point?  If not it is good :)
         if (!token.Text.Contains("."))
         {
            return;
         }

         context.ReportDiagnostic( Diagnostic.Create( DescriptorNotFollowed, token.GetLocation(), token.Text ) );
      }
   }
}
