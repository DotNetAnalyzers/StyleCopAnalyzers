// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.Helpers
{
    using System;
    using System.Collections.Immutable;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// The tokens of a syntax tree, collected in one walk and shared by the analyzers of a compilation that would
    /// otherwise each walk the tree.
    /// </summary>
    internal sealed class SyntaxTreeTokens
    {
        private static readonly SyntaxTreeValueProvider<SyntaxTreeTokens> ValueProvider =
            new SyntaxTreeValueProvider<SyntaxTreeTokens>(static tree => new SyntaxTreeTokens(tree));

        private readonly SyntaxTree tree;
        private readonly object gate = new object();
        private Collected collected;

        /// <summary>
        /// Initializes a new instance of the <see cref="SyntaxTreeTokens"/> class.
        /// </summary>
        /// <param name="tree">The syntax tree.</param>
        internal SyntaxTreeTokens(SyntaxTree tree)
        {
            this.tree = tree;
        }

        /// <summary>
        /// Gets the shared tokens of a syntax tree.
        /// </summary>
        /// <param name="context">The analysis context of the compilation that contains <paramref name="tree"/>.</param>
        /// <param name="tree">The syntax tree.</param>
        /// <returns>The tokens of <paramref name="tree"/>.</returns>
        [SuppressMessage("MicrosoftCodeAnalysisPerformance", "RS1012:Start action has no registered actions", Justification = "This is not a start action")]
        public static SyntaxTreeTokens GetOrCreate(CompilationStartAnalysisContext context, SyntaxTree tree)
        {
            return context.TryGetValue(tree, ValueProvider, out var value) ? value : new SyntaxTreeTokens(tree);
        }

        /// <summary>
        /// Gets the tokens of the tree in document order, as <see cref="SyntaxNode.DescendantTokens(Func{SyntaxNode, bool}, bool)"/>
        /// returns them with <c>descendIntoTrivia</c> set to <see langword="false"/>.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token that the operation will observe.</param>
        /// <returns>The tokens of the tree.</returns>
        public ImmutableArray<SyntaxToken> GetTokens(CancellationToken cancellationToken)
        {
            return this.GetCollected(cancellationToken).Tokens;
        }

        /// <summary>
        /// Gets the tokens of the tree and of its structured trivia in document order, as
        /// <see cref="SyntaxNode.DescendantTokens(Func{SyntaxNode, bool}, bool)"/> returns them with <c>descendIntoTrivia</c>
        /// set to <see langword="true"/>.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token that the operation will observe.</param>
        /// <returns>The tokens of the tree, including the tokens of its structured trivia.</returns>
        public ImmutableArray<SyntaxToken> GetTokensIncludingStructuredTrivia(CancellationToken cancellationToken)
        {
            return this.GetCollected(cancellationToken).TokensIncludingStructuredTrivia;
        }

        private static void AddStructuredTriviaTokens(SyntaxTriviaList triviaList, ImmutableArray<SyntaxToken>.Builder builder)
        {
            foreach (var trivia in triviaList)
            {
                if (trivia.HasStructure)
                {
                    builder.AddRange(trivia.GetStructure().DescendantTokens(descendIntoTrivia: true));
                }
            }
        }

        private Collected GetCollected(CancellationToken cancellationToken)
        {
            var result = Volatile.Read(ref this.collected);
            if (result != null)
            {
                return result;
            }

            lock (this.gate)
            {
                if (this.collected == null)
                {
                    Volatile.Write(ref this.collected, this.Collect(cancellationToken));
                }

                return this.collected;
            }
        }

        private Collected Collect(CancellationToken cancellationToken)
        {
            var tokens = ImmutableArray.CreateBuilder<SyntaxToken>();
            var tokensIncludingStructuredTrivia = ImmutableArray.CreateBuilder<SyntaxToken>();
            foreach (var token in this.tree.GetRoot(cancellationToken).DescendantTokens())
            {
                tokens.Add(token);
                if (token.HasStructuredTrivia)
                {
                    AddStructuredTriviaTokens(token.LeadingTrivia, tokensIncludingStructuredTrivia);
                    tokensIncludingStructuredTrivia.Add(token);
                    AddStructuredTriviaTokens(token.TrailingTrivia, tokensIncludingStructuredTrivia);
                }
                else
                {
                    tokensIncludingStructuredTrivia.Add(token);
                }
            }

            return new Collected(tokens.ToImmutable(), tokensIncludingStructuredTrivia.ToImmutable());
        }

        private sealed class Collected
        {
            public Collected(ImmutableArray<SyntaxToken> tokens, ImmutableArray<SyntaxToken> tokensIncludingStructuredTrivia)
            {
                this.Tokens = tokens;
                this.TokensIncludingStructuredTrivia = tokensIncludingStructuredTrivia;
            }

            public ImmutableArray<SyntaxToken> Tokens { get; }

            public ImmutableArray<SyntaxToken> TokensIncludingStructuredTrivia { get; }
        }
    }
}
