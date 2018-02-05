// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.MaintainabilityRules
{
   using System;
   using System.Collections.Generic;
   using System.Collections.Immutable;
   using System.Composition;
   using System.Threading;
   using System.Threading.Tasks;
   using Helpers;
   using Microsoft.CodeAnalysis;
   using Microsoft.CodeAnalysis.CodeActions;
   using Microsoft.CodeAnalysis.CodeFixes;
   using Microsoft.CodeAnalysis.CSharp;
   using Microsoft.CodeAnalysis.CSharp.Syntax;
   using System.Linq;

   /// <summary>
   /// Implements a code fix for <see cref="TS1003UseLinqAnyOverCountGreaterThan0"/>.
   /// </summary>
   /// <remarks>
   /// <para>To fix a violation of this rule, add an access modifier to the declaration of the element.</para>
   /// </remarks>
   [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof( TS1003CodeFixProvider ) )]
    [Shared]
    internal class TS1003CodeFixProvider : CodeFixProvider
    {
        /// <inheritdoc/>
        public override ImmutableArray<string> FixableDiagnosticIds { get; } =
            ImmutableArray.Create(TS1003UseLinqAnyOverCountGreaterThan0.DiagnosticId);

      /// <inheritdoc/>
      public override FixAllProvider GetFixAllProvider()
      {
         return CustomFixAllProviders.BatchFixer;
      }

      /// <inheritdoc/>
      public override async Task RegisterCodeFixesAsync( CodeFixContext context )
      {
         foreach ( var diagnostic in context.Diagnostics )
         {
            context.RegisterCodeFix(
               CodeAction.Create(
                  "Use Linq's Any() :)",
                  cancellationToken => GetTransformedDocumentAsync( context.Document, diagnostic, cancellationToken ),
                  nameof( TS1002CodeFixProvider ) ),
               diagnostic );
         }
      }

      private static async Task<Document> GetTransformedDocumentAsync( Document document, Diagnostic diagnostic, CancellationToken cancellationToken )
      {
         var root = await document.GetSyntaxRootAsync( cancellationToken ).ConfigureAwait( false );
         var token = root.FindToken( diagnostic.Location.SourceSpan.Start );

         var memberAccess = token.Parent as MemberAccessExpressionSyntax;
         var compareToZero = memberAccess.Parent as BinaryExpressionSyntax;

         var memberExpression =
            SyntaxFactory.MemberAccessExpression( SyntaxKind.SimpleMemberAccessExpression, memberAccess.Expression, SyntaxFactory.IdentifierName( "Any" ) );
         var newExpression =
            SyntaxFactory.InvocationExpression( memberExpression );

         var newRoot = root.ReplaceNode( compareToZero, newExpression );

         //Add using linq :)
         SyntaxNode rootWithUsing = newRoot;
         if ( !newRoot.ChildNodes().Where( c => c.Kind() == SyntaxKind.UsingDirective && ((UsingDirectiveSyntax)c).Name.ToFullString() == "System.Linq").Any())
         {
            var name = SyntaxFactory.ParseName( "System.Linq" );
            var linqUsing = SyntaxFactory.UsingDirective( name );
            var newNodes = new List<SyntaxNode>() { linqUsing };

            var existingUsingDirectives = newRoot.ChildNodes().Where( c => c.Kind() == SyntaxKind.UsingDirective );
            if ( existingUsingDirectives.Any() )
            {
               var placeToAddUsing = existingUsingDirectives.Where( c => string.Compare( ( (UsingDirectiveSyntax)c ).Name.ToFullString(), "System.Linq" ) < 0 ).Last();
               rootWithUsing = newRoot.InsertNodesAfter( placeToAddUsing, newNodes );
            }
            else
            {
               var placeToAddUsing = newRoot.ChildNodes().First();
               rootWithUsing = newRoot.InsertNodesBefore( placeToAddUsing, newNodes );
            }
         }

         return document.WithSyntaxRoot( rootWithUsing );
      }
   }
}
