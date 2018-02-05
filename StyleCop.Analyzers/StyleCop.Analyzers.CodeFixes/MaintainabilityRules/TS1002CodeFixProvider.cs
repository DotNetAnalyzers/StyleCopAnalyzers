// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.MaintainabilityRules
{
   using System;
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

   /// <summary>
   /// Implements a code fix for <see cref="TS1002LiteralNumericMustHaveSuffix"/>.
   /// </summary>
   /// <remarks>
   /// <para>To fix a violation of this rule, add an access modifier to the declaration of the element.</para>
   /// </remarks>
   [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof( TS1002CodeFixProvider ) )]
    [Shared]
    internal class TS1002CodeFixProvider : CodeFixProvider
    {
        /// <inheritdoc/>
        public override ImmutableArray<string> FixableDiagnosticIds { get; } =
            ImmutableArray.Create(TS1002NumericLiteralsMustHaveSuffix.DiagnosticId);

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
                  "Fix numeric literals :)",
                  cancellationToken => GetTransformedDocumentAsync( context.Document, diagnostic, cancellationToken ),
                  nameof( TS1002CodeFixProvider ) ),
               diagnostic );
         }
      }

      private static async Task<Document> GetTransformedDocumentAsync( Document document, Diagnostic diagnostic, CancellationToken cancellationToken )
      {
         var root = await document.GetSyntaxRootAsync( cancellationToken ).ConfigureAwait( false );
         var token = root.FindToken( diagnostic.Location.SourceSpan.Start );

         SyntaxToken newToken = SyntaxFactory.Literal( token.ValueText + "d", double.Parse( token.ValueText ) );

         var newRoot = root.ReplaceToken( token, newToken );

         return document.WithSyntaxRoot( newRoot );
      }
   }
}
