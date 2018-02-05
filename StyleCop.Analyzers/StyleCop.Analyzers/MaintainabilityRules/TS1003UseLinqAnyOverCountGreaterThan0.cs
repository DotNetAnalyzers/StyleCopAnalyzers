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
   internal class TS1003UseLinqAnyOverCountGreaterThan0 : DiagnosticAnalyzer
   {
      public const string DiagnosticId = "TS1003";
      private const string Title = "Use Linq's Any() over Count > 0 :)";
      private const string Description = "Use Linq's Any() over comparing to a magic value of 0 :)";
      private const string HelpLink = "https://github.com/TechSmith/CamtasiaWin/wiki/Automated-Code-Standards";

      private const string MessageNotFollowed = "Should not compare a count to a magic value of 0 :)";

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

         SyntaxToken prevToken3 = SyntaxFactory.Token( SyntaxKind.None );
         SyntaxToken prevToken2 = SyntaxFactory.Token( SyntaxKind.None );
         SyntaxToken prevToken = SyntaxFactory.Token( SyntaxKind.None );
         SyntaxToken curToken = SyntaxFactory.Token( SyntaxKind.None );
         foreach ( var token in root.DescendantTokens( descendIntoTrivia: true ) )
         {
            prevToken3 = prevToken2;
            prevToken2 = prevToken;
            prevToken = curToken;
            curToken = token;

            if ( !curToken.IsKind( SyntaxKind.NumericLiteralToken ) )
            {
               continue;
            }

            int nValue;
            if ( !int.TryParse( curToken.ValueText, out nValue ) )
            {
               continue;
            }

            if ( nValue != 0)
            {
               continue;
            }

            if ( !prevToken.IsKind( SyntaxKind.GreaterThanToken ))
            {
               continue;
            }

            if ( !prevToken2.IsKind( SyntaxKind.IdentifierToken))
            {
               continue;
            }

            if ( !string.Equals( prevToken2.ValueText, "Count", StringComparison.CurrentCultureIgnoreCase ) )
            {
               continue;
            }

            if ( !prevToken3.IsKind(SyntaxKind.DotToken))
            {
               continue;
            }

            context.ReportDiagnostic( Diagnostic.Create( DescriptorNotFollowed, prevToken3.GetLocation(), "Count compared to 0" ) );
         }

      }
   }
}
